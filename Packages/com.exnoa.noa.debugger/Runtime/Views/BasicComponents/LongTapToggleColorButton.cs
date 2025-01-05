using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger
{
    [RequireComponent(typeof(LongTapButton))]
    sealed class LongTapToggleColorButton : MonoBehaviour
    {
        [Serializable]
        public sealed class ToggleEvent  : UnityEvent<bool> {}

        LongTapButton _component;

        [SerializeField]
        Color _off = NoaDebuggerDefine.ImageColors.Default;
        [SerializeField]
        Color _on = NoaDebuggerDefine.ImageColors.Default;
        [SerializeField]
        Color _disable = NoaDebuggerDefine.ImageColors.Disabled;
        [SerializeField]
        bool _isUseDisable = false;
        [SerializeField]
        Graphic[]  _targetGraphics;

        public bool IsOn { private set; get; }

        public Button.ButtonClickedEvent OnPointerDown = new Button.ButtonClickedEvent();

        public Button.ButtonClickedEvent OnClick = new Button.ButtonClickedEvent();

        public ToggleEvent OnToggle;

        bool _clickEventEnabled = true;

        LongTapButton LongTapButton
        {
            get
            {
                if (_component == null)
                {
                    _component = GetComponent<LongTapButton>();
                }
                return _component;
            }
        }

        public bool Interactable
        {
            get
            {
                return LongTapButton.interactable;
            }
            set
            {
                LongTapButton.interactable = value;
                _Refresh();
            }
        }

        void Awake()
        {
            foreach (Graphic graphic in _targetGraphics)
            {
                Assert.IsNotNull(graphic);
            }

            _Refresh();
            LongTapButton._onPointerDown.AddListener(_OnPointerDown);
            LongTapButton.onClick.AddListener(_OnClick);
            LongTapButton._onLongTap.AddListener(_Toggle);
            LongTapButton._onPointerExit.AddListener(_OnPointerExit);
        }

        public void Init(bool isOn)
        {
            IsOn = isOn;
            _Refresh();
        }

        public void Clear()
        {
            IsOn = false;
            OnToggle?.Invoke(IsOn);
            _Refresh();
        }

        void _OnPointerDown()
        {
            _clickEventEnabled = true;

            OnPointerDown.Invoke();
        }

        void _OnPointerExit(PointerEventData eventData)
        {
            LongTapButton.OnPointerUp(eventData);
        }

        void _OnClick()
        {
            if (!_clickEventEnabled)
            {
                return;
            }
            OnClick?.Invoke();
        }

        void _Toggle()
        {
            _clickEventEnabled = false;

            IsOn = !IsOn;
            OnToggle?.Invoke(IsOn);
            _Refresh();
        }

        void _Refresh()
        {
            Color newColor;
            if (_isUseDisable && !Interactable)
            {
                newColor = _disable;
            }
            else
            {
                newColor = IsOn ? _on : _off;
            }

            foreach (Graphic graphic in _targetGraphics)
            {
                graphic.color = newColor;
            }
        }
    }
}
