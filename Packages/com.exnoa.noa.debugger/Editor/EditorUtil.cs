using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    static class EditorUtil
    {
        private static Texture2D _logo;

        static Texture2D GetLogo()
        {
            if (_logo != null)
            {
                return _logo;
            }

            var path = $"{NoaPackageManager.NoaDebuggerPackagePath}/Runtime/UIParts/Sprites/logo.png";
            return _logo = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
        }

        public static void DrawTitle(string titleName)
        {
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                var logo = GetLogo();
                if (logo != null)
                {
                    EditorGUILayout.LabelField(new GUIContent(logo), GUILayout.Width(30), GUILayout.Height(30));
                }
                EditorGUILayout.LabelField(titleName, EditorBaseStyle.Title(25));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            Rect titleRect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(titleRect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseUp && titleRect.Contains(Event.current.mousePosition))
            {
                Application.OpenURL(EditorDefine.AssetStoreUrl);
            }

            EditorGUILayout.Separator();
        }

        public static void OpenFile(string filePath)
        {
            var file = AssetDatabase.LoadAssetAtPath<Object>(filePath);
            AssetDatabase.OpenAsset(file);
        }
    }
}
