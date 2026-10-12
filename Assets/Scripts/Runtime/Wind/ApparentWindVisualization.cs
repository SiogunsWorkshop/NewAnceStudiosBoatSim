using System;
using NewAnceBoatSimulation.Runtime.Boat;
using UnityEngine;

namespace NewAnceBoatSimulation.Runtime.Wind
{
    public class ApparentWindVisualization : MonoBehaviour
    {
        [SerializeField] private WindController windController;
        [SerializeField] private BoatController boatController;
        [SerializeField] private Transform windArrowAnchor;
        [SerializeField] private Transform windArrowArmAnchor;

        [SerializeField]
        private AnimationCurve arrowArmScaleToWindSpeedCurve = AnimationCurve.Linear(0, 0.5f, 10, 1.5f);

        private void OnEnable()
        {
            if (!windController)
            {
                Debug.LogError("WindController is not assigned in ApparentWindVisualization.");
                enabled = false;
                return;
            }

            if (!boatController)
            {
                Debug.LogError("BoatController is not assigned in ApparentWindVisualization.");
                enabled = false;
                return;
            }

            if (!windArrowAnchor)
            {
                Debug.LogError("WindArrowAnchor is not assigned in ApparentWindVisualization.");
                enabled = false;
                return;
            }

            if (!windArrowArmAnchor)
            {
                Debug.LogError("WindArrowArmAnchor is not assigned in ApparentWindVisualization.");
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            UpdateWindArrow();
        }

        private void Reset()
        {
            windController = GetComponentInParent<WindController>();
            boatController = FindAnyObjectByType<BoatController>();
        }

        private void UpdateWindArrow()
        {
            if (!windController || !windArrowAnchor || !boatController) return;

            var windVector = windController.GetApparentWindVector(boatController.CurrentVelocity);
            AlignWindArrow(windVector.normalized);
            AdjustArrowArmLength(windVector.magnitude);
        }

        private void AlignWindArrow(Vector2 apparentWindDirection)
        {
            if (!windController || !windArrowAnchor || !boatController) return;

            var newRotation = windArrowAnchor.rotation;
            newRotation.eulerAngles = new Vector3(0,
                Mathf.Atan2(apparentWindDirection.x, apparentWindDirection.y) * Mathf.Rad2Deg, 0);
            windArrowAnchor.rotation = newRotation;
        }

        private void AdjustArrowArmLength(float apparentWindSpeed)
        {
            if (!windArrowArmAnchor) return;

            var newScale = arrowArmScaleToWindSpeedCurve.Evaluate(apparentWindSpeed);
            windArrowArmAnchor.localScale =
                new Vector3(windArrowArmAnchor.localScale.x, windArrowArmAnchor.localScale.y, newScale);
        }
    }
}