#if NOA_DEBUGGER_DEBUG && UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

namespace NoaDebugger
{
    sealed class NoaDebuggerInfoUpdater : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (BuildPipeline.isBuildingPlayer)
            {
                return;
            }

            const string packageInfoFileName = "package.json";
            const string noaDebuggerInfoFileName = "NoaDebuggerInfo.asset";
            NoaDebuggerInfo.UnityPackageInfo? updatedPackageInfo = null;

            foreach (string assetPath
                     in importedAssets.Where(asset => asset.StartsWith(NoaPackageManager.NoaDebuggerPackagePath)))
            {
                if (Path.GetFileName(assetPath).Equals(packageInfoFileName))
                {
                    var json = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                    updatedPackageInfo = JsonUtility.FromJson<NoaDebuggerInfo.UnityPackageInfo>(json.text);
                }
            }

            if (updatedPackageInfo == null)
            {
                return;
            }

            NoaDebuggerInfo noaDebuggerInfo = null;
            foreach (string guid in AssetDatabase.FindAssets($"t:{typeof(NoaDebuggerInfo)}"))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.StartsWith(NoaPackageManager.NoaDebuggerPackagePath)
                    && Path.GetFileName(assetPath).Equals($"{noaDebuggerInfoFileName}"))
                {
                    noaDebuggerInfo = AssetDatabase.LoadAssetAtPath<NoaDebuggerInfo>(assetPath);
                }
            }

            Assert.IsNotNull(noaDebuggerInfo);

            if (!noaDebuggerInfo.NoaDebuggerVersion.Equals(updatedPackageInfo.Value.version)
                || !noaDebuggerInfo.MinimumUnityVersion.Equals(updatedPackageInfo.Value.unity))
            {
                noaDebuggerInfo.NoaDebuggerVersion = updatedPackageInfo.Value.version;
                noaDebuggerInfo.MinimumUnityVersion = updatedPackageInfo.Value.unity;
                EditorUtility.SetDirty(noaDebuggerInfo);
                AssetDatabase.SaveAssets();
                Debug.LogWarning($"The updates from {packageInfoFileName} have been reflected in {noaDebuggerInfoFileName}. Please verify the contents.");
            }
        }
    }
}
#endif
