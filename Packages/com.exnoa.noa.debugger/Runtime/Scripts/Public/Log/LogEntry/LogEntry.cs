using System;

namespace NoaDebugger
{
    /// <summary>
    /// Information for the log details contained in a log entry.
    /// </summary>
    public interface ILogDetail
    {
        /// <summary>
        /// Gets text to be copied to the clipboard.
        /// </summary>
        /// <returns>The text to be copied.</returns>
        internal string GetClipboardText();
    }

    /// <summary>
    /// An entry in the log.
    /// </summary>
    public abstract class LogEntry<TLogDetail> where TLogDetail : ILogDetail
    {
        /// <summary>
        /// The string output in the log.
        /// </summary>
        public string LogString { private set; get; }

        /// <summary>
        /// The string used to display the log on the view.
        /// </summary>
        /// <remarks>
        /// The view string is formatted at the time of log recording to reduce load on the presenter side.
        /// </remarks>
        internal string LogViewString { private set; get; }

        /// <summary>
        /// The type of log.
        /// </summary>
        public LogType LogType { private set; get; }

        /// <summary>
        /// The date and time the log was retrieved.
        /// </summary>
        public DateTime ReceivedAt { private set; get; }

        /// <summary>
        /// Detail of the log.
        /// </summary>
        public abstract TLogDetail LogDetail { protected set; get; }

        /// <summary>
        /// Number of matching logs
        /// </summary>
        /// <returns></returns>
        public int NumberOfMatchingLogs { set; get; }

        /// <summary>
        /// Generates a LogEntry
        /// </summary>
        /// <param name="logString">Specifies the string output in the log</param>
        /// <param name="logViewString">Specifies the log display string on the view</param>
        /// <param name="logType">Specifies the type of log</param>
        /// <param name="receivedAt">Specifies the date and time the log was retrieved</param>
        internal LogEntry(string logString, string logViewString, LogType logType, DateTime receivedAt)
        {
            LogString = logString;
            LogViewString = logViewString;
            LogType = logType;
            ReceivedAt = receivedAt;
            NumberOfMatchingLogs = 1;
        }
    }
}
