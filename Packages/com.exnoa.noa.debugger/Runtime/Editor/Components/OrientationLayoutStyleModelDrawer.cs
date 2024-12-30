using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    [CustomPropertyDrawer(typeof(OrientationLayoutStyleModel))]
    sealed class OrientationLayoutStyleModelDrawer : PropertyDrawer
    {
        sealed class PropertyData
        {
            public SerializedProperty _size;
            public SerializedProperty _flag;
        }

        SerializedProperty _ignoreLayout;
        PropertyData _minWidthData;
        PropertyData _minHeightData;
        PropertyData _preferredWidthData;
        PropertyData _preferredHeightData;
        PropertyData _flexibleWidthData;
        PropertyData _flexibleHeightData;
        SerializedProperty _layoutPriority;

        PropertyData _relativeWidthData;
        PropertyData _relativeHeightData;

        List<PropertyData> _propertyDataList;

        void _Init(SerializedProperty property)
        {
            _propertyDataList = new List<PropertyData>();

            _ignoreLayout = property.FindPropertyRelative(nameof(_ignoreLayout));

            _minWidthData = _CreatePropertyData(property, "_minWidth");
            _minHeightData = _CreatePropertyData(property, "_minHeight");
            _preferredWidthData = _CreatePropertyData(property, "_preferredWidth");
            _preferredHeightData = _CreatePropertyData(property, "_preferredHeight");
            _flexibleWidthData = _CreatePropertyData(property, "_flexibleWidth");
            _flexibleHeightData = _CreatePropertyData(property, "_flexibleHeight");

            _layoutPriority = property.FindPropertyRelative(nameof(_layoutPriority));

            _relativeWidthData = _CreatePropertyData(property, "_relativeWidth");
            _relativeHeightData = _CreatePropertyData(property, "_relativeHeight");
        }

        PropertyData _CreatePropertyData(SerializedProperty property, string propertyName)
        {
            var propertyData = new PropertyData();
            propertyData._size = property.FindPropertyRelative(propertyName);
            propertyData._flag = property.FindPropertyRelative(propertyName + "Flag");
            _propertyDataList.Add(propertyData);
            return propertyData;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _Init(property);

            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;
            var currentPosition = new Rect(position.x, position.y, position.width, position.height);

            _TitleField(currentPosition, label);


            _ToggleField(ref currentPosition, _ignoreLayout);

            if (!_ignoreLayout.boolValue)
            {
                _FloatField(ref currentPosition, _minWidthData);
                _FloatField(ref currentPosition, _minHeightData);
                _FloatField(ref currentPosition, _preferredWidthData);
                _FloatField(ref currentPosition, _preferredHeightData);
                _FloatField(ref currentPosition, _flexibleWidthData);
                _FloatField(ref currentPosition, _flexibleHeightData);

                _SliderField(ref currentPosition, _relativeWidthData);
                _SliderField(ref currentPosition, _relativeHeightData);
            }

            _IntField(ref currentPosition, _layoutPriority);

            EditorGUI.EndProperty();
        }

        void _TitleField(Rect position, GUIContent label)
        {
            var boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.fontSize = 11;
            boxStyle.fontStyle = FontStyle.Bold | FontStyle.Italic;
            boxStyle.alignment = TextAnchor.MiddleLeft;
            boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
            GUI.Box(position, label, boxStyle);
        }

        void _ToggleField(ref Rect position, SerializedProperty propertyData)
        {
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.BeginChangeCheck();
            var priorityValue = EditorGUI.Toggle(position, new GUIContent(propertyData.displayName), propertyData.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                propertyData.boolValue = priorityValue;
            }
        }

        void _FloatField(ref Rect position, PropertyData propertyData)
        {
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var label = EditorGUI.BeginProperty(position, null, propertyData._size);
            var fieldPosition = EditorGUI.PrefixLabel(position, label);

            var toggleRect = fieldPosition;
            toggleRect.width = 16;

            var floatFieldRect = fieldPosition;
            floatFieldRect.xMin += 16;

            EditorGUI.BeginChangeCheck();
            propertyData._flag.boolValue = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, propertyData._flag.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                propertyData._size.floatValue = (propertyData._flag.boolValue ? propertyData._size.floatValue : -1f);
            }

            if (propertyData._flag.boolValue)
            {
                EditorGUIUtility.labelWidth = 4;
                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUI.FloatField(floatFieldRect, new GUIContent(" "), propertyData._size.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    propertyData._size.floatValue = Mathf.Max(0, newValue);
                }

                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUI.EndProperty();
        }

        void _IntField(ref Rect position, SerializedProperty propertyData)
        {
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.BeginChangeCheck();
            var priorityValue = EditorGUI.IntField(position, new GUIContent(propertyData.displayName), propertyData.intValue);
            if (EditorGUI.EndChangeCheck())
            {
                propertyData.intValue = priorityValue;
            }
        }

        void _SliderField(ref Rect position, PropertyData propertyData)
        {
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var label = EditorGUI.BeginProperty(position, null, propertyData._size);
            var fieldPosition = EditorGUI.PrefixLabel(position, label);

            var toggleRect = fieldPosition;
            toggleRect.width = 16;

            var floatFieldRect = fieldPosition;
            floatFieldRect.xMin += 16;

            EditorGUI.BeginChangeCheck();
            propertyData._flag.boolValue = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, propertyData._flag.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                propertyData._size.floatValue = (propertyData._flag.boolValue ? 0.5f : -1f);
            }

            if (propertyData._flag.boolValue)
            {
                EditorGUIUtility.labelWidth = 4;
                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUI.Slider(floatFieldRect, new GUIContent(" "), propertyData._size.floatValue, 0, 1);
                if (EditorGUI.EndChangeCheck())
                {
                    propertyData._size.floatValue = Mathf.Max(0, newValue);
                }

                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _Init(property);
            var type = typeof(OrientationLayoutStyleModel);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            return base.GetPropertyHeight(property, label) * (fields.Length - _propertyDataList.Count + 2) + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}
