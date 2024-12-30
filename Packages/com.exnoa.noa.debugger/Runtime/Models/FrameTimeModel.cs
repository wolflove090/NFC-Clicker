using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NoaDebugger
{
    sealed class FrameTimeModel : ModelBase
    {
        static readonly string OnChangedBackgroundState = "FrameTimeModelOnBackgroundStateChanged";

        public FrameTimeInfo FrameTimeInfo { get; }

        public UnityAction OnFrameTimeInfoChanged { get; set; }

        bool _isSuspended;

        bool _isResumed;

        public FrameTimeModel()
        {
            bool isEnabled = NoaDebuggerPrefs.GetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsFrameTimeProfiling, true);
            bool isActive = NoaDebuggerPrefs.GetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsFpsProfiling, true);
            FrameTimeInfo = new FrameTimeInfo(NoaDebuggerDefine.ProfilerChartHistoryCount);
            OnToggleEnabled(isEnabled);
            OnToggleActive(isActive);

            ApplicationBackgroundManager.SetAction(OnChangedBackgroundState, OnBackgroundStateChanged);
#if UNITY_EDITOR
            EditorApplication.pauseStateChanged += OnEditorPauseStateChanged;
#endif
        }

        public void Dispose()
        {
            FrameTimeInfo.ClearHistory();
            FrameTimeMeasurer.GetInstance().OnUpdateFrameTime -= OnUpdateFrameTime;

            ApplicationBackgroundManager.DeleteAction(FrameTimeModel.OnChangedBackgroundState);
#if UNITY_EDITOR
            EditorApplication.pauseStateChanged -= OnEditorPauseStateChanged;
#endif
        }

        public void ToggleEnabled(bool isEnabled)
        {
            NoaDebuggerPrefs.SetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsFrameTimeProfiling, isEnabled);
            OnToggleEnabled(isEnabled);
        }

        public void ToggleActive(bool isActive)
        {
            OnToggleActive(isActive);
        }

        void OnToggleEnabled(bool isEnabled)
        {
            if (FrameTimeInfo.IsEnabled == isEnabled)
            {
                return;
            }

            FrameTimeInfo.IsEnabled = isEnabled;

            if (isEnabled && FrameTimeInfo.IsActive)
            {
                FrameTimeMeasurer.GetInstance().OnUpdateFrameTime += OnUpdateFrameTime;
            }
            else
            {
                FrameTimeInfo.ClearHistory();
                FrameTimeMeasurer.GetInstance().OnUpdateFrameTime -= OnUpdateFrameTime;
            }
        }

        void OnToggleActive(bool isActive)
        {
            if (FrameTimeInfo.IsActive == isActive)
            {
                return;
            }

            FrameTimeInfo.IsActive = isActive;

            if (isActive && FrameTimeInfo.IsEnabled)
            {
                FrameTimeMeasurer.GetInstance().OnUpdateFrameTime += OnUpdateFrameTime;
            }
            else
            {
                FrameTimeMeasurer.GetInstance().OnUpdateFrameTime -= OnUpdateFrameTime;
            }
        }

        void OnUpdateFrameTime(double updateMilliseconds, double renderingMilliseconds, double othersMilliseconds)
        {
            if (_isSuspended)
            {
                return;
            }

            if (_isResumed)
            {
                _isResumed = false;
                return;
            }

            FrameTimeInfo.AddHistory(
                (float)updateMilliseconds,
                (float)renderingMilliseconds,
                (float)othersMilliseconds);

            OnFrameTimeInfoChanged?.Invoke();
        }

        void OnBackgroundStateChanged(bool isBackground)
        {
            _isSuspended = isBackground;
            _isResumed = !isBackground;
        }

#if UNITY_EDITOR
        void OnEditorPauseStateChanged(PauseState state)
        {
            _isSuspended = state == PauseState.Paused;
            _isResumed = state == PauseState.Unpaused;
        }
#endif
    }
}
