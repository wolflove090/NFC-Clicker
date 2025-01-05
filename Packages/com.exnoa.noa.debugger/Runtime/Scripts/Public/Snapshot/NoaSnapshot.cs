using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    /// <summary>
    /// Controls the snapshot function of NOA Debugger.
    /// </summary>
    public class NoaSnapshot
    {
        /// <summary>
        /// Log background colour
        /// </summary>
        public enum BgColor
        {
            Default,
            Brown,
            DarkBrown,
            Green,
            LightBlue,
            DarkPurple,
            Black,
            DarkGreen,
            YellowGreen,
            Blue,
            Purple
        }

        /// <summary>
        /// Font colour for additional log detail
        /// </summary>
        public enum FontColor
        {
            White,
            Black,
            Gray,
            LightBlue,
            Orange,
            Yellow,
            Blue,
            Purple,
            Green,
            Red
        }

        /// <summary>
        /// Returns the list of logs being held
        /// </summary>
        public static List<SnapshotLogRecordInformation> LogList => _GetLogList();

        /// <summary>
        /// Event triggered when a snapshot log is copied to the clipboard.
        /// Users can register their custom actions to be executed when a log entry is copied to the clipboard.
        /// The event provides the log entry data and the copied text data.
        /// </summary>
        public static UnityAction<SnapshotLogRecordInformation, string> OnLogCopied
        {
            get
            {
                var presenter = NoaDebugger.GetPresenter<SnapshotPresenter>();

                return presenter != null ? presenter._onLogCopied : null;
            }
            set
            {
                var presenter = NoaDebugger.GetPresenter<SnapshotPresenter>();

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
                var presenter = NoaDebugger.GetPresenter<SnapshotPresenter>();

                return presenter != null ? presenter._onLogDownload : null;
            }
            set
            {
                var presenter = NoaDebugger.GetPresenter<SnapshotPresenter>();

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
        /// Deletes all logs and resets the elapsed time.
        /// </summary>
        public static void ClearLogsAndTimer()
        {
            SnapshotPresenter presenter = NoaDebugger.GetPresenter<SnapshotPresenter>();

            if (presenter == null)
            {
                return;
            }

            presenter.ClearLogsAndTimer();
        }

        /// <summary>
        /// Captures the log.
        /// </summary>
        /// <param name="label">Label name</param>
        /// <param name="backgroundColor">Background colour of the log</param>
        /// <param name="hasNoaProfilerInfo">Whether to hold profiler information provided by NOA Debugger</param>
        /// <param name="additionalInfo">Additional information to display in log detail</param>
        public static void CaptureLog(string label = null, BgColor? backgroundColor = null,
                                      bool hasNoaProfilerInfo = true,
                                      Dictionary<string, NoaSnapshotCategory> additionalInfo = null)
        {
            SnapshotPresenter presenter = NoaDebugger.GetPresenter<SnapshotPresenter>();

            if (presenter == null)
            {
                return;
            }

            Color? color = null;

            switch (backgroundColor)
            {
                case BgColor.Default:
                    break;

                case BgColor.Brown:
                    color = NoaDebuggerDefine.BackgroundColors.LogBrown;
                    break;

                case BgColor.DarkBrown:
                    color = NoaDebuggerDefine.BackgroundColors.LogDarkBrown;
                    break;

                case BgColor.Green:
                    color = NoaDebuggerDefine.BackgroundColors.LogGreen;
                    break;

                case BgColor.LightBlue:
                    color = NoaDebuggerDefine.BackgroundColors.LogLightBlue;
                    break;

                case BgColor.DarkPurple:
                    color = NoaDebuggerDefine.BackgroundColors.LogDarkPurple;
                    break;

                case BgColor.Black:
                    color = NoaDebuggerDefine.BackgroundColors.LogBlack;
                    break;

                case BgColor.DarkGreen:
                    color = NoaDebuggerDefine.BackgroundColors.LogDarkGreen;
                    break;

                case BgColor.YellowGreen:
                    color = NoaDebuggerDefine.BackgroundColors.LogYellowGreen;
                    break;

                case BgColor.Blue:
                    color = NoaDebuggerDefine.BackgroundColors.LogBlue;
                    break;

                case BgColor.Purple:
                    color = NoaDebuggerDefine.BackgroundColors.LogPurple;
                    break;
            }

            presenter.CaptureLog(label, color, hasNoaProfilerInfo, additionalInfo, true);
        }

        /// <summary>
        /// Returns the list of logs being held.
        /// </summary>
        /// <returns>Returns the list of logs being held.</returns>
        static List<SnapshotLogRecordInformation> _GetLogList()
        {
            SnapshotPresenter presenter = NoaDebugger.GetPresenter<SnapshotPresenter>();

            if (presenter == null)
            {
                return null;
            }

            return presenter.GetLogList();
        }
    }
}
