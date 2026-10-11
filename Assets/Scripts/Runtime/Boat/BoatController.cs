using NewAnceBoatSimulation.Runtime.Wind;
using UnityEngine;

namespace NewAnceBoatSimulation.Runtime.Boat
{
    public class BoatController : MonoBehaviour
    {
        [SerializeField] private WindController wind;
        [SerializeField] private SailController sail;

        private Vector2 _currentVelocity;

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

        private void Update()
        {
            if (!AreComponentsValid())
            {
                Debug.LogError(Messages.Errors.ComponentsNotValid, this);
                enabled = false;
                return;
            }

            var apparentWind = wind.GetApparentWindVector(_currentVelocity);
            var sailEffectiveArea = sail.GetActiveSailAreaInSquareMeters();
            var worldSpaceSailDirection =
                transform.forward * Mathf.Cos(sail.transform.localEulerAngles.y * Mathf.Deg2Rad) +
                transform.right * Mathf.Sin(sail.transform.localEulerAngles.y * Mathf.Deg2Rad);

            return;

            bool AreComponentsValid() => this && sail && wind;
        }

        private void Reset()
        {
            sail = GetComponentInChildren<SailController>();
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