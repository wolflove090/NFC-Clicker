namespace NoaDebugger
{
    /// <summary>
    /// Holds system information
    /// </summary>
    public sealed class SystemInformation
    {
        /// <summary>
        /// Application information
        /// </summary>
        public ApplicationInfo ApplicationInfo { private set; get; }

        /// <summary>
        /// Device information
        /// </summary>
        public DeviceInfo DeviceInfo { private set; get; }

        /// <summary>
        /// CPU information
        /// </summary>
        public CpuInfo CpuInfo { private set; get; }

        /// <summary>
        /// GPU information
        /// </summary>
        public GpuInfo GpuInfo { private set; get; }

        /// <summary>
        /// Memory information
        /// </summary>
        public SystemMemoryInfo SystemMemoryInfo { private set; get; }

        /// <summary>
        /// Display information
        /// </summary>
        public DisplayInfo DisplayInfo { private set; get; }

        /// <summary>
        /// Generates SystemInformation
        /// </summary>
        /// <param name="model">Specifies the source model for reference</param>
        internal SystemInformation(SystemInformationModel model)
        {
            ApplicationInfo = model.ApplicationInfo;
            DeviceInfo = model.DeviceInfo;
            CpuInfo = model.CpuInfo;
            GpuInfo = model.GpuInfo;
            SystemMemoryInfo = model.SystemMemoryInfo;
            DisplayInfo = model.DisplayInfo;
        }
    }
}
