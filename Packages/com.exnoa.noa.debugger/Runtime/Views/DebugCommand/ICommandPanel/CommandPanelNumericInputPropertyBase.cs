using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger.DebugCommand
{
    abstract class CommandPanelNumericInputPropertyBase<TCommand, TNumericType> : CommandPanelBase<TCommand>
        where TCommand : MutableNumericPropertyCommandBase<TNumericType>
        where TNumericType : struct
    {
        const int DRAG_SENSITIVITY = 2;

        readonly LayoutElement _layoutElement = null;
        readonly float _horizontalPadding = 0f;

        TNumericType _tmpValue;
        TNumericType _beginDragValue;

        protected CommandPanelNumericInputPropertyBase(DebugCommandPanel panel) : base(panel)
        {
            panel._actions.SetActive(true);

            panel._inputSwipeRoot.gameObject.SetActive(true);
            panel._swipe.gameObject.SetActive(true);
            panel._swipe.Initialize(panel.ParentScroll, panel._inputSwipeInput);
            panel._swipe.OnBeginSlider.AddListener(_OnBeginDrag);
            panel._swipe.OnSliding.AddListener(_OnSliding);
            panel._swipe.OnEndSlider.AddListener(_SetValue);

            panel._inputSwipeInput.gameObject.SetActive(true);
            panel._inputSwipeInput._onEndEdit.RemoveAllListeners();
            panel._inputSwipeInput._onEndEdit.AddListener(OnEndInputEdit);

            panel._leftArrowButton._onPointerDown.RemoveAllListeners();
            panel._leftArrowButton._onPointerDown.AddListener(_OnDownArrowButton);
            panel._leftArrowButton._onClick.RemoveAllListeners();
            panel._leftArrowButton._onClick.AddListener(_OnClickLeftArrowButton);
            panel._leftArrowButton._onLongPress.RemoveAllListeners();
            panel._leftArrowButton._onLongPress.AddListener(_DecrementValue);
            panel._rightArrowButton._onPointerDown.RemoveAllListeners();
            panel._rightArrowButton._onPointerDown.AddListener(_OnDownArrowButton);
            panel._rightArrowButton._onClick.RemoveAllListeners();
            panel._rightArrowButton._onClick.AddListener(_OnClickRightArrowButton);
            panel._rightArrowButton._onLongPress.RemoveAllListeners();
            panel._rightArrowButton._onLongPress.AddListener(_IncrementValue);

            _panel._displayName.alignment = GetDisplayNameAlignmentFromDisplayFormat();

            _layoutElement = panel._inputSwipeRoot.GetComponent<LayoutElement>();

            if (panel._leftArrowButton.transform is RectTransform leftButtonRect &&
                panel._rightArrowButton.transform is RectTransform rightButtonRect)
            {
                _horizontalPadding = leftButtonRect.rect.width + rightButtonRect.rect.width;
            }
        }

        public override void OnUpdateWidth(float maxContentWidth)
        {
            TMP_Text text = _panel._inputSwipeInput.TextComponent;
            SetPreferredWidthFromText(_layoutElement, _horizontalPadding, text, maxContentWidth);

            text.rectTransform.localPosition = Vector3.zero;
        }

        public override void Refresh()
        {
            if (!_panel._inputSwipeInput.IsInputMode)
            {
                TNumericType value = _command.GetValue();
                _panel._inputSwipeInput.Text = value.ToString();

                _panel._leftArrowButton.SetInteractable(!_command.IsEqualOrUnderMin(value));
                _panel._rightArrowButton.SetInteractable(!_command.IsEqualOrOverMax(value));
            }
        }

        protected abstract void OnEndInputEdit(string text);

        void _OnBeginDrag()
        {
            _beginDragValue = _command.GetValue();
        }

        void _OnSliding(float distance)
        {
            _FluctuateValue(_beginDragValue, _DelimitDistance(distance));
        }

        void _OnDownArrowButton()
        {
            _panel.ParentScroll._canMoveScroll = false;
        }

        void _OnClickLeftArrowButton()
        {
            _panel.ParentScroll._canMoveScroll = true;

            _DecrementValue();
        }

        void _DecrementValue()
        {
            _FluctuateValue(_command.GetValue(), -1);
            _SetValue();
        }

        void _OnClickRightArrowButton()
        {
            _panel.ParentScroll._canMoveScroll = true;

            _IncrementValue();
        }

        void _IncrementValue()
        {
            _FluctuateValue(_command.GetValue(), 1);
            _SetValue();
        }

        void _FluctuateValue(TNumericType beginValue, int count)
        {
            _tmpValue = _command.ValidateValueForFluctuation(beginValue, count);
            _panel._inputSwipeInput.Text = _tmpValue.ToString();
        }

        void _SetValue()
        {
            _command.SetValue(_tmpValue);
            Refresh();
        }

        int _DelimitDistance(float distance)
        {
            return Mathf.FloorToInt(distance / DRAG_SENSITIVITY);
        }
    }
}
