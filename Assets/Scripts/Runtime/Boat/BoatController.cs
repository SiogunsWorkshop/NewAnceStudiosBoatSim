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

        private void FixedUpdate()
        {
            if (!AreComponentsValid())
            {
                Debug.LogError(Messages.Errors.ComponentsNotValid, this);
                enabled = false;
                return;
            }

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