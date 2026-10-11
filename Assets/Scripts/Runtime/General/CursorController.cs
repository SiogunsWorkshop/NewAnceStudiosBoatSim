using UnityEngine;

namespace NewAnceBoatSimulation.Runtime.General
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private bool showCursorOnStart = true;

        private void Awake()
        {
            if (showCursorOnStart)
            {
                CursorManagement.ShowCursor();
            }
            else
            {
                CursorManagement.HideCursor();
            }
        }
    }
}