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
            if (EnsureTargetIsValid()) return;
            enabled = false;
        }

        private void LateUpdate()
        {
            MatchPositionNow();
        }

        private bool EnsureTargetIsValid()
        {
            if (targetTransform) return true;

            Debug.LogError("Target Transform is not assigned in MatchPosition.");
            return false;
        }

        [ContextMenu("Match Position Now")]
        private void MatchPositionNow()
        {
            if (!EnsureTargetIsValid())
            {
                enabled = false;
                return;
            }

            transform.position = targetTransform.position + offset;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(transform);
                UnityEditor.Undo.RecordObject(transform, "Match Position Now");
            }
#endif
        }
    }
}