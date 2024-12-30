#if NOA_DEBUGGER && UNITY_EDITOR

using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS
using System.IO;
using UnityEditor.iOS.Xcode;
#endif

namespace NoaDebugger
{
    static class OnPostBuildProcess
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget == BuildTarget.iOS)
            {
#if UNITY_IOS
                string plistPath = Path.Combine(path, "Info.plist");
                var plist = new PlistDocument();
                plist.ReadFromFile(plistPath);
                PlistElementDict root = plist.root;
                root.SetBoolean("LSSupportsOpeningDocumentsInPlace", true);
                root.SetBoolean("UIFileSharingEnabled", true);
                plist.WriteToFile(plistPath);
#endif
            }

            DebugCommand.LinkXmlManager.Remove();
            FontAssetReplacer.RestoreSettings();
        }
    }
}

#endif
