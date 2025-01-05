using UnityEditor;
using UnityEditor.Callbacks;

namespace NoaDebugger
{
    sealed class ImportPackagePostprocessor : AssetPostprocessor
    {
        static string _packageName = NoaPackageManager.NoaDebuggerPackageInfo.name;

        static void OnPostprocessAllAssets (
            string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach(var importedAsset in importedAssets)
            {
                if (_CheckUpdate(importedAsset))
                {
                    return;
                }
            }
            foreach(var deletedAsset in deletedAssets)
            {
                if (_CheckUpdate(deletedAsset))
                {
                    return;
                }
            }
            foreach(var movedAsset in movedAssets)
            {
                if (_CheckUpdate(movedAsset))
                {
                    return;
                }
            }
            foreach(var movedFromAssetPath in movedFromAssetPaths)
            {
                if (_CheckUpdate(movedFromAssetPath))
                {
                    return;
                }
            }
        }

        static bool _CheckUpdate(string assetPath)
        {
            if (assetPath.Contains(_packageName))
            {
                NoaPackageManager.InitializeOnPackageUpdate();
                return true;
            }
            return false;
        }


        [DidReloadScripts]
        static void OnPostCompile()
        {
            NoaPackageManager.UpdateCustomMenu();
        }
    }
}
