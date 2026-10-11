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
        }

        private void Update()
        {
            AlignWindArrow();
        }

        private void Reset()
        {
            windController = GetComponentInParent<WindController>();
            boatController = FindAnyObjectByType<BoatController>();
        }

        private void AlignWindArrow()
        {
            if (!windController || !windArrowAnchor || !boatController) return;

            var windVector = windController.GetApparentWindVector(boatController.CurrentVelocity);
            var direction = windVector.normalized;

            var newRotation = windArrowAnchor.rotation;
            newRotation.eulerAngles = new Vector3(0, Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg, 0);
            windArrowAnchor.rotation = newRotation;
        }
    }
}