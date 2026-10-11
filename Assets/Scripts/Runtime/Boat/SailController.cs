using System.Collections;
using NewAnceBoatSimulation.Generated.Input;
using NewAnceBoatSimulation.Runtime.Input;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace NewAnceBoatSimulation.Runtime.Boat
{
    public class SailController : MonoBehaviour, GameInputSystem.ISailControllerActions
    {
        [field: SerializeField] public UnityEvent<float> OnSailOpenPercentageChanged { get; private set; }
        [field: SerializeField] public UnityEvent<float> OnSailTurnDegreeChanged { get; private set; }

        [Space, SerializeField] private float sailTurnSpeed = 22.5f;
        [SerializeField] private float initialSailTurnDegree;
        [SerializeField] private Vector2 sailTurnLimitDegrees = new(-45f, 45f);

        [Space, SerializeField] private float sailOpenSpeed = 1f;
        [SerializeField] private float initialSailOpen;
        [SerializeField] private Vector2 sailOpenLimit = new(0f, 1f);

        private GameInputSystem _inputSystem;

        private Coroutine _sailTurnCoroutine;
        private float _currentSailTurnValue;
        private float _sailTurnDirection;

        private Coroutine _sailOpenCoroutine;
        private float _currentSailOpenValue;
        private float _sailOpenDirection;

        private void OnEnable()
        {
            _inputSystem.SailController.SetCallbacks(this);
            _inputSystem.SailController.Enable();
        }

        private void OnDisable()
        {
            _inputSystem.SailController.SetCallbacks(null);
            _inputSystem.SailController.Disable();
        }

        private void OnDestroy()
        {
            if (_sailTurnCoroutine != null)
                StopCoroutine(_sailTurnCoroutine);

            if (_sailOpenCoroutine != null)
                StopCoroutine(_sailOpenCoroutine);
        }

        private void Awake()
        {
            _inputSystem = InputManager.GetInputSystem();

            if (sailTurnLimitDegrees.x > sailTurnLimitDegrees.y)
            {
                sailTurnLimitDegrees = new Vector2(sailTurnLimitDegrees.y, sailTurnLimitDegrees.x);
            }

            if (sailOpenLimit.x > sailOpenLimit.y)
            {
                sailOpenLimit = new Vector2(sailOpenLimit.y, sailOpenLimit.x);
            }
        }

        public void OnRiseOrLowerSail(InputAction.CallbackContext context)
        {
            if (context.started)
                OnStarted();
            else if (context.performed)
                OnPerformed();
            else if (context.canceled)
                OnCanceled();

            return;

            void OnStarted()
            {
                _sailOpenCoroutine = StartCoroutine(HandleRiseOrLowerSailCoroutine());
            }

            void OnPerformed()
            {
                _sailOpenDirection = context.ReadValue<float>();
            }

            void OnCanceled()
            {
                StopCoroutine(_sailOpenCoroutine);
            }
        }

        public void OnTurnSail(InputAction.CallbackContext context)
        {
            if (context.started)
                OnStarted();
            else if (context.performed)
                OnPerformed();
            else if (context.canceled)
                OnCanceled();

            return;

            void OnStarted()
            {
                _sailTurnCoroutine = StartCoroutine(HandleTurnSailCoroutine());
            }

            void OnPerformed()
            {
                _sailTurnDirection = context.ReadValue<float>();
            }

            void OnCanceled()
            {
                StopCoroutine(_sailTurnCoroutine);
            }
        }

        private void SetSailTurnValue(float value)
        {
            _currentSailTurnValue = Mathf.Clamp(value, sailTurnLimitDegrees.x, sailTurnLimitDegrees.y);
        }

        private void SetSailOpenValue(float value)
        {
            _currentSailOpenValue = Mathf.Clamp(value, sailOpenLimit.x, sailOpenLimit.y);
        }

        private IEnumerator HandleRiseOrLowerSailCoroutine()
        {
            while (this)
            {
                var newSailOpenValue = _currentSailOpenValue + (_sailOpenDirection * sailOpenSpeed * Time.deltaTime);
                SetSailOpenValue(newSailOpenValue);
                OnSailOpenPercentageChanged.Invoke(_currentSailOpenValue / sailOpenLimit.y);
                yield return null;
            }
        }

        private IEnumerator HandleTurnSailCoroutine()
        {
            while (this)
            {
                var newSailTurnValue = _currentSailTurnValue + (_sailTurnDirection * sailTurnSpeed * Time.deltaTime);
                SetSailTurnValue(newSailTurnValue);
                OnSailTurnDegreeChanged.Invoke(_currentSailTurnValue);
                yield return null;
            }
        }
    }
}