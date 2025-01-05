#if NOA_DEBUGGER && UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace NoaDebugger
{
    sealed class OnPreBuildProcess : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            DebugCommand.LinkXmlManager.Export();

            FontAssetReplacer.ReplaceToUserSettings();
        }
    }
}
#endif
