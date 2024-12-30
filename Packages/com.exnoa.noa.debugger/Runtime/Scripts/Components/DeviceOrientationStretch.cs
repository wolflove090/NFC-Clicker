#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace NoaDebugger
{
    [RequireComponent(typeof(RectTransform))]
    sealed class DeviceOrientationStretch : MonoBehaviour
    {
        const string KEY = "DeviceOrientationStretch";

        [SerializeField]
        Vector2 _landscapeOffsetMin;
        [SerializeField]
        Vector2 _landscapeOffsetMax;

        [SerializeField]
        Vector2 _portraitOffsetMin;
        [SerializeField]
        Vector2 _portraitOffsetMax;

        RectTransform _rectTransform;
        string _key;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _key = $"{DeviceOrientationStretch.KEY}-{gameObject.GetHashCode()}";
            if (!DeviceOrientationManager.ContainsKey(_key))
            {
                DeviceOrientationManager.SetAction(_key, _OnChangeDeviceOrientation);
            }

            _OnChangeDeviceOrientation(DeviceOrientationManager.IsPortrait);
        }

        void _OnChangeDeviceOrientation(bool isPortrait)
        {
            if (this == null)
            {
                return;
            }

            if (DeviceOrientationManager.IsPortrait)
            {
                _rectTransform.offsetMin = _portraitOffsetMin;
                _rectTransform.offsetMax = _portraitOffsetMax;
            }
            else
            {
                _rectTransform.offsetMin = _landscapeOffsetMin;
                _rectTransform.offsetMax = _landscapeOffsetMax;
            }
        }

        public void CopyToLandscapeOffset()
        {
            var rect = GetComponent<RectTransform>();
            _landscapeOffsetMin = rect.offsetMin;
            _landscapeOffsetMax = rect.offsetMax;
        }

        public void CopyToPortraitOffset()
        {
            var rect = GetComponent<RectTransform>();
            _portraitOffsetMin = rect.offsetMin;
            _portraitOffsetMax = rect.offsetMax;
        }

        void OnDestroy()
        {
            if (DeviceOrientationManager.ContainsKey(_key))
            {
                DeviceOrientationManager.DeleteAction(_key);
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(DeviceOrientationStretch))]
    sealed class DeviceOrientationStretchEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var component = (DeviceOrientationStretch) target;

            if (GUILayout.Button("Copy to landscape offset"))
            {
                component.CopyToLandscapeOffset();
                EditorUtility.SetDirty(component);
            }

            if (GUILayout.Button("Copy to portrait offset"))
            {
                component.CopyToPortraitOffset();
                EditorUtility.SetDirty(component);
            }
        }
    }
#endif
}
