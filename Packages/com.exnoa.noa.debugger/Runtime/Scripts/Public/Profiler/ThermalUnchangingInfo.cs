namespace NoaDebugger
{
    /// <summary>
    /// Temperature information that does not change.
    /// </summary>
    public sealed class ThermalUnchangingInfo
    {
        /// <summary>
        /// Current device temperature.
        /// </summary>
        public float CurrentTemperature { get; }

        /// <summary>
        /// Current device temperature status.
        /// </summary>
        public ThermalStatus CurrentThermalStatus { get; }

        /// <summary>
        /// Maximum device temperature.
        /// </summary>
        public float MaxTemperature { get; }

        /// <summary>
        /// Maximum device temperature status.
        /// </summary>
        public ThermalStatus MaxThermalStatus { get; }

        /// <summary>
        /// Minimum device temperature.
        /// </summary>
        public float MinTemperature { get; }

        /// <summary>
        /// Minimum device temperature status.
        /// </summary>
        public ThermalStatus MinThermalStatus { get; }

        /// <summary>
        /// Average device temperature.
        /// </summary>
        public float AverageTemperature { get; }

        /// <summary>
        /// Whether device is in profiling state.
        /// </summary>
        public bool IsProfiling { get; }

        /// <summary>
        /// Whether to display hyphen.
        /// </summary>
        internal bool IsViewHyphen { get; }

        /// <summary>
        /// Generates a ThermalUnchangingInfo.
        /// </summary>
        /// <param name="info">Specifies the information to be referenced.</param>
        internal ThermalUnchangingInfo(ThermalInfo info)
        {
            bool isViewHyphen = !info.IsProfiling;
            isViewHyphen &= !info.IsStartProfiling;
            isViewHyphen &= ThermalModel.CanProfiling(); 
            CurrentTemperature = info.CurrentTemperature;
            MaxTemperature = info.MaxTemperature;
            MinTemperature = info.MinTemperature;
            AverageTemperature = info.AverageTemperature;
            CurrentThermalStatus = info.CurrentThermalStatus;
            MaxThermalStatus = info.MaxThermalStatus;
            MinThermalStatus = info.MinThermalStatus;
            IsProfiling = info.IsProfiling;
            IsViewHyphen = isViewHyphen;
        }

        /// <summary>
        /// Returns text adapted to the temperature status.
        /// </summary>
        /// <param name="status">Specifies the temperature status.</param>
        /// <returns>Returns the display text for the temperature status.</returns>
        public static string ConvertThermalStatusText(ThermalStatus status)
        {
            return status switch
            {
                ThermalStatus.Nominal => "nominal",
                ThermalStatus.Fair => "fair",
                ThermalStatus.Serious => "serious",
                ThermalStatus.Critical => "critical",
                _ => ""
            };
        }
    }
}
