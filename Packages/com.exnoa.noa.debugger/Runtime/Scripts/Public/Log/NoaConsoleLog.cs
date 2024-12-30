using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    /// <summary>
    /// This class allows you to access the information of the ConsoleLog functionality
    /// </summary>
    public sealed class NoaConsoleLog
    {
        /// <summary>
        /// Returns the list of logs being held
        /// </summary>
        public static LinkedList<ConsoleLogEntry> LogList => _GetLogList();

        /// <summary>
        /// Callback for when an error is detected
        /// </summary>
        public static UnityAction<ConsoleLogEntry> OnError
        {
            get
            {
                var presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

                return presenter != null ? presenter._onErrorLogReceived : null;
            }
            set
            {
                var presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

                if (presenter == null)
                {
                    return;
                }

                if (value == null)
                {
                    presenter._onErrorLogReceived = null;
                }
                else
                {
                    presenter._onErrorLogReceived += value;
                }
            }
        }

        /// <summary>
        /// Callback for determining whether to display notifications when an error is detected
        /// </summary>
        public static Func<ConsoleLogEntry, bool> OnFilterErrorNotification
        {
            get
            {
                var presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

                return presenter != null ? presenter._onFilterErrorNotification : null;
            }
            set
            {
                var presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

                if (presenter == null)
                {
                    return;
                }

                if (value == null)
                {
                    presenter._onFilterErrorNotification = null;
                }
                else
                {
                    presenter._onFilterErrorNotification += value;
                }
            }
        }

        /// <summary>
        /// Event triggered when a console log is copied to the clipboard.
        /// Users can register their custom actions to be executed when a log entry is copied to the clipboard.
        /// The event provides the log entry data and the copied text data.
        /// </summary>
        public static UnityAction<ConsoleLogEntry, string> OnLogCopied
        {
            get
            {
                var presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

                return presenter != null ? presenter._onLogCopied : null;
            }
            set
            {
                var presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

                if (presenter == null)
                {
                    return;
                }

                if (value == null)
                {
                    presenter._onLogCopied = null;
                }
                else
                {
                    presenter._onLogCopied += value;
                }
            }
        }

        /// <summary>
        /// Event triggered when logs are downloaded.
        /// Users can register their custom actions to be executed upon log download.
        /// The event provides the filename and the JSON data as strings.
        /// If the event handler returns true, the logs will be downloaded locally.
        /// If the event handler returns false, the logs will not be downloaded locally.
        /// </summary>
        public static Func<string, string, bool> OnLogDownload
        {
            get
            {
                var presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

                return presenter != null ? presenter._onLogDownload : null;
            }
            set
            {
                var presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

                if (presenter == null)
                {
                    return;
                }

                if (value == null)
                {
                    presenter._onLogDownload = null;
                }
                else
                {
                    presenter._onLogDownload += value;
                }
            }
        }

        /// <summary>
        /// Returns the list of logs being held
        /// </summary>
        /// <returns>Returns the list of logs being held</returns>
        static LinkedList<ConsoleLogEntry> _GetLogList()
        {
            ConsoleLogPresenter presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

            return presenter != null ? presenter.GetLogList() : null;
        }

        /// <summary>
        /// Adds logs to the ConsoleLog functionality
        /// </summary>
        /// <param name="type">Type of log</param>
        /// <param name="message">Log text</param>
        /// <param name="stackTrace">Log stack trace</param>
        public static void Add(UnityEngine.LogType logType, string message, string stackTrace = null)
        {
            ConsoleLogPresenter presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

            if (presenter == null)
            {
                return;
            }

            if (stackTrace == null)
            {
                stackTrace = StackTraceUtility.ExtractStackTrace();
            }

            presenter.ReceiveLogGenerateStackTraceFirstLine(message, stackTrace, logType);
        }

        /// <summary>
        /// Deletes all logs
        /// </summary>
        public static void Clear()
        {
            ConsoleLogPresenter presenter = NoaDebugger.GetPresenter<ConsoleLogPresenter>();

            if (presenter == null)
            {
                return;
            }

            presenter.ClearLog();
        }
    }
}
