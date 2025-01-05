using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class LogSwitchToggleDrawer : MonoBehaviour
    {
        [SerializeField, Header("Log type display toggle")]
        LogTypeToggle _messageLogToggle;
        [SerializeField]
        LogTypeToggle _warningLogToggle;
        [SerializeField]
        LogTypeToggle _errorLogToggle;

        public event UnityAction<LogType, bool> OnSwitchByLogType;

        void _OnValidateUI()
        {
            Assert.IsNotNull(_messageLogToggle);
            Assert.IsNotNull(_warningLogToggle);
            Assert.IsNotNull(_errorLogToggle);
        }

        void Awake()
        {
            _OnValidateUI();

            _messageLogToggle.Listener = _OnSwitchMessage;
            _warningLogToggle.Listener = _OnSwitchWarning;
            _errorLogToggle.Listener = _OnSwitchError;
        }

        public void Draw(LogViewLinker.LogTypeToggles linker)
        {
            _messageLogToggle.IsOn = linker._messageToggle;
            _messageLogToggle.LogCount = linker._messageNum;
            _warningLogToggle.IsOn = linker._warningToggle;
            _warningLogToggle.LogCount = linker._warningNum;
            _errorLogToggle.IsOn = linker._errorToggle;
            _errorLogToggle.LogCount = linker._errorNum;
        }

        void _OnSwitchMessage(bool isOn)
        {
            OnSwitchByLogType?.Invoke(LogType.Log, isOn);
        }

        void _OnSwitchWarning(bool isOn)
        {
            OnSwitchByLogType?.Invoke(LogType.Warning, isOn);
        }

        void _OnSwitchError(bool isOn)
        {
            OnSwitchByLogType?.Invoke(LogType.Error, isOn);
        }

        void OnDestroy()
        {
            _messageLogToggle = default;
            _warningLogToggle = default;
            _errorLogToggle = default;
            OnSwitchByLogType = default;
        }
    }
}
