using System;

namespace NoaDebugger
{
    /// <summary>
    /// The details of a ConsoleLog.
    /// </summary>
    public class ConsoleLogDetail : ILogDetail
    {
        /// <summary>
        /// The detail text of the log.
        /// </summary>
        public string LogDetailString { get; }

        /// <summary>
        /// Registration via API.
        /// </summary>
        internal bool IsRegisteredApi { get; }

        /// <summary>
        /// Generates a ConsoleLogDetail.
        /// </summary>
        /// <param name="logDetailString">Specifies the log detail string</param>
        /// <param name="isRegisteredApi">Specify true if registering via API.</param>
        internal ConsoleLogDetail(string logDetailString, bool isRegisteredApi)
        {
            LogDetailString = logDetailString;
            IsRegisteredApi = isRegisteredApi;
        }

        /// <summary>
        /// Gets text to be copied to the clipboard.
        /// </summary>
        /// <returns>The text to be copied.</returns>
        string ILogDetail.GetClipboardText()
        {
            return LogDetailString.TrimEnd();
        }

        /// <summary>
        /// Format log details from log and stack trace.
        /// </summary>
        /// <param name="logString">Specifies the log string.</param>
        /// <param name="stackTrace">Specifies the stack trace.</param>
        /// <returns>Returns formatted log details.</returns>
        internal static string FormatLogDetail(string logString, string stackTrace)
        {
            if (string.IsNullOrEmpty(stackTrace))
            {
                return logString;
            }

            return $"{logString}\n{stackTrace}";
        }

        /// <summary>
        /// Returns whether a stack trace exists.
        /// </summary>
        /// <returns>Returns true if there is a stack trace.</returns>
        internal bool HasStackTrace()
        {
            string[] lines = LogDetailString.Split('\n');
            return lines.Length > 1;
        }
    }

    /// <summary>
    /// An entry in the ConsoleLog.
    /// </summary>
    public sealed class ConsoleLogEntry : LogEntry<ConsoleLogDetail>
    {
        /// <summary>
        /// The stack trace string.
        /// </summary>
        public string StackTraceString { private set; get; }

        /// <summary>
        /// Overrides the LogDetail of LogEntry.
        /// </summary>
        public override ConsoleLogDetail LogDetail { protected set; get; }

        /// <summary>
        /// Generates a ConsoleLogEntry.
        /// </summary>
        /// <param name="logString">Specifies the string output in the log</param>
        /// <param name="stackTraceString">Specifies the StackTrace string</param>
        /// <param name="logViewString">Specifies the log display string on the view</param>
        /// <param name="logDetail">Specifies the log detail</param>
        /// <param name="logType">Specifies the type of log</param>
        /// <param name="receivedAt">Specifies the date and time the log was retrieved</param>
        internal ConsoleLogEntry(string logString, string stackTraceString, string logViewString, ConsoleLogDetail logDetail,
                                 LogType logType, DateTime receivedAt)
            : base(logString, logViewString, logType, receivedAt)
        {
            StackTraceString = stackTraceString;
            LogDetail = logDetail;
        }
    }
}
