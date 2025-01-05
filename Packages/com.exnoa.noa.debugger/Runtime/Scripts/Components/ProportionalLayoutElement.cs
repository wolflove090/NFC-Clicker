using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
#if NOA_DEBUGGER_DEBUG
    [AddComponentMenu("Layout/Proportional Layout Element")]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    sealed class ProportionalLayoutElement : LayoutElement
    {
        [SerializeField, Range(0, 1)]
        float _relativeWidth = 0.5f;
        [SerializeField]
        bool _relativeWidthFlag;

        [SerializeField, Range(0, 1)]
        float _relativeHeight = 0.5f;
        [SerializeField]
        bool _relativeHeightFlag;

        public float RelativeWidth
        {
            get => _relativeWidth;
            set
            {
                _relativeWidth = value;
                _SetDirty();
            }
        }
        public bool RelativeWidthFlag
        {
            get => _relativeWidthFlag;
            set
            {
                _relativeWidthFlag = value;
            }
        }

        public float RelativeHeight
        {
            get => _relativeHeight;
            set
            {
                _relativeHeight = value;
                _SetDirty();
            }
        }
        public bool RelativeHeightFlag
        {
            get => _relativeHeightFlag;
            set
            {
                _relativeHeightFlag = value;
            }
        }

        RectTransform _parentRectTransform;
        RectTransform _rectTransform;

        protected override void OnEnable()
        {
            base.OnEnable();
            _rectTransform = transform as RectTransform;
            _parentRectTransform = transform.parent as RectTransform;
            _SetDirty();
        }

        void _SetDirty()
        {
            if (!IsActive())
            {
                return;
            }

            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

        public override void CalculateLayoutInputHorizontal()
        {
            if (!_relativeWidthFlag || _parentRectTransform == null)
            {
                return;
            }

            preferredWidth = _parentRectTransform.rect.width * _relativeWidth;
            _SetDirty();
        }

        public override void CalculateLayoutInputVertical()
        {
            if (!_relativeHeightFlag || _parentRectTransform == null)
            {
                return;
            }

            preferredHeight = _parentRectTransform.rect.height * _relativeHeight;
            _SetDirty();
        }

        void Update()
        {
            if (_parentRectTransform == null)
            {
                return;
            }

            _SetLayoutStyle();
        }

        void _SetLayoutStyle()
        {
            if (_relativeWidthFlag)
            {
                preferredWidth = _parentRectTransform.rect.width * _relativeWidth;
            }

            if (_relativeHeightFlag)
            {
                preferredHeight = _parentRectTransform.rect.height * _relativeHeight;
            }

            _SetDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (_rectTransform != null && _parentRectTransform != null)
            {
                CalculateLayoutInputHorizontal();
                CalculateLayoutInputVertical();
                _SetDirty();
            }
        }
#endif
    }
}
