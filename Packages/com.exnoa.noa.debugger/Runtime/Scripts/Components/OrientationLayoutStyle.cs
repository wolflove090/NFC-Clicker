using System;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    [Serializable]
    sealed class OrientationLayoutStyleModel
    {
        public bool _ignoreLayout;

        public float _minWidth = -1;
        public bool _minWidthFlag;
        public float _minHeight = -1;
        public bool _minHeightFlag;
        public float _preferredWidth = -1;
        public bool _preferredWidthFlag;
        public float _preferredHeight = -1;
        public bool _preferredHeightFlag;
        public float _flexibleWidth = -1;
        public bool _flexibleWidthFlag;
        public float _flexibleHeight = -1;
        public bool _flexibleHeightFlag;

        public float _relativeWidth = 0.5f;
        public bool _relativeWidthFlag;
        public float _relativeHeight = 0.5f;
        public bool _relativeHeightFlag;

        public int _layoutPriority = 1;
    }

#if NOA_DEBUGGER_DEBUG
    [AddComponentMenu("Layout/Orientation Layout Style")]
#endif
    [RequireComponent(typeof(RectTransform), typeof(ProportionalLayoutElement))]
    sealed class OrientationLayoutStyle : MonoBehaviour
    {
        [SerializeField] OrientationLayoutStyleModel _portraitLayoutStyle;
        [SerializeField] OrientationLayoutStyleModel _landscapeLayoutStyle;

        ProportionalLayoutElement _proportionalLayoutElement;
        OrientationAwareLayoutGroup _parentLayoutGroup;
        UIBehaviourComponent _parentUIBehaviour;
        RectTransform _rectTransform;
        RectTransform _parentRectTransform;

        bool _isInitialized = false;

        public OrientationLayoutRuntimeAdjusterHandle Handle { get; set; }

        void Awake()
        {
            _Initialize();
        }

        void OnEnable()
        {
            _SetDirty();
        }

        void _Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            _rectTransform = transform as RectTransform;
            _proportionalLayoutElement = GetComponent<ProportionalLayoutElement>();

            Transform parent = transform.parent;
            if (parent == null)
            {
                return;
            }

            _parentRectTransform = parent as RectTransform;
            _parentLayoutGroup = parent.GetComponent<OrientationAwareLayoutGroup>();
            if (_parentLayoutGroup == null)
            {
                _parentLayoutGroup = parent.gameObject.AddComponent<OrientationAwareLayoutGroup>();
                Debug.Log("[NOA Debugger] OrientationAwareLayoutGroup was not found on the parent and has been added.");
            }
            _parentUIBehaviour = parent.GetComponent<UIBehaviourComponent>();
            if (_parentUIBehaviour == null)
            {
                _parentUIBehaviour = parent.gameObject.AddComponent<UIBehaviourComponent>();
            }

            _parentUIBehaviour.OnRectTransformDimensionsChanged += _OnTargetRectTransformDimensionsChanged;
        }

        void _OnTargetRectTransformDimensionsChanged()
        {
            if (_proportionalLayoutElement == null)
            {
                _Initialize();
                _SetDirty();
            }
            _ValueChanged();
        }

        void _SetDirty()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

        void _ValueChanged()
        {
            if (_parentLayoutGroup == null)
            {
                return;
            }
            _SetLayoutStyle(GetCurrentLayoutStyle());
        }

        void _SetLayoutStyle(OrientationLayoutStyleModel layoutStyle)
        {
            _proportionalLayoutElement.ignoreLayout = layoutStyle._ignoreLayout;

            _proportionalLayoutElement.minWidth = layoutStyle._minWidthFlag ? layoutStyle._minWidth : -1;
            _proportionalLayoutElement.minHeight = layoutStyle._minHeightFlag ? layoutStyle._minHeight : -1;
            if (!layoutStyle._relativeWidthFlag)
            {
                _proportionalLayoutElement.preferredWidth = layoutStyle._preferredWidthFlag ? layoutStyle._preferredWidth : -1;
            }
            if (!layoutStyle._relativeHeightFlag)
            {
                _proportionalLayoutElement.preferredHeight = layoutStyle._preferredHeightFlag ? layoutStyle._preferredHeight : -1;
            }
            _proportionalLayoutElement.flexibleWidth = layoutStyle._flexibleWidthFlag ? layoutStyle._flexibleWidth : -1;
            _proportionalLayoutElement.flexibleHeight = layoutStyle._flexibleHeightFlag ? layoutStyle._flexibleHeight : -1;

            _proportionalLayoutElement.layoutPriority = layoutStyle._layoutPriority;

            _proportionalLayoutElement.RelativeWidth = layoutStyle._relativeWidthFlag ? layoutStyle._relativeWidth : -1;
            _proportionalLayoutElement.RelativeHeight = layoutStyle._relativeHeightFlag ? layoutStyle._relativeHeight : -1;
            _proportionalLayoutElement.RelativeWidthFlag = layoutStyle._relativeWidthFlag;
            _proportionalLayoutElement.RelativeHeightFlag = layoutStyle._relativeHeightFlag;

            _SetDirty();
        }

        public void OverwriteLayoutByCurrentSize(int layoutsCount)
        {
            OrientationLayoutStyleModel layoutStyle;
            float totalSize;
            float totalSpacing = _parentLayoutGroup.spacing * (layoutsCount - 1);

            if (_parentLayoutGroup.IsPortrait)
            {
                layoutStyle = _portraitLayoutStyle;

                totalSize = _parentRectTransform.sizeDelta.y
                            - _parentLayoutGroup.padding.top
                            - _parentLayoutGroup.padding.bottom
                            - totalSpacing;
            }
            else
            {
                layoutStyle = _landscapeLayoutStyle;

                totalSize = _parentRectTransform.sizeDelta.x
                            - _parentLayoutGroup.padding.left
                            - _parentLayoutGroup.padding.right
                            - totalSpacing;
            }

            if (layoutStyle._relativeWidthFlag)
            {
                layoutStyle._relativeWidth = _rectTransform.rect.width / totalSize;
            }
            if (layoutStyle._relativeHeightFlag)
            {
                layoutStyle._relativeHeight = _rectTransform.rect.height / totalSize;
            }

            _SetLayoutStyle(layoutStyle);
        }

        public void SetHandleFromLayoutStyle()
        {
            if (Handle == null || _parentLayoutGroup == null)
            {
                return;
            }

            Handle.SetFromLayoutStyle();
        }

        public OrientationLayoutStyleModel GetCurrentLayoutStyle()
        {
            return _parentLayoutGroup.IsPortrait ? _portraitLayoutStyle : _landscapeLayoutStyle;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (_proportionalLayoutElement == null)
            {
                _Initialize();
                _SetDirty();
                return;
            }

            _ValueChanged();
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            OnValidate();
        }
#endif

    }
}
