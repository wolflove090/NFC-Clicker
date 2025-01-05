namespace NoaDebugger
{
    /// <summary>
    /// FPS information
    /// </summary>
    public sealed class FpsInfo
    {
        /// <summary>
        /// Recent FPS
        /// </summary>
        public int CurrentFps { private set; get; }

        /// <summary>
        /// Elapsed time in the recent frame
        /// </summary>
        public float ElapsedTime { private set; get; }

        /// <summary>
        /// Maximum FPS value while measuring
        /// </summary>
        public int Max { private set; get; } = -1;

        /// <summary>
        /// Maximum FPS value while measuring (converted to string)
        /// </summary>
        internal string MaxStr { private set; get; } = "0";

        /// <summary>
        /// Minimum FPS value during measurement
        /// </summary>
        public int Min { private set; get; } = int.MaxValue;

        /// <summary>
        /// Minimum FPS value during measurement (converted to string)
        /// </summary>
        internal string MinStr { private set; get; } = "0";

        /// <summary>
        /// Total number of processed frames while measuring
        /// </summary>
        public int TotalFrames { private set; get; }

        /// <summary>
        /// Total elapsed time while measuring
        /// </summary>
        public float TotalSeconds { private set; get; }

        /// <summary>
        /// Whether it is being measured or not
        /// </summary>
        public bool IsProfiling { private set; get; }

        /// <summary>
        /// Whether profiling has started at least once
        /// </summary>
        internal bool IsStartProfiling { private set; get; }

        /// <summary>
        /// Updates FPS information
        /// </summary>
        /// <param name="currentFps">Specifies the current FPS</param>
        /// <param name="elapsedTime">Specifies the elapsed time in the most recent frame</param>
        /// <param name="totalFrames">Specifies the total number of processed frames while measuring</param>
        /// <param name="totalSeconds">Specifies the total elapsed time while measuring</param>
        internal void Refresh(int currentFps, float elapsedTime, int totalFrames, float totalSeconds)
        {
            CurrentFps = currentFps;

            if (Max < currentFps)
            {
                Max = currentFps;
                MaxStr = currentFps.ToString();
            }

            if (Min > currentFps)
            {
                Min = currentFps;
                MinStr = currentFps.ToString();
            }

            ElapsedTime = elapsedTime;
            TotalFrames = totalFrames;
            TotalSeconds = totalSeconds;
        }

        /// <summary>
        /// Starts measuring
        /// </summary>
        internal void StartProfiling()
        {
            IsStartProfiling = true;
        }

        /// <summary>
        /// Changes the measurement state
        /// </summary>
        /// <param name="isProfiling">Specifies the measurement state</param>
        internal void SetIsProfiling(bool isProfiling)
        {
            IsProfiling = isProfiling;
        }
    }
}
