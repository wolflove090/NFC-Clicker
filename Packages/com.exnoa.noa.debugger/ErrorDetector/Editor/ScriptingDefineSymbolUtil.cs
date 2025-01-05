using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Compilation;
using UnityEngine;
#if UNITY_6000_0_OR_NEWER
using UnityEditor.Build.Profile;
using System.Reflection;
#endif

namespace NoaDebugger
{
    public static class ScriptingDefineSymbolUtil
    {
        const string NOA_DEBUGGER_SYMBOL = "NOA_DEBUGGER";

        public static void AddNoaDebuggerSymbol()
        {
            ScriptingDefineSymbolUtil.AddNoaDebuggerSymbolInternal();
        }

        public static void AddNoaDebuggerSymbolIfOpenDialog()
        {
            bool isUpdatedBuildProfile = ScriptingDefineSymbolUtil.AddNoaDebuggerSymbolInternal();

            if (isUpdatedBuildProfile && !Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("Build Profile Symbol Update", "There was an override in the PlayerSettings for the build profile, so the NOA_DEBUGGER symbol has been added to the scripting defines.", "OK");
            }
        }

        static bool AddNoaDebuggerSymbolInternal()
        {
            string symbol = ScriptingDefineSymbolUtil.NOA_DEBUGGER_SYMBOL;
            return ScriptingDefineSymbolUtil.UpdateNoaDebuggerSymbol(
                symbolList =>
                {
                    if (!symbolList.Contains(symbol))
                    {
                        symbolList.Add(symbol);
                    }
                });
        }

        public static void RemoveNoaDebuggerSymbol()
        {
            string symbol = ScriptingDefineSymbolUtil.NOA_DEBUGGER_SYMBOL;
            Action<List<string>> actionToSymbolList = symbolList => symbolList.Remove(symbol);
            ScriptingDefineSymbolUtil.UpdateNoaDebuggerSymbol(actionToSymbolList);

#if UNITY_6000_0_OR_NEWER
            RemoveNoaDebuggerSymbolWithActiveBuildProfile(actionToSymbolList);
#endif
        }

        static bool UpdateNoaDebuggerSymbol(Action<List<string>> actionToSymbolList)
        {
#if UNITY_6000_0_OR_NEWER
            var current = BuildProfile.GetActiveBuildProfile();
            if (current != null)
            {
                BuildProfile.SetActiveBuildProfile(null);
            }
#endif

            var buildTargets = new List<NamedBuildTarget>
            {
                NamedBuildTarget.iOS,
                NamedBuildTarget.Android,
                NamedBuildTarget.Standalone,
                NamedBuildTarget.WebGL,
            };

            foreach (NamedBuildTarget buildTarget in buildTargets)
            {
                string[] defineSymbols = PlayerSettings.GetScriptingDefineSymbols(buildTarget).Split(';');
                var symbolList = new List<string>(defineSymbols);

                actionToSymbolList.Invoke(symbolList);

                string symbolListString = string.Join(";", symbolList.ToArray());
                PlayerSettings.SetScriptingDefineSymbols(buildTarget, symbolListString);
            }

            bool isUpdatedBuildProfile = UpdateNoaDebuggerSymbolWithBuildProfile(actionToSymbolList);

#if UNITY_6000_0_OR_NEWER
            if (current != null)
            {
                BuildProfile.SetActiveBuildProfile(current);
            }
#endif

            if (!Application.isBatchMode)
            {
                CompilationPipeline.RequestScriptCompilation();
            }

            return isUpdatedBuildProfile;
        }

#if UNITY_6000_0_OR_NEWER
        static bool UpdateNoaDebuggerSymbolWithBuildProfile(Action<List<string>> actionToSymbolList)
        {
            string[] guids = AssetDatabase.FindAssets("t:BuildProfile");
            List<BuildProfile> targetBuildProfiles = new List<BuildProfile>(guids.Length);

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                BuildProfile buildProfile = AssetDatabase.LoadAssetAtPath<BuildProfile>(assetPath);

                PlayerSettings overridePlayerSettings = GetPlayerSettings(buildProfile);

                if (overridePlayerSettings != null)
                {
                    targetBuildProfiles.Add(buildProfile);
                }
            }

            if (targetBuildProfiles.Count == 0)
            {
                return false;
            }

            foreach (BuildProfile profile in targetBuildProfiles)
            {
                List<string> symbolList = new List<string>(profile.scriptingDefines);
                actionToSymbolList.Invoke(symbolList);
                profile.scriptingDefines = symbolList.ToArray();
                EditorUtility.SetDirty(profile);
            }

            return true;
        }

        static void RemoveNoaDebuggerSymbolWithActiveBuildProfile(Action<List<string>> actionToSymbolList)
        {
            BuildProfile current = BuildProfile.GetActiveBuildProfile();

            if (current == null)
            {
                return;
            }

            if (GetPlayerSettings(current) == null)
            {
                return;
            }

            NamedBuildTarget buildTarget = GetNamedBuildTargetFromBuildTarget(EditorUserBuildSettings.activeBuildTarget);
            string[] defineSymbols = PlayerSettings.GetScriptingDefineSymbols(buildTarget).Split(';');
            var symbolList = new List<string>(defineSymbols);

            actionToSymbolList.Invoke(symbolList);

            string symbolListString = string.Join(";", symbolList.ToArray());
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, symbolListString);
        }

        static PlayerSettings GetPlayerSettings(BuildProfile buildProfile)
        {
            FieldInfo field = typeof(BuildProfile).GetField("m_PlayerSettings", BindingFlags.NonPublic | BindingFlags.Instance);
            return (PlayerSettings)field?.GetValue(buildProfile);
        }

#else
        static bool UpdateNoaDebuggerSymbolWithBuildProfile(Action<List<string>> actionToSymbolList)
        {
            return false;
        }
#endif

        static NamedBuildTarget GetNamedBuildTargetFromBuildTarget(BuildTarget buildTarget)
        {
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            return NamedBuildTarget.FromBuildTargetGroup(targetGroup);
        }
    }
}
