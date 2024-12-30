using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Device info
    /// </summary>
    public sealed class DeviceInfo
    {
        /// <summary>
        /// The device's Operating System Name
        /// </summary>
        public string OS { private set; get; }

        /// <summary>
        /// The device's model name
        /// </summary>
        public string Model { private set; get; }

        /// <summary>
        /// Device type of the terminal
        /// </summary>
        public string Type { private set; get; }

        /// <summary>
        /// The device's device name
        /// </summary>
        public string Name { private set; get; }

        /// <summary>
        /// Generates DeviceInfo
        /// </summary>
        internal DeviceInfo()
        {
            OS = SystemInfo.operatingSystem;
            Model = SystemInfo.deviceModel;
            Type = SystemInfo.deviceType.ToString();
            Name = SystemInfo.deviceName;
        }
    }
}
