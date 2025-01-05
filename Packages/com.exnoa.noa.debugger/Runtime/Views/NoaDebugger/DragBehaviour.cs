using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NoaDebugger
{
    class DragBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        protected RectTransform _rect;
        Vector2 _originalPivot;

        public bool CanMove { get; set; }

        public bool isDragging { get; private set; }

        public event UnityAction<Vector2> OnDragging;

        public event UnityAction<Vector2> OnDragEnd;

        void Start()
        {
            _rect = transform as RectTransform;
            _originalPivot = _rect.pivot;
            CanMove = true;
            isDragging = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;

            RectTransform parentRect = transform.parent as RectTransform;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect, eventData.pointerCurrentRaycast.screenPosition, eventData.pressEventCamera, out Vector2 localPoint))
            {
                var rightEndLocalPos = _rect.transform.localPosition.x + (1f - _rect.pivot.x) * _rect.rect.width;
                var leftEndLocalPos = _rect.transform.localPosition.x - _rect.pivot.x * _rect.rect.width;
                var bottomEndLocalPos = _rect.transform.localPosition.y - _rect.pivot.y * _rect.rect.height;
                var topEndLocalPos = _rect.transform.localPosition.y + (1f - _rect.pivot.y) * _rect.rect.height;

                Vector2 originalPivot = _rect.pivot;
                Vector2 newPivot = Vector2.zero;
                newPivot.x = Mathf.InverseLerp(leftEndLocalPos, rightEndLocalPos, localPoint.x);
                newPivot.y = Mathf.InverseLerp(bottomEndLocalPos, topEndLocalPos, localPoint.y);
                _rect.pivot = newPivot;

                Vector2 diffPivot = newPivot - originalPivot;
                float diffPosX = _rect.sizeDelta.x * diffPivot.x * _rect.localScale.x;
                float diffPosY = _rect.sizeDelta.y * diffPivot.y * _rect.localScale.y;
                _rect.anchoredPosition += new Vector2(diffPosX, diffPosY);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!CanMove)
            {
                return;
            }

            Vector2 pos = Input.GetCursorPosition();

            if (pos.x > Screen.width || pos.y > Screen.height ||
                pos.x < 0 || pos.y < 0)
            {
                return;
            }

            OnDragging?.Invoke(this.transform.position);

            _rect.position = pos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 originalPivot = _rect.pivot;
            Vector2 newPivot = _originalPivot;
            _rect.pivot = _originalPivot;

            Vector2 diffPivot = newPivot - originalPivot;
            float diffPosX = _rect.sizeDelta.x * diffPivot.x * _rect.localScale.x;
            float diffPosY = _rect.sizeDelta.y * diffPivot.y * _rect.localScale.y;
            _rect.anchoredPosition += new Vector2(diffPosX, diffPosY);

            OnDragEnd?.Invoke(this.transform.position);

            isDragging = false;
        }
    }
}
