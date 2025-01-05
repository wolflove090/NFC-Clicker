using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// FPS information that cannot be updated
    /// </summary>
    public sealed class FpsUnchangingInfo
    {
        /// <summary>
        /// Most recent FPS
        /// </summary>
        public int CurrentFps { get; }

        /// <summary>
        /// Elapsed time in the most recent frame
        /// </summary>
        public float ElapsedTime { get; }

        /// <summary>
        /// Maximum FPS value while measuring
        /// </summary>
        public int Max { get; }

        /// <summary>
        /// Maximum FPS value while measuring (converted to string)
        /// </summary>
        internal string MaxStr { get; }

        /// <summary>
        /// Minimum FPS value while measuring
        /// </summary>
        public int Min { get; }

        /// <summary>
        /// Minimum FPS value while measuring (converted to string)
        /// </summary>
        internal string MinStr { get; }

        /// <summary>
        /// Average FPS value while measuring
        /// </summary>
        public int AverageFps { get; }

        /// <summary>
        /// Whether FPS is being measured or not
        /// </summary>
        public bool IsProfiling { get; }

        /// <summary>
        /// Whether to display a hyphen
        /// </summary>
        internal bool IsViewHyphen { get; }

        /// <summary>
        /// Generate FpsUnchangingInfo
        /// </summary>
        /// <param name="info">Specifies the information to be referred to</param>
        internal FpsUnchangingInfo(FpsInfo info)
        {
            CurrentFps = info.CurrentFps;

            ElapsedTime = (Mathf.Ceil(info.ElapsedTime * 10000) / 10);
            Max = info.Max;
            MaxStr = info.MaxStr;
            Min = info.Min;
            MinStr = info.MinStr;

            AverageFps = info.TotalFrames == 0
                ? 0
                : Mathf.FloorToInt(info.TotalFrames / info.TotalSeconds);

            IsProfiling = info.IsProfiling;

            IsViewHyphen = !info.IsProfiling && !info.IsStartProfiling;
        }
    }
}
