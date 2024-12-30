using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class SnapshotModel : ModelBase
    {
        static readonly string SnapshotModelOnUpdate = "SnapshotModelOnUpdate";

        static readonly int SnapshotLogCountLimit = 999;

        int _logIdCounter = 0;

        int _logNumberCounter = 0;

        int _beforeHighlightedLogId = 0;

        int _firstToggleCheckedLogId = 0;

        int _secondToggleCheckedLogId = 0;

        public SnapshotInformation SnapshotInfo { get; private set; }

        public UnityAction OnTimeDataUpdated { get; set; }

        public SnapshotModel()
        {
            SnapshotInfo = new SnapshotInformation();
            SnapshotInfo._logList = new List<SnapshotLogRecordInformation>();
            UpdateProfilingStartTime();
        }

        public void Dispose()
        {
            UpdateManager.DeleteAction(SnapshotModelOnUpdate);
        }

        public void SetUpdateAction()
        {
            UpdateManager.AddOrOverwriteAction(SnapshotModelOnUpdate, _OnUpdate);
        }

        public void DeleteUpdateAction()
        {
            UpdateManager.DeleteAction(SnapshotModelOnUpdate);
        }

        void _OnUpdate()
        {
            float totalElapsedTimeSeconds = Time.realtimeSinceStartup;

            int elapsedTimeMilliseconds = Mathf.FloorToInt((totalElapsedTimeSeconds - SnapshotInfo._profilingStartTime) * 1000);
            if (SnapshotInfo._elapsedTimeMilliseconds < elapsedTimeMilliseconds)
            {
                SnapshotInfo._elapsedTimeMilliseconds = elapsedTimeMilliseconds;
                SnapshotInfo._elapsedTime = TimeSpan.FromMilliseconds(elapsedTimeMilliseconds);
            }
            OnTimeDataUpdated?.Invoke();
        }

        public void UpdateProfilingStartTime()
        {
            SnapshotInfo._profilingStartTime = Time.realtimeSinceStartup;
            SnapshotInfo._elapsedTimeMilliseconds = 0;
        }

        public void UpdateProfilingElapsedTime()
        {
            _OnUpdate();
        }

        public void ChangeComparisonState(bool forceUpdate = false, bool isComparison = false)
        {
            isComparison = forceUpdate ? isComparison : !SnapshotInfo._isComparison;
            SnapshotInfo._isComparison = isComparison;
        }

        public void InsertLog(ProfilerSnapshotData snapshotData, string label = null, Color? backgroundColor = null, Dictionary<string, NoaSnapshotCategory> additionalInfo = null)
        {
            _logIdCounter++;

            _logNumberCounter++;

            var elapsedTime = TimeSpan.Zero;
            if (_logNumberCounter > 1)
            {
                var prevLog = SnapshotInfo._logList.Last();
                elapsedTime = SnapshotInfo._elapsedTime - prevLog.Time;
            }
            var labelReplaced = label ?? $"No.{_logNumberCounter}";
            if (labelReplaced.Length > NoaDebuggerDefine.SnapshotLogMaxLabelCharNum)
            {
                labelReplaced = labelReplaced.Substring(0, NoaDebuggerDefine.SnapshotLogMaxLabelCharNum);
            }
            SnapshotLogRecordInformation record = new SnapshotLogRecordInformation
            (
                _logIdCounter,
                labelReplaced,
                SnapshotInfo._elapsedTime,
                elapsedTime,
                snapshotData,
                backgroundColor != null ? backgroundColor.Value : (Color?) null,
                additionalInfo != null ? new Dictionary<string, NoaSnapshotCategory>(additionalInfo) : null
            );
            SnapshotInfo._logList.Add(record);
            _OnLogInfoUpdated();
        }

        public void RemoveAtLog(int index)
        {
            if (SnapshotInfo._logList.Count < index)
            {
                return;
            }
            SnapshotLogRecordInformation record = SnapshotInfo._logList[index];
            if (record.ToggleState == ToggleState.SelectedFirst || record.ToggleState == ToggleState.SelectedSecond)
            {
                SetCheckedLogId(record.Id);
            }
            SnapshotInfo._logList.RemoveAt(index);
        }

        public void ClearAllLog()
        {
            SnapshotInfo._logList.Clear();
            _OnLogInfoUpdated();
        }

        public void ClearAllLogSelected()
        {
            foreach (var record in SnapshotInfo._logList)
            {
                if (record.IsSelected)
                {
                    _SetLogHighlight(record.Id, false);
                }
                record.SetSelected(false);
            }
        }

        public void ClearAllLogChecked()
        {
            foreach (var record in SnapshotInfo._logList)
            {
                record.SetToggleState(ToggleState.None);
            }

            _firstToggleCheckedLogId = 0;
            _secondToggleCheckedLogId = 0;
            SnapshotInfo._isComparison = false;
        }

        void _OnLogInfoUpdated()
        {
            if (!SnapshotInfo._logList.Any())
            {
                _logNumberCounter = 0;
            }

            if (SnapshotInfo._logList.Count > SnapshotLogCountLimit)
            {
                RemoveAtLog(0);
            }
        }

        public void UpdateLogLabel(int logId, string text)
        {
            SnapshotLogRecordInformation record = SnapshotInfo._logList.FirstOrDefault(x => x.Id == logId);
            if (record == null)
            {
                return;
            }
            record.SetLabel(text);
        }

        public void SetHighlightLogId(int logId)
        {
            if (_beforeHighlightedLogId == logId)
            {
                return;
            }

            _SetLogHighlight(logId, isHighlighted:true);
            _SetLogHighlight(_beforeHighlightedLogId, isHighlighted:false);

            _beforeHighlightedLogId = logId;
        }

        public void SetSelectedLogId (int logId)
        {
            foreach (var record in SnapshotInfo._logList)
            {
                if (record.IsSelected && record.Id != logId)
                {
                    record.SetSelected(false);
                }

                if (record.Id == logId)
                {
                    record.SetSelected(!record.IsSelected); 
                }
            }
        }

        public void SetCheckedLogId (int logId)
        {
            SnapshotLogRecordInformation firstLog = SnapshotInfo._logList.FirstOrDefault(x => x.Id == _firstToggleCheckedLogId);
            SnapshotLogRecordInformation secondLog = SnapshotInfo._logList.FirstOrDefault(x => x.Id == _secondToggleCheckedLogId);

            bool isToggleOn = _firstToggleCheckedLogId != logId && _secondToggleCheckedLogId != logId;
            foreach (var record in SnapshotInfo._logList)
            {
                if (record.Id == logId)
                {
                    if (isToggleOn)
                    {
                        if (firstLog == null)
                        {
                            record.SetToggleState(ToggleState.SelectedFirst);
                            _firstToggleCheckedLogId = logId;
                        }
                        else if (secondLog == null)
                        {
                            record.SetToggleState(ToggleState.SelectedSecond);
                            _secondToggleCheckedLogId = logId;
                        }
                    }
                    else
                    {
                        if (record.ToggleState == ToggleState.SelectedFirst)
                        {
                            if (secondLog != null)
                            {
                                secondLog.SetToggleState(ToggleState.SelectedFirst);
                                _firstToggleCheckedLogId = secondLog.Id;
                            }
                            else
                            {
                                _firstToggleCheckedLogId = 0;
                            }
                        }
                        if (record.ToggleState == ToggleState.SelectedSecond)
                        {
                            firstLog.SetToggleState(ToggleState.SelectedFirst);
                        }
                        record.SetToggleState(ToggleState.None);
                        _secondToggleCheckedLogId = 0;
                    }
                }
            }
        }

        void _SetLogHighlight(int logId, bool isHighlighted)
        {
            SnapshotLogRecordInformation record = SnapshotInfo._logList.FirstOrDefault(x => x.Id == logId);
            if (record == null)
            {
                return;
            }

            if (!isHighlighted && record.IsHighlighted)
            {
                _beforeHighlightedLogId = 0;
            }

            record.SetHighlighted(isHighlighted);
        }

        public enum ToggleState
        {
            None,
            SelectedFirst,
            SelectedSecond,
            Disabled,
        }

        public sealed class SnapshotInformation
        {
            public bool _isComparison = false;

            public float _profilingStartTime = 0f;

            public int _elapsedTimeMilliseconds;

            public TimeSpan _elapsedTime;

            public List<SnapshotLogRecordInformation> _logList;
        }
    }

}
