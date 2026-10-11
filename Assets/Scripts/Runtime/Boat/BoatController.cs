using NewAnceBoatSimulation.Runtime.Wind;
using UnityEngine;

namespace NewAnceBoatSimulation.Runtime.Boat
{
    public class BoatController : MonoBehaviour
    {
        [SerializeField] private WindController wind;
        [SerializeField] private SailController sail;

        [Header("Body")] [SerializeField] private float massInKilograms = 500f;
        [SerializeField] private float yawInertia = 800f;
        [SerializeField] private float yawDamping = 2000f;

        [Header("Sail aerodynamics")] [SerializeField]
        private float airDensity = 1.2f;

        // Indexed by absolute angle of attack in degrees (0-180).
        [SerializeField] private AnimationCurve liftCoefficient = new(
            new Keyframe(0f, 0f), new Keyframe(20f, 1.2f), new Keyframe(40f, 0.8f),
            new Keyframe(90f, 0.3f), new Keyframe(180f, 0f));

        [SerializeField] private AnimationCurve dragCoefficient = new(
            new Keyframe(0f, 0.05f), new Keyframe(20f, 0.2f), new Keyframe(90f, 1.2f),
            new Keyframe(180f, 0.4f));

        [Header("Hydrodynamics")] [SerializeField]
        private float forwardLinearDrag = 20f;

        [SerializeField] private float forwardQuadraticDrag = 20f;
        [SerializeField] private float lateralLinearDrag = 200f;
        [SerializeField] private float lateralQuadraticDrag = 400f;

        [Header("Force application points (forward, right) relative to centre of mass")] [SerializeField]
        private Vector2 centreOfEffort = new(0.5f, 0f);

        [SerializeField] private Vector2 centreOfLateralResistance = new(0f, 0f);

        [Header("Model")] [SerializeField] private float initialHeadingDegrees;
        [SerializeField] private float maxDeltaTime = 0.05f;

        private Vector2 _currentVelocity;
        private float _headingRadians;
        private float _yawRate;

        private void Awake()
        {
            _headingRadians = initialHeadingDegrees * Mathf.Deg2Rad;
        }

        private void OnEnable()
        {
            if (!sail)
            {
                Debug.LogError(Messages.Errors.SailControllerNotFound, this);
                enabled = false;
                return;
            }

            if (!wind)
            {
                Debug.LogError(Messages.Errors.WindControllerNotFound, this);
                enabled = false;
                return;
            }
        }

        private void FixedUpdate()
        {
            if (!AreComponentsValid())
            {
                Debug.LogError(Messages.Errors.ComponentsNotValid, this);
                enabled = false;
                return;
            }

            var deltaTime = Mathf.Min(Time.deltaTime, maxDeltaTime);
            if (deltaTime <= 0f)
                return;

            var forward = new Vector2(Mathf.Cos(_headingRadians), Mathf.Sin(_headingRadians));
            var right = new Vector2(forward.y, -forward.x);

            var sailForce = CalculateSailForce(forward);
            var waterForce = CalculateWaterForce(forward, right);

            _currentVelocity += (sailForce + waterForce) / massInKilograms * deltaTime;

            var sailTorque = Cross(LocalToWorld(centreOfEffort, forward, right), sailForce);
            var lateralWaterForce = Vector2.Dot(waterForce, right) * right;
            var waterTorque = Cross(LocalToWorld(centreOfLateralResistance, forward, right), lateralWaterForce);
            var torque = sailTorque + waterTorque - yawDamping * _yawRate;

            _yawRate += torque / yawInertia * deltaTime;
            _headingRadians += _yawRate * deltaTime;

            transform.position += new Vector3(_currentVelocity.x, 0f, _currentVelocity.y) * deltaTime;
            transform.rotation = Quaternion.Euler(0f, 90f - _headingRadians * Mathf.Rad2Deg, 0f);

            return;

            bool AreComponentsValid() => this && sail && wind;
        }

        private Vector2 CalculateSailForce(Vector2 forward)
        {
            var apparentWind = wind.GetApparentWindVector(_currentVelocity);
            var apparentWindSpeedSqr = apparentWind.sqrMagnitude;
            if (apparentWindSpeedSqr < Mathf.Epsilon)
                return Vector2.zero;

            var sailChord = Rotate(-forward, sail.GetSailTurnDegrees());
            var flowDirection = apparentWind.normalized;

            var angleOfAttack = Vector2.SignedAngle(sailChord, flowDirection);
            var absoluteAngleOfAttack = Mathf.Abs(angleOfAttack);
            var liftSide = angleOfAttack >= 0f ? 1f : -1f;

            var dynamicPressureTimesArea =
                0.5f * airDensity * sail.GetActiveSailAreaInSquareMeters() * apparentWindSpeedSqr;

            var liftMagnitude = dynamicPressureTimesArea * liftCoefficient.Evaluate(absoluteAngleOfAttack);
            var dragMagnitude = dynamicPressureTimesArea * dragCoefficient.Evaluate(absoluteAngleOfAttack);

            var lift = new Vector2(-flowDirection.y, flowDirection.x) * (liftSide * liftMagnitude);
            var drag = flowDirection * dragMagnitude;

            return lift + drag;
        }

        private Vector2 CalculateWaterForce(Vector2 forward, Vector2 right)
        {
            var forwardSpeed = Vector2.Dot(_currentVelocity, forward);
            var lateralSpeed = Vector2.Dot(_currentVelocity, right);

            var forwardForce = -(forwardLinearDrag * forwardSpeed
                                 + forwardQuadraticDrag * forwardSpeed * Mathf.Abs(forwardSpeed));
            var lateralForce = -(lateralLinearDrag * lateralSpeed
                                 + lateralQuadraticDrag * lateralSpeed * Mathf.Abs(lateralSpeed));

            return forward * forwardForce + right * lateralForce;
        }

        private static Vector2 Rotate(Vector2 vector, float degrees)
        {
            var radians = degrees * Mathf.Deg2Rad;
            var cos = Mathf.Cos(radians);
            var sin = Mathf.Sin(radians);
            return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
        }

        private static Vector2 LocalToWorld(Vector2 local, Vector2 forward, Vector2 right) =>
            forward * local.x + right * local.y;

        private static float Cross(Vector2 r, Vector2 force) => r.x * force.y - r.y * force.x;

        private void Reset()
        {
            sail = GetComponentInChildren<SailController>();
            wind = FindAnyObjectByType<WindController>();
        }

        private static class Messages
        {
            public static class Errors
            {
                public const string SailControllerNotFound =
                    "SailController component is not assigned to BoatController.";

                public const string WindControllerNotFound =
                    "WindController component is not assigned to BoatController.";

                public const string ComponentsNotValid =
                    "One or more required components are missing or invalid in BoatController.";
            }
        }
    }
}