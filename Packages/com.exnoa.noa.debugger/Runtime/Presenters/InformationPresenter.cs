using System;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class InformationPresenter : NoaDebuggerToolBase, INoaDebuggerTool
    {
        class InformationExportData : IExportData
        {
            const string EXPORT_FILE_PREFIX = "information";

            readonly DownloadInfo _downloadData = new DownloadInfo(InformationExportData.EXPORT_FILE_PREFIX);

            public DownloadInfo GetDownloadInfo()
            {
                return _downloadData;
            }
        }

        [Header("MainView")]
        [SerializeField]
        InformationView _mainViewPrefab;
        InformationView _mainView;

        [SerializeField, Header("DownloadDialog")]
        DownloadDialog _dialogPrefab;
        DownloadDialogPresenter _downloadDialogPresenter;

        SystemInformationModel _systemInfoModel;
        UnityInformationModel _unityInfoModel;

        ScreenOrientation _lastOrientation;

        InformationView.ToggleTabType _tabType = InformationView.ToggleTabType.System;

        public ToolNotificationStatus NotifyStatus => ToolNotificationStatus.None;

        public Func<string, string, bool> _onDownload;

        public void Init()
        {
            _systemInfoModel = new SystemInformationModel();
            _unityInfoModel = new UnityInformationModel();
        }

        class InformationMenuInfo : IMenuInfo
        {
            public string Name => "Information";
            public string MenuName => "Information";
            public int SortNo => NoaDebuggerDefine.INFORMATION_MENU_SORT_NO;
        }

        InformationMenuInfo _informationMenuInfo;

        public IMenuInfo MenuInfo()
        {
            if (_informationMenuInfo == null)
            {
                _informationMenuInfo = new InformationMenuInfo();
            }

            return _informationMenuInfo;
        }

        public void ShowView(Transform parent)
        {
            if (_mainView == null)
            {
                _mainView = GameObject.Instantiate(_mainViewPrefab, parent);
                _InitView(_mainView);
            }
            _tabType = InformationView.ToggleTabType.System;
            _UpdateTabView();
        }

        void _InitView(InformationView view)
        {
            view.OnClickTab += _OnClickTabButton;
            view.OnClickReload += _UpdateTabView;
            view.OnClickDownload += _OnDownload;
        }

        public ToolPinStatus GetPinStatus()
        {
            return ToolPinStatus.None;
        }

        public void TogglePin(Transform parent)
        {
        }

        public void InitFloatingWindow(Transform parent)
        {
        }

        public void AlignmentUI(bool isReverse)
        {
            _mainView.AlignmentUI(isReverse);
        }

        void _OnHidden()
        {
            if (_mainView != null)
            {
                _mainView.gameObject.SetActive(false);
            }

            if (_downloadDialogPresenter != null)
            {
                _downloadDialogPresenter.Dispose();
            }
        }

        public void OnHidden()
        {
            _OnHidden();
        }

        public void OnToolDispose()
        {
            _OnHidden();

            _systemInfoModel = null;
            _unityInfoModel = null;
        }

        void _OnClickTabButton(InformationView.ToggleTabType tabType)
        {
            _tabType = tabType;
            _UpdateTabView();
        }

        void _UpdateTabView()
        {
            var viewData = new InformationViewLinker()
            {
                _tabType = _tabType,
            };

            switch (_tabType)
            {
                case InformationView.ToggleTabType.System:
                    viewData._systemInformationViewLinker = _GetSystemViewLinker();
                    break;

                case InformationView.ToggleTabType.Unity:
                    viewData._unityInformationViewLinker = _GetUnitySystemViewLinker();
                    break;
            }

            _mainView.Show(viewData);
            _mainView.gameObject.SetActive(true);
        }

        SystemInformationViewLinker _GetSystemViewLinker()
        {
            _systemInfoModel.OnUpdate();

            var application = _systemInfoModel.ApplicationInfo;
            var device = _systemInfoModel.DeviceInfo;
            var cpu = _systemInfoModel.CpuInfo;
            var gpu = _systemInfoModel.GpuInfo;
            var memory = _systemInfoModel.SystemMemoryInfo;
            var display = _systemInfoModel.DisplayInfo;

            string memorySizeLabel = "";
            if (memory.MemorySizeMB > -1)
            {
                var memoryByte = (long)DataUnitConverterModel.MBToByte(memory.MemorySizeMB);
                memorySizeLabel = DataUnitConverterModel.ToHumanReadableBytes(memoryByte);
            }

            return new SystemInformationViewLinker()
            {
                _identification = application.Identification,
                _version = application.Version,
                _language = application.Language,
                _platform = application.Platform,

                _os = device.OS,
                _deviceModel = device.Model,
                _deviceType = device.Type,
                _deviceName = device.Name,

                _cpuType = cpu.Type,
                _cpuCount = cpu.Count,

                _gpuDeviceName = gpu.DeviceName,
                _gpuDeviceType = gpu.DeviceType,
                _gpuDeviceVersion = gpu.DeviceVersion,
                _gpuDeviceVendor = gpu.DeviceVendor,
                _gpuDeviceSize = $"{gpu.DeviceSizeGB} GB",

                _memorySize = memorySizeLabel,

                _refreshRate = $"{display.RefreshRate}Hz",
                _resolution = $"{display.ScreenWidth} x {display.ScreenHeight}",
                _aspect = $"{display.AspectX} : {display.AspectY} ({display.AspectRatioValue})",
                _dpi = display.Dpi,
                _orientation = display.Orientation,
            };
        }

        UnityInformationViewLinker _GetUnitySystemViewLinker()
        {
            _unityInfoModel.OnUpdate();

            UnityInfo unityInfo = _unityInfoModel.UnityInfo;
            RuntimeInfo runTimeInfo = _unityInfoModel.RuntimeInfo;
            FeaturesInfo featuresInfo = _unityInfoModel.FeaturesInfo;
            GraphicsInfo graphicsInfo = _unityInfoModel.GraphicsInfo;

            var unityInfoViewData = new UnityInformationViewLinker.UnityInfo()
            {
                _version = unityInfo.Version,
                _debug = unityInfo.Debug,
                _il2CPP = unityInfo.IL2CPP,
                _vSyncCount = unityInfo.VSyncCount,
                _targetFrameRate = unityInfo.TargetFrameRate
            };

            var runtimeViewData = new UnityInformationViewLinker.Runtime()
            {
                _playTime = runTimeInfo.PlayTime,
                _levelPlayTime = runTimeInfo.LevelPlayTime,
                _currentLevelSceneName = runTimeInfo.CurrentLevelSceneName,
                _currentLevelBuildIndex = runTimeInfo.CurrentLevelBuildIndex,
                _qualityLevel = runTimeInfo.QualityLevel,
            };

            var featuresViewData = new UnityInformationViewLinker.Features()
            {
                _location = featuresInfo.Location,
                _accelerometer = featuresInfo.Accelerometer,
                _gyroscope = featuresInfo.Gyroscope,
                _vibration = featuresInfo.Vibration,
            };

            var graphicsViewData = new UnityInformationViewLinker.Graphics()
            {
                _maxTexSize = graphicsInfo.MaxTexSize,
                _npotSupport = graphicsInfo.NpotSupport,
                _computeShaders = graphicsInfo.ComputeShaders,
                _shadows = graphicsInfo.Shadows,
                _sparseTextures= graphicsInfo.SparseTextures,
            };

            return new UnityInformationViewLinker()
            {
                _unityInfo = unityInfoViewData,
                _runtimeInfo = runtimeViewData,
                _featuresInfo = featuresViewData,
                _graphicsInfo = graphicsViewData,
            };
        }

        void _OnDownload()
        {
            if (_downloadDialogPresenter == null)
            {
                _downloadDialogPresenter = new DownloadDialogPresenter(_dialogPrefab);
                _downloadDialogPresenter.OnExecDownload += _OnExecDownload;
            }

            _downloadDialogPresenter.ShowDialog();
        }

        void _OnExecDownload(string label, UnityAction<FileDownloader.DownloadInfo> completed)
        {
            DownloadInfo downloadInfo = new InformationExportData().GetDownloadInfo();
            string fileName = downloadInfo.GetExportFileName(label, DateTime.Now);
            string json = _CreateExportJsonString(label);
            if (_onDownload != null)
            {
                var flag = _onDownload(fileName, json);
                if (!flag)
                    return;
            }
            LocalDataExportModel.ExportText(fileName, json, completed);
        }

        string _CreateExportJsonString(string label)
        {
            KeyValueSerializer[] exportData = new KeyValueSerializer[2];
            exportData[0] = _CreateInformationExportData();

            exportData[1] = KeyValueSerializer.CreateSubData(label);

            return KeyValueSerializer.SerializeToJson(exportData);
        }

        KeyValueSerializer _CreateInformationExportData()
        {
            _systemInfoModel.OnUpdate();
            _unityInfoModel.OnUpdate();
            var systemInfoParser = new KeyObjectParser("System", _systemInfoModel.CreateExportData());
            var unityInfoParser = new KeyObjectParser("Unity", _unityInfoModel.CreateExportData());
            return new KeyValueSerializer("Information", new IKeyValueParser[] {systemInfoParser, unityInfoParser});
        }

        public SystemInformation CreateSystemInformation()
        {
            _systemInfoModel.OnUpdate();
            return new SystemInformation(_systemInfoModel);
        }

        public UnityInformation CreateUnityInformation()
        {
            _unityInfoModel.OnUpdate();
            return new UnityInformation(_unityInfoModel);
        }

        void OnDestroy()
        {
            _mainViewPrefab = default;
            _mainView = default;
            _dialogPrefab = default;
            _downloadDialogPresenter = default;
            _systemInfoModel = default;
            _unityInfoModel = default;
            _informationMenuInfo = default;
        }
    }
}
