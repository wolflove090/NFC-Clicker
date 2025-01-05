namespace NoaDebugger
{
    /// <summary>
    /// Memory information that cannot be updated
    /// </summary>
    public sealed class MemoryUnchangingInfo
    {
        /// <summary>
        /// Total memory capacity
        /// </summary>
        public float TotalMemoryMB { get; }

        /// <summary>
        /// Most recent memory amount
        /// </summary>
        public float CurrentMemoryMB { get; }

        /// <summary>
        /// Maximum memory amount while measuring
        /// </summary>
        public float MaxMemoryMB { get; }

        /// <summary>
        /// Minimum memory amount while measuring
        /// </summary>
        public float MinMemoryMB { get; }

        /// <summary>
        /// Average memory amount while measuring
        /// </summary>
        public float AverageMemoryMB { get; }

        /// <summary>
        /// Whether it is being measured or not
        /// </summary>
        public bool IsProfiling { get; }

        /// <summary>
        /// Memory consumption history (for graph display)
        /// </summary>
        internal RingBuffer<float[]> CurrentMemoryHistory { get; }

        /// <summary>
        /// Whether to show the graph or not
        /// </summary>
        internal bool IsGraphShowing { get; }

        /// <summary>
        /// Whether to display a hyphen
        /// </summary>
        internal bool IsViewHyphen { get; }

        /// <summary>
        /// Generate MemoryUnchangingInfo
        /// </summary>
        /// <param name="info">Specifies the information to be referred to</param>
        internal MemoryUnchangingInfo(MemoryInfo info)
        {
            bool isViewHyphen = !info.IsProfiling;
            isViewHyphen &= !info.IsStartProfiling;
            isViewHyphen &= MemoryModel.CanProfiling(); 
            TotalMemoryMB = info.TotalMemoryMB;
            CurrentMemoryMB = info.CurrentMemoryMB;
            MaxMemoryMB = info.MaxMemoryMB;
            MinMemoryMB = info.MinMemoryMB;
            AverageMemoryMB = info.AverageMemoryMB;
            IsProfiling = info.IsProfiling;
            CurrentMemoryHistory = info.CurrentMemoryHistory;
            IsGraphShowing = info.IsGraphShowing;
            IsViewHyphen = isViewHyphen;
        }
    }
}
