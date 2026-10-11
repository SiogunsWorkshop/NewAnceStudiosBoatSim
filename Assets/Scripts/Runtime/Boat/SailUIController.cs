using UnityEngine;
using UnityEngine.UIElements;

namespace NewAnceBoatSimulation.Runtime.Boat
{
    public class SailUIController : MonoBehaviour
    {
        [SerializeField] private PanelRenderer sailInfoPanelRenderer;

        [Space, SerializeField] private string sailOpenLabelName = "SailOpenPercentage";
        [SerializeField] private string sailTurnLabelName = "SailTurnDegree";

        [Space, SerializeField] private string sailTurnMessageFormat = "Sail Turn: {0}°";
        [SerializeField] private string sailOpenMessageFormat = "Sail Open: {0}%";

        private Label _sailOpenLabel;
        private Label _sailTurnLabel;

        private void OnEnable()
        {
            sailInfoPanelRenderer.RegisterUIReloadCallback(OnUIReload);
        }

        private void OnDisable()
        {
            sailInfoPanelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }

        private void Reset()
        {
            sailInfoPanelRenderer = GetComponent<PanelRenderer>();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void UpdateSailRotationInfo(float sailTurnDegree)
        {
            if (_sailTurnLabel == null)
            {
                Debug.LogWarning(Messages.Warnings.SailTurnLabelNotFound);
                return;
            }

            _sailTurnLabel.text = string.Format(sailTurnMessageFormat, sailTurnDegree);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void UpdateSailOpenInfo(float sailOpenPercentage)
        {
            if (_sailOpenLabel == null)
            {
                Debug.LogWarning(Messages.Warnings.SailOpenLabelNotFound);
                return;
            }

            _sailOpenLabel.text = string.Format(sailOpenMessageFormat, sailOpenPercentage);
        }

        private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            _sailOpenLabel = rootElement.Q<Label>(sailOpenLabelName);
            _sailTurnLabel = rootElement.Q<Label>(sailTurnLabelName);

            if (version != 1) return;
            UpdateSailOpenInfo(0f);
            UpdateSailRotationInfo(0f);
        }

        private static class Messages
        {
            public static class Warnings
            {
                public const string SailOpenLabelNotFound = "Sail Open Label not found in the UI.";
                public const string SailTurnLabelNotFound = "Sail Turn Label not found in the UI.";
            }
        }
    }
}