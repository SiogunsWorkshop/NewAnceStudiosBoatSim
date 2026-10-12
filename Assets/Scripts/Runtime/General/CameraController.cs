using System;
using NewAnceBoatSimulation.Generated.Input;
using NewAnceBoatSimulation.Runtime.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NewAnceBoatSimulation.Runtime.General
{
    public class CameraController : MonoBehaviour, GameInputSystem.ICameraControllerActions
    {
        [SerializeField] private Transform cameraHandle;
        [SerializeField] private Vector2 verticalRotationLimit = new(10f, 45f);
        [SerializeField] private float rotationSpeed = 1f;

        [SerializeField] private Vector2 zoomRange = new(5f, 20f);
        [SerializeField] private float zoomSpeed = 1f;

        private GameInputSystem _inputSystem;
        private Vector3 _pendingRotation;

        private void OnEnable()
        {
            _inputSystem.CameraController.SetCallbacks(this);
            _inputSystem.CameraController.Enable();
        }

        private void OnDisable()
        {
            _inputSystem.CameraController.SetCallbacks(null);
            _inputSystem.CameraController.Disable();
        }

        private void Awake()
        {
            _inputSystem = InputManager.GetInputSystem();
        }

        private void LateUpdate()
        {
            if (_pendingRotation == Vector3.zero) return;

            var currentRotation = cameraHandle.localEulerAngles;
            currentRotation.x = Mathf.Clamp(currentRotation.x + _pendingRotation.x, verticalRotationLimit.x,
                verticalRotationLimit.y);
            currentRotation.y += _pendingRotation.y;
            cameraHandle.localEulerAngles = currentRotation;

            _pendingRotation = Vector3.zero;
        }

        private void Reset()
        {
            cameraHandle = transform.parent;
        }

        public void OnMoveCamera(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            var input = context.ReadValue<Vector2>();
            var inputRotation = new Vector3(-input.y, input.x, 0f) * rotationSpeed;
            _pendingRotation += inputRotation;
        }

        public void OnZoomCamera(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            var direction = context.ReadValue<float>();
            var distance = direction * zoomSpeed; // No deltaTime for snappy scroll-wheel zooming
            var newZ = Mathf.Clamp(transform.localPosition.z + distance, zoomRange.x, zoomRange.y);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZ);
        }
    }
}