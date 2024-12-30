using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class DoubleTapButton : Button
    {
        const float DOUBLE_TAP_INTERVAL = 0.2f;

        float _firstClickTime;

        public ButtonClickedEvent _onDoubleTap;

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (_firstClickTime > 0)
            {
                float secondClickTime = Time.realtimeSinceStartup;
                float interval = secondClickTime - _firstClickTime;
                if (interval <= DOUBLE_TAP_INTERVAL)
                {
                    _onDoubleTap?.Invoke();
                }
            }

            _firstClickTime = Time.realtimeSinceStartup;
            onClick?.Invoke();
        }
    }
}
