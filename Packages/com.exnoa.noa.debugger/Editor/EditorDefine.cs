using UnityEngine;

namespace NoaDebugger
{
    static class EditorDefine
    {
        public const string NOA_DEBUGGER_RUNTIME_ASSEMBLY_DEFINITION_FILE = "Runtime/Noa.NoaDebugger.Runtime.asmdef";
        public const int NOA_DEBUGGER_EDITOR_WINDOW_WIDTH = 450;
        public const int NOA_DEBUGGER_EDITOR_WINDOW_HEIGHT = 610;
        public const int NOA_DEBUGGER_EDITOR_LABEL_WIDTH = 190;
        public static readonly string EditorPrefsKeyPackageVersion = $"{Application.productName}:PACKAGE_VERSION";

        public static readonly string SettingsDataPath = $"{Application.dataPath}/NoaDebuggerSettings";

        public static readonly string ResourcesDataPath = $"{EditorDefine.SettingsDataPath}/Resources";

        public static readonly string UnityDiscussionsUrl = "https://discussions.unity.com/t/noa-debugger-for-unity-feedback-questions-and-feature-requests";

        public static readonly string AssetStoreUrl = "https://u3d.as/3cCN";
    }
}
