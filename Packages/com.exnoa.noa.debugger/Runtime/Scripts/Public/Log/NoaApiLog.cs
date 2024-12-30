using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace NoaDebugger
{
    /// <summary>
    /// This class allows you to access the information of the ApiLog functionality
    /// </summary>
    public sealed class NoaApiLog
    {
        /// <summary>
        /// Returns the list of logs being held
        /// </summary>
        public static LinkedList<ApiLogEntry> LogList => _GetLogList();

        /// <summary>
        /// Callback for when an error is detected
        /// </summary>
        public static UnityAction<ApiLogEntry> OnError
        {
            get
            {
                var presenter = NoaDebugger.GetPresenter<ApiLogPresenter>();

                if (presenter == null)
                {
                    return null;
                }

                return presenter._onErrorLogReceived;
            }
            set
            {
                var presenter = NoaDebugger.GetPresenter<ApiLogPresenter>();

                if (presenter != null)
                {
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
        }

        /// <summary>
        /// Event triggered when a api log is copied to the clipboard.
        /// Users can register their custom actions to be executed when a log entry is copied to the clipboard.
        /// The event provides the log entry data and the copied text data.
        /// </summary>
        public static UnityAction<ApiLogEntry, string> OnLogCopied
        {
            get
            {
                var presenter = NoaDebugger.GetPresenter<ApiLogPresenter>();

                return presenter != null ? presenter._onLogCopied : null;
            }
            set
            {
                var presenter = NoaDebugger.GetPresenter<ApiLogPresenter>();

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
                var presenter = NoaDebugger.GetPresenter<ApiLogPresenter>();

                return presenter != null ? presenter._onLogDownload : null;
            }
            set
            {
                var presenter = NoaDebugger.GetPresenter<ApiLogPresenter>();

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
        static LinkedList<ApiLogEntry> _GetLogList()
        {
            ApiLogPresenter presenter = NoaDebugger.GetPresenter<ApiLogPresenter>();

            if (presenter == null)
            {
                return null;
            }

            return presenter.GetLogList();
        }

        /// <summary>
        /// Deletes all logs
        /// </summary>
        public static void Clear()
        {
            ApiLogPresenter presenter = NoaDebugger.GetPresenter<ApiLogPresenter>();

            if (presenter == null)
            {
                return;
            }

            presenter.ClearLog();
        }
    }
}
