namespace NoaDebugger
{
    /// <summary>
    /// Rendering information that does not change.
    /// </summary>
    public sealed class RenderingUnchangingInfo
    {
        /// <summary>
        /// Number of SetPassCalls for the most recent frame.
        /// </summary>
        public long CurrentSetPassCalls { get; }

        /// <summary>
        /// Max number of SetPassCalls during measurement.
        /// </summary>
        public long MaxSetPassCalls { get; }

        /// <summary>
        /// Max number of SetPassCalls during measurement. (converted to string)
        /// </summary>
        internal string MaxSetPassCallsStr { get; }

        /// <summary>
        /// Number DrawCalls sent in the most recent frame.
        /// </summary>
        public long CurrentDrawCalls { get; }

        /// <summary>
        /// Max number of DrawCalls sent during measurement.
        /// </summary>
        public long MaxDrawCalls { get; }

        /// <summary>
        /// Max number of DrawCalls sent during measurement. (converted to string)
        /// </summary>
        internal string MaxDrawCallsStr { get; }

        /// <summary>
        /// Number of batches processed in the most recent frame.
        /// </summary>
        public long CurrentBatches { get; }

        /// <summary>
        /// Max number of batches processed during measurement.
        /// </summary>
        public long MaxBatches { get; }

        /// <summary>
        /// Max number of batches processed during measurement. (converted to string)
        /// </summary>
        internal string MaxBatchesStr { get; }

        /// <summary>
        /// Number of triangles processed in the most recent frame.
        /// </summary>
        public long CurrentTriangles { get; }

        /// <summary>
        /// Max number of triangles processed during measurement.
        /// </summary>
        public long MaxTriangles { get; }

        /// <summary>
        /// Max number of triangles processed during measurement. (converted to string)
        /// </summary>
        internal string MaxTrianglesStr { get; }

        /// <summary>
        /// Number of vertices processed in the most recent frame.
        /// </summary>
        public long CurrentVertices { get; }

        /// <summary>
        /// Max number of vertices processed during measurement.
        /// </summary>
        public long MaxVertices { get; }

        /// <summary>
        /// Max number of vertices processed during measurement. (converted to string)
        /// </summary>
        internal string MaxVerticesStr { get; }

        /// <summary>
        /// Whether rendering measurement is ongoing.
        /// </summary>
        public bool IsProfiling { get; }

        /// <summary>
        /// History of memory consumption (for graph display).
        /// </summary>
        internal RingBuffer<float[]> CurrentValueHistory { get; }

        /// <summary>
        /// The target of the graph display.
        /// </summary>
        internal RenderingGraphTarget GraphTarget { get; }

        /// <summary>
        /// Whether the graph is being displayed.
        /// </summary>
        internal bool IsGraphShowing { get; }

        /// <summary>
        /// Whether to display hyphen.
        /// </summary>
        internal bool IsViewHyphen { get; }

        /// <summary>
        /// Generates a RenderingUnchangingInfo.
        /// </summary>
        /// <param name="info">Specifies the information to be referenced.</param>
        internal RenderingUnchangingInfo(RenderingInfo info)
        {
            CurrentSetPassCalls = info.CurrentSetPassCalls;
            MaxSetPassCalls = info.MaxSetPassCalls;
            MaxSetPassCallsStr = info.MaxSetPassCallsStr;
            CurrentDrawCalls = info.CurrentDrawCalls;
            MaxDrawCalls = info.MaxDrawCalls;
            MaxDrawCallsStr = info.MaxDrawCallsStr;
            CurrentBatches = info.CurrentBatches;
            MaxBatches = info.MaxBatches;
            MaxBatchesStr = info.MaxBatchesStr;
            CurrentTriangles = info.CurrentTriangles;
            MaxTriangles = info.MaxTriangles;
            MaxTrianglesStr = info.MaxTrianglesStr;
            CurrentVertices = info.CurrentVertices;
            MaxVertices = info.MaxVertices;
            MaxVerticesStr = info.MaxVerticesStr;
            IsProfiling = info.IsProfiling;
            CurrentValueHistory = info.CurrentValueHistory;
            GraphTarget = info.GraphTarget;
            IsGraphShowing = info.IsGraphShowing;

            IsViewHyphen = !info.IsProfiling && !info.IsStartProfiling;
        }
    }
}
