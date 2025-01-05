namespace NoaDebugger
{
    /// <summary>
    /// Battery information
    /// </summary>
    public sealed class BatteryInfo
    {
        /// <summary>
        /// Battery measurement situation
        /// </summary>
        public BatteryStatus Status { private set; get; } = BatteryStatus.Unknown;

        /// <summary>
        /// Most recent battery level
        /// </summary>
        public float CurrentLevelPercent { private set; get; }

        /// <summary>
        /// Battery consumption per minute
        /// </summary>
        public float ConsumptionPerMinute { private set; get; }

        /// <summary>
        /// Estimated runtime in seconds
        /// </summary>
        public int OperatingTimeSec { private set; get; }

        /// <summary>
        /// Whether it is being measured or not
        /// </summary>
        public bool IsProfiling { private set; get; }

        /// <summary>
        /// Whether profiling has started at least once
        /// </summary>
        internal bool IsStartProfiling { private set; get; }

        /// <summary>
        /// Updates battery information
        /// </summary>
        /// <param name="batteryStatus"></param>
        /// <param name="currentLevelPercent"></param>
        /// <param name="consumptionPerMinute"></param>
        /// <param name="operatingTimeSec"></param>
        internal void Refresh(UnityEngine.BatteryStatus batteryStatus, float currentLevelPercent,
                              float consumptionPerMinute, int operatingTimeSec)
        {
            CurrentLevelPercent = currentLevelPercent;
            ConsumptionPerMinute = consumptionPerMinute;
            OperatingTimeSec = operatingTimeSec;

            if (StatusCheck(batteryStatus) == BatteryStatus.Charging)
            {
                ConsumptionPerMinute = 0;
                OperatingTimeSec = 0;
            }
        }

        /// <summary>
        /// Starts measuring
        /// </summary>
        internal void StartProfiling()
        {
            IsStartProfiling = true;
        }

        /// <summary>
        /// Changes the measurement situation
        /// </summary>
        /// <param name="isProfiling">Specifies the measurement situation</param>
        internal void SetIsProfiling(bool isProfiling)
        {
            IsProfiling = isProfiling;
        }

        /// <summary>
        /// Status update
        /// </summary>
        /// <param name="batteryStatus">Battery status</param>
        internal void StatusUpdate(UnityEngine.BatteryStatus batteryStatus)
        {
            Status = StatusCheck(batteryStatus);
        }

        /// <summary>
        /// Returns the appropriate status from the current situation
        /// </summary>
        /// <param name="batteryStatus">Battery status</param>
        /// <returns>Returns the battery measurement status</returns>
        internal BatteryStatus StatusCheck(UnityEngine.BatteryStatus batteryStatus)
        {
            if (batteryStatus == UnityEngine.BatteryStatus.Unknown)
            {
                return BatteryStatus.Unknown;
            }

            if (batteryStatus == UnityEngine.BatteryStatus.Charging || batteryStatus == UnityEngine.BatteryStatus.Full)
            {
                return BatteryStatus.Charging;
            }

            if (ConsumptionPerMinute <= 0)
            {
                return BatteryStatus.Profiling;
            }

            return BatteryStatus.Default;
        }
    }
}
