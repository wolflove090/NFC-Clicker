using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Device temperature information.
    /// </summary>
    public sealed class ThermalInfo
    {
        /// <summary>
        /// Current device temperature.
        /// </summary>
        public float CurrentTemperature { private set; get; } = -1;

        /// <summary>
        /// Current device status.
        /// </summary>
        public ThermalStatus CurrentThermalStatus { private set; get; }

        /// <summary>
        /// Maximum device temperature.
        /// </summary>
        public float MaxTemperature { private set; get; } = -1;

        /// <summary>
        /// Maximum device status.
        /// </summary>
        public ThermalStatus MaxThermalStatus { private set; get; }

        /// <summary>
        /// Minimum device temperature.
        /// </summary>
        public float MinTemperature { private set; get; } = float.MaxValue;

        /// <summary>
        /// Minimum device status.
        /// </summary>
        public ThermalStatus MinThermalStatus { private set; get; } = ThermalStatus.Critical;

        /// <summary>
        /// Average temperature.
        /// </summary>
        public float AverageTemperature { private set; get; } = -1;

        /// <summary>
        /// Whether measurement is ongoing.
        /// </summary>
        public bool IsProfiling { private set; get; }

        /// <summary>
        /// Whether measurement has started at least once.
        /// </summary>
        internal bool IsStartProfiling { private set; get; }

        /// <summary>
        /// Updating device temperature.
        /// </summary>
        /// <param name="currentTemperature">Specify the current device temperature.</param>
        /// <param name="averageTemperature">Specify the average device temperature.</param>
        internal void RefreshTemperature(float currentTemperature, float averageTemperature)
        {
            var maxTemp = Mathf.Max(currentTemperature, MaxTemperature);

            var minTemp = Mathf.Min(currentTemperature, MinTemperature);
            CurrentTemperature = currentTemperature;
            MaxTemperature = maxTemp;
            MinTemperature = minTemp;
            AverageTemperature = averageTemperature;
        }

        /// <summary>
        /// Updating device temperature status.
        /// </summary>
        /// <param name="currentStatus">Specify the current status.</param>
        internal void RefreshThermalStatus(ThermalStatus currentStatus)
        {
            int maxThermalState = Mathf.Max((int)currentStatus, (int)MaxThermalStatus);

            int minThermalState = Mathf.Min((int)currentStatus, (int)MinThermalStatus);
            CurrentThermalStatus = currentStatus;
            MaxThermalStatus = (ThermalStatus)maxThermalState;
            MinThermalStatus = (ThermalStatus)minThermalState;
        }

        /// <summary>
        /// Starts measurement.
        /// </summary>
        internal void StartProfiling()
        {
            IsStartProfiling = true;
        }

        /// <summary>
        /// Changes the state of measurement.
        /// </summary>
        /// <param name="isProfiling">Specify the state of measurement.</param>
        internal void SetIsProfiling(bool isProfiling)
        {
            IsProfiling = isProfiling;
        }
    }
}
