using NewAnceBoatSimulation.Generated.Input;
using UnityEngine;

namespace NewAnceBoatSimulation.Runtime.Input
{
    public class InputManager : MonoBehaviour
    {
        private static GameInputSystem InputSystem { get; set; }

        // Since the project consists of a single scene
        // we can use OnDestroy for cleanup of the input system when the scene is unloaded
        private void OnDestroy()
        {
            if (InputSystem == null) return;
            InputSystem.Disable();
            InputSystem = null;
        }

        public static GameInputSystem GetInputSystem()
        {
            if (InputSystem != null) return InputSystem;

            InputSystem = new GameInputSystem();
            InputSystem.Enable();
            return InputSystem;
        }
    }
}