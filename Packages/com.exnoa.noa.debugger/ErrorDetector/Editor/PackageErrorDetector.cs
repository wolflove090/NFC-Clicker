#if UNITY_EDITOR && !NOA_DEBUGGER_DEBUG
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEditor.Build;

namespace NoaDebugger
{
    class PackageErrorDetector : AssetPostprocessor
    {
        static string _noaDebuggerDirectoryPath;

        static bool _isInit;
        static bool _hasShownError;

        public static void Init()
        {
            if (_isInit)
            {
                return;
            }

            _isInit = true;

            CompilationPipeline.assemblyCompilationFinished -= OnCompilationFinished;

            CompilationPipeline.assemblyCompilationFinished += OnCompilationFinished;
        }

        static void OnCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            foreach (var message in messages)
            {
                if (message.type == CompilerMessageType.Error)
                {
                    if (assemblyPath.Contains("Noa.NoaDebugger") || assemblyPath.Contains("Packages/com.exnoa.noa.debugger"))
                    {
                        PackageErrorDetector._hasShownError = true;

                        break;
                    }
                }
            }
        }

        static void OnPostprocessAllAssets (
            string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            PackageErrorDetector.Init();

            if (PackageErrorDetector._hasShownError)
            {
                PackageErrorDetector._hasShownError = false;
                if (EditorUtility.DisplayDialog(
                        "Attention", "There is an error inside the NOA Debugger package. The issue may be resolved by deleting and re-importing the package. For more details, please refer to the release notes.\nDo you want to remove the package? Please retrieve and re-import the latest version after deletion.", "OK", "Cancel"))
                {
                    CompilationPipeline.assemblyCompilationFinished -= OnCompilationFinished;
                    ScriptingDefineSymbolUtil.RemoveNoaDebuggerSymbol();
                    Delete(PackageErrorDetector.NoaDebuggerPackagePath);
                    File.Delete($"{PackageErrorDetector.NoaDebuggerPackagePath}.meta");
                    AssetDatabase.Refresh();
                }
            }
        }

        static string NoaDebuggerPackagePath
        {
            get
            {
                if (string.IsNullOrEmpty(PackageErrorDetector._noaDebuggerDirectoryPath))
                {
                    string folderPath = null;
                    Type type = typeof(PackageErrorDetector);
                    string[] guids = AssetDatabase.FindAssets($"t:script {type.Name}");

                    foreach (string guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);

                        if (AssetDatabase.IsValidFolder(path))
                        {
                            continue;
                        }

                        var scriptAsset = AssetDatabase.LoadMainAssetAtPath(path) as MonoScript;

                        if (scriptAsset != null && scriptAsset.GetClass() == type)
                        {
                            folderPath = Path.GetDirectoryName(path);

                            break;
                        }
                    }

                    PackageErrorDetector._noaDebuggerDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(folderPath));
                }

                return PackageErrorDetector._noaDebuggerDirectoryPath;
            }
        }

        static void Delete(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            var filePaths = Directory.GetFiles(path);

            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            var directoryPaths = Directory.GetDirectories(path);

            foreach (var directoryPath in directoryPaths)
            {
                Delete(directoryPath);
            }

            Directory.Delete(path, false);
        }
    }

    [InitializeOnLoad]
    static class PackageErrorDetectorOnLoad
    {
        static PackageErrorDetectorOnLoad()
        {
            PackageErrorDetector.Init();
        }
    }
}
#endif
