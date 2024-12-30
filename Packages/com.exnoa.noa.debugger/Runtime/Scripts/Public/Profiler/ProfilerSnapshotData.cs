namespace NoaDebugger
{
    /// <summary>
    /// Snapshot data of the Profiler.
    /// </summary>
    public sealed class ProfilerSnapshotData
    {
        /// <summary>
        /// FPS Information.
        /// </summary>
        public FpsUnchangingInfo FpsInfo { get; }

        /// <summary>
        /// Memory Information.
        /// </summary>
        public MemoryUnchangingInfo MemoryInfo { get; }

        /// <summary>
        /// Rendering Information.
        /// </summary>
        public RenderingUnchangingInfo RenderingInfo { get; }

        /// <summary>
        /// Battery Information.
        /// </summary>
        public BatteryUnchangingInfo BatteryInfo { get; }

        /// <summary>
        /// Thermal Information.
        /// </summary>
        public ThermalUnchangingInfo ThermalInfo { get; }

        /// <summary>
        /// Creates the snapshot data.
        /// </summary>
        /// <param name="profiler">Creates snapshot data from ProfilerPresenter.</param>
        internal ProfilerSnapshotData(ProfilerPresenter profiler)
        {
            FpsInfo = ProfilerPresenter.CreateFpsViewInfo(profiler.FpsModel.FpsInfo);
            MemoryInfo = ProfilerPresenter.CreateMemoryViewInfo(profiler.MemoryModel.MemoryInfo);
            RenderingInfo = ProfilerPresenter.CreateRenderingViewInfo(profiler.RenderingModel.RenderingInfo);
            BatteryInfo = ProfilerPresenter.CreateBatteryViewInfo(profiler.BatteryModel.BatteryInfo);
            ThermalInfo = ProfilerPresenter.CreateThermalViewInfo(profiler.ThermalModel.ThermalInfo);
        }
    }
}
