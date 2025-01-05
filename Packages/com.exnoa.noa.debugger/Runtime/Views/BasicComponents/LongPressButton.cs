using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class LongPressButton : Button
    {
        [Serializable]
        public class DisableTarget
        {
            [SerializeField]
            Image _image;
            [SerializeField]
            Color _normalColor;
            [SerializeField]
            Color _disabledColor;

            public void SetColor(bool isEnable)
            {
                _image.color = isEnable ? _normalColor : _disabledColor;
            }
        }

        enum PressState
        {
            None,

            PointerDown,

            Pressing,

            PressingSpeedUp,
        }

        [SerializeField]
        DisableTarget[] _disableTargets;

        PressState _pressState = PressState.None;
        float _lastCheckedTime;
        float _currentInterval;
        int _pressingActionCount;

        public ButtonClickedEvent _onPointerDown;

        public ButtonClickedEvent _onClick;

        public ButtonClickedEvent _onLongPress;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (_pressState != PressState.None)
            {
                return;
            }

            base.OnPointerDown(eventData);
            _onPointerDown?.Invoke();
            _pressState = PressState.PointerDown;
            _lastCheckedTime = Time.realtimeSinceStartup;
            _pressingActionCount = 0;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (_pressState == PressState.PointerDown)
            {
                _onClick?.Invoke();
            }

            _pressState = PressState.None;

            EventSystem.current.SetSelectedGameObject(null);
        }

        void Update()
        {
            switch (_pressState)
            {
                case PressState.None:
                    return;

                case PressState.PointerDown:
                    if (_IsOverThresholdTime(NoaDebuggerDefine.PressTimeSeconds))
                    {
                        _pressState = PressState.Pressing;
                    }

                    break;

                case PressState.Pressing:
                    if (_IsOverThresholdTime(NoaDebuggerDefine.PressActionFirstInterval))
                    {
                        _InvokeLongPressAction();
                        _pressingActionCount++;

                        if (_pressingActionCount >= NoaDebuggerDefine.PressActionIntervalChangeCount)
                        {
                            _pressState = PressState.PressingSpeedUp;
                        }
                    }

                    break;

                case PressState.PressingSpeedUp:
                    if (_IsOverThresholdTime(NoaDebuggerDefine.PressActionSecondInterval))
                    {
                        _InvokeLongPressAction();
                    }

                    break;
            }
        }

        bool _IsOverThresholdTime(float thresholdTime)
        {
            return Time.realtimeSinceStartup - _lastCheckedTime >= thresholdTime;
        }

        void _InvokeLongPressAction()
        {
            _onLongPress?.Invoke();

            _lastCheckedTime = Time.realtimeSinceStartup;
        }

        public void SetInteractable(bool isInteractable)
        {
            interactable = isInteractable;

            foreach (DisableTarget target in _disableTargets)
            {
                target.SetColor(isInteractable);
            }
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(LongPressButton))]
    public class LongPressButtonEditor : UnityEditor.UI.ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UnityEditor.EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_disableTargets"), new GUIContent("Disable Targets"));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
