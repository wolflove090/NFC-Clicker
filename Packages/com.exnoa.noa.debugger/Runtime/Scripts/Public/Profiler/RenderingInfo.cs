using System.Collections.Generic;

namespace NoaDebugger
{
    /// <summary>
    /// Rendering process information.
    /// </summary>
    public sealed class RenderingInfo
    {
        /// <summary>
        /// Number of SetPassCalls for the most recent frame.
        /// </summary>
        public long CurrentSetPassCalls { private set; get; }

        /// <summary>
        /// Max number of SetPassCalls during measurement.
        /// </summary>
        public long MaxSetPassCalls { private set; get; } = -1;

        /// <summary>
        /// Max number of SetPassCalls during measurement (converted to string).
        /// </summary>
        internal string MaxSetPassCallsStr { private set; get; } = "0";

        /// <summary>
        /// Number DrawCalls sent in the most recent frame.
        /// </summary>
        public long CurrentDrawCalls { private set; get; }

        /// <summary>
        /// Max number of DrawCalls sent during measurement.
        /// </summary>
        public long MaxDrawCalls { private set; get; } = -1;

        /// <summary>
        /// Max number of DrawCalls sent during measurement (converted to string).
        /// </summary>
        internal string MaxDrawCallsStr { private set; get; } = "0";

        /// <summary>
        /// Number of batches processed in the most recent frame.
        /// </summary>
        public long CurrentBatches { private set; get; }

        /// <summary>
        /// Max number of batches processed during measurement.
        /// </summary>
        public long MaxBatches { private set; get; } = -1;

        /// <summary>
        /// Max number of batches processed during measurement (converted to string).
        /// </summary>
        internal string MaxBatchesStr { private set; get; } = "0";

        /// <summary>
        /// Number of triangles processed in the most recent frame.
        /// </summary>
        public long CurrentTriangles { private set; get; }

        /// <summary>
        /// Max number of triangles processed during measurement.
        /// </summary>
        public long MaxTriangles { private set; get; } = -1;

        /// <summary>
        /// Max number of triangles processed during measurement (converted to string).
        /// </summary>
        internal string MaxTrianglesStr { private set; get; } = "0";

        /// <summary>
        /// Number of vertices processed in the most recent frame.
        /// </summary>
        public long CurrentVertices { private set; get; }

        /// <summary>
        /// Max number of vertices processed during measurement.
        /// </summary>
        public long MaxVertices { private set; get; } = -1;

        /// <summary>
        /// Max number of vertices processed during measurement (converted to string).
        /// </summary>
        internal string MaxVerticesStr { private set; get; } = "0";

        /// <summary>
        /// Whether measurement is ongoing.
        /// </summary>
        public bool IsProfiling { private set; get; }

        /// <summary>
        /// History of current values (for graph display).
        /// </summary>
        internal RingBuffer<float[]> CurrentValueHistory { private set; get; }

        List<float[]> _currentValueHistoryValueBuffer = null;

        /// <summary>
        /// The target of the graph display.
        /// </summary>
        internal RenderingGraphTarget GraphTarget { private set; get; }

        /// <summary>
        /// Whether the graph is being displayed.
        /// </summary>
        internal bool IsGraphShowing { private set; get; }

        /// <summary>
        /// Whether the measurement has started at least once.
        /// </summary>
        internal bool IsStartProfiling { private set; get; }

        /// <summary>
        /// Generates a RenderingInfo.
        /// </summary>
        internal RenderingInfo()
        {
            _InitializeHistoryBuffer();
        }

        /// <summary>
        /// Resets the measured value.
        /// </summary>
        internal void ResetProfiledValue()
        {
            CurrentSetPassCalls = 0;
            MaxSetPassCalls = -1;
            MaxSetPassCallsStr = "0";
            CurrentDrawCalls = 0;
            MaxDrawCalls = -1;
            MaxDrawCallsStr = "0";
            CurrentBatches = 0;
            MaxBatches = -1;
            MaxBatchesStr = "0";
            CurrentTriangles = 0;
            MaxTriangles = -1;
            MaxTrianglesStr = "0";
            CurrentVertices = 0;
            MaxVertices = -1;
            MaxVerticesStr = "0";
        }

