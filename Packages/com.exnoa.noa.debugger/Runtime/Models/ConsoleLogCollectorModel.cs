using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class ConsoleLogCollectorModel : LogCollectorModel
    {
        public event UnityAction<string, string, UnityEngine.LogType> OnLogReceived;

        public ConsoleLogCollectorModel() : base(NoaDebuggerPrefsDefine.PrefsKeyIsConsoleLogCollecting)
        {
        }

        protected override void OnCollectEnabled()
        {
            Application.logMessageReceived += _ReceiveLog;
        }

        protected override void OnCollectDisabled()
        {
            Application.logMessageReceived -= _ReceiveLog;
        }

        void _ReceiveLog(string logString, string stackTrace, UnityEngine.LogType type)
        {
            OnLogReceived?.Invoke(logString, stackTrace, type);
        }
    }
}
