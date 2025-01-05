using UnityEngine;
using UnityEngine.EventSystems;

namespace NoaDebugger
{
    sealed class DragBehaviourFitWithInScreen : DragBehaviour
    {
        [SerializeField]
        RectTransform[] _fitWithInScreenRects;

        public override void OnDrag(PointerEventData eventData)
        {
            if (!CanMove)
            {
                return;
            }

            Vector2 pos = Input.GetCursorPosition();

            _rect.position = pos;

            Vector3[] corners = _GetOutermostPositions();

            var screenBottomLeft = Vector3.zero;
            var screenTopRight = new Vector3(Screen.width, Screen.height, 0);

            Vector3 diffPos = Vector3.zero;
            if (corners[3].x > screenTopRight.x)
            {
                diffPos.x = screenTopRight.x - corners[3].x;
            }

            if (corners[2].y > screenTopRight.y)
            {
                diffPos.y = screenTopRight.y - corners[2].y;
            }

            if (corners[1].x < screenBottomLeft.x)
            {
                diffPos.x = screenBottomLeft.x - corners[1].x;
            }

            if (corners[0].y < screenBottomLeft.y)
            {
                diffPos.y = screenBottomLeft.y - corners[0].y;
            }

            _rect.position += diffPos;
        }

        Vector3[] _GetOutermostPositions()
        {
            Vector3[] baseCorners = new Vector3[4];
            RectTransform rect = transform as RectTransform;
            rect.GetWorldCorners(baseCorners);

            Vector3 outerBottom = baseCorners[0];
            Vector3 outerLeft = baseCorners[0];
            Vector3 outerTop = baseCorners[2];
            Vector3 outerRight = baseCorners[2];

            foreach (RectTransform childRect in _fitWithInScreenRects)
            {
                Vector3[] corners = new Vector3[4];
                childRect.GetWorldCorners(corners);

                if (corners[0].y < outerBottom.y)
                {
                    outerBottom = corners[0];
                }
                if (corners[0].x < outerLeft.x)
                {
                    outerLeft = corners[0];
                }

                if (corners[2].y > outerTop.y)
                {
                    outerTop = corners[2];
                }
                if (corners[2].x > outerRight.x)
                {
                    outerRight = corners[2];
                }
            }

            return new Vector3[]
            {
                outerBottom, outerLeft, outerTop, outerRight
            };
        }
    }
}
