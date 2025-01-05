using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class ConsoleLogPresenter : LogPresenterBase<ConsoleLogEntry, ConsoleLogDetail>, INoaDebuggerTool
    {
        const int LOG_STACKTRACE_EXCEPTION_INDEX = 0; 
#if UNITY_EDITOR || UNITY_WEBGL || UNITY_STANDALONE
        const int LOG_STACKTRACE_OTHER_INDEX = 1; 
#elif !ENABLE_IL2CPP 
        const int LOG_STACKTRACE_OTHER_INDEX = 1;     
#else
        const int LOG_STACKTRACE_OTHER_INDEX = 0;     
#endif

        ToolNotificationStatus _notifyStatus;

        RingBuffer<ConsoleLogEntry> _errorLogBuffer;

        RingBuffer<ConsoleLogEntry> _warningLogBuffer;

        RingBuffer<ConsoleLogEntry> _normalLogBuffer;

        public ToolNotificationStatus NotifyStatus => _notifyStatus;

        public UnityAction<ConsoleLogEntry> _onErrorLogReceived;

        public Func<ConsoleLogEntry, bool> _onFilterErrorNotification;

        public UnityAction<ConsoleLogEntry, string> _onLogCopied;

        public Func<string, string, bool> _onLogDownload;

        protected override string GetToolName()
        {
            return MenuInfo().Name;
        }

        protected override LogCollectorModel CreateLogCollectorModel()
        {
            var model = new ConsoleLogCollectorModel();
            model.OnLogReceived += _ReceiveLogGenerateStackTrace;
            return model;
        }

        public void ReceiveLogGenerateStackTraceFirstLine(string logString, string stackTrace, UnityEngine.LogType type)
        {
            string stackTraceString = string.Empty;
            if (!string.IsNullOrEmpty(stackTrace) && _IsStackTraceLogType(type, StackTraceLogType.ScriptOnly))
            {
                string[] splited = stackTrace.Split(NewLine, StringSplitOptions.RemoveEmptyEntries);
                stackTraceString = splited[0];
            }

            ConsoleLogDetail logDetail = new ConsoleLogDetail(
                logDetailString: ConsoleLogDetail.FormatLogDetail(logString, stackTrace),
                isRegisteredApi : true
            );

            _ReceiveLog(logString, logDetail, type, stackTraceString);
        }

        void _ReceiveLogGenerateStackTrace(string logString, string stackTrace, UnityEngine.LogType type)
        {
            string stackTraceString = _CreateStackTraceString(stackTrace, type);

            ConsoleLogDetail logDetail = new ConsoleLogDetail(
                logDetailString: ConsoleLogDetail.FormatLogDetail(logString, stackTrace),
                isRegisteredApi : false
                );

            _ReceiveLog(logString, logDetail, type, stackTraceString);
        }

        void _ReceiveLog(string logString, ConsoleLogDetail logDetail, UnityEngine.LogType type, string stackTraceString)
        {
            var targetBuffer = _normalLogBuffer;

            LogType noaLogType;
            switch (type)
            {
                case UnityEngine.LogType.Error:
                case UnityEngine.LogType.Exception:
                case UnityEngine.LogType.Assert:
                    noaLogType = LogType.Error;
                    targetBuffer = _errorLogBuffer;
                    break;
                case UnityEngine.LogType.Warning:
                    noaLogType = LogType.Warning;
                    targetBuffer = _warningLogBuffer;
                    break;
                case UnityEngine.LogType.Log:
                default:
                    noaLogType = LogType.Log;
                    break;
            }

            DateTime receivedAt = DateTime.Now;
            string receivedDateTimeString = receivedAt.ToString("HH:mm:ss");
            string logViewStringBody = (logString.Length < NoaDebuggerDefine.ConsoleLogSummaryStringLengthMax)
                ? logString
                : logString.Substring(0, NoaDebuggerDefine.ConsoleLogSummaryStringLengthMax);
            string logViewString = $"[{receivedDateTimeString}] {logViewStringBody}";

            var entry = new ConsoleLogEntry(
                logString,
                stackTraceString,
                logViewString,
                logDetail,
                noaLogType,
                receivedAt);

            if (targetBuffer.IsFull)
            {
                var oldLogEntry = targetBuffer.At(0);
                _logBuffer.Remove(oldLogEntry);
            }

            bool isMatchLog = false;
            if (_logBuffer.Count > 0)
            {
                var prevLog = _logBuffer.Last();

                if (_IsMatchLog(prevLog, entry))
                {
                    prevLog.NumberOfMatchingLogs++;
                    isMatchLog = true;
                }
            }

            if (!isMatchLog)
            {
                targetBuffer.Append(entry);
                _logBuffer.AddLast(entry);
            }

            if (entry.LogType == LogType.Error
                && (_onFilterErrorNotification == null || _onFilterErrorNotification(entry)))
            {
                _SetNotificationStatus(ToolNotificationStatus.Error);
                _onErrorLogReceived?.Invoke(entry);
            }

            _UpdateView();
        }

        bool _IsMatchLog(ConsoleLogEntry target, ConsoleLogEntry source)
        {
            bool isMatch = target.LogString == source.LogString;
            isMatch &= target.StackTraceString == source.StackTraceString;
            return isMatch;
        }

        string _CreateStackTraceString(string stackTrace, UnityEngine.LogType type)
        {
            string stackTraceString = string.Empty;

            if (!string.IsNullOrEmpty(stackTrace) && _IsStackTraceLogType(type, StackTraceLogType.ScriptOnly))
            {
                string[] splited = stackTrace.Split(NewLine, StringSplitOptions.RemoveEmptyEntries);
                int stackTraceMethodCallerIndex = type == UnityEngine.LogType.Exception
                    ? LOG_STACKTRACE_EXCEPTION_INDEX
                    : ConsoleLogPresenter.LOG_STACKTRACE_OTHER_INDEX;
                if (stackTraceMethodCallerIndex >= splited.Length)
                {
                    stackTraceMethodCallerIndex = splited.Length - 1;
                }

                stackTraceString = splited[stackTraceMethodCallerIndex];
            }

            return stackTraceString;
        }

        bool _IsStackTraceLogType(UnityEngine.LogType logType, StackTraceLogType stackTraceLogType)
        {
            return stackTraceLogType == Application.GetStackTraceLogType(logType);
        }

        protected override void _SetNotificationStatus(ToolNotificationStatus notifyStatus)
        {
            _notifyStatus = notifyStatus;
            NoaDebuggerManager.OnChangeNotificationStatus<ConsoleLogPresenter>(_notifyStatus);
        }

        protected override int GetLogCapacity()
        {
            var capacity = _settings ? _settings.ConsoleLogCount : NoaDebuggerDefine.CONSOLE_LOG_COUNT_DEFAULT;

            if (capacity < NoaDebuggerDefine.ConsoleLogCountMin || capacity > NoaDebuggerDefine.ConsoleLogCountMax)
            {
                capacity = NoaDebuggerDefine.CONSOLE_LOG_COUNT_DEFAULT;
            }

            return capacity;
        }

        protected override void SetLogPanelInfo(ref LogViewLinker.LogPanelInfo logPanelInfo, ConsoleLogEntry log)
        {
            logPanelInfo._logString = log.LogViewString;
            logPanelInfo._stackTraceString = log.StackTraceString;
            logPanelInfo._logDetail = log.LogDetail;
            logPanelInfo._logType = log.LogType;
            logPanelInfo._receivedTime = log.ReceivedAt;
        }

        protected override string GetStatusString() => string.Empty;

        protected override void OnClearLog() { }

        protected override void OnLogCopied(ConsoleLogEntry log, string clipboardText)
        {
            _onLogCopied?.Invoke(log, clipboardText);
        }

        protected override bool? OnLogDownload(string fileName, string json)
        {
            if (_onLogDownload == null)
                return null;

            return _onLogDownload.Invoke(fileName, json);
        }

        protected override string GetExportFilenamePrefix() => "consolelog";

        protected override string GetWindowInfoPrefsKey() => NoaDebuggerPrefsDefine.PrefsKeyIsConsoleLogWindowInfo;

        protected override KeyValueSerializer CreateLogKeyValueSerializer()
        {
            var logCount = new Dictionary<LogType, int>
            {
                {LogType.Log, 0},
                {LogType.Warning, 0},
                {LogType.Error, 0}
            };

            var objectParsers = new KeyValueArrayParser.ObjectParser[_logBuffer.Count];
            var objectParserIndex = 0;
            foreach (ConsoleLogEntry log in _logBuffer)
            {
                var parsers = new IKeyValueParser[4];
                parsers[0] = new KeyValueParser(nameof(log.LogString), log.LogString);
                string detailString = log.LogDetail.LogDetailString;
                parsers[1] = new ArrayParser("_stackTrace", detailString.Split("\n", StringSplitOptions.RemoveEmptyEntries));
                parsers[2] = new KeyValueParser(nameof(log.LogType), log.LogType.ToString());
                parsers[3] = new KeyValueParser(nameof(log.ReceivedAt), log.ReceivedAt.ToString("yyyyMMdd-HHmmss"));
                objectParsers[objectParserIndex] = new KeyValueArrayParser.ObjectParser(parsers);
                logCount[log.LogType] += 1;
                ++objectParserIndex;
            }

            var logs = new KeyValueArrayParser("_logs", objectParsers);

            var messageCount = new KeyValueParser("_messageCount", logCount[LogType.Log].ToString());
            var warningCount = new KeyValueParser("_warningCount", logCount[LogType.Warning].ToString());
            var errorCount = new KeyValueParser("_errorCount", logCount[LogType.Error].ToString());

            return new KeyValueSerializer("LogData", new IKeyValueParser[] {logs, messageCount, warningCount, errorCount});
        }

        public void Init()
        {
            _Init();
            _normalLogBuffer = new RingBuffer<ConsoleLogEntry>(GetLogCapacity());
            _warningLogBuffer = new RingBuffer<ConsoleLogEntry>(GetLogCapacity());
            _errorLogBuffer = new RingBuffer<ConsoleLogEntry>(GetLogCapacity());
        }

        class ConsoleLogMenuInfo : IMenuInfo
        {
            public string Name => "ConsoleLog";
            public string MenuName => "ConsoleLog";
            public int SortNo => NoaDebuggerDefine.CONSOLE_LOG_MENU_SORT_NO;
        }

        ConsoleLogMenuInfo _consoleLogMenuInfo;

        public IMenuInfo MenuInfo()
        {
            if (_consoleLogMenuInfo == null)
            {
                _consoleLogMenuInfo = new ConsoleLogMenuInfo();
            }

            return _consoleLogMenuInfo;
        }

        public void ShowView(Transform parent) => _ShowView(parent);

        public ToolPinStatus GetPinStatus() => _GetPinStatus();

        public void TogglePin(Transform parent) => _TogglePin(parent);

        public void InitFloatingWindow(Transform parent) => _InitFloatingWindow(parent);

        public void AlignmentUI(bool isReverse) => _AlignmentMainView(isReverse);

        public void OnHidden() => _OnHidden();

        public void OnToolDispose()
        {
            _OnDispose();
            _onErrorLogReceived = null;
            _onLogCopied = null;
            _onLogDownload = null;
        }

        protected override void OnDestroy()
        {
            _errorLogBuffer = default;
            _warningLogBuffer = default;
            _normalLogBuffer = default;
            _onErrorLogReceived = default;
            _onLogCopied = default;
            _onLogDownload = default;
            _consoleLogMenuInfo = default;

            base.OnDestroy();
        }
    }
}
