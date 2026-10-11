using UnityEngine;

namespace NewAnceBoatSimulation.Runtime.Wind
{
    public class WindController : MonoBehaviour
    {
        [SerializeField] private float windSpeed = 5f;
        [SerializeField] private float windDirectionInDegrees = 30f;

        // As long as we assume the wind does not change at runtime
        // (and as per task requirements, it doesn't),
        // we can cache the absolute wind vector to avoid recalculating it every time.
        private Vector2 _absoluteWindVectorCached;

        private void Awake()
        {
            _absoluteWindVectorCached = CalculateAbsoluteWindVector(windSpeed, windDirectionInDegrees);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public Vector2 GetAbsoluteWindVector()
        {
            return _absoluteWindVectorCached;
        }

        public Vector2 GetApparentWindVector(Vector2 boatVector)
        {
            var absoluteWindVector = GetAbsoluteWindVector();
            return absoluteWindVector - boatVector;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Vector2 CalculateAbsoluteWindVector(float newWindSpeed, float newWindDirectionInDegrees)
        {
            var windDirectionInRadians = newWindDirectionInDegrees * Mathf.Deg2Rad;
            var windX = newWindSpeed * Mathf.Cos(windDirectionInRadians);
            var windY = newWindSpeed * Mathf.Sin(windDirectionInRadians);
            return new Vector2(windX, windY);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            var windVector = CalculateAbsoluteWindVector(windSpeed, windDirectionInDegrees);
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(windVector.x, 0f, windVector.y));
        }
    }
}