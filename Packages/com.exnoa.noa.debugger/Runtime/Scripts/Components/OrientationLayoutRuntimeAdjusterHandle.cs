using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class OrientationLayoutRuntimeAdjusterHandle : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        bool _isHorizontal;
        RectTransform _rect;

        OrientationLayoutStyle _formerLayoutStyle;
        OrientationLayoutStyle _latterLayoutStyle;
        RectTransform _formerRect;
        RectTransform _latterRect;
        HorizontalOrVerticalLayoutGroup _layoutGroup;
        float _size;
        float _minWidth;
        float _minHeight;

        Vector2 _dragStartPos;
        Vector2 _dragStartSizeFormer;
        Vector2 _dragStartSizeLatter;

        bool _isSetStyleBeforeInitialize = false;

        public event UnityAction OnEndDragHandle;

        void Awake()
        {
            _rect = transform as RectTransform;
        }

        public void Initialize(OrientationLayoutStyle formerLayout, OrientationLayoutStyle latterLayout,
                               HorizontalOrVerticalLayoutGroup layoutGroup, float minWidth, float minHeight)
        {
            _formerLayoutStyle = formerLayout;
            _latterLayoutStyle = latterLayout;
            _formerRect = formerLayout.transform as RectTransform;
            _latterRect = latterLayout.transform as RectTransform;
            _layoutGroup = layoutGroup;
            _size = layoutGroup.spacing;
            _minWidth = minWidth;
            _minHeight = minHeight;

            if (_isSetStyleBeforeInitialize)
            {
                SetFromLayoutStyle();
                _isSetStyleBeforeInitialize = false;
            }
        }

        public void SetFromLayoutStyle()
        {
            if (_formerLayoutStyle == null)
            {
                _isSetStyleBeforeInitialize = true;
                return;
            }

            OrientationLayoutStyleModel layoutStyle = _formerLayoutStyle.GetCurrentLayoutStyle();
            _isHorizontal = layoutStyle._relativeWidthFlag || layoutStyle._preferredWidthFlag;

            if (_isHorizontal)
            {
                _rect.anchorMin = Vector2.zero;
                _rect.anchorMax = Vector2.up;
                _rect.pivot = new Vector2(1f, 0.5f);
                _rect.anchoredPosition = Vector2.zero;
                _rect.sizeDelta = new Vector2(_size, _rect.sizeDelta.y);
            }
            else
            {
                _rect.anchorMin = Vector2.up;
                _rect.anchorMax = Vector2.one;
                _rect.pivot = new Vector2(0.5f, 0f);
                _rect.anchoredPosition = Vector2.zero;
                _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, _size);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isHorizontal)
            {
                _formerRect.pivot = Vector2.up;
                _latterRect.pivot = Vector2.one;
            }
            else
            {
                _formerRect.pivot = Vector2.one;
                _latterRect.pivot = Vector2.right;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _layoutGroup.enabled = false;
            _dragStartPos = _GetLocalPositionFromPointerEventData(eventData);
            _dragStartSizeFormer = _formerRect.sizeDelta;
            _dragStartSizeLatter = _latterRect.sizeDelta;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos = _GetLocalPositionFromPointerEventData(eventData);
            Vector2 formerSize;
            Vector2 latterSize;

            if(_isHorizontal)
            {
                Vector2 horizontalDiff = new Vector2(_dragStartPos.x - pos.x, 0);
                formerSize = _dragStartSizeFormer - horizontalDiff;
                latterSize = _dragStartSizeLatter + horizontalDiff;

                if (formerSize.x < _minWidth)
                {
                    latterSize.x -= _minWidth - formerSize.x;
                    formerSize.x = _minWidth;
                }
                else if (latterSize.x < _minWidth)
                {
                    formerSize.x -= _minWidth - latterSize.x;
                    latterSize.x = _minWidth;
                }
            }
            else
            {
                Vector2 verticalDiff = new Vector2(0, pos.y - _dragStartPos.y);
                formerSize = _dragStartSizeFormer - verticalDiff;
                latterSize = _dragStartSizeLatter + verticalDiff;

                if (formerSize.y < _minHeight)
                {
                    latterSize.y -= _minHeight - formerSize.y;
                    formerSize.y = _minHeight;
                }
                else if (latterSize.y < _minHeight)
                {
                    formerSize.y -= _minHeight - latterSize.y;
                    latterSize.y = _minHeight;
                }
            }

            _formerRect.sizeDelta = formerSize;
            _latterRect.sizeDelta = latterSize;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragHandle?.Invoke();

            _layoutGroup.enabled = true;
        }

        Vector2 _GetLocalPositionFromPointerEventData(PointerEventData eventData)
        {
            Vector2 pos = Vector2.zero;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _latterRect, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera,
                    out Vector2 localPoint))
            {
                pos = localPoint;
            }

            return pos;
        }
    }
}
