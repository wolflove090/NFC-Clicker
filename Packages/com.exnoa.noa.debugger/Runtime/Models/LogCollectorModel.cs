namespace NoaDebugger
{
    class LogCollectorModel : ModelBase
    {
        readonly string _prefKey;

        public bool IsCollecting { private set; get; }

        protected LogCollectorModel(string prefKey = "")
        {
            IsCollecting = string.IsNullOrEmpty(prefKey)
                           || NoaDebuggerPrefs.GetBoolean(prefKey, true);
            _prefKey = prefKey;
            OnCollectToggled();
        }

        public void ToggleCollect(bool isCollecting)
        {
            if (IsCollecting != isCollecting)
            {
                IsCollecting = isCollecting;
                OnCollectToggled();
            }
        }

        protected virtual void OnCollectEnabled() {}

        protected virtual void OnCollectDisabled() {}

        void OnCollectToggled()
        {
            if (IsCollecting)
            {
                OnCollectEnabled();
            }
            else
            {
                OnCollectDisabled();
            }
            if (!string.IsNullOrEmpty(_prefKey))
            {
                NoaDebuggerPrefs.SetBoolean(_prefKey, IsCollecting);
            }
        }

        public void Destroy ()
        {
            OnCollectDisabled();
        }
    }
}
