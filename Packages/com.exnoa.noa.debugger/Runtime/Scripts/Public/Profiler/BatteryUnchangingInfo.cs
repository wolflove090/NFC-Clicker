namespace NoaDebugger
{
    /// <summary>
    /// Battery information that cannot be updated
    /// </summary>
    public sealed class BatteryUnchangingInfo
    {
        /// <summary>
        /// Battery measurement situation
        /// </summary>
        public BatteryStatus Status { get; }

        /// <summary>
        /// Most recent battery level
        /// </summary>
        public float CurrentLevelPercent { get; }

        /// <summary>
        /// Battery consumption per minute
        /// </summary>
        public float ConsumptionPerMinute { get; }

        /// <summary>
        /// Estimated operation time in seconds
        /// </summary>
        public int OperatingTimeSec { get; }

        /// <summary>
        /// Whether the battery is being measured or not
        /// </summary>
        public bool IsProfiling { get; }

        /// <summary>
        /// Whether to display a hyphen
        /// </summary>
        internal bool IsViewHyphen { get; }

        /// <summary>
        /// Generate BatteryUnchangingInfo
        /// </summary>
        /// <param name="info">Specifies the information to be referred to</param>
        internal BatteryUnchangingInfo(BatteryInfo info)
        {
            bool isViewHyphen = !info.IsProfiling;
            isViewHyphen &= !info.IsStartProfiling;

            isViewHyphen &= BatteryModel.CanProfiling();

            Status = info.Status;
            CurrentLevelPercent = info.CurrentLevelPercent;
            ConsumptionPerMinute = info.ConsumptionPerMinute;
            OperatingTimeSec = info.OperatingTimeSec;
            IsProfiling = info.IsProfiling;
            IsViewHyphen = isViewHyphen;
        }
    }
}
