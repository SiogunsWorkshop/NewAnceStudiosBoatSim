using UnityEngine;

namespace NewAnceBoatSimulation.Runtime.General
{
    public static class CursorManagement
    {
        public static void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public static void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}