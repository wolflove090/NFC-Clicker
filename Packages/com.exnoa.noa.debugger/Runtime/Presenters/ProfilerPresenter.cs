using System;
using UnityEngine;

namespace NoaDebugger
{
    sealed class ProfilerPresenter : NoaDebuggerToolBase, INoaDebuggerTool
    {
        [Header("MainView")]
        [SerializeField]
        ProfilerView _mainViewPrefab;
        ProfilerView _mainView;

        [Header("FloatingWindowView")]
        [SerializeField]
        ProfilerFloatingWindowView _floatingWindowPrefab;
        FloatingWindowPresenter<ProfilerFloatingWindowView, ProfilerViewLinker> _floatingWindowPresenter;

        public ToolNotificationStatus NotifyStatus => ToolNotificationStatus.None;

        class ProfilerMenuInfo : IMenuInfo
        {
            public string Name => "Profiler";
            public string MenuName => "Profiler";
            public int SortNo => NoaDebuggerDefine.PROFILER_MENU_SORT_NO;
        }

        ProfilerMenuInfo _profilerMenuInfo;

        public IMenuInfo MenuInfo()
        {
            if (_profilerMenuInfo == null)
            {
                _profilerMenuInfo = new ProfilerMenuInfo();
            }

            return _profilerMenuInfo;
        }


        public FpsModel FpsModel { get; private set; }

        public FrameTimeModel FrameTimeModel { get; private set; }

        public MemoryModel MemoryModel { get; private set; }

        public BatteryModel BatteryModel { get; private set; }

        public RenderingModel RenderingModel { get; private set; }

        public ThermalModel ThermalModel { get; private set; }


        public void Init()
        {
            FpsModel = new FpsModel();
            FpsModel.OnFpsInfoChanged = _UpdateFpsView;

            FrameTimeModel = new FrameTimeModel();
            FrameTimeModel.OnFrameTimeInfoChanged = _UpdateFrameTimeView;

            MemoryModel = new MemoryModel();
            MemoryModel.OnMemoryInfoChanged = _UpdateMemoryView;

            BatteryModel = new BatteryModel();
            BatteryModel.OnBatteryInfoChanged = _UpdateBatteryView;

            RenderingModel = new RenderingModel();
            RenderingModel.OnRenderingInfoChanged = _UpdateRenderingView;

            ThermalModel = new ThermalModel();
            ThermalModel.OnThermalInfoChanged = _UpdateThermalView;

            _floatingWindowPresenter =
                new FloatingWindowPresenter<ProfilerFloatingWindowView, ProfilerViewLinker>(
                    _floatingWindowPrefab, NoaDebuggerPrefsDefine.PrefsKeyIsProfilerWindowInfo, MenuInfo().Name);
            _floatingWindowPresenter.OnInitAction += _InitFloatingWindow;
        }


        public void ShowView(Transform parent)
        {
            if (_mainView == null)
            {
                _mainView = GameObject.Instantiate(_mainViewPrefab, parent);
                _InitView(_mainView);
            }

            _UpdateAllView();
            _mainView.gameObject.SetActive(true);
        }

        void _InitView(ProfilerView view)
        {
            view.OnFpsProfilingStateChanged += _OnFpsProfilingStateChanged;
            view.OnFrameTimeProfilingStateChanged += _OnFrameTimeProfilingStateChanged;
            view.OnMemoryProfilingStateChanged += _OnMemoryProfilingStateChanged;
            view.OnMemoryGraphStateChanged += _OnMemoryGraphShowingStateChanged;
            view.OnGcCollectExecuted += _OnGcCollectExecuted;
            view.OnBatteryProfilingStateChanged += _OnBatteryProfilingStateChanged;
            view.OnRenderingProfilingStateChanged += _OnRenderingProfilingStateChanged;
            view.OnRenderingGraphStateChanged += _OnRenderingGraphShowingStateChanged;
            view.OnRenderingGraphSelected += _OnRenderingGraphSelected;
            view.OnThermalProfilingStateChanged += _OnThermalProfilingStateChanged;
        }

        public void AlignmentUI(bool isReverse)
        {
            _mainView.AlignmentUI(isReverse);
        }


        public ToolPinStatus GetPinStatus()
        {
            return _floatingWindowPresenter.IsActive ? ToolPinStatus.On : ToolPinStatus.Off;
        }

        public void TogglePin(Transform parent)
        {
            _floatingWindowPresenter.ToggleActive(parent);
        }

        public void InitFloatingWindow(Transform parent)
        {
            var isWindowDrawing = _floatingWindowPresenter.IsActive;
            if (isWindowDrawing)
            {
                _floatingWindowPresenter.InstantiateWindow(parent);
            }
        }

        void _InitFloatingWindow(ProfilerFloatingWindowView window)
        {
            window.OnFpsProfilingStateChanged += _OnFpsProfilingStateChanged;
            window.OnFrameTimeProfilingStateChanged += _OnFrameTimeProfilingStateChanged;
            window.OnMemoryProfilingStateChanged += _OnMemoryProfilingStateChanged;
            window.OnMemoryGraphStateChanged += _OnMemoryGraphShowingStateChanged;
            window.OnGcCollectExecuted += _OnGcCollectExecuted;
            window.OnRenderingProfilingStateChanged += _OnRenderingProfilingStateChanged;
            window.OnRenderingGraphStateChanged += _OnRenderingGraphShowingStateChanged;
            window.OnRenderingGraphSelected += _OnRenderingGraphSelected;
            _UpdateAllView();
        }


        void _UpdateAllView()
        {
            _UpdateFpsView();
            _UpdateFrameTimeView();
            _UpdateMemoryView();
            _UpdateRenderingView();
            _UpdateBatteryView();
            _UpdateThermalView();
        }

        void _UpdateFpsView()
        {
            var fps = FpsModel.FpsInfo;
            var viewData = new ProfilerViewLinker
            {
                _fpsInfo = CreateFpsViewInfo(fps),
            };

            if (_mainView != null)
            {
                _mainView.Show(viewData);
            }

            if (_floatingWindowPresenter.IsActive)
            {
                _floatingWindowPresenter.ShowWindowView(viewData);
            }
        }

        public static FpsUnchangingInfo CreateFpsViewInfo(FpsInfo info)
        {
            return new FpsUnchangingInfo(info);
        }

        void _UpdateFrameTimeView()
        {
            var frameTime = FrameTimeModel.FrameTimeInfo;
            var viewData = new ProfilerViewLinker
            {
                _frameTimeInfo = CreateFrameTimeInfo(frameTime)
            };

            if (_mainView != null)
            {
                _mainView.Show(viewData);
            }

            if (_floatingWindowPresenter.IsActive)
            {
                _floatingWindowPresenter.ShowWindowView(viewData);
            }
        }

        public static ProfilerFrameTimeViewInformation CreateFrameTimeInfo(FrameTimeInfo info)
        {
            return new ProfilerFrameTimeViewInformation
            {
                _histories = info.HistoryBuffer,
                _isEnabled = info.IsEnabled,
                _isActive = info.IsActive
            };
        }

        void _UpdateMemoryView()
        {
            var memory = MemoryModel.MemoryInfo;
            var viewData = new ProfilerViewLinker()
            {
                _memoryInfo = CreateMemoryViewInfo(memory)
            };

            if (_mainView != null)
            {
                _mainView.Show(viewData);
            }

            if (_floatingWindowPresenter.IsActive)
            {
                _floatingWindowPresenter.ShowWindowView(viewData);
            }
        }

        public static MemoryUnchangingInfo CreateMemoryViewInfo(MemoryInfo info)
        {
            return new MemoryUnchangingInfo(info);
        }

        void _UpdateBatteryView()
        {
            var battery = BatteryModel.BatteryInfo;
            var viewData = new ProfilerViewLinker()
            {
                _batteryInfo = CreateBatteryViewInfo(battery),
            };

            if (_mainView != null)
            {
                _mainView.Show(viewData);
            }

            if (_floatingWindowPresenter.IsActive)
            {
                _floatingWindowPresenter.ShowWindowView(viewData);
            }
        }

        public static BatteryUnchangingInfo CreateBatteryViewInfo(BatteryInfo info)
        {
            return new BatteryUnchangingInfo(info);
        }

        void _UpdateRenderingView()
        {
            var rendering = RenderingModel.RenderingInfo;
            var viewData = new ProfilerViewLinker
            {
                _renderingInfo = CreateRenderingViewInfo(rendering)
            };

            if (_mainView != null)
            {
                _mainView.Show(viewData);
            }

            if (_floatingWindowPresenter.IsActive)
            {
                _floatingWindowPresenter.ShowWindowView(viewData);
            }
        }

        public static RenderingUnchangingInfo CreateRenderingViewInfo(RenderingInfo info)
        {
            return new RenderingUnchangingInfo(info);
        }

        void _UpdateThermalView()
        {
            var thermal = ThermalModel.ThermalInfo;
            var viewData = new ProfilerViewLinker()
            {
                _thermalInfo = CreateThermalViewInfo(thermal),
            };

            if (_mainView != null)
            {
                _mainView.Show(viewData);
            }

            if (_floatingWindowPresenter.IsActive)
            {
                _floatingWindowPresenter.ShowWindowView(viewData);
            }
        }

        public static ThermalUnchangingInfo CreateThermalViewInfo(ThermalInfo info)
        {
            return new ThermalUnchangingInfo(info);
        }


        void _OnFpsProfilingStateChanged(bool isProfiling)
        {
            ChangeFpsProfiling(isProfiling);
        }

        void _OnFrameTimeProfilingStateChanged(bool isProfiling)
        {
            ChangeFrameTimeProfiling(isProfiling);
        }

        void _OnMemoryProfilingStateChanged(bool isProfiling)
        {
            ChangeMemoryProfiling(isProfiling);
        }

        void _OnMemoryGraphShowingStateChanged(bool isShowing)
        {
            MemoryModel.ChangeGraphShowingState(isShowing);
            _UpdateMemoryView();
        }

        void _OnGcCollectExecuted()
        {
            System.GC.Collect();
        }

        void _OnBatteryProfilingStateChanged(bool isProfiling)
        {
            ChangeBatteryProfiling(isProfiling);
        }

        void _OnRenderingProfilingStateChanged(bool isProfiling)
        {
            ChangeRenderingProfiling(isProfiling);
        }

        void _OnRenderingGraphShowingStateChanged(bool isShowing)
        {
            RenderingModel.ChangeGraphShowingState(isShowing);
            _UpdateRenderingView();
        }

        void _OnRenderingGraphSelected(RenderingGraphTarget target)
        {
            if (target != RenderingModel.RenderingInfo.GraphTarget)
            {
                RenderingModel.SwitchGraphTarget(target);
                _UpdateRenderingView();
            }
        }

        void _OnThermalProfilingStateChanged(bool isProfiling)
        {
            ChangeThermalProfiling(isProfiling);
        }


        void _OnHidden()
        {
            if (_mainView == null)
            {
                return;
            }
            _mainView.gameObject.SetActive(false);
        }

        public void OnHidden()
        {
            _OnHidden();
        }

        public void OnToolDispose()
        {
            _OnHidden();

            if (FpsModel != null)
            {
                FpsModel.Dispose();
                FpsModel = null;
            }
            if(FrameTimeModel != null)
            {
                FrameTimeModel.Dispose();
                FrameTimeModel = null;
            }
            if (MemoryModel != null)
            {
                MemoryModel.Dispose();
                MemoryModel = null;
            }
            if(RenderingModel != null)
            {
                RenderingModel.Dispose();
                RenderingModel = null;
            }
            if(BatteryModel != null)
            {
                BatteryModel.Dispose();
                BatteryModel = null;
            }
            if (ThermalModel != null)
            {
                ThermalModel.Dispose();
                ThermalModel = null;
            }
        }


        public ProfilerSnapshotData CaptureSnapshot()
        {
            return new ProfilerSnapshotData(this);
        }


        public FpsInfo GetFpsInfo()
        {
            return FpsModel.FpsInfo;
        }

        public bool IsFpsProfiling()
        {
            return FpsModel.FpsInfo.IsProfiling;
        }

        public void ChangeFpsProfiling(bool isProfiling)
        {
            FpsModel.ChangeFpsProfilingState(isProfiling);
            FrameTimeModel.ToggleActive(isProfiling);
            _UpdateFpsView();
        }


        public void ChangeFrameTimeProfiling(bool isProfiling)
        {
            FrameTimeModel.ToggleEnabled(isProfiling);
            _UpdateFrameTimeView();
        }


        public MemoryInfo GetMemoryInfo()
        {
            return MemoryModel.MemoryInfo;
        }

        public bool IsMemoryProfiling()
        {
            if (!MemoryModel.CanProfiling())
            {
                return false;
            }

            return MemoryModel.MemoryInfo.IsProfiling;
        }

        public void ChangeMemoryProfiling(bool isProfiling)
        {
            MemoryModel.ChangeProfilingState(isProfiling);
            _UpdateMemoryView();
        }

        public float GetTotalMemoryMB()
        {
            return MemoryModel.MemoryInfo.TotalMemoryMB;
        }

        public void SetTotalMemoryMB(float totalMemoryMB = -1)
        {
            MemoryModel.MemoryInfo.SetTotalMemoryMB(totalMemoryMB);
        }


        public RenderingInfo GetRenderingInfo()
        {
            return RenderingModel.RenderingInfo;
        }

        public bool IsRenderingProfiling()
        {
            return RenderingModel.RenderingInfo.IsProfiling;
        }

        public void ChangeRenderingProfiling(bool isProfiling)
        {
            RenderingModel.ChangeProfilingState(isProfiling);
            _UpdateRenderingView();
        }


        public BatteryInfo GetBatteryInfo()
        {
            return BatteryModel.BatteryInfo;
        }

        public bool IsBatteryProfiling()
        {
            if (!BatteryModel.CanProfiling())
            {
                return false;
            }

            return BatteryModel.BatteryInfo.IsProfiling;
        }

        public void ChangeBatteryProfiling(bool isProfiling)
        {
            BatteryModel.ChangeBatteryProfilingState(isProfiling);
        }


        public ThermalInfo GetThermalInfo()
        {
            return ThermalModel.ThermalInfo;
        }

        public bool IsThermalProfiling()
        {
            if (!ThermalModel.CanProfiling())
            {
                return false;
            }

            return ThermalModel.ThermalInfo.IsProfiling;
        }

        public void ChangeThermalProfiling(bool isProfiling)
        {
            ThermalModel.ChangeProfilingState(isProfiling);
        }

        void OnDestroy()
        {
            _mainViewPrefab = default;
            _mainView = default;
            _floatingWindowPrefab = default;
            _floatingWindowPresenter = default;
            _profilerMenuInfo = default;
            FpsModel = default;
            FrameTimeModel = default;
            MemoryModel = default;
            BatteryModel = default;
            RenderingModel = default;
            ThermalModel = default;
        }
    }
}
