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
        [SerializeField] private float initialSailOpenArea;
        [SerializeField] private float sailAreaInSquareMeters = 10f;

        private GameInputSystem _inputSystem;

        private Coroutine _sailTurnCoroutine;
        private float _currentSailTurnValue;
        private float _sailTurnDirection;

        private Coroutine _sailOpenCoroutine;
        private float _currentSailArea;
        private float _sailOpenDirection;

        private const float MinSailArea = 1f;

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

            SetSailTurnValue(initialSailTurnDegree);

            if (sailAreaInSquareMeters <= MinSailArea)
            {
                sailAreaInSquareMeters = MinSailArea;
            }

            SetSailOpenValue(initialSailOpenArea);
        }

        public float GetActiveSailAreaInSquareMeters()
        {
            return _currentSailArea;
        }

        public float GetSailTurnDegree()
        {
            return _currentSailTurnValue;
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
            OnSailTurnDegreeChanged.Invoke(_currentSailTurnValue);
        }

        private void SetSailOpenValue(float value)
        {
            _currentSailArea = Mathf.Clamp(value, 0, sailAreaInSquareMeters);
            OnSailOpenPercentageChanged.Invoke(_currentSailArea / sailAreaInSquareMeters);
        }

        private IEnumerator HandleRiseOrLowerSailCoroutine()
        {
            while (this)
            {
                var newSailOpenValue = _currentSailArea + (_sailOpenDirection * sailOpenSpeed * Time.deltaTime);
                SetSailOpenValue(newSailOpenValue);
                yield return null;
            }
        }

        private IEnumerator HandleTurnSailCoroutine()
        {
            while (this)
            {
                var newSailTurnValue = _currentSailTurnValue + (_sailTurnDirection * sailTurnSpeed * Time.deltaTime);
                SetSailTurnValue(newSailTurnValue);
                yield return null;
            }
        }
    }
}