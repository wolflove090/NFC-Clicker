using Unity.Profiling;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class RenderingModel : ModelBase
    {
        static readonly string RenderingModelOnUpdate = "RenderingModelOnUpdate";

        ProfilerRecorder _setPassCallsRecorder;
        ProfilerRecorder _drawCallsRecorder;
        ProfilerRecorder _batchesRecorder;
        ProfilerRecorder _trianglesRecorder;
        ProfilerRecorder _verticesRecorder;

        public RenderingInfo RenderingInfo { get; private set; }

        public UnityAction OnRenderingInfoChanged { get; set; }

        public RenderingModel()
        {
            _setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            _drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            _batchesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
            _trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
            _verticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");

            RenderingInfo = new RenderingInfo();
            bool isProfiling = NoaDebuggerPrefs.GetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsRenderProfiling, true);
            bool isGraphShowing = NoaDebuggerPrefs.GetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsRenderGraphShowing, true);
            RenderingInfo.ToggleProfiling(isProfiling);
            RenderingInfo.ToggleGraphShowing(isGraphShowing);
            _HandleOnUpdate(isProfiling);
        }

        public void Dispose()
        {
            _setPassCallsRecorder.Dispose();
            _drawCallsRecorder.Dispose();
            _batchesRecorder.Dispose();
            _trianglesRecorder.Dispose();
            _verticesRecorder.Dispose();

            UpdateManager.DeleteAction(RenderingModelOnUpdate);
        }

        void _OnUpdate()
        {
            _OnUpdateRenderingCheck();
        }

        void _OnUpdateRenderingCheck()
        {
            if (!RenderingInfo.IsProfiling)
            {
                return;
            }

            RenderingInfo.StartProfiling();

            long currentSetPassCalls = _setPassCallsRecorder.LastValue;
            long currentDrawCalls = _drawCallsRecorder.LastValue;
            long currentBatches = _batchesRecorder.LastValue;
            long currentTriangles = _trianglesRecorder.LastValue;
            long currentVertices = _verticesRecorder.LastValue;
            RenderingInfo.RefreshCurrent(currentSetPassCalls, currentDrawCalls, currentBatches, currentTriangles, currentVertices);
            OnRenderingInfoChanged?.Invoke();
        }

        public void ChangeProfilingState(bool isProfiling)
        {
            RenderingInfo.ToggleProfiling(isProfiling);

            NoaDebuggerPrefs.SetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsRenderProfiling, isProfiling);
            _HandleOnUpdate(isProfiling);
        }

        public void ChangeGraphShowingState(bool isGraphShowing)
        {
            RenderingInfo.ToggleGraphShowing(isGraphShowing);

            NoaDebuggerPrefs.SetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsRenderGraphShowing, isGraphShowing);
        }

        public void SwitchGraphTarget(RenderingGraphTarget target)
        {
            RenderingInfo.SwitchGraphTarget(target);
        }

        void _HandleOnUpdate(bool isProfiling)
        {
            string key = RenderingModel.RenderingModelOnUpdate;

            if (isProfiling)
            {
                if (UpdateManager.ContainsKey(key))
                {
                    return;
                }

                _setPassCallsRecorder.Start();
                _drawCallsRecorder.Start();
                _batchesRecorder.Start();
                _trianglesRecorder.Start();
                _verticesRecorder.Start();

                RenderingInfo.ResetProfiledValue();

                UpdateManager.SetAction(key, _OnUpdate);
            }
            else
            {
                _setPassCallsRecorder.Stop();
                _drawCallsRecorder.Stop();
                _batchesRecorder.Stop();
                _trianglesRecorder.Stop();
                _verticesRecorder.Stop();

                UpdateManager.DeleteAction(key);
            }
        }
    }
}
