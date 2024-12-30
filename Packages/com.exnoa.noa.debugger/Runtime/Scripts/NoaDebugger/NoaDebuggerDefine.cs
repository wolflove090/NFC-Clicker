using NoaDebugger.DebugCommand;
using UnityEngine;

namespace NoaDebugger
{
    static class NoaDebuggerDefine
    {
        public static readonly string InternalErrorStacktraceRegexPattern = @"^NoaDebugger\.";

        public static readonly string DebugCommandInvocationStacktraceRegexPattern =
            @"^NoaDebugger\.DebugCommand\."
            + @"("
            + @"GetOnlyPropertyCommand[:\.]GetValue"
            + @"|"
            + @"MutablePropertyCommandBase`1\[T\][:\.]Invoke[GS]etter"
            + @"|"
            + @"(Method|Coroutine|HandleMethod)Command[:\.]Invoke"
            + @")\b";

        public static readonly string RootObjectName = "NoaDebuggerRoot";

        public static readonly string EditorPrefsKeyPackageSettingsData =
            $"{Application.productName}:PACKAGE_SETTINGS_DATA";

        public const ButtonPosition DEFAULT_START_BUTTON_POSITION = ButtonPosition.LowerLeft;

        public const float DEFAULT_START_BUTTON_SCALE = 1.0f;

        public static readonly float StartButtonScaleMin = 0.5f;

        public static readonly float StartButtonScaleMax = 1.0f;

        public const ButtonMovementType DEFAULT_START_BUTTON_MOVEMENT_TYPE = ButtonMovementType.Draggable;

        public const bool DEFAULT_SAVE_START_BUTTON_POSITION = true;

        public const float TOOL_START_BUTTON_ALPHA_DEFAULT = 0.6f;

        public static readonly float ToolStartButtonAlphaMin = 0.0f;

        public static readonly float ToolStartButtonAlphaMax = 1.0f;

        public const float CANVAS_ALPHA_DEFAULT = 0.7f;

        public static readonly float CanvasAlphaMin = 0.1f;

        public static readonly float CanvasAlphaMax = 1;

        public const int NOA_DEBUGGER_CANVAS_SORT_ORDER_DEFAULT = 1000;

        public const bool IS_UI_REVERSE_PORTRAIT_DEFAULT = false;

        public static readonly int ProfilerChartHistoryCount = 600;

        public static readonly int MaxNumberOfMatchingLogsToDisplay = 99;

        public const int CONSOLE_LOG_COUNT_DEFAULT = 999;

        public static readonly int ConsoleLogCountMin = 99;

        public static readonly int ConsoleLogCountMax = 999;

        public static readonly int ConsoleLogSummaryStringLengthMax = 256;

        public const int API_LOG_COUNT_DEFAULT = 999;

        public static readonly int ApiLogCountMin = 99;

        public static readonly int ApiLogCountMax = 999;

        public static readonly int SnapshotLogMaxLabelCharNum = 30;

        public static readonly float PressTimeSeconds = 0.5f;

        public static readonly int PressActionIntervalChangeCount = 5;

        public static readonly float PressActionFirstInterval = 0.2f;

        public static readonly float PressActionSecondInterval = 0.1f;

        public static readonly float DragThresholdDistanceOnScreen = 50f;

        public const bool DEFAULT_AUTO_CREATE_EVENT_SYSTEM = true;

        public const ErrorNotificationType DEFAULT_ERROR_NOTIFICATION_TYPE = ErrorNotificationType.Full;

        public const bool DEFAULT_AUTO_INITIALIZE = true;

        public const bool IS_CUSTOM_FONT_SETTINGS_ENABLED_DEFAULT = false;

        public const float DEFAULT_FONT_SIZE_RATE = 1f;

        public const int DEFAULT_HIERARCHY_LEVELS = 3;

        public static readonly int HierarchyLevelsMin = 1;

        public static readonly int HierarchyLevelsMax = 10;

        public static readonly float DebugCommandAutoRefreshInterval = 1f;

        public const CommandDisplayFormat DEFAULT_COMMAND_FORMAT_LANDSCAPE = CommandDisplayFormat.Panel;

        public const CommandDisplayFormat DEFAULT_COMMAND_FORMAT_PORTRAIT = CommandDisplayFormat.List;

        public const bool DEFAULT_AUTO_SAVE = true;

        public const string MISSING_VALUE = "Not Supported";

        public static readonly string SupportedValue = "Supported";

        public static readonly string HyphenValue = "-";

        public static readonly string DegreesCelsiusLabel = "deg C";

        public static readonly char NoHasFontAssetReplacementCharacter = '_';

        public static readonly string ClipboardCopyText = "Copied to clipboard";

        public static readonly string DownloadCompletedText = "Download completed";

        public static readonly string DownloadCanceledText = "Download canceled";

        public static readonly string DownloadFailedText = "Download failed";

        public static readonly string DeleteSaveDataText = "Save data deleted";

        public static readonly string ShowErrorText = "Sorry, an error occurred.";

