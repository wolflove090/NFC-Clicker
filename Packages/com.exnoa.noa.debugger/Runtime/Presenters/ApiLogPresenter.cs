using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class ApiLogPresenter : LogPresenterBase<ApiLogEntry, ApiLog>, INoaDebuggerTool
    {
        const int LOG_CATEGORY_COUNT = 2;
        const string DETAIL_HEADER_PREFIX = "[";
        const string DETAIL_HEADER_SUFFIX = "]";

        public const string DETAIL_HEADER_GENERAL = "General";
        public const string DETAIL_CONTENT_URL = "URL";
        public const string DETAIL_CONTENT_REQUEST_METHOD = "Request Method";
        public const string DETAIL_CONTENT_STATUS_CODE = "Status Code";
        public const string DETAIL_HEADER_REQUEST_HEADERS = "Request Headers";
        public const string DETAIL_HEADER_REQUEST_BODY = "Request Body";
        public const string DETAIL_HEADER_RESPONSE_HEADERS = "Response Headers";
        public const string DETAIL_HEADER_RESPONSE_BODY = "Response Body";

        struct ApiLogAttributes
        {
            public long _contentSize;
            public long _responseTimeMilliSeconds;
            public LogType _logType;
            public long _totalResponseTimeMilliSeconds;
        }

        RingBuffer<ApiLogAttributes> _logAttributes;
        int _requestCount = 0;
        long _totalReceivedBytes = 0;
        long _totalResponseTimeMilliSeconds = 0;
        ToolNotificationStatus _notifyStatus;

        RingBuffer<ApiLogEntry> _errorLogBuffer;

        RingBuffer<ApiLogEntry> _normalLogBuffer;

        public ToolNotificationStatus NotifyStatus => _notifyStatus;

        public UnityAction<ApiLogEntry> _onErrorLogReceived;

        public UnityAction<ApiLogEntry, string> _onLogCopied;

        public Func<string, string, bool> _onLogDownload;

        protected override string GetToolName()
        {
            return MenuInfo().Name;
        }

        protected override LogCollectorModel CreateLogCollectorModel()
        {
            var model = new ApiLogCollectorModel();
            model.OnLogReceived += _ReceiveLog;
            return model;
        }

        protected override int GetLogCapacity()
        {
            var capacity = _settings ? _settings.ApiLogCount : NoaDebuggerDefine.API_LOG_COUNT_DEFAULT;

            if (capacity < NoaDebuggerDefine.ApiLogCountMin || capacity > NoaDebuggerDefine.ApiLogCountMax)
            {
                capacity = NoaDebuggerDefine.API_LOG_COUNT_DEFAULT;
            }

            return capacity;
        }

        protected override void SetLogPanelInfo(ref LogViewLinker.LogPanelInfo logPanelInfo, ApiLogEntry log)
        {
            logPanelInfo._logString = log.LogViewString;
            logPanelInfo._stackTraceString = ""; 
            logPanelInfo._logDetail = log.LogDetail;
            logPanelInfo._logType = log.LogType;
            logPanelInfo._receivedTime = log.ReceivedAt;
        }

        protected override string GetStatusString()
        {
            return $"Request: {_requestCount} | Size: {DataUnitConverterModel.ToHumanReadableBytes(_totalReceivedBytes)} | Time: {_totalResponseTimeMilliSeconds} ms";
        }

        protected override void OnClearLog()
        {
            _requestCount = 0;
            _totalReceivedBytes = 0;
            _totalResponseTimeMilliSeconds = 0;
            _logAttributes.Clear();
            _normalLogBuffer.Clear();
            _errorLogBuffer.Clear();
        }

        protected override void OnLogCopied(ApiLogEntry log, string clipboardText)
        {
            _onLogCopied?.Invoke(log, clipboardText);
        }

        protected override bool? OnLogDownload(string fileName, string json)
        {
            if (_onLogDownload == null)
                return null;

            return _onLogDownload.Invoke(fileName, json);
        }

        protected override string GetExportFilenamePrefix() => "apilog";

        protected override string GetWindowInfoPrefsKey() => NoaDebuggerPrefsDefine.PrefsKeyIsApiLogWindowInfo;

        protected override KeyValueSerializer CreateLogKeyValueSerializer()
        {
            var logCount = new Dictionary<LogType, int>
            {
                {LogType.Log, 0},
                {LogType.Error, 0}
            };

            var objectParsers = new KeyValueArrayParser.ObjectParser[_logBuffer.Count];
            var objectParserIndex = 0;
            foreach (ApiLogEntry log in _logBuffer)
            {
                var logElementParsers = new List<IKeyValueParser>();

                string[] headerElements = log.LogString.Split(' ', 2);
                logElementParsers.Add(new KeyValueParser("_apiPath", headerElements[0]));
                logElementParsers.Add(new KeyValueParser("_contentSize", $"{_logAttributes.At(objectParserIndex)._contentSize * log.NumberOfMatchingLogs}"));
                logElementParsers.Add(new KeyValueParser("_responseTimeMilliSeconds", $"{_logAttributes.At(objectParserIndex)._totalResponseTimeMilliSeconds}"));

                string logDetailString = ApiLogPresenter.CreateLogDetailString(log.LogDetail);
                logElementParsers = ApiLogPresenter.AddLogDetailParsers(logElementParsers, logDetailString);

                logElementParsers.Add(new KeyValueParser(nameof(log.LogType), log.LogType.ToString()));

                logElementParsers.Add(new KeyValueParser(nameof(log.ReceivedAt), log.ReceivedAt.ToString("yyyyMMdd-HHmmss")));

                objectParsers[objectParserIndex] = new KeyValueArrayParser.ObjectParser(logElementParsers.ToArray());
                logCount[log.LogType] += 1;
                ++objectParserIndex;
            }

            var logs = new KeyValueArrayParser("_logs", objectParsers);

            var successCount = new KeyValueParser("_successCount", logCount[LogType.Log].ToString());
            var errorCount = new KeyValueParser("_errorCount", logCount[LogType.Error].ToString());
            var requestCount = new KeyValueParser("_requestCount", $"{_requestCount}");
            var totalReceivedBytes = new KeyValueParser("_totalReceivedBytes", $"{_totalReceivedBytes}");
            var totalResponseTimeMilliSeconds = new KeyValueParser("_totalResponseTimeMilliSeconds", $"{_totalResponseTimeMilliSeconds}");

            return new KeyValueSerializer(
                "LogData",
                new IKeyValueParser[]
                {
                    logs,
                    successCount,
                    errorCount,
                    requestCount,
                    totalReceivedBytes,
                    totalResponseTimeMilliSeconds
                });
        }

        public void Init()
        {
            _Init();

            _logAttributes = new RingBuffer<ApiLogAttributes>(GetLogCapacity() * LOG_CATEGORY_COUNT);
            _normalLogBuffer = new RingBuffer<ApiLogEntry>(GetLogCapacity());
            _errorLogBuffer = new RingBuffer<ApiLogEntry>(GetLogCapacity());
        }

        class ApiLogMenuInfo : IMenuInfo
        {
            public string Name => "APILog";
            public string MenuName => "APILog";
            public int SortNo => NoaDebuggerDefine.API_LOG_MENU_SORT_NO;
        }

        ApiLogMenuInfo _apiLogMenuInfo;

        public IMenuInfo MenuInfo()
        {
            if (_apiLogMenuInfo == null)
            {
                _apiLogMenuInfo = new ApiLogMenuInfo();
            }

            return _apiLogMenuInfo;
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

        void _ReceiveLog(ApiLog log)
        {
            DateTime receivedAt = DateTime.Now;
            string logViewString = $"[{receivedAt:HH:mm:ss}] {log.Url.PathAndQuery} ({DataUnitConverterModel.ToHumanReadableBytes(log.ContentSize)} / {log.ResponseTimeMilliSeconds} ms)";

            var entry = new ApiLogEntry(
                logString: log.Url.PathAndQuery,
                logViewString: logViewString,
                logDetail: log,
                logType: log.IsSuccess ? LogType.Log : LogType.Error,
                receivedAt: receivedAt);

            var targetBuffer = entry.LogType == LogType.Log ? _normalLogBuffer : _errorLogBuffer;
            if (targetBuffer.IsFull)
            {
                var oldLogEntry = targetBuffer.At(0);
                var mathingLogs = oldLogEntry.NumberOfMatchingLogs;
                _logBuffer.Remove(oldLogEntry);

                int index = 0;
                ApiLogAttributes top = _GetApiLogAttribute(entry, ref index);
                _totalReceivedBytes -= top._contentSize * mathingLogs;
                _totalResponseTimeMilliSeconds -= top._totalResponseTimeMilliSeconds;
                _logAttributes.Remove(index);
                _requestCount -= mathingLogs;
            }

            bool isMatchLog = false;
            if (_logBuffer.Count > 0)
            {
                var prevLog = _logBuffer.Last();

                if (_IsMatchLog(prevLog.LogDetail, entry.LogDetail))
                {
                    prevLog.NumberOfMatchingLogs++;
                    int index = 0;
                    ApiLogAttributes top = _GetApiLogAttribute(prevLog, ref index);
                    top._totalResponseTimeMilliSeconds += log.ResponseTimeMilliSeconds;
                    isMatchLog = true;
                }
            }

            if (!isMatchLog)
            {
                targetBuffer.Append(entry);
                _logBuffer.AddLast(entry);
            }
            ++_requestCount;
            _totalReceivedBytes += log.ContentSize;
            _totalResponseTimeMilliSeconds += log.ResponseTimeMilliSeconds;

            if (!isMatchLog)
            {
                var logAttributes = new ApiLogAttributes
                {
                    _contentSize = log.ContentSize,
                    _responseTimeMilliSeconds = log.ResponseTimeMilliSeconds,
                    _logType = entry.LogType,
                    _totalResponseTimeMilliSeconds = log.ResponseTimeMilliSeconds
                };
                _logAttributes.Append(logAttributes);
            }

            if (entry.LogType == LogType.Error)
            {
                _SetNotificationStatus(ToolNotificationStatus.Error);
                _onErrorLogReceived?.Invoke(entry);
            }

            _UpdateView();
        }

        bool _IsMatchLog(ApiLog target, ApiLog source)
        {
            bool isMatch = target.StatusCode == source.StatusCode;
            isMatch &= target.Method == source.Method;
            isMatch &= target.Url == source.Url;
            isMatch &= target.ContentSize == source.ContentSize;
            isMatch &= target.RequestBodyRawData == null && source.RequestBodyRawData == null &&
                       target.RequestBody == source.RequestBody;
            if (target.RequestHeaders != null && source.RequestHeaders != null && target.RequestHeaders.Count == source.RequestHeaders.Count)
            {
                var targetDictionaryToString = String.Join("", target.RequestHeaders.Select(header => header.Key + header.Value));
                var sourceDictionaryToString = String.Join("", target.RequestHeaders.Select(header => header.Key + header.Value));
                isMatch &= targetDictionaryToString == sourceDictionaryToString;
            }
            return isMatch;
        }

        ApiLogAttributes _GetApiLogAttribute(ApiLogEntry target, ref int index)
        {
            ApiLogAttributes top = _logAttributes.At(index);
            while (top._logType != target.LogType)
            {
                index++;
                top = _logAttributes.At(index);
            }

            return top;
        }

        protected override void _SetNotificationStatus(ToolNotificationStatus notifyStatus)
        {
            _notifyStatus = notifyStatus;
            NoaDebuggerManager.OnChangeNotificationStatus<ApiLogPresenter>(_notifyStatus);
        }

        public static string CreateLogDetailString(ApiLog log)
        {
            log.ConvertBody();

            string toHeaderString(string text) => $"{ApiLogPresenter.DETAIL_HEADER_PREFIX}{text}{ApiLogPresenter.DETAIL_HEADER_SUFFIX}";

            StringBuilder logDetailsBuilder = new();
            logDetailsBuilder.AppendLine(toHeaderString(ApiLogPresenter.DETAIL_HEADER_GENERAL))
                             .AppendLine($"{ApiLogPresenter.DETAIL_CONTENT_URL}: {log.Url}")
                             .AppendLine($"{ApiLogPresenter.DETAIL_CONTENT_REQUEST_METHOD}: {log.Method}")
                             .AppendLine($"{ApiLogPresenter.DETAIL_CONTENT_STATUS_CODE}: {log.StatusCode}");
            if (log.RequestHeaders != null && log.RequestHeaders.Any())
            {
                logDetailsBuilder.AppendLine(toHeaderString(ApiLogPresenter.DETAIL_HEADER_REQUEST_HEADERS));
                foreach ((string key, string value) in log.RequestHeaders)
                {
                    logDetailsBuilder.AppendLine($"{key}: {value}");
                }
            }

            if (!string.IsNullOrEmpty(log.RequestBody))
            {
                logDetailsBuilder.AppendLine(toHeaderString(ApiLogPresenter.DETAIL_HEADER_REQUEST_BODY));
                logDetailsBuilder.AppendLine(log.PrettyPrintedRequestBody);
            }

            if (log.ResponseHeaders != null && log.ResponseHeaders.Any())
            {
                logDetailsBuilder.AppendLine(toHeaderString(ApiLogPresenter.DETAIL_HEADER_RESPONSE_HEADERS));
                foreach ((string key, string value) in log.ResponseHeaders)
                {
                    logDetailsBuilder.AppendLine($"{key}: {value}");
                }
            }

            if (!string.IsNullOrEmpty(log.ResponseBody))
            {
                logDetailsBuilder.AppendLine(toHeaderString(ApiLogPresenter.DETAIL_HEADER_RESPONSE_BODY));
                logDetailsBuilder.AppendLine(log.PrettyPrintedResponseBody);
            }

            return logDetailsBuilder.ToString().TrimEnd();
        }

        static List<IKeyValueParser> AddLogDetailParsers(List<IKeyValueParser> parsers, string logDetails)
        {
            string[] detailString = logDetails.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            for (var line = 0; line < detailString.Length; ++line)
            {
                if (detailString[line].StartsWith(ApiLogPresenter.DETAIL_HEADER_PREFIX))
                {
                    string detailHeader = detailString[line]
                                          .Replace(ApiLogPresenter.DETAIL_HEADER_PREFIX, "")
                                          .Replace(ApiLogPresenter.DETAIL_HEADER_SUFFIX, "");
                    var header = string.Empty;
                    var hasMultipleDetailElements = false;
                    switch (detailHeader)
                    {
                        case "API Detail":
                            header = "_apiDetail";
                            hasMultipleDetailElements = true;
                            break;
                        case "Request Headers":
                            header = "_requestHeaders";
                            hasMultipleDetailElements = true;
                            break;
                        case "Request Body":
                            header = "_requestBody";
                            break;
                        case "Response Headers":
                            header = "_responseHeaders";
                            hasMultipleDetailElements = true;
                            break;
                        case "Response Body":
                            header = "_responseBody";
                            break;
                    }

                    if (string.IsNullOrEmpty(header) || (detailString.Length <= (line + 1)))
                    {
                        continue;
                    }

                    if (hasMultipleDetailElements)
                    {
                        var logDetailParsers = new List<KeyValueParser>();
                        var elementCount = 0;
                        for (int i = line + 1;
                             (i < detailString.Length) && !detailString[i].StartsWith(ApiLogPresenter.DETAIL_HEADER_PREFIX);
                             ++i)
                        {
                            string[] element = detailString[i].Split(':', 2);
                            logDetailParsers.Add(new KeyValueParser(element[0].Trim(), element[1].Trim()));
                            ++elementCount;
                        }

                        parsers.Add(new KeyObjectParser(header, logDetailParsers.ToArray()));
                        line += elementCount;
                    }
                    else
                    {
                        var rawStringValues = new List<string>();
                        for (int i = line + 1;
                             (i < detailString.Length) &&
                             !detailString[i].StartsWith(ApiLogPresenter.DETAIL_HEADER_PREFIX);
                             ++i)
                        {
                            rawStringValues.Add(detailString[i]);
                            ++line;
                        }

                        parsers.Add(new ArrayParser(header, rawStringValues.ToArray()));
                    }
                }
            }

            return parsers;
        }

        protected override void OnDestroy()
        {
            _logAttributes = default;
            _errorLogBuffer = default;
            _normalLogBuffer = default;
            _onErrorLogReceived = default;
            _onLogCopied = default;
            _onLogDownload = default;
            _apiLogMenuInfo = default;

            base.OnDestroy();
        }
    }
}
