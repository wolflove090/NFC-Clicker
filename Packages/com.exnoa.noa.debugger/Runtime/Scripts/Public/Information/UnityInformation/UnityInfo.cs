using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Application Information
    /// </summary>
    public sealed class UnityInfo
    {
        /// <summary>
        /// Unity version information
        /// </summary>
        public string Version { private set; get; }

        /// <summary>
        /// Whether it's built in Debug mode
        /// </summary>
        public bool Debug { private set; get; }

        /// <summary>
        /// Whether it's built with IL2CPP
        /// </summary>
        public bool IL2CPP { private set; get; }

        /// <summary>
        /// The number of vertical sync waits between frames.
        /// </summary>
        public int VSyncCount { private set; get; }

        /// <summary>
        /// Target frame rate
        /// </summary>
        public int TargetFrameRate { private set; get; }

        /// <summary>
        /// Generates UnityInfo
        /// </summary>
        internal UnityInfo()
        {
            Version = Application.unityVersion;
            Debug = UnityEngine.Debug.isDebugBuild;
#if ENABLE_IL2CPP
            IL2CPP = true;
#else
            IL2CPP = false;
#endif
            VSyncCount = QualitySettings.vSyncCount;
            TargetFrameRate = Application.targetFrameRate;
        }

        /// <summary>
        /// Updates the Unity information
        /// </summary>
        internal void Refresh()
        {
            VSyncCount = QualitySettings.vSyncCount;
            TargetFrameRate = Application.targetFrameRate;
        }
    }
}