        /// <summary>
        /// Updates rendering information.
        /// </summary>
        /// <param name="currentSetPassCalls">Specifies the current setPassCalls.</param>
        /// <param name="currentDrawCalls">Specifies the current drawCalls.</param>
        /// <param name="currentBatches">Specifies the current batches.</param>
        /// <param name="currentTriangles">Specifies the current triangles.</param>
        /// <param name="currentVertices">Specifies the current vertices.</param>
        internal void RefreshCurrent(long currentSetPassCalls, long currentDrawCalls, long currentBatches,
                                     long currentTriangles, long currentVertices)
        {
            CurrentSetPassCalls = currentSetPassCalls;

            if (MaxSetPassCalls < currentSetPassCalls)
            {
                MaxSetPassCalls = currentSetPassCalls;
                MaxSetPassCallsStr = currentSetPassCalls.ToString();
            }

            CurrentDrawCalls = currentDrawCalls;

            if (MaxDrawCalls < currentDrawCalls)
            {
                MaxDrawCalls = currentDrawCalls;
                MaxDrawCallsStr = currentDrawCalls.ToString();
            }

            CurrentBatches = currentBatches;

            if (MaxBatches < currentBatches)
            {
                MaxBatches = currentBatches;
                MaxBatchesStr = currentBatches.ToString();
            }

            CurrentTriangles = currentTriangles;

            if (MaxTriangles < currentTriangles)
            {
                MaxTriangles = currentTriangles;
                MaxTrianglesStr = currentTriangles.ToString();
            }

            CurrentVertices = currentVertices;

            if (MaxVertices < currentVertices)
            {
                MaxVertices = currentVertices;
                MaxVerticesStr = currentVertices.ToString();
            }

            if (IsGraphShowing)
            {
                int valueIndex = CurrentValueHistory.Tail;

                _currentValueHistoryValueBuffer[valueIndex][0] = GraphTarget switch
                {
                    RenderingGraphTarget.SetPassCalls => CurrentSetPassCalls,
                    RenderingGraphTarget.DrawCalls => CurrentDrawCalls,
                    RenderingGraphTarget.Batches => CurrentBatches,
                    RenderingGraphTarget.Triangles => CurrentTriangles,
                    RenderingGraphTarget.Vertices => CurrentVertices,
                    _ => 0
                };

                float[] newValue = _currentValueHistoryValueBuffer[valueIndex];
                CurrentValueHistory.Append(newValue);
            }
        }

        /// <summary>
        /// Starts measurement.
        /// </summary>
        internal void StartProfiling()
        {
            IsStartProfiling = true;
        }

        /// <summary>
        /// Changes the state of measurement.
        /// </summary>
        /// <param name="isProfiling">Specifies the state of measurement.</param>
        internal void ToggleProfiling(bool isProfiling)
        {
            IsProfiling = isProfiling;
        }

        /// <summary>
        /// Changes the state of graph display.
        /// </summary>
        /// <param name="isShowing">Specifies the state of graph display.</param>
        internal void ToggleGraphShowing(bool isShowing)
        {
            IsGraphShowing = isShowing;

            if (!isShowing)
            {
                CurrentValueHistory.Clear();
            }
        }

        /// <summary>
        /// Changes the display target of the rendering load graph.
        /// </summary>
        /// <param name="target">Specifies the target to change the display.</param>
        internal void SwitchGraphTarget(RenderingGraphTarget target)
        {
            if (target != GraphTarget)
            {
                _InitializeHistoryBuffer();
                GraphTarget = target;
            }
        }

        /// <summary>
        /// Initializes the history buffer.
        /// </summary>
        void _InitializeHistoryBuffer()
        {
            int renderingHistoryCapacity = NoaDebuggerDefine.ProfilerChartHistoryCount;
            CurrentValueHistory = new RingBuffer<float[]>(renderingHistoryCapacity);
            _currentValueHistoryValueBuffer = new List<float[]>(renderingHistoryCapacity);

            for (var i = 0; i < _currentValueHistoryValueBuffer.Capacity; ++i)
            {
                _currentValueHistoryValueBuffer.Add(new float[1]);
            }
        }
    }
}
