using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace NoaDebugger
{
    class LogViewBase : NoaDebuggerToolViewBase<LogViewLinker>
    {
        [SerializeField, Header("Log status")]
        GameObject _logStatusRoot;
        [SerializeField]
        TextMeshProUGUI _logStatus;

        [SerializeField, Header("Log details display route")]
        GameObject _logDetailScroll;

        [SerializeField, Header("Button")]
        ToggleButtonBase _record;
        [SerializeField]
        NoaDebuggerDisableButton _clear;
        [SerializeField]
        NoaDebuggerDisableButton _download;

        [SerializeField]
        public LogScrollDrawer _logScrollDrawer;

        [SerializeField]
        public LogSwitchToggleDrawer _logSwitchToggleDrawer;


        public event UnityAction OnRecord;

        public event UnityAction OnClear;

        public event UnityAction OnDownload;

        public UnityAction<ILogDetail> OnCopy { get; set; }


        void _OnValidateUI()
        {
            Assert.IsNotNull(_logStatusRoot);
            Assert.IsNotNull(_logStatus);
            Assert.IsNotNull(_logDetailScroll);
            Assert.IsNotNull(_record);
            Assert.IsNotNull(_clear);
            Assert.IsNotNull(_download);
        }

        protected override void _Init()
        {
            _OnValidateUI();
            _record._onClick.RemoveAllListeners();
            _record._onClick.AddListener(_OnRecord);
            _clear.onClick.RemoveAllListeners();
            _clear.onClick.AddListener(_OnClear);
            _download.onClick.RemoveAllListeners();
            _download.onClick.AddListener(_OnDownload);
            _OnInit();
        }

        protected virtual void _OnInit() { }

        protected virtual void _OnUpdateDetail(ILogDetail detail) { }

        protected override void _OnShow(LogViewLinker linker)
        {
            var logs = linker._logs;
            bool logExists = logs is {Count: > 0};

            if (logExists)
            {
                _RefreshLogDetailView(linker._isViewLogDetail);

                if (linker._selectLogIndex >= 0 && linker._selectLogIndex < logs.Count)
                {
                    var selectLog = logs[linker._selectLogIndex];
                    _OnUpdateDetail(selectLog._logDetail);
                }
            }

            _logScrollDrawer.Draw(linker);

            if (string.IsNullOrEmpty(linker._logStatus))
            {
                _logStatusRoot.SetActive(false);
            }
            else
            {
                _logStatusRoot.SetActive(true);
                _logStatus.text = linker._logStatus;
            }

            _record.Init(linker._isCollecting);

            int logSum = linker._logTypeToggles._messageNum
                         + linker._logTypeToggles._warningNum
                         + linker._logTypeToggles._errorNum;

            bool isShowDownload = logSum > 0;
            _download.Interactable = isShowDownload;

            _logSwitchToggleDrawer.Draw(linker._logTypeToggles);
        }

        void _RefreshLogDetailView(bool isView)
        {
            _logDetailScroll.gameObject.SetActive(isView);
        }


        void _OnRecord(bool isOn)
        {
            OnRecord?.Invoke();
        }

        void _OnClear()
        {
            OnClear?.Invoke();
        }

        void _OnDownload()
        {
            OnDownload?.Invoke();
        }

        void OnDestroy()
        {
            _logStatusRoot = default;
            _logStatus = default;
            _logDetailScroll = default;
            _record = default;
            _clear = default;
            _download = default;
            _logScrollDrawer = default;
            _logSwitchToggleDrawer = default;
            OnRecord = default;
            OnClear = default;
            OnDownload = default;
            OnCopy = default;
        }
    }

    sealed class LogViewLinker : ViewLinkerBase
    {
        public bool _forceUpdate;

        public List<LogPanelInfo> _logs;

        public string _logStatus;

        public int _selectLogIndex;

        public bool _isCollecting;

        public LogTypeToggles _logTypeToggles;

        public bool _resetScrollPos;

        public bool _isViewLogDetail;

        public string _filterText;

        public struct LogTypeToggles
        {
            public bool _messageToggle;

            public bool _warningToggle;

            public bool _errorToggle;

            public int _messageNum;

            public int _warningNum;

            public int _errorNum;
        }

        public class LogPanelInfo
        {
            public int _index;

            public int _viewIndex;

            public bool _isSelect;

            public string _logString;

            public string _stackTraceString;

            public ILogDetail _logDetail;

            public LogType _logType;

            public DateTime _receivedTime;

            public UnityAction<int> _onClick;

            public UnityAction<int> _onPointerDown;

            public UnityAction _onLongTap;

            public int _numberOfMatchingLogs;
        }
    }
}
