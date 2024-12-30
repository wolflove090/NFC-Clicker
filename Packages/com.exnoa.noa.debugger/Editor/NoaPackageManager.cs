using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace NoaDebugger
{
    static class NoaPackageManager
    {
        static string _noaDebuggerDirectoryPath;

        public static string NoaDebuggerPackagePath
        {
            get
            {
                if (string.IsNullOrEmpty(NoaPackageManager._noaDebuggerDirectoryPath))
                {
                    string folderPath = null;
                    Type type = typeof(NoaPackageManager);
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

                    NoaPackageManager._noaDebuggerDirectoryPath = Path.GetDirectoryName(folderPath);
                }

                return NoaPackageManager._noaDebuggerDirectoryPath;
            }
        }

        static UnityPackageInfo _noaDebuggerPackageInfo;

        public static UnityPackageInfo NoaDebuggerPackageInfo
        {
            get
            {
                if (NoaPackageManager._noaDebuggerPackageInfo == null)
                {
                    var packageJsonPath = $"{NoaPackageManager.NoaDebuggerPackagePath}/package.json";

                    if (!File.Exists(packageJsonPath))
                    {
                        return null;
                    }

                    string text = File.ReadAllText(packageJsonPath);
                    NoaPackageManager._noaDebuggerPackageInfo = JsonUtility.FromJson<UnityPackageInfo>(text);
                }

                return NoaPackageManager._noaDebuggerPackageInfo;
            }
        }

        public static void ExcludeFromCompile()
        {
            if (NoaPackageManager.NoaDebuggerPackagePath == null)
            {
                return;
            }

            ScriptingDefineSymbolUtil.RemoveNoaDebuggerSymbol();

            AssetDatabase.Refresh();

            if (!Directory.Exists($"{NoaPackageManager.NoaDebuggerPackagePath}/.Runtime"))
            {
                ExcludeDir($"{NoaPackageManager.NoaDebuggerPackagePath}/Runtime");
            }

            if (!Directory.Exists($"{EditorDefine.ResourcesDataPath}/.Custom"))
            {
                ExcludeDir($"{EditorDefine.ResourcesDataPath}/Custom");
            }

            AssetDatabase.Refresh();
        }

        public static void ExcludeDir(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            if (!directoryInfo.Exists)
            {
                return;
            }

            string renamedDirPath = directoryInfo.FullName.Replace(directoryInfo.Name, $".{directoryInfo.Name}");
            directoryInfo.MoveTo(renamedDirPath);

            var fileInfo = new FileInfo($"{path}.meta");
            string renamedFilePath = fileInfo.FullName.Replace(fileInfo.Name, $".{fileInfo.Name}");
            fileInfo.MoveTo(renamedFilePath);
        }

        public static void IncludeInCompile()
        {
            if (NoaPackageManager.NoaDebuggerPackagePath == null)
            {
                return;
            }

            var runtimeHiddenPath = $"{NoaPackageManager.NoaDebuggerPackagePath}/.Runtime";

            if (!Directory.Exists(runtimeHiddenPath))
            {
                PackageEditorWindow.IsWait = false;

                return;
            }

            IncludeDir(runtimeHiddenPath);
            var customMenuHiddenPath = $"{EditorDefine.ResourcesDataPath}/.Custom";

            if (Directory.Exists(customMenuHiddenPath))
            {
                IncludeDir(customMenuHiddenPath);
            }

            AssetDatabase.Refresh();

            ScriptingDefineSymbolUtil.AddNoaDebuggerSymbol();
            AssetDatabase.Refresh();
        }

        public static void IncludeDir(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            var directoryInfo = new DirectoryInfo(path);
            string renamedDirName = directoryInfo.Name.Substring(1);
            string renamedDirPath = directoryInfo.FullName.Replace(directoryInfo.Name, renamedDirName);
            directoryInfo.MoveTo(renamedDirPath);

            var fileInfo = new FileInfo($"{path}.meta");
            string renamedFileName = fileInfo.Name.Substring(1);
            string renamedFilePath = fileInfo.FullName.Replace(fileInfo.Name, renamedFileName);
            fileInfo.MoveTo(renamedFilePath);
        }

        public static bool HasRuntimePackage()
        {
            var runtimePath = $"{NoaPackageManager.NoaDebuggerPackagePath}/Runtime";

            return Directory.Exists(runtimePath);
        }

        public static void InitializeOnPackageUpdate()
        {
            if (NoaPackageManager.NoaDebuggerPackagePath == null)
            {
                return;
            }

#if NOA_DEBUGGER
            var directoryInfo = new DirectoryInfo(EditorDefine.ResourcesDataPath);

            var assetsPath =
                $"Assets{EditorDefine.ResourcesDataPath.Replace(Application.dataPath, "")}/{nameof(NoaDebuggerSettings)}.asset";

            if ((!Directory.Exists(EditorDefine.ResourcesDataPath) &&
                 !Directory.Exists(directoryInfo.FullName.Replace(directoryInfo.Name, $".{directoryInfo.Name}"))) ||
                !File.Exists(assetsPath))
            {
                Directory.CreateDirectory(EditorDefine.ResourcesDataPath);
                var scriptableObject = ScriptableObject.CreateInstance<NoaDebuggerSettings>().Init();
                AssetDatabase.CreateAsset(scriptableObject, assetsPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            NoaDebuggerSettingsManager.GetNoaDebuggerSettings()?.Update();
#endif

            if (!EditorPrefs.HasKey(EditorDefine.EditorPrefsKeyPackageVersion))
            {
                ScriptingDefineSymbolUtil.AddNoaDebuggerSymbolIfOpenDialog();

                EditorPrefs.SetString(
                    EditorDefine.EditorPrefsKeyPackageVersion, NoaPackageManager.NoaDebuggerPackageInfo.version);
            }

            else if (EditorPrefs.GetString(EditorDefine.EditorPrefsKeyPackageVersion) !=
                     NoaPackageManager.NoaDebuggerPackageInfo.version)
            {
                ScriptingDefineSymbolUtil.AddNoaDebuggerSymbol();

                EditorPrefs.SetString(
                    EditorDefine.EditorPrefsKeyPackageVersion, NoaPackageManager.NoaDebuggerPackageInfo.version);
            }

            NoaPackageManager.DetectNewInputSystem();
        }

        public static void UpdateCustomMenu()
        {
#if NOA_DEBUGGER
            NoaDebuggerSettingsManager.GetNoaDebuggerSettings()?.UpdateCustomMenu();
#endif
        }

        public static void DeletePackage()
        {
            ScriptingDefineSymbolUtil.RemoveNoaDebuggerSymbol();
            Delete(NoaPackageManager.NoaDebuggerPackagePath);
            File.Delete($"{NoaPackageManager.NoaDebuggerPackagePath}.meta");
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

        public static void ClearEditorPrefs()
        {
            EditorPrefs.DeleteKey(EditorDefine.EditorPrefsKeyPackageVersion);

            EditorUtility.RequestScriptReload();
            AssetDatabase.Refresh();
        }

#if NOA_DEBUGGER && !NOA_DEBUGGER_DEBUG
        static async void DetectNewInputSystem()
        {
            if (BuildPipeline.isBuildingPlayer || Application.isBatchMode)
            {
                return;
            }

            const string inputSystemPackageName = "com.unity.inputsystem";
            const string inputSystemAssemblyReference = "Unity.InputSystem";

            ListRequest request = Client.List();

            while (!request.IsCompleted)
            {
                await Task.Delay(100);
            }

            bool hasNewInputSystem = null != request.Result.FirstOrDefault(info => info.name == inputSystemPackageName);

            if (!Directory.Exists($"{NoaDebuggerPackagePath}/Runtime"))
            {
                return;
            }

            string assemblyDefinitionFilePath = Path.Combine(
                NoaDebuggerPackagePath, EditorDefine.NOA_DEBUGGER_RUNTIME_ASSEMBLY_DEFINITION_FILE);

            string assemblyDefinitionJson = await File.ReadAllTextAsync(assemblyDefinitionFilePath);
            var assemblyDefinition = JsonUtility.FromJson<AssemblyDefinition>(assemblyDefinitionJson);
            List<string> references = new(assemblyDefinition.references);

            if (hasNewInputSystem)
            {
                if (!references.Contains(inputSystemAssemblyReference))
                {
                    references.Add(inputSystemAssemblyReference);
                }
            }
            else
            {
                if (references.Contains(inputSystemAssemblyReference))
                {
                    references.Remove(inputSystemAssemblyReference);
                }
            }

            assemblyDefinition.references = references.ToArray();
            await File.WriteAllTextAsync(assemblyDefinitionFilePath, JsonUtility.ToJson(assemblyDefinition, true));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#else
        static void DetectNewInputSystem() { }
#endif

        class AssemblyDefinition
        {
            public string name;
            public string rootNamespace;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public string[] precompiledReferences;
            public bool autoReferenced;
            public string[] defineConstraints;
            public string[] versionDefines;
            public bool noEngineReferences;
        }
    }
}
