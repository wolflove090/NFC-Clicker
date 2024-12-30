using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class SnapshotView : NoaDebuggerToolViewBase<SnapshotViewLinker>
    {
        const int TOGGLE_SELECTABLE_MAX_COUNT = 2;

        [SerializeField]
        NoaDebuggerDisableButton _allClearButton;

        [SerializeField]
        NoaDebuggerDisableButton _downloadButton;

        [SerializeField]
        TextMeshProUGUI _elapsedTimeText;

        [SerializeField]
        NoaDebuggerDisableButton _takeButton;

        [SerializeField]
        NoaDebuggerDisableButton _comparisonButton;

        [Header("Scroll")]
        [SerializeField]
        ObjectPoolScroll _scroll;

        [SerializeField]
        GameObject _scrollEmptyObject;

        [SerializeField, Header("Log details")]
        GameObject _logDetailsArea;
        [SerializeField]
        GameObject _dummy; 
        [SerializeField]
        SnapshotLogDetailView _logDetailA;
        [SerializeField]
        SnapshotLogDetailView _logDetailB;

        int _currentListCount = -1;
        List<SnapshotLogRecordInformation> _logDataList;

        public event UnityAction OnInsertLog;

        public event UnityAction<int> OnToggleChanged;

        public event UnityAction OnClearAllLog;

        public event UnityAction<int, string> OnUpdateLogLabel;

        public event UnityAction<int> OnClickLog;

        public event UnityAction<int> OnLongTapLog;

        public event UnityAction OnDownloadLog;

        public event UnityAction OnComparison;

        public event UnityAction<int> OnClickCopyButton;

        protected override void _Init()
        {
            base._Init();

            _allClearButton.onClick.RemoveAllListeners();
            _allClearButton.onClick.AddListener(_OnClickAllClearButton);
            _downloadButton.onClick.RemoveAllListeners();
            _downloadButton.onClick.AddListener(_OnClickDownloadButton);
            _takeButton.onClick.RemoveAllListeners();
            _takeButton.onClick.AddListener(_OnClickTakeButton);
            _comparisonButton.onClick.RemoveAllListeners();
            _comparisonButton.onClick.AddListener(_OnClickComparisonButton);

            _comparisonButton.Interactable = false;

            _RefreshLogDetailView(false);
        }

        protected override void _OnShow(SnapshotViewLinker linker)
        {
            if (linker._logList != null)
            {
                _ShowByLogDataList(linker._logList, linker._isNeedResetScrollPos);
            }
            bool isComparison = linker._isComparison;
            if (_logDataList.Count == 0)
            {
                _RefreshLogDetailView(false);
                _comparisonButton.Interactable = false;
            }
            else if(!isComparison)
            {
                bool isViewLogDetail = false;
                var selectLog = _logDataList.Where(logData => logData.IsSelected).FirstOrDefault();
                if (selectLog != null)
                {
                    isViewLogDetail = true;
                    UpdateSnapshotDetail(linker);
                }
                _RefreshLogDetailView(isViewLogDetail);
                _logDetailB.gameObject.SetActive(false);
                _comparisonButton.Interactable = _isToggleSelectMax();
            }

            if (isComparison)
            {
                _RefreshLogDetailView(true);
                _comparisonButton.Interactable = true;
                _logDetailB.gameObject.SetActive(true);
                _dummy.gameObject.SetActive(false);
                UpdateSnapshotDetail(linker, true);
            }
        }

        public void ShowOnUpdate(SnapshotViewLinker linker)
        {
            if (linker._elapsedTime != null)
            {
                _ShowByElapsedTime(linker._elapsedTime.Value);
            }
        }

        void _RefreshLogDetailView(bool isView)
        {
            _logDetailsArea.gameObject.SetActive(isView);
            _dummy.gameObject.SetActive(isView);
            _logDetailA.gameObject.SetActive(isView);
            _logDetailB.gameObject.SetActive(isView);
        }

        void UpdateSnapshotDetail(SnapshotViewLinker linker, bool isComparison = false)
        {
            _logDetailA.OnClickCopyButton = OnClickCopyButton;
            _logDetailA.Show(linker);
            if (isComparison)
            {
                _logDetailB.OnClickCopyButton = OnClickCopyButton;
                _logDetailB.Show(linker);
            }
        }

        void _OnClickAllClearButton()
        {
            OnClearAllLog?.Invoke();
        }

        void _OnClickDownloadButton()
        {
            OnDownloadLog?.Invoke();
        }

        void _OnClickTakeButton()
        {
            OnInsertLog?.Invoke();
        }

        void _OnClickComparisonButton ()
        {
            OnComparison?.Invoke();
        }

        void _RefreshPanel(int index, GameObject target)
        {
            var cell = target.GetComponent<SnapshotLogCellView>();
            if (cell == null)
            {
                return;
            }

            SnapshotLogRecordInformation log = _logDataList[index];
            var cellViewLinker = new SnapshotLogCellViewLinker();
            cellViewLinker._id = log.Id;
            cellViewLinker._viewIndex = index;
            cellViewLinker._label = log.Label;
            cellViewLinker._time = log.Time;
            cellViewLinker._elapsedTime = log.ElapsedTime;
            cellViewLinker._isHighlighted = log.IsHighlighted;
            cellViewLinker._onClickCell = OnClickLog;
            cellViewLinker._onLongTapCell = OnLongTapLog;
            cellViewLinker._onUpdateLabel = OnUpdateLogLabel;
            cellViewLinker._onToggleChanged = OnToggleChanged;
            cellViewLinker._toggleState = _ToggleState(log);
            cellViewLinker._backgroundColor = log.BackgroundColor;
            cell.Show(cellViewLinker);
        }

        SnapshotModel.ToggleState _ToggleState (SnapshotLogRecordInformation record)
        {
            if (record.ToggleState != SnapshotModel.ToggleState.None)
            {
                return record.ToggleState;
            }

            bool hasAdditionalInfo = record.AdditionalInfo != null && record.AdditionalInfo.Count > 0;
            if (_isToggleSelectMax() || (record.Snapshot == null && !hasAdditionalInfo))
            {
                return SnapshotModel.ToggleState.Disabled;
            }

            return SnapshotModel.ToggleState.None;
        }

        void _ShowByElapsedTime(TimeSpan elapsedTime)
        {
            _elapsedTimeText.text = SnapshotPresenter.TimeSpanToHourTimeFormatString(elapsedTime);
        }

        void _ShowByLogDataList(List<SnapshotLogRecordInformation> logDataList, bool isNeedResetScrollPos)
        {
            int beforeCount = _currentListCount;

            _logDataList = logDataList;
            _currentListCount = _logDataList.Count;
            bool hasLogData = _currentListCount > 0;

            _allClearButton.Interactable = hasLogData;
            _downloadButton.Interactable = hasLogData;

            _scrollEmptyObject.SetActive(!hasLogData);

            if (_currentListCount != beforeCount)
            {
                _scroll.Init(_logDataList.Count, _RefreshPanel);
            }
            else
            {
                _scroll.RefreshPanels();
            }

            if (isNeedResetScrollPos)
            {
                _scroll.verticalNormalizedPosition = 0;
            }
        }

        bool _isToggleSelectMax()
        {
            var toggleSelectedCount = _logDataList.Count(
                log => log.ToggleState == SnapshotModel.ToggleState.SelectedFirst ||
                       log.ToggleState == SnapshotModel.ToggleState.SelectedSecond);
            return TOGGLE_SELECTABLE_MAX_COUNT == toggleSelectedCount;
        }
    }

    sealed class SnapshotViewLinker : ViewLinkerBase
    {
        public int _index = -1;

        public bool? _isSpan = null;

        public TimeSpan? _elapsedTime = null;

        public List<SnapshotLogRecordInformation> _logList = null;

        public DateTime _captureTime;

        public ProfilerSnapshotData _snapshot;

        public Vector2 _scrollPosition = new Vector2(0, 1f);

        public bool _isNeedResetScrollPos = false;

        public bool _isComparison = false;
    }
}
