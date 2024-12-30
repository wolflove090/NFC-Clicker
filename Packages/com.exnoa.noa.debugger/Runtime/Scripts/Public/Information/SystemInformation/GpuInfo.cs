using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// GPU Information
    /// </summary>
    public sealed class GpuInfo
    {
        /// <summary>
        /// Device's GPU name
        /// </summary>
        public string DeviceName { private set; get; }

        /// <summary>
        /// Graphics API type supported by the GPU
        /// </summary>
        public string DeviceType { private set; get; }

        /// <summary>
        /// The graphics API type and driver version supported by the GPU
        /// </summary>
        public string DeviceVersion { private set; get; }

        /// <summary>
        /// GPU's vendor
        /// </summary>
        public string DeviceVendor { private set; get; }

        /// <summary>
        /// GPU memory capacity
        /// </summary>
        public float DeviceSizeGB { private set; get; }

        /// <summary>
        /// Generates GpuInfo
        /// </summary>
        public GpuInfo()
        {
            DeviceName = SystemInfo.graphicsDeviceName;
            DeviceType = SystemInfo.graphicsDeviceType.ToString();
            DeviceVersion = SystemInfo.graphicsDeviceVersion;
            DeviceVendor = SystemInfo.graphicsDeviceVendor;
            DeviceSizeGB = (float)SystemInfo.graphicsMemorySize / 1024;
        }
    }
}
