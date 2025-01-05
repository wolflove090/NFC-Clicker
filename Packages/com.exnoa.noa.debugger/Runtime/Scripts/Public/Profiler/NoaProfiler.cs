namespace NoaDebugger
{
    /// <summary>
    /// Through this class you can retrieve various values of the Profiler function.
    /// </summary>
    public class NoaProfiler
    {
        /// <summary>
        /// Returns the Profiler information being held.
        /// </summary>
        public static ProfilerInfo ProfilerInfo => NoaProfiler._GetProfilerInfo();

        /// <summary>
        /// Returns the most recent FPS information.
        /// </summary>
        public static FpsInfo LatestFpsInfo => _GetLatestFpsInfo();

        /// <summary>
        /// Returns the most recent Memory information.
        /// </summary>
        public static MemoryInfo LatestMemoryInfo => _GetLatestMemoryInfo();

        /// <summary>
        /// Returns the most recent Rendering information.
        /// </summary>
        public static RenderingInfo LatestRenderingInfo => _GetLatestRenderingInfo();

        /// <summary>
        /// Returns the most recent Battery information.
        /// </summary>
        public static BatteryInfo LatestBatteryInfo => _GetLatestBatteryInfo();

        /// <summary>
        /// Returns the most recent Thermal information.
        /// </summary>
        public static ThermalInfo LatestThermalInfo => _GetLatestThermalInfo();

        /// <summary>
        /// The FPS measurement status.
        /// </summary>
        public static bool IsFpsProfiling
        {
            get => _IsFpsProfiling();
            set => _ChangeFpsProfiling(value);
        }

        /// <summary>
        /// The measurement state of Memory.
        /// </summary>
        public static bool IsMemoryProfiling
        {
            get => _IsMemoryProfiling();
            set => _ChangeMemoryProfiling(value);
        }

        /// <summary>
        /// The total memory capacity in MB units. If a negative value is specified, it will be the RAM capacity of the device.
        /// </summary>
        public static float TotalMemoryMB
        {
            get => _GetTotalMemoryMB();
            set => _SetTotalMemoryMB(value);
        }

        /// <summary>
        /// The measurement state of Rendering.
        /// </summary>
        public static bool IsRenderingProfiling
        {
            get => _IsRenderingProfiling();
            set => _ChangeRenderingProfiling(value);
        }

        /// <summary>
        /// The measurement state of Battery.
        /// </summary>
        public static bool IsBatteryProfiling
        {
            get => _IsBatteryProfiling();
            set => _ChangeBatteryProfiling(value);
        }

        /// <summary>
        /// The measurement state of Thermal.
        /// </summary>
        public static bool IsThermalProfiling
        {
            get => _IsThermalProfiling();
            set => _ChangeThermalProfiling(value);
        }

        /// <summary>
        /// Returns the Profiler information being held.
        /// </summary>
        /// <returns>Returns the Profiler information being held.</returns>
        static ProfilerInfo _GetProfilerInfo()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return null;
            }

            return new ProfilerInfo(presenter);
        }


        /// <summary>
        /// Returns the most recent Fps information.
        /// </summary>
        /// <returns>Returns the most recent Fps information.</returns>
        static FpsInfo _GetLatestFpsInfo()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return null;
            }

            return presenter.GetFpsInfo();
        }

        /// <summary>
        /// Returns the FPS measurement state.
        /// </summary>
        /// <returns>Measurement state.</returns>
        static bool _IsFpsProfiling()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return false;
            }

            return presenter.IsFpsProfiling();
        }

        /// <summary>
        /// Changes the FPS measurement state.
        /// </summary>
        /// <param name="isProfiling">Specifies the measurement state you want to change to.</param>
        static void _ChangeFpsProfiling(bool isProfiling)
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return;
            }

            presenter.ChangeFpsProfiling(isProfiling);
        }


        /// <summary>
        /// Returns the most recent Memory information.
        /// </summary>
        /// <returns>Returns the most recent Memory information.</returns>
        static MemoryInfo _GetLatestMemoryInfo()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return null;
            }

            return presenter.GetMemoryInfo();
        }

        /// <summary>
        /// Returns the Memory measurement state.
        /// </summary>
        /// <returns>Measurement state.</returns>
        static bool _IsMemoryProfiling()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return false;
            }

            return presenter.IsMemoryProfiling();
        }

        /// <summary>
        /// Changes the Memory measurement state.
        /// </summary>
        /// <param name="isProfiling">Specifies the measurement state you want to change to.</param>
        static void _ChangeMemoryProfiling(bool isProfiling)
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return;
            }

            presenter.ChangeMemoryProfiling(isProfiling);
        }

        /// <summary>
        /// Returns total memory capacity.
        /// </summary>
        /// <returns>Total memory capacity (in MB). -1 if not available.</returns>
        static float _GetTotalMemoryMB()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return -1;
            }

            return presenter.GetTotalMemoryMB();
        }

        /// <summary>
        /// Sets total memory capacity.
        /// </summary>
        /// <param name="totalMemoryMB">Set the total memory capacity in MB units. If omitted, it becomes the RAM capacity of the device.</param>
        static void _SetTotalMemoryMB(float totalMemoryMB = -1)
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return;
            }

            presenter.SetTotalMemoryMB(totalMemoryMB);
        }


        /// <summary>
        /// Returns the most recent RenderingInfo.
        /// </summary>
        /// <returns>Most recent RenderingInfo.</returns>
        static RenderingInfo _GetLatestRenderingInfo()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return null;
            }

            return presenter.GetRenderingInfo();
        }

        /// <summary>
        /// Returns the measurement state of Rendering.
        /// </summary>
        /// <returns>Measurement state.</returns>
        static bool _IsRenderingProfiling()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return false;
            }

            return presenter.IsRenderingProfiling();
        }

        /// <summary>
        /// Changes the measurement state of Rendering.
        /// </summary>
        /// <param name="isProfiling">Specifies the measurement state you want to change to.</param>
        static void _ChangeRenderingProfiling(bool isProfiling)
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return;
            }

            presenter.ChangeRenderingProfiling(isProfiling);
        }


        /// <summary>
        /// Returns the most recent BatteryInfo.
        /// </summary>
        /// <returns>Most recent BatteryInfo.</returns>
        static BatteryInfo _GetLatestBatteryInfo()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return null;
            }

            return presenter.GetBatteryInfo();
        }

        /// <summary>
        /// Returns the measurement state of Battery.
        /// </summary>
        /// <returns>Measurement state.</returns>
        static bool _IsBatteryProfiling()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return false;
            }

            return presenter.IsBatteryProfiling();
        }

        /// <summary>
        /// Changes the measurement state of Battery.
        /// </summary>
        /// <param name="isProfiling">Specifies the measurement state you want to change to.</param>
        static void _ChangeBatteryProfiling(bool isProfiling)
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return;
            }

            presenter.ChangeBatteryProfiling(isProfiling);
        }


        /// <summary>
        /// Returns the most recent ThermalInfo.
        /// </summary>
        /// <returns>Most recent ThermalInfo.</returns>
        static ThermalInfo _GetLatestThermalInfo()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return null;
            }

            return presenter.GetThermalInfo();
        }

        /// <summary>
        /// Returns the measurement state of Thermal.
        /// </summary>
        /// <returns>Measurement state.</returns>
        static bool _IsThermalProfiling()
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return false;
            }

            return presenter.IsThermalProfiling();
        }

        /// <summary>
        /// Changes the measurement state of Thermal.
        /// </summary>
        /// <param name="isProfiling">Specifies the measurement state you want to change to.</param>
        static void _ChangeThermalProfiling(bool isProfiling)
        {
            ProfilerPresenter presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();

            if (presenter == null)
            {
                return;
            }

            presenter.ChangeThermalProfiling(isProfiling);
        }
    }
}
