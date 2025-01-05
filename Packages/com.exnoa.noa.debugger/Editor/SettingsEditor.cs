#if NOA_DEBUGGER
using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    [CustomEditor(typeof (NoaDebuggerSettings))]
    internal sealed class SettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorUtil.DrawTitle("NOA Debugger Settings");

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.HelpBox("This asset contains the settings used by NOA Debugger at runtime. Please edit these settings via the dedicated WindowEditor provided by NOA Debugger.", MessageType.Info);
            }
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Open Settings Window"))
            {
                PackageEditorWindow.ShowWindow(PackageEditorWindow.TabMenu.Settings);
            }

#if NOA_DEBUGGER_DEBUG
            EditorGUILayout.Separator();

            base.OnInspectorGUI();
#endif
        }
    }
}
#endif
