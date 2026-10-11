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

        public void UpdateSailRotationInfo(float sailTurnDegree) // Wrapper for inspector UnityEvents
        {
            TryUpdateSailRotationInfo(sailTurnDegree);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public bool TryUpdateSailRotationInfo(float sailTurnDegree)
        {
            if (_sailTurnLabel == null)
            {
                return false;
            }

            _sailTurnLabel.text = string.Format(sailTurnMessageFormat, sailTurnDegree);
            return true;
        }

        public void UpdateSailOpenInfo(float sailOpenPercentage) // Wrapper for inspector UnityEvents
        {
            TryUpdateSailOpenInfo(sailOpenPercentage);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public bool TryUpdateSailOpenInfo(float sailOpenPercentage)
        {
            if (_sailOpenLabel == null)
            {
                return false;
            }

            _sailOpenLabel.text = string.Format(sailOpenMessageFormat, sailOpenPercentage);
            return true;
        }

        private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            _sailOpenLabel = rootElement.Q<Label>(sailOpenLabelName);
            _sailTurnLabel = rootElement.Q<Label>(sailTurnLabelName);

            if (version != 1) return;
            UpdateSailOpenInfo(0f);
            UpdateSailRotationInfo(0f);
        }
    }
}