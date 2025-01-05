using System.Collections.Generic;
using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Memory information
    /// </summary>
    public sealed class MemoryInfo
    {
        /// <summary>
        /// Default total memory capacity (RAM capacity of device)
        /// </summary>
        internal static readonly float DefaultTotalMemoryMB = SystemInfo.systemMemorySize;

        /// <summary>
        /// Total memory capacity (default is the RAM capacity of device).
        /// </summary>
        public float TotalMemoryMB { private set; get; } = MemoryInfo.DefaultTotalMemoryMB;

        /// <summary>
        /// Recent memory consumption
        /// </summary>
        public float CurrentMemoryMB { private set; get; } = -1;

        /// <summary>
        /// Maximum measured memory consumption
        /// </summary>
        public float MaxMemoryMB { private set; get; }

        /// <summary>
        /// Minimum measured memory consumption
        /// </summary>
        public float MinMemoryMB { private set; get; } = float.MaxValue;

        /// <summary>
        /// Average memory consumption
        /// </summary>
        public float AverageMemoryMB { private set; get; }

        /// <summary>
        /// Whether it is being measured or not
        /// </summary>
        public bool IsProfiling { private set; get; }

        /// <summary>
        /// Memory consumption history (for graph display)
        /// </summary>
        internal RingBuffer<float[]> CurrentMemoryHistory { private set; get; }

        List<float[]> _currentMemoryHistoryValueBuffer = null;

        /// <summary>
        /// Whether to display the graph or not
        /// </summary>
        internal bool IsGraphShowing { private set; get; }

        /// <summary>
        /// Whether profiling has started at least once
        /// </summary>
        internal bool IsStartProfiling { private set; get; }

        /// <summary>
        /// Generates MemoryInfo
        /// </summary>
        internal MemoryInfo()
        {
            _InitializeHistoryBuffer();
        }

        /// <summary>
        /// Specifies the total memory capacity.
        /// </summary>
        /// <param name="totalMemoryMB">Specifies the total memory capacity. If omitted, it will be the RAM capacity of the device.</param>
        internal void SetTotalMemoryMB(float totalMemoryMB = -1)
        {
            TotalMemoryMB = (totalMemoryMB < 0)
                ? MemoryInfo.DefaultTotalMemoryMB
                : totalMemoryMB;
        }

        /// <summary>
        /// Resets the measured values.
        /// </summary>
        internal void ResetProfiledValue()
        {
            CurrentMemoryMB = -1;
            MaxMemoryMB = 0;
            MinMemoryMB = float.MaxValue;
            AverageMemoryMB = 0;
        }

        /// <summary>
        /// Updates the memory information
        /// </summary>
        /// <param name="currentMemoryMB">Specifies the current memory usage</param>
        /// <param name="maxMemoryMB">Specifies the maximum memory usage</param>
        /// <param name="minMemoryMB">Specifies the minimum memory usage</param>
        /// <param name="averageMemoryMB">Specifies the average memory usage</param>
        internal void Refresh(float currentMemoryMB, float maxMemoryMB, float minMemoryMB, float averageMemoryMB)
        {
            CurrentMemoryMB = currentMemoryMB;
            MaxMemoryMB = maxMemoryMB;
            MinMemoryMB = minMemoryMB;
            AverageMemoryMB = averageMemoryMB;

            if (IsGraphShowing)
            {
                int valueIndex = CurrentMemoryHistory.Tail;
                _currentMemoryHistoryValueBuffer[valueIndex][0] = CurrentMemoryMB;
                float[] newValue = _currentMemoryHistoryValueBuffer[valueIndex];
                CurrentMemoryHistory.Append(newValue);
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
        /// Changes the measurement state
        /// </summary>
        /// <param name="isProfiling">Specifies the measurement state</param>
        internal void ToggleProfiling(bool isProfiling)
        {
            IsProfiling = isProfiling;
        }

        /// <summary>
        /// Changes the graph display state
        /// </summary>
        /// <param name="isShowing">Specifies the graph display state</param>
        internal void ToggleGraphShowing(bool isShowing)
        {
            IsGraphShowing = isShowing;

            if (!isShowing)
            {
                CurrentMemoryHistory.Clear();
            }
        }

        /// <summary>
        /// Initializes the history buffer
        /// </summary>
        void _InitializeHistoryBuffer()
        {
            int memoryHistoryCapacity = NoaDebuggerDefine.ProfilerChartHistoryCount / MemoryModel.UpdateIntervalFrames;
            CurrentMemoryHistory = new RingBuffer<float[]>(memoryHistoryCapacity);
            _currentMemoryHistoryValueBuffer = new List<float[]>(memoryHistoryCapacity);

            for (var i = 0; i < _currentMemoryHistoryValueBuffer.Capacity; ++i)
            {
                _currentMemoryHistoryValueBuffer.Add(new float[1]);
            }
        }
    }
}
