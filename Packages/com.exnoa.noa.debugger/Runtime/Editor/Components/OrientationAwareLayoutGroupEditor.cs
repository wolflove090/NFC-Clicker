using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    [CustomEditor(typeof(OrientationAwareLayoutGroup))]
    [CanEditMultipleObjects]
    sealed class OrientationAwareLayoutGroupEditor : Editor
    {
        SerializedProperty _padding;
        SerializedProperty _spacing;
        SerializedProperty _reverseArrangement;
        SerializedProperty _childAlignment;
        SerializedProperty _childControlWidth;
        SerializedProperty _childControlHeight;

        SerializedProperty _portraitLayoutType;
        SerializedProperty _landscapeLayoutType;
        SerializedProperty _isPortrait;

        void OnEnable()
        {
            _padding = serializedObject.FindProperty("m_Padding");
            _spacing = serializedObject.FindProperty("m_Spacing");
            _reverseArrangement = serializedObject.FindProperty("m_ReverseArrangement");
            _childAlignment = serializedObject.FindProperty("m_ChildAlignment");
            _childControlWidth = serializedObject.FindProperty("m_ChildControlWidth");
            _childControlHeight = serializedObject.FindProperty("m_ChildControlHeight");

            _portraitLayoutType = serializedObject.FindProperty("_portraitLayoutType");
            _landscapeLayoutType = serializedObject.FindProperty("_landscapeLayoutType");
            _isPortrait = serializedObject.FindProperty("_isPortrait");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_padding, true);
            EditorGUILayout.PropertyField(_spacing, true);
            EditorGUILayout.PropertyField(_childAlignment, true);
            EditorGUILayout.PropertyField(_reverseArrangement, true);

            Rect rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Control Child Size"));
            rect.width = Mathf.Max(50, (rect.width - 4) / 3);
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, _childControlWidth, EditorGUIUtility.TrTextContent("Width"));
            rect.x += rect.width + 2;
            ToggleLeft(rect, _childControlHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUIUtility.labelWidth = 0;

            var layoutGroup = target as OrientationAwareLayoutGroup;

            layoutGroup._portraitLayoutType =
                (OrientationAwareLayoutGroup.LayoutType)EditorGUILayout.EnumPopup(_portraitLayoutType.displayName,
                    layoutGroup._portraitLayoutType);

            layoutGroup._landscapeLayoutType =
                (OrientationAwareLayoutGroup.LayoutType)EditorGUILayout.EnumPopup(_landscapeLayoutType.displayName,
                    layoutGroup._landscapeLayoutType);

            EditorGUI.BeginDisabledGroup(true);
            var portraitValue = EditorGUILayout.Toggle(_isPortrait.displayName, _isPortrait.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                _isPortrait.boolValue = portraitValue;
            }

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        void ToggleLeft(Rect position, SerializedProperty property, GUIContent label)
        {
            bool toggle = property.boolValue;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            toggle = EditorGUI.ToggleLeft(position, label, toggle);
            EditorGUI.indentLevel = oldIndent;
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = property.hasMultipleDifferentValues ? true : !property.boolValue;
            }

            EditorGUI.EndProperty();
        }
    }
}
