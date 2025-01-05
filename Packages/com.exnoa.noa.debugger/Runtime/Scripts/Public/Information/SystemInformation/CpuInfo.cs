using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// CPU information
    /// </summary>
    public sealed class CpuInfo
    {
        /// <summary>
        /// Device's CPU type
        /// </summary>
        public string Type { private set; get; }

        /// <summary>
        /// The number of CPU cores in the device
        /// </summary>
        public string Count { private set; get; }

        /// <summary>
        /// Generates CpuInfo
        /// </summary>
        internal CpuInfo()
        {
            Type = SystemInfo.processorType;
            Count = SystemInfo.processorCount.ToString();
        }
    }
}
