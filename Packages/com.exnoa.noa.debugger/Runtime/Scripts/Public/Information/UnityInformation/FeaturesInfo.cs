using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Features information
    /// </summary>
    public sealed class FeaturesInfo
    {
        /// <summary>
        /// Whether GPS can be used or not
        /// </summary>
        public bool Location { private set; get; }

        /// <summary>
        /// Whether accelerometer can be used or not
        /// </summary>
        public bool Accelerometer { private set; get; }

        /// <summary>
        /// Whether gyroscope can be used or not
        /// </summary>
        public bool Gyroscope { private set; get; }

        /// <summary>
        /// Whether haptic feedback can be used or not
        /// </summary>
        public bool Vibration { private set; get; }

        /// <summary>
        /// Generates FeaturesInfo
        /// </summary>
        internal FeaturesInfo()
        {
            Location = SystemInfo.supportsLocationService;
            Accelerometer = SystemInfo.supportsAccelerometer;
            Gyroscope = SystemInfo.supportsGyroscope;
            Vibration = SystemInfo.supportsVibration;
        }
    }
}
