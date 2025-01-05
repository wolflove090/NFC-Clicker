using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class LongTapButton : Button
    {
        bool _isPressing;
        float _pressedTime;

        [Serializable]
        public sealed class PointerExitEvent  : UnityEvent<PointerEventData> {}

        public PointerExitEvent _onPointerExit;

        public ButtonClickedEvent _onPointerDown;

        public ButtonClickedEvent _onLongTap;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _isPressing = true;
            _pressedTime = Time.realtimeSinceStartup;
            _onPointerDown?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _onPointerExit?.Invoke(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _isPressing = false;

            EventSystem.current.SetSelectedGameObject(null);
        }

        void Update()
        {
            if (!_isPressing)
            {
                return;
            }

            var pressingTime = Time.realtimeSinceStartup - _pressedTime;

            if (pressingTime >= NoaDebuggerDefine.PressTimeSeconds)
            {
                _onLongTap?.Invoke();
                _isPressing = false;
            }
        }
    }
}
