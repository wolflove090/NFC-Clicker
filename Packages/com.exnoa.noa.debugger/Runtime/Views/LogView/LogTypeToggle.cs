using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class LogTypeToggle : MonoBehaviour
    {
        [SerializeField, Tooltip("The show/hide toggle button.")]
        ToggleButtonBase _toggle;

        [SerializeField, Tooltip("The text component displayed when the log entry is hidden.")]
        TextMeshProUGUI _logCountOnDisabled;

        [SerializeField, Tooltip("The text component displayed when the log entry is shown.")]
        TextMeshProUGUI _logCountOnEnabled;

        public int LogCount
        {
            set
            {
                var text = $"{value}";
                _logCountOnDisabled.text = text;
                _logCountOnEnabled.text = text;
            }
        }

        public bool IsOn
        {
            set => _toggle.Init(value);
        }

        public UnityAction<bool> Listener
        {
            set
            {
                _toggle._onClick.RemoveAllListeners();
                _toggle._onClick.AddListener(value);
            }
        }

        void OnDestroy()
        {
            _toggle = default;
            _logCountOnDisabled = default;
            _logCountOnEnabled = default;
        }
    }
}
