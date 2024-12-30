using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NoaDebugger
{
    abstract class LogPresenterBase<TLogEntry, TLogDetail> : NoaDebuggerToolBase
        where TLogEntry : LogEntry<TLogDetail>
        where TLogDetail : ILogDetail
    {
        class LogExportData : IExportData
        {
            readonly DownloadInfo _downloadData;

            public LogExportData(string exportFilenamePrefix)
            {
                _downloadData = new DownloadInfo(exportFilenamePrefix);
            }

            public DownloadInfo GetDownloadInfo() => _downloadData;
        }

        protected static readonly char[] NewLine = new[] {'\n', '\r'};

        protected LinkedList<TLogEntry> _logBuffer;

        [Header("MainView")]
        [SerializeField]
        LogViewBase _mainViewPrefab;
        LogViewBase _mainView;

        [SerializeField, Header("FloatingWindowView")]
        LogFloatingWindowView _landscapeFloatingWindowPrefab;
        [SerializeField]
        LogFloatingWindowView _portraitFloatingWindowPrefab;
        FloatingWindowPresenter<LogFloatingWindowView, LogViewLinker> _floatingWindowPresenter;

        [SerializeField, Header("DownloadDialog")]
        DownloadDialog _dialogPrefab;
        DownloadDialogPresenter _downloadDialogPresenter;

        protected NoaDebuggerSettings _settings;

        List<LogViewLinker.LogPanelInfo> _logs;
        Dictionary<LogType, int> _logCounts;
        int _selectLogIndex = -1;

        List<LogViewLinker.LogPanelInfo> _panels;
        LogViewLinker.LogPanelInfo[] _panelInstances;

        UnityAction<int> _onSelectLog = null;
        UnityAction<int> _onPointerDown = null;
        UnityAction _onLongTap = null;

        int _longTapStartLogIndex;

        Dictionary<LogType, bool> _showLogFlagDictionary;
        string _filterText;

        LogCollectorModel _logModel;

        bool _scrollInitLock;

        bool _resetScroll;

        bool _hideLogDetailView;

        bool _isShow;

        protected abstract string GetToolName();

        protected abstract LogCollectorModel CreateLogCollectorModel();

        protected abstract int GetLogCapacity();

        protected abstract void SetLogPanelInfo(ref LogViewLinker.LogPanelInfo logPanelInfo, TLogEntry log);

        protected abstract string GetStatusString();

        protected abstract void OnClearLog();

        protected abstract void OnLogCopied(TLogEntry log, string clipboardText);

        protected abstract bool? OnLogDownload(string fileName, string json);

        protected abstract string GetExportFilenamePrefix();

        protected abstract string GetWindowInfoPrefsKey();

        protected abstract KeyValueSerializer CreateLogKeyValueSerializer();

        protected void _Init()
        {
            _settings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings();
            _logBuffer = new LinkedList<TLogEntry>();
            _logModel = CreateLogCollectorModel();

            _showLogFlagDictionary = new Dictionary<LogType, bool>();
            _showLogFlagDictionary.Add(LogType.Log, true);
            _showLogFlagDictionary.Add(LogType.Warning, true);
            _showLogFlagDictionary.Add(LogType.Error, true);

            _logCounts = new Dictionary<LogType, int>();
            _logCounts.Add(LogType.Log, 0);
            _logCounts.Add(LogType.Warning, 0);
            _logCounts.Add(LogType.Error, 0);

            _floatingWindowPresenter = new FloatingWindowPresenter<LogFloatingWindowView, LogViewLinker>(
                _landscapeFloatingWindowPrefab, _portraitFloatingWindowPrefab, GetWindowInfoPrefsKey(), GetToolName());
            _floatingWindowPresenter.OnInitAction += _OnInitFloatingWindow;

            _downloadDialogPresenter = new DownloadDialogPresenter(_dialogPrefab);
            _downloadDialogPresenter.OnExecDownload += _OnExecDownload;

            _hideLogDetailView = true;

            int logCapacity = GetLogCapacity() * Enum.GetValues(typeof(LogType)).Length;
            _panels = new List<LogViewLinker.LogPanelInfo>(logCapacity);
            _panelInstances = new LogViewLinker.LogPanelInfo[logCapacity];
            for (int i = 0; i < _panelInstances.Length; ++i)
            {
                _panelInstances[i] = new LogViewLinker.LogPanelInfo();
            }

            _onSelectLog = _OnSelectLog;
            _onPointerDown = _SaveLongTapStartLog;
            _onLongTap = _CopySavedLog;
        }


        protected void _UpdateView() => _DoUpdateView();

        protected void _ShowView(Transform parent)
        {
            if (_mainView == null)
            {
                _mainView = GameObject.Instantiate(_mainViewPrefab, parent);
                _InitView(_mainView);
            }

            _isShow = true;
            _mainView.gameObject.SetActive(true);
            _DoUpdateView();
        }

        void _InitView(LogViewBase view)
        {
            view.OnRecord += _OnRecord;
            view.OnClear += _OnClearLogs;
            view.OnDownload += _OnDownload;
            view.OnCopy = _CopyLog;
            view._logSwitchToggleDrawer.OnSwitchByLogType += _OnSwitchByType;
            view._logScrollDrawer.OnChangeFilterText += _OnChangeFilter;
        }

        void _HideView()
        {
            if (_mainView == null)
            {
                return;
            }

            _isShow = false;
            _mainView.gameObject.SetActive(false);
        }

        void _DoUpdateView()
        {
            _logs = _CreateLogViewInfos(out int selectLogIndex);
            var linker = new LogViewLinker()
            {
                _isCollecting = _logModel.IsCollecting,
                _logs = _logs,
                _logStatus = GetStatusString(),
                _selectLogIndex = selectLogIndex,
                _logTypeToggles = new LogViewLinker.LogTypeToggles()
                {
                    _messageToggle = _showLogFlagDictionary[LogType.Log],
                    _warningToggle = _showLogFlagDictionary[LogType.Warning],
                    _errorToggle = _showLogFlagDictionary[LogType.Error],
                    _messageNum = _logCounts[LogType.Log],
                    _warningNum = _logCounts[LogType.Warning],
                    _errorNum = _logCounts[LogType.Error],
                },
                _forceUpdate = !_scrollInitLock,
                _resetScrollPos = _resetScroll,
                _isViewLogDetail = !_hideLogDetailView,
                _filterText = _filterText,
            };
            if (_isShow)
            {
                _mainView.Show(linker);
                _SetNotificationStatus(ToolNotificationStatus.None);
            }

            if (_floatingWindowPresenter.IsActive)
            {
                _floatingWindowPresenter.ShowWindowView(linker);
                if (!NoaDebugger.IsShowNormalView)
                {
                    _SetNotificationStatus(ToolNotificationStatus.None);
                }
            }

            _resetScroll = false;
            _scrollInitLock = false;
        }

        protected abstract void _SetNotificationStatus(ToolNotificationStatus notifyStatus);

        List<LogViewLinker.LogPanelInfo> _CreateLogViewInfos(out int selectLogIndex)
        {
            selectLogIndex = -1;
            _logCounts[LogType.Log] = 0;
            _logCounts[LogType.Warning] = 0;
            _logCounts[LogType.Error] = 0;
            _panels.Clear();

            var logInfoIndex = 0;
            var logIndex = 0;
            var viewIndex = 0;
            foreach (TLogEntry log in _logBuffer)
            {
                int index = logIndex++;

                _logCounts[log.LogType]++;

                var isShow = true;
                isShow &= _showLogFlagDictionary[log.LogType];
                if (!string.IsNullOrEmpty(_filterText))
                {
                    isShow &= log.LogString.Contains(_filterText);
                }

                if (!isShow)
                {
                    continue;
                }

                bool isSelect = index == _selectLogIndex;
                LogViewLinker.LogPanelInfo panel = _panelInstances[index];
                SetLogPanelInfo(ref panel, log);
                panel._index = index;
                panel._viewIndex = viewIndex++;
                panel._isSelect = isSelect;
                panel._onClick = _onSelectLog;
                panel._onPointerDown = _onPointerDown;
                panel._onLongTap = _onLongTap;
                panel._numberOfMatchingLogs = log.NumberOfMatchingLogs;
                _panels.Add(panel);

                if (isSelect)
                {
                    selectLogIndex = logInfoIndex;
                }

                logInfoIndex++;
            }

            return _panels;
        }

        protected void _AlignmentMainView(bool isReverse) => _mainView.AlignmentUI(isReverse);


        void _OnRecord()
        {
            _logModel.ToggleCollect(!_logModel.IsCollecting);
            _DoUpdateView();
        }

        void _OnClearLogs()
        {
            _logBuffer.Clear();
            _selectLogIndex = -1;
            OnClearLog();
            _DoUpdateView();

            _SetNotificationStatus(ToolNotificationStatus.None);
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

        void _OnSwitchByType(LogType logType, bool isOn)
        {
            _showLogFlagDictionary[logType] = isOn;
            _DoUpdateView();
        }

        void _OnChangeFilter(string text)
        {
            _filterText = text;
            _resetScroll = true;
            _DoUpdateView();
        }


        void _OnSelectLog(int index)
        {
            if (_hideLogDetailView && (_selectLogIndex == index))
            {
                _selectLogIndex = -1;
            }

            _hideLogDetailView = _selectLogIndex == index;
            _scrollInitLock = true;
            _selectLogIndex = index;
            _DoUpdateView();
        }

        void _SaveLongTapStartLog(int index)
        {
            _longTapStartLogIndex = index;
        }

        void _CopySavedLog()
        {
            _CopyLog(_GetLog(_longTapStartLogIndex).LogDetail);
        }

        void _CopyLog(ILogDetail logDetail)
        {
            string clipboardText = logDetail.GetClipboardText();
            ClipboardModel.Copy(clipboardText);
            OnLogCopied(_GetLog(_longTapStartLogIndex), clipboardText);
            NoaDebugger.ShowToast(new ToastViewLinker {_label = NoaDebuggerDefine.ClipboardCopyText});
            _scrollInitLock = true;
            _DoUpdateView();
        }

        TLogEntry _GetLog(int index)
        {
            if (index < 0 || _logBuffer.Count <= index)
            {
                throw new IndexOutOfRangeException();
            }

            int current = -1;
            TLogEntry target = null;
            foreach (var log in _logBuffer)
            {
                current += 1;
                if (index == current)
                {
                    target = log;
                    break;
                }
            }

            return target;
        }


        void _OnExecDownload(string label, UnityAction<FileDownloader.DownloadInfo> completed)
        {
            DownloadInfo downloadInfo = new LogExportData(GetExportFilenamePrefix()).GetDownloadInfo();
            string fileName = downloadInfo.GetExportFileName(label, DateTime.Now);
            string json = _CreateExportJsonString(label);
            var flag = OnLogDownload(fileName, json);
            if (flag != null && flag == false)
                return;
            LocalDataExportModel.ExportText(fileName, json, completed);
        }

        string _CreateExportJsonString(string label)
        {
            List<KeyValueSerializer> exportData = new List<KeyValueSerializer>();
            KeyValueSerializer logData = CreateLogKeyValueSerializer();
            exportData.Add(logData);

            exportData.Add(KeyValueSerializer.CreateSubData(label));
            return KeyValueSerializer.SerializeToJson(exportData.ToArray());
        }


        protected ToolPinStatus _GetPinStatus()
        {
            return _floatingWindowPresenter.IsActive ? ToolPinStatus.On : ToolPinStatus.Off;
        }

        protected void _TogglePin(Transform parent)
        {
            _floatingWindowPresenter.ToggleActive(parent);
        }

        protected void _InitFloatingWindow(Transform parent)
        {
            var isWindowDrawing = _floatingWindowPresenter.IsActive;
            if (isWindowDrawing)
            {
                _floatingWindowPresenter.InstantiateWindow(parent);
            }
        }

        void _OnInitFloatingWindow(LogFloatingWindowView window)
        {
            window.OnRecord += _OnRecord;
            window.OnClear += _OnClearLogs;
            window.OnDownload += _OnDownload;
            window.OnSwitchByLogType += _OnSwitchByType;
            window._logScrollDrawer.OnChangeFilterText += _OnChangeFilter;
            _DoUpdateView();
        }


        protected void _OnHidden()
        {
            _HideView();
            _selectLogIndex = -1;
            _hideLogDetailView = true;
            _downloadDialogPresenter?.Dispose();
        }


        protected void _OnDispose()
        {
            _OnHidden();
            _logModel?.Destroy();
        }


        public LinkedList<TLogEntry> GetLogList()
        {
            return _logBuffer;
        }

        public void ClearLog()
        {
            _OnClearLogs();
        }

        protected virtual void OnDestroy()
        {
            _logBuffer = default;
            _mainViewPrefab = default;
            _mainView = default;
            _landscapeFloatingWindowPrefab = default;
            _portraitFloatingWindowPrefab = default;
            _floatingWindowPresenter = default;
            _dialogPrefab = default;
            _downloadDialogPresenter = default;
            _settings = default;
            _logs = default;
            _logCounts = default;
            _panels = default;
            _panelInstances = default;
            _onSelectLog = default;
            _onPointerDown = default;
            _onLongTap = default;
            _showLogFlagDictionary = default;
            _filterText = default;
            _logModel = default;
        }
    }
}
