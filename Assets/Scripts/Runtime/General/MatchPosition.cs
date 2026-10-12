using System;
using UnityEngine;

namespace NewAnceBoatSimulation.Runtime.General
{
    public class MatchPosition : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Vector3 offset;

        private void OnEnable()
        {
            if (!targetTransform)
            {
                Debug.LogError("Target Transform is not assigned in MatchPosition.");
                enabled = false;
                return;
            }
        }

        private void LateUpdate()
        {
            transform.position = targetTransform.position + offset;
        }
    }
}