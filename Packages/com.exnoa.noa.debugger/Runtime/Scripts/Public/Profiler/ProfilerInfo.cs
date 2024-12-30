namespace NoaDebugger
{
    /// <summary>
    /// Holds Profiler information.
    /// </summary>
    public sealed class ProfilerInfo
    {
        /// <summary>
        /// FPS information.
        /// </summary>
        public FpsInfo FpsInfo { private set; get; }

        /// <summary>
        /// Memory information.
        /// </summary>
        public MemoryInfo MemoryInfo { private set; get; }

        /// <summary>
        /// Rendering information.
        /// </summary>
        public RenderingInfo RenderingInfo { private set; get; }

        /// <summary>
        /// Battery information.
        /// </summary>
        public BatteryInfo BatteryInfo { private set; get; }

        /// <summary>
        /// Thermal information.
        /// </summary>
        public ThermalInfo ThermalInfo { private set; get; }

        /// <summary>
        /// Creates ProfilerInfo.
        /// </summary>
        /// <param name="presenter">Specify the presenter being referred to.</param>
        internal ProfilerInfo(ProfilerPresenter presenter)
        {
            FpsInfo = presenter.FpsModel.FpsInfo;
            MemoryInfo = presenter.MemoryModel.MemoryInfo;
            RenderingInfo = presenter.RenderingModel.RenderingInfo;
            BatteryInfo = presenter.BatteryModel.BatteryInfo;
            ThermalInfo = presenter.ThermalModel.ThermalInfo;
        }
    }
}
