using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    [CustomEditor(typeof(ProportionalLayoutElement))]
    [CanEditMultipleObjects]
    public class ProportionalLayoutElementEditor : Editor
    {
        SerializedProperty _ignoreLayout;
        SerializedProperty _minWidth;
        SerializedProperty _minHeight;
        SerializedProperty _preferredWidth;
        SerializedProperty _preferredHeight;
        SerializedProperty _flexibleWidth;
        SerializedProperty _flexibleHeight;
        SerializedProperty _layoutPriority;

        SerializedProperty _relativeWidth;
        SerializedProperty _relativeWidthFlag;
        SerializedProperty _relativeHeight;
        SerializedProperty _relativeHeightFlag;

        protected virtual void OnEnable()
        {
            _ignoreLayout = serializedObject.FindProperty("m_IgnoreLayout");
            _minWidth = serializedObject.FindProperty("m_MinWidth");
            _minHeight = serializedObject.FindProperty("m_MinHeight");
            _preferredWidth = serializedObject.FindProperty("m_PreferredWidth");
            _preferredHeight = serializedObject.FindProperty("m_PreferredHeight");
            _flexibleWidth = serializedObject.FindProperty("m_FlexibleWidth");
            _flexibleHeight = serializedObject.FindProperty("m_FlexibleHeight");
            _layoutPriority = serializedObject.FindProperty("m_LayoutPriority");

            _relativeWidth = serializedObject.FindProperty("_relativeWidth");
            _relativeWidthFlag = serializedObject.FindProperty("_relativeWidthFlag");
            _relativeHeight = serializedObject.FindProperty("_relativeHeight");
            _relativeHeightFlag = serializedObject.FindProperty("_relativeHeightFlag");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_ignoreLayout);

            if (!_ignoreLayout.boolValue)
            {
                EditorGUILayout.Space();

                _FloatField(_minWidth, 0);
                _FloatField(_minHeight, 0);
                _FloatField(_preferredWidth, t => t.rect.width);
                _FloatField(_preferredHeight, t => t.rect.height);
                _FloatField(_flexibleWidth, 1);
                _FloatField(_flexibleHeight, 1);

                _SliderField(_relativeWidth, _ => 1, _relativeWidthFlag);
                _SliderField(_relativeHeight, _ => 1, _relativeHeightFlag);
            }

            EditorGUILayout.PropertyField(_layoutPriority);

            serializedObject.ApplyModifiedProperties();
        }

        void _FloatField(SerializedProperty property, float defaultValue)
        {
            _FloatField(property, _ => defaultValue);
        }

        void _FloatField(SerializedProperty property, System.Func<RectTransform, float> defaultValue)
        {
            Rect position = EditorGUILayout.GetControlRect();
            var label = EditorGUI.BeginProperty(position, null, property);
            var fieldPosition = EditorGUI.PrefixLabel(position, label);

            var toggleRect = fieldPosition;
            toggleRect.width = 16;

            var floatFieldRect = fieldPosition;
            floatFieldRect.xMin += 16;

            EditorGUI.BeginChangeCheck();
            bool enabled = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, property.floatValue >= 0);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue =
                    (enabled ? defaultValue((target as ProportionalLayoutElement).transform as RectTransform) : -1);
            }

            if (!property.hasMultipleDifferentValues && property.floatValue >= 0)
            {
                EditorGUIUtility.labelWidth = 4;
                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUI.FloatField(floatFieldRect, new GUIContent(" "), property.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = Mathf.Max(0, newValue);
                }

                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUI.EndProperty();
        }

        void _SliderField(SerializedProperty property, System.Func<RectTransform, float> defaultValue, SerializedProperty flagProperty)
        {
            var position = EditorGUILayout.GetControlRect();
            var label = EditorGUI.BeginProperty(position, null, property);
            var fieldPosition = EditorGUI.PrefixLabel(position, label);

            var toggleRect = fieldPosition;
            toggleRect.width = 16;

            var floatFieldRect = fieldPosition;
            floatFieldRect.xMin += 16;

            EditorGUI.BeginChangeCheck();
            var enabled = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, property.floatValue >= 0);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue =
                    (enabled ? defaultValue((target as ProportionalLayoutElement).transform as RectTransform) : -1);
            }

            flagProperty.boolValue = enabled;

            if (!property.hasMultipleDifferentValues && property.floatValue >= 0)
            {
                EditorGUIUtility.labelWidth = 4;
                EditorGUI.BeginChangeCheck();

                var newValue = EditorGUI.Slider(floatFieldRect, new GUIContent(" "), property.floatValue, 0, 1);
                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = Mathf.Max(0, newValue);
                }

                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUI.EndProperty();
        }
    }
}
