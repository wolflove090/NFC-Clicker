#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class NoaDebuggerDisableButton : Button
    {
        [SerializeField]
        GameObject _enable;
        [SerializeField]
        GameObject _disable;
        
        public bool Interactable
        {
            set
            {
                interactable = value;
                _ViewRefresh();
            }
            get
            {
                return interactable;
            }
        }

        void _ViewRefresh()
        {
            if (_enable != null)
            {
                _enable.SetActive(Interactable);
            }

            if (_disable != null)
            {
                _disable.SetActive(!Interactable);
            }
        }
    }

#if UNITY_EDITOR
    
    [CustomEditor(typeof(NoaDebuggerDisableButton))]
    sealed class NoaDebuggerDisableButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var t = target as NoaDebuggerDisableButton;
            
            SerializedProperty enable = serializedObject.FindProperty("_enable");
            if (enable == null)
            {
                Debug.LogError("Could not get the variable _enable");
                return;
            }
            EditorGUILayout.PropertyField(enable);
            
            SerializedProperty disable = serializedObject.FindProperty("_disable");
            if (disable == null)
            {
                Debug.LogError("Could not get the variable _disable");
                return;
            }
            EditorGUILayout.PropertyField(disable);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
