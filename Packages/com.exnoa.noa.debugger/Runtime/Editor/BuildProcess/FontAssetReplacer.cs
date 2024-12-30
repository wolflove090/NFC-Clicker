#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    class FontAssetReplacer
    {

        static readonly string RuntimeDirectoryPath = $"{NoaPackageManager.NoaDebuggerPackagePath}/Runtime";
        static readonly string DefaultFontSettingPath = $"{FontAssetReplacer.RuntimeDirectoryPath}/Fonts/NoaDebuggerDefaultFontSettings.asset";

        public static void ReplaceToUserSettings()
        {
            NoaDebuggerSettings userSettings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings();

            if (!userSettings.IsCustomFontSpecified)
            {
                return;
            }

            FontAssetReplacer instance = new FontAssetReplacer();
            instance._Exec(userSettings);

            string dirPath = $"{FontAssetReplacer.RuntimeDirectoryPath}/Fonts";

            if (Directory.Exists(dirPath))
            {
                NoaPackageManager.ExcludeDir(dirPath);
                AssetDatabase.Refresh();
            }
        }

        public static void RestoreSettings()
        {
            FontAssetReplacer instance = new FontAssetReplacer();
            NoaDebuggerSettings userSettings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings();

            if (!userSettings.IsCustomFontSpecified)
            {
                return;
            }

            string dirPath = $"{FontAssetReplacer.RuntimeDirectoryPath}/.Fonts";
            if (Directory.Exists(dirPath))
            {
                NoaPackageManager.IncludeDir(dirPath);
                AssetDatabase.Refresh();
            }

            NoaDebuggerSettings defaultSettings =
                        AssetDatabase.LoadAssetAtPath<NoaDebuggerSettings>(FontAssetReplacer.DefaultFontSettingPath);

            instance._Exec(defaultSettings);
        }


        enum ReplaceState
        {
            NoReplace = 1,
            Replace = 2,
            ReplaceNested = 3,
        }

        readonly List<string> _replacedPrefabPaths = new List<string>();

        void _Exec(NoaDebuggerSettings settings)
        {
            if (settings.FontAsset == null)
            {
                return;
            }

            foreach (string prefabPath in _GetTargetPrefabs())
            {
                _ReplaceForPrefabPath(prefabPath, settings);
            }
        }

        string[] _GetTargetPrefabs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { NoaPackageManager.NoaDebuggerPackagePath });
            return guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
        }

        void _ReplaceForPrefabPath(string prefabPath, NoaDebuggerSettings settings)
        {
            if (_replacedPrefabPaths.Contains(prefabPath))
            {
                return;
            }

            GameObject loadedPrefab = PrefabUtility.LoadPrefabContents(prefabPath);
            var state = _ReplaceFontAsset(loadedPrefab.transform, settings);

            if (state == ReplaceState.Replace)
            {
                PrefabUtility.SaveAsPrefabAsset(loadedPrefab,prefabPath);
                _replacedPrefabPaths.Add(prefabPath);
            }
            PrefabUtility.UnloadPrefabContents(loadedPrefab);

            if (state == ReplaceState.ReplaceNested)
            {
                _ReplaceForPrefabPath(prefabPath, settings);
            }
        }

        ReplaceState _ReplaceFontAsset(Transform target, NoaDebuggerSettings settings)
        {
           ReplaceState replaceState = ReplaceState.NoReplace;

            bool isOutermost = PrefabUtility.IsOutermostPrefabInstanceRoot(target.gameObject);
            if (isOutermost)
            {
                string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
                if (!_replacedPrefabPaths.Contains(prefabPath))
                {
                    _ReplaceForPrefabPath(prefabPath, settings);
                    _replacedPrefabPaths.Add(prefabPath);
                    return ReplaceState.ReplaceNested; 
                }

                PropertyModification[] mods = PrefabUtility.GetPropertyModifications(target.gameObject);
                if (mods != null)
                {
                    List<int> fontModInstanceIds = new List<int>();
                    List<int> materialModInstanceIds = new List<int>();

                    foreach (PropertyModification mod in mods)
                    {
                        if (mod.propertyPath == "m_fontAsset")
                        {
                            fontModInstanceIds.Add(mod.target.GetInstanceID());
                        }

                        if (mod.propertyPath == "m_sharedMaterial")
                        {
                            materialModInstanceIds.Add(mod.target.GetInstanceID());
                        }
                    }

                    foreach (NoaDebuggerText component in target.GetComponentsInChildren<NoaDebuggerText>())
                    {
                        GameObject outermostRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(component.gameObject);
                        if (outermostRoot != target.gameObject)
                        {
                            continue;
                        }

                        int instanceId = PrefabUtility.GetCorrespondingObjectFromSource(component)?.GetInstanceID() ?? 0;

                        if (fontModInstanceIds.Contains(instanceId))
                        {
#if NOA_DEBUGGER_DEBUG
                            Debug.LogWarning($"The font has overrides applied. Since these are not necessary, please revert them.: {target.root.name}");
#endif
                            _Replace(component, settings);
                            replaceState = ReplaceState.Replace;
                        }

                        if (materialModInstanceIds.Contains(instanceId))
                        {
#if NOA_DEBUGGER_DEBUG
                            Debug.LogWarning($"The material has overrides applied. Since these are not necessary, please revert them.: {target.root.name}");
#endif
                            SerializedObject serializedObject = new SerializedObject(component);
                            SerializedProperty materialProperty = serializedObject.FindProperty("m_sharedMaterial");
                            PrefabUtility.RevertPropertyOverride(materialProperty, InteractionMode.AutomatedAction);
                        }
                    }
                }
            }

            GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(target.gameObject);
            if (root == null)
            {
                NoaDebuggerText textMeshPro = target.GetComponent<NoaDebuggerText>();
                if (textMeshPro != null)
                {
                    _Replace(textMeshPro, settings);
                    replaceState = ReplaceState.Replace;
                }
            }

            foreach (Transform child in target)
            {
                var state = _ReplaceFontAsset(child, settings);

                if (replaceState == ReplaceState.NoReplace || state == ReplaceState.ReplaceNested)
                {
                    replaceState = state;
                }
            }

            return replaceState;
        }

        void _Replace(NoaDebuggerText target, NoaDebuggerSettings settings)
        {
            target.font = settings.FontAsset;
            target.fontMaterial = settings.FontMaterial;
        }
    }
}
#endif