        public static readonly string TakeScreenshotText = "Took a screenshot";

        public static readonly string UnmarkedSnapshotCategoryNameToSetAdditionalInfo = "Others";

        public static readonly string[] SnapshotDuplicateCategoryNames =
            {"FPS", "Memory", "Rendering", "Battery", "Thermal"};

        public static readonly string[] SnapshotDownloadLogInfoLabels = {"_label", "_time", "_elapsedTime"};

        public static readonly string SnapshotDuplicateCategoryNamePrefix = "[Additional]";

        public static readonly char DecimalPoint = '.';

        public const int INFORMATION_MENU_SORT_NO = 0;
        public const int PROFILER_MENU_SORT_NO = 1;
        public const int SNAPSHOT_MENU_SORT_NO = 2;
        public const int CONSOLE_LOG_MENU_SORT_NO = 3;
        public const int API_LOG_MENU_SORT_NO = 4;
        public const int HIERARCHY_MENU_SORT_NO = 5;
        public const int COMMAND_MENU_SORT_NO = 6;

        public const int CUSTOM_MENU_SORT_NO = 0;

        public struct TextColors
        {
            public static readonly Color Default = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            public static readonly Color Success = new Color32(0x4A, 0xFF, 0xF3, 0xFF);
            public static readonly Color Warning = new Color32(0xFF, 0x8D, 0x00, 0xFF);
            public static readonly Color Dynamic = new Color32(0x00, 0xFF, 0x5A, 0xFF);
            public static readonly Color InProgress = new Color32(0xFF, 0xFF, 0x00, 0xFF);

            public static readonly Color LogGray = new Color32(0xBA, 0xBA, 0xBA, 0xFF);
            public static readonly Color LogLightBlue = new Color32(0x42, 0xA9, 0xBC, 0xFF);
            public static readonly Color LogOrange = new Color32(0xC3, 0x8C, 0x50, 0xFF);
            public static readonly Color LogYellow = new Color32(0xC0, 0xB9, 0x3D, 0xFF);
            public static readonly Color LogBlue = new Color32(0x55, 0x71, 0xD4, 0xFF);
            public static readonly Color LogPurple = new Color32(0xB4, 0x18, 0xBC, 0xFF);
            public static readonly Color LogGreen = new Color32(0x57, 0xB2, 0x50, 0xFF);
            public static readonly Color LogRed = new Color32(0xDE, 0x19, 0x26, 0xFF);
        }

        public struct ImageColors
        {
            public static readonly Color Default = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            public static readonly Color Disabled = new Color32(0xA7, 0xA7, 0xA7, 0xFF);
            public static readonly Color Clear = new Color32(0xFF, 0xFF, 0xFF, 0x00);
            public static readonly Color Warning = new Color32(0xFF, 0x8D, 0x00, 0xFF);
            public static readonly Color SnapshotFirstSelected = new Color32(0x4D, 0xD6, 0xE8, 0xFF);
            public static readonly Color SnapshotSecondSelected = new Color32(0x26, 0x5B, 0x78, 0xFF);
        }

        public struct BackgroundColors
        {
            public static readonly Color NoaDebuggerButtonAlert = new Color32(0xFF, 0x00, 0x00, 0xFF);
            public static readonly Color NoaDebuggerButtonDefault = new Color32(0x4B, 0x4B, 0x4B, 0xFF);
            public static readonly Color LogBright = new Color32(0x7A, 0x7A, 0x7A, 0xC0);
            public static readonly Color LogDark = new Color32(0x67, 0x67, 0x67, 0x9B);
            public static readonly Color LogBrown = new Color32(0xFF, 0x80, 0x80, 0x9B);
            public static readonly Color LogDarkBrown = new Color32(0xE6, 0xA5, 0x72, 0x9B);
            public static readonly Color LogGreen = new Color32(0x8A, 0xCC, 0x66, 0x9B);
            public static readonly Color LogLightBlue = new Color32(0x80, 0xB5, 0xFF, 0x9B);
            public static readonly Color LogDarkPurple = new Color32(0x99, 0x4C, 0x92, 0x9B);
            public static readonly Color LogBlack = new Color32(0x33, 0x10, 0x04, 0x9B);
            public static readonly Color LogDarkGreen = new Color32(0x4C, 0x34, 0x0F, 0x9B);
            public static readonly Color LogYellowGreen = new Color32(0x99, 0x93, 0x00, 0x9B);
            public static readonly Color LogBlue = new Color32(0x1F, 0x5B, 0x99, 0x9B);
            public static readonly Color LogPurple = new Color32(0xFF, 0x4C, 0xEE, 0x9B);
        }

        public enum LogType
        {
            Error,
            Warning,
            Log,
        }

        public struct LogColors
        {
            public static readonly Color LogError = new Color32(188, 0, 0, 255);
            public static readonly Color LogWarning = new Color32(203, 198, 17, 255);
            public static readonly Color LogMessage = new Color32(200, 200, 200, 255);
        }
    }

    public enum ErrorNotificationType
    {
        Full, 
        Flashing, 
        None 
    }
}
