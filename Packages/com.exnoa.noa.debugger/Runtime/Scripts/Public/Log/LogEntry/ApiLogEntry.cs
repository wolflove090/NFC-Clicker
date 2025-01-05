using System;

namespace NoaDebugger
{
    /// <summary>
    /// An entry in the ApiLog.
    /// </summary>
    public sealed class ApiLogEntry : LogEntry<ApiLog>
    {
        /// <summary>
        /// Overrides the LogDetail of LogEntry.
        /// </summary>
        public override ApiLog LogDetail { protected set; get; }

        /// <summary>
        /// Generates an ApiLogEntry.
        /// </summary>
        /// <param name="logString">Specifies the string output in the log</param>
        /// <param name="logViewString">Specifies the log display string on the view</param>
        /// <param name="logDetail">Specifies the log detail</param>
        /// <param name="logType">Specifies the type of log</param>
        /// <param name="receivedAt">Specifies the date and time the log was retrieved</param>
        internal ApiLogEntry(string logString, string logViewString, ApiLog logDetail, LogType logType,
                             DateTime receivedAt)
            : base(logString, logViewString, logType, receivedAt)
        {
            LogDetail = logDetail;
        }
    }
}
