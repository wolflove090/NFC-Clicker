using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class SnapshotPresenter : NoaDebuggerToolBase, INoaDebuggerTool
    {
        class SnapshotExportData : IExportData
        {
            const string EXPORT_FILE_PREFIX = "snapshot";

            public SnapshotModel.SnapshotInformation _snapshotInfo;

            readonly DownloadInfo _downloadData = new DownloadInfo(EXPORT_FILE_PREFIX);

            public DownloadInfo GetDownloadInfo()
            {
                return _downloadData;
            }
        }

        [Header("MainView")]
        [SerializeField]
        SnapshotView _mainViewPrefab;
        SnapshotView _mainView;

        [Header("DownloadDialog")]
        [SerializeField]
        DownloadDialog _dialogPrefab;
        DownloadDialogPresenter _downloadDialogPresenter;

        public ToolNotificationStatus NotifyStatus => ToolNotificationStatus.None;

        public UnityAction<SnapshotLogRecordInformation, string> _onLogCopied;

        public Func<string, string, bool>_onLogDownload;

        public SnapshotModel Model { get; private set; }

        public void Init()
        {
            Model = new SnapshotModel();
            Model.OnTimeDataUpdated = _UpdateFromTime;
        }

        class SnapshotMenuInfo : IMenuInfo
        {
            public string Name => "Snapshot";
            public string MenuName => "Snapshot";
            public int SortNo => NoaDebuggerDefine.SNAPSHOT_MENU_SORT_NO;
        }

        SnapshotMenuInfo _snapshotMenuInfo;

        public IMenuInfo MenuInfo()
        {
            if (_snapshotMenuInfo == null)
            {
                _snapshotMenuInfo = new SnapshotMenuInfo();
            }

            return _snapshotMenuInfo;
        }

        public void ShowView(Transform parent)
        {
            if (_mainView == null)
            {
                _mainView = GameObject.Instantiate(_mainViewPrefab, parent);
                _InitView(_mainView);
            }
            _UpdateView();
            _mainView.gameObject.SetActive(true);
        }

        void _InitView(SnapshotView view)
        {
            view.OnInsertLog += _OnInsertLog;
            view.OnToggleChanged += _OnToggleChanged;
            view.OnClearAllLog += _OnClearAllLog;
            view.OnUpdateLogLabel += Model.UpdateLogLabel;
            view.OnClickLog += _OnClickLog;
            view.OnLongTapLog += _OnCopyLog;
            view.OnClickCopyButton += _OnCopyLog;
            view.OnDownloadLog += _OnDownloadLog;
            view.OnComparison += _OnComparison;
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

        void _UpdateView()
        {
            SnapshotViewLinker linker = new SnapshotViewLinker();
            _SetViewLinkerFromTime(linker);
            _SetViewLinkerFromLogData(linker);
            _SetViewLinkerFromComparison(linker);

            linker._isNeedResetScrollPos = true;

            _OnActiveChanged();

            _UpdateByLinker(linker);
        }

        void _UpdateByLinker(SnapshotViewLinker linker, bool onUpdate = false)
        {
            if (_mainView != null)
            {
                if (onUpdate)
                {
                    _mainView.ShowOnUpdate(linker);
                }
                else
                {
                    _mainView.Show(linker);
                }
            }
        }

        public void OnHidden()
        {
            if (Model != null)
            {
                Model.ClearAllLogSelected();
                Model.ClearAllLogChecked();
            }
            _DestroyView();
        }

        public void OnToolDispose()
        {
            OnHidden();

            if (Model != null)
            {
                Model.Dispose();
                Model = null;
            }

            _onLogCopied = null;
            _onLogDownload = null;
        }

        public void ClearLogsAndTimer()
        {
            Model.ClearAllLog();
            Model.ClearAllLogSelected();
            Model.ClearAllLogChecked();
            Model.UpdateProfilingStartTime();
        }

        public void CaptureLog(string label = null, Color? backgroundColor = null, bool hasNoaProfilerInfo = true, Dictionary<string, NoaSnapshotCategory> additionalInfo = null, bool updateProfilingElapsedTime = false)
        {
            ProfilerSnapshotData snapshotData = null;
            if (hasNoaProfilerInfo)
            {
                var presenter = NoaDebugger.GetPresenter<ProfilerPresenter>();
                snapshotData = presenter.CaptureSnapshot();
            }

            if (updateProfilingElapsedTime)
            {
                Model.UpdateProfilingElapsedTime();
            }
            Model.InsertLog(snapshotData, label, backgroundColor, additionalInfo);
        }

        void _DestroyView()
        {
            if (_mainView != null)
            {
                _mainView.gameObject.SetActive(false);
            }

            if (_downloadDialogPresenter != null)
            {
                _downloadDialogPresenter.Dispose();
            }
            _OnActiveChanged();
        }

        void _OnActiveChanged()
        {
            if (Model == null)
            {
                return;
            }

            if (_mainView != null)
            {
                Model.SetUpdateAction();
            }
            else
            {
                Model.DeleteUpdateAction();
            }
        }

        void _SetViewLinkerFromTime(SnapshotViewLinker linker)
        {
            linker._elapsedTime = Model.SnapshotInfo._elapsedTime;
        }

        void _SetViewLinkerFromLogData(SnapshotViewLinker linker)
        {
            linker._logList = Model.SnapshotInfo._logList;
        }

        void _SetViewLinkerFromComparison(SnapshotViewLinker linker)
        {
            linker._isComparison = Model.SnapshotInfo._isComparison;
        }

        void _OnInsertLog()
        {
            var profilerPresenter = NoaDebugger.GetPresenter<ProfilerPresenter>();
            if (profilerPresenter == null)
            {
                return;
            }

            var snapshot = profilerPresenter.CaptureSnapshot();
            Model.InsertLog(snapshot);
            SnapshotViewLinker linker = new SnapshotViewLinker();
            _SetViewLinkerFromLogData(linker);

            linker._isNeedResetScrollPos = true;
            Model.ChangeComparisonState(true);

            _UpdateByLinker(linker);
        }

        void _OnToggleChanged(int id)
        {
            Model.SetCheckedLogId(id);
            _ReloadLogScroll();
        }

        void _OnClearAllLog()
        {
            ClearLogsAndTimer();
            _ReloadLogScroll();
        }

        void _OnClickLog(int id)
        {
            Model.SetHighlightLogId(id);
            Model.SetSelectedLogId(id);
            _ReloadLogScroll();
        }

        void _OnCopyLog(int id)
        {
            SnapshotLogRecordInformation snapshotLog = Model.SnapshotInfo._logList.FirstOrDefault(log => log.Id == id);
            if (snapshotLog == null)
            {
                return;
            }
            var purser = new KeyValueArrayParser.ObjectParser(_CreateSnapshotLogExportData(snapshotLog));
            ClipboardModel.Copy(purser.ToJsonString());
            _onLogCopied?.Invoke(snapshotLog, purser.ToJsonString());
            NoaDebugger.ShowToast(new ToastViewLinker {_label = NoaDebuggerDefine.ClipboardCopyText});
        }

        void _ReloadLogScroll()
        {
            SnapshotViewLinker linker = new SnapshotViewLinker();
            _SetViewLinkerFromLogData(linker);
            _SetViewLinkerFromComparison(linker);
            _UpdateByLinker(linker);
        }

        void _UpdateFromTime()
        {
            SnapshotViewLinker linker = new SnapshotViewLinker();
            _SetViewLinkerFromTime(linker);

            _UpdateByLinker(linker, true);
        }

        void _OnDownloadLog()
        {
            if (_downloadDialogPresenter == null)
            {
                _downloadDialogPresenter = new DownloadDialogPresenter(_dialogPrefab);
                _downloadDialogPresenter.OnExecDownload += _OnExecDownload;
            }

            _downloadDialogPresenter.ShowDialog();
        }

        void _OnComparison()
        {
            Model.ChangeComparisonState();
            Model.ClearAllLogSelected();
            _ReloadLogScroll();
        }

        void _OnExecDownload(string label, UnityAction<FileDownloader.DownloadInfo> completed)
        {
            DownloadInfo downloadInfo = new SnapshotExportData().GetDownloadInfo();
            string fileName = downloadInfo.GetExportFileName(label, DateTime.Now);
            string json = _CreateExportJsonString(label);
            if (_onLogDownload != null)
            {
                var flag = _onLogDownload(fileName, json);
                if (!flag)
                    return;
            }
            LocalDataExportModel.ExportText(fileName, json, completed);
        }

        string _CreateExportJsonString(string label)
        {
            KeyValueSerializer[] exportData = new KeyValueSerializer[2];
            exportData[0] = _CreateSnapshotExportData();

            exportData[1] = KeyValueSerializer.CreateSubData(label);

            return KeyValueSerializer.SerializeToJson(exportData);
        }

        KeyValueSerializer _CreateSnapshotExportData()
        {
            List<SnapshotLogRecordInformation> logList = Model.SnapshotInfo._logList;
            var snapshotLogParser = new KeyValueArrayParser.ObjectParser[logList.Count];
            for (int i = 0; i < logList.Count; i++)
            {
                snapshotLogParser[i] = new KeyValueArrayParser.ObjectParser(_CreateSnapshotLogExportData(logList[i]));
            }
            var snapshotLog = new KeyValueArrayParser("_log", snapshotLogParser);
            return new KeyValueSerializer("SnapshotLog", new IKeyValueParser[] {snapshotLog});
        }

        IKeyValueParser[] _CreateSnapshotLogExportData(SnapshotLogRecordInformation log)
        {
            List<IKeyValueParser> parsers = new List<IKeyValueParser>();

            var label = new KeyValueParser(NoaDebuggerDefine.SnapshotDownloadLogInfoLabels[0], log.Label);
            parsers.Add(label);

            var time = new KeyValueParser(NoaDebuggerDefine.SnapshotDownloadLogInfoLabels[1], TimeSpanToHourTimeFormatString(log.Time));
            parsers.Add(time);

            var elapsedTime = new KeyValueParser(NoaDebuggerDefine.SnapshotDownloadLogInfoLabels[2], TimeSpanToHourTimeFormatString(log.ElapsedTime));
            parsers.Add(elapsedTime);

            ProfilerSnapshotData snapshot = log.Snapshot;
            if (snapshot != null)
            {
                parsers.Add(KeyObjectParser.CreateFromClass(snapshot.FpsInfo, "FPS"));
                parsers.Add(KeyObjectParser.CreateFromClass(snapshot.MemoryInfo, "Memory"));
                parsers.Add(KeyObjectParser.CreateFromClass(snapshot.RenderingInfo, "Rendering"));
                parsers.Add(KeyObjectParser.CreateFromClass(snapshot.BatteryInfo, "Battery"));
                parsers.Add(KeyObjectParser.CreateFromClass(snapshot.ThermalInfo, "Thermal"));
            }

            if (log.AdditionalInfo != null)
            {
                foreach (var additionalInfo in log.AdditionalInfo)
                {
                    var keyValueParsers = new List<KeyValueParser>();
                    foreach (var categoryItem in additionalInfo.Value.CategoryItems)
                    {
                        keyValueParsers.Add(new KeyValueParser(categoryItem.Key, categoryItem.Value));
                    }
                    parsers.Add( new KeyObjectParser(ConvertCategoryName(additionalInfo.Key, true), keyValueParsers.ToArray()));
                }
            }

            return parsers.ToArray();
        }

        public static string ConvertCategoryName(string categoryName, bool isDownload = false)
        {
            categoryName = string.IsNullOrEmpty(categoryName)
                ? NoaDebuggerDefine.UnmarkedSnapshotCategoryNameToSetAdditionalInfo
                : categoryName;
            if (NoaDebuggerDefine.SnapshotDuplicateCategoryNames.Contains(categoryName))
            {
                categoryName = $"{NoaDebuggerDefine.SnapshotDuplicateCategoryNamePrefix} {categoryName}";
            }

            if (isDownload && NoaDebuggerDefine.SnapshotDownloadLogInfoLabels.Contains(categoryName))
            {
                categoryName = $"{NoaDebuggerDefine.SnapshotDuplicateCategoryNamePrefix} {categoryName}";
            }

            return categoryName;
        }

        public static string TimeSpanToHourTimeFormatString(TimeSpan time)
        {
            return $"{(int) time.TotalHours}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
        }


        public List<SnapshotLogRecordInformation> GetLogList()
        {
            return Model.SnapshotInfo._logList;
        }

        void OnDestroy()
        {
            _mainViewPrefab = default;
            _mainView = default;
            _dialogPrefab = default;
            _onLogCopied = default;
            _onLogDownload = default;
            _downloadDialogPresenter = default;
            _snapshotMenuInfo = default;
            Model = default;
        }
    }
}
