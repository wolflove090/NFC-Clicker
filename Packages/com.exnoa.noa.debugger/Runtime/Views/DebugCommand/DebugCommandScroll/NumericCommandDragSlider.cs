using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger.DebugCommand
{
    sealed class NumericCommandDragSlider : MonoBehaviour, IPointerDownHandler
    {
        static readonly float DragDirectionCheckDistance = 3f;

        enum DragState
        {
            None,
            PointerDown,
            HorizontalDrag,
            VerticalDrag,
        }

        public sealed class DragSliderEvent : UnityEvent<float> {}

        DragState _dragState = DragState.None;
        Vector2 _beginCursorPos;
        PointerEventData _pointerEventData;

        CommandScroll _parentScroll;
        NoaDebuggerScrollableInputComponent _numericCommandInput;

        public DragSliderEvent OnSliding = new();
        public UnityEvent OnBeginSlider = new();
        public UnityEvent OnEndSlider = new();

        public void Initialize(CommandScroll parentScroll, NoaDebuggerScrollableInputComponent numericCommandInput)
        {
            _parentScroll = parentScroll;
            _numericCommandInput = numericCommandInput;

            _numericCommandInput.OnInputModeChanged -= _OnInputModeChanged;
            _numericCommandInput.OnInputModeChanged += _OnInputModeChanged;

            OnSliding.RemoveAllListeners();
            OnBeginSlider.RemoveAllListeners();
            OnEndSlider.RemoveAllListeners();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_dragState != DragState.None)
            {
                return;
            }

            _dragState = DragState.PointerDown;
            _beginCursorPos = eventData.position;

            _pointerEventData = eventData;
        }

        void OnButtonUp()
        {
            switch (_dragState)
            {
                case DragState.PointerDown:
                    _numericCommandInput.Select();
                    break;

                case DragState.HorizontalDrag:
                    OnEndSlider?.Invoke();
                    break;

                case DragState.VerticalDrag:
                    break;
            }

            _dragState = DragState.None;
            _parentScroll._canMoveScroll = true;
        }

        void Update()
        {
            switch (_dragState)
            {
                case DragState.None:
                    return;

                case DragState.PointerDown:
                    _CheckDragDirection(_pointerEventData);
                    break;

                case DragState.HorizontalDrag:
                    float distX = _pointerEventData.position.x - _beginCursorPos.x;
                    OnSliding?.Invoke(distX);
                    break;

                case DragState.VerticalDrag:
                    break;
            }

            if (Input.IsButtonUp())
            {
                OnButtonUp();
            }
        }

        void _OnInputModeChanged(bool isInputMode)
        {
            gameObject.SetActive(!isInputMode);
        }

        void _CheckDragDirection(PointerEventData eventData)
        {
            float distX = Mathf.Abs(eventData.position.x - _beginCursorPos.x);
            float distY = Mathf.Abs(eventData.position.y - _beginCursorPos.y);

            if (distX < distY)
            {
                if (_IsNeedCheckDirection(distY))
                {
                    _dragState = DragState.VerticalDrag;
                    _parentScroll._canMoveScroll = true;
                }
            }
            else
            {
                if (_IsNeedCheckDirection(distX))
                {
                    _dragState = DragState.HorizontalDrag;
                    OnBeginSlider?.Invoke();

                    _parentScroll._canMoveScroll = false;
                }
            }
        }

        bool _IsNeedCheckDirection(float distance)
        {
            return distance >= NumericCommandDragSlider.DragDirectionCheckDistance;
        }
    }
}
