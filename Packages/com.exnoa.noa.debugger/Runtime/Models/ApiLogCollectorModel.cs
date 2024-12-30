using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class ApiLogCollectorModel : LogCollectorModel
    {
        public event UnityAction<ApiLog> OnLogReceived;

        public ApiLogCollectorModel() : base(NoaDebuggerPrefsDefine.PrefsKeyIsApiLogCollecting)
        {
        }

        protected override void OnCollectEnabled()
        {
            ApiLogger.OnLogReceived += _ReceiveLog;
        }

        protected override void OnCollectDisabled()
        {
            ApiLogger.OnLogReceived -= _ReceiveLog;
        }

        void _ReceiveLog(ApiLog log)
        {
            OnLogReceived?.Invoke(log);
        }
    }
}
