using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class ScrollRectHeightFitter : UIBehaviour
    {
        [SerializeField]
        ScrollRect _resizeTargetScrollRect;

        [SerializeField]
        bool _layoutElementMinHeightIsTarget;

        RectTransform _scrollRectTransform;
        
        LayoutElement _layoutElement;

        RectTransform _selfTransform;

        float _horizontalScrollbarHeight;

        protected override void Awake()
        {
            Assert.IsNotNull(_resizeTargetScrollRect);

            _scrollRectTransform = _resizeTargetScrollRect.transform as RectTransform;
            if (_resizeTargetScrollRect.horizontalScrollbar != null)
            {
                var horizontalScrollbarRect = _resizeTargetScrollRect.horizontalScrollbar.transform as RectTransform;
                if (horizontalScrollbarRect != null)
                {
                    _horizontalScrollbarHeight = horizontalScrollbarRect.sizeDelta.y +
                                                 _resizeTargetScrollRect.horizontalScrollbarSpacing;
                }

                if (_layoutElementMinHeightIsTarget)
                {
                    _layoutElement = _resizeTargetScrollRect.GetComponent<LayoutElement>();
                }
            }
            _selfTransform = transform as RectTransform;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (!IsActive())
            {
                return;
            }

            Vector2 selfSize = _selfTransform.sizeDelta;
            Vector2 scrollRectSize = _scrollRectTransform.sizeDelta;
            bool showsScrollbar = selfSize.x > scrollRectSize.x;
            float scrollRectWidth = scrollRectSize.x;
            float scrollRectHeight = (_resizeTargetScrollRect.horizontalScrollbar != null && showsScrollbar)
                ? selfSize.y + _horizontalScrollbarHeight
                : selfSize.y;
            _scrollRectTransform.sizeDelta = new Vector2(scrollRectWidth, scrollRectHeight);
            if (_layoutElement != null)
            {
                _layoutElement.minHeight = scrollRectHeight;
            }
        }
    }
}
