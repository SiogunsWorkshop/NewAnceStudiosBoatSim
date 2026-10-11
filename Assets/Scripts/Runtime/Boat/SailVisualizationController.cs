using UnityEngine;

namespace NewAnceBoatSimulation.Runtime.Boat
{
    public class SailVisualizationController : MonoBehaviour
    {
        [SerializeField] private Transform sailTransformToScale;
        [SerializeField] private Transform sailTransformToTurn;
        [SerializeField] private float scaleBump = 0.1f;

        public void UpdateSailScale(float sailOpenPercentage)
        {
            if (!sailTransformToScale)
            {
                Debug.LogWarning(Messages.Warnings.SailTransformToScaleNotAssigned, this);
                return;
            }

            var newScale = sailTransformToScale.localScale;
            newScale.y = Mathf.Clamp(sailOpenPercentage + scaleBump, 0f, 1f);
            sailTransformToScale.localScale = newScale;
        }

        public void UpdateSailRotation(float sailRotationAngle)
        {
            if (!sailTransformToTurn)
            {
                Debug.LogWarning(Messages.Warnings.SailTransformToTurnNotAssigned, this);
                return;
            }

            var newRotation = sailTransformToTurn.localEulerAngles;
            newRotation.y = sailRotationAngle;
            sailTransformToTurn.localEulerAngles = newRotation;
        }

        private static class Messages
        {
            public static class Warnings
            {
                public const string SailTransformToScaleNotAssigned =
                    "SailTransformToScale is not assigned in SailVisualizationController.";

                public const string SailTransformToTurnNotAssigned =
                    "SailTransformToTurn is not assigned in SailVisualizationController.";
            }
        }
    }
}