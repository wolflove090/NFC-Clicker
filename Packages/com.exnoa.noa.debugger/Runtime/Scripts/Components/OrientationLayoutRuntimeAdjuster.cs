using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NoaDebugger
{
    [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
    sealed class OrientationLayoutRuntimeAdjuster : UIBehaviourComponent
    {
        static readonly float SPACING_MIN = 6.0f;

        [SerializeField]
        List<OrientationLayoutStyle> _layoutStyleList;
        [SerializeField]
        OrientationLayoutStyle _layoutStyleFirst;
        [SerializeField]
        OrientationLayoutStyle _layoutStyleSecond;
        [SerializeField]
        OrientationLayoutRuntimeAdjusterHandle _handlePrefab;
        [SerializeField]
        float _minWidthRatio = 0.2f;
        [SerializeField]
        float _minHeightRatio = 0.2f;

        HorizontalOrVerticalLayoutGroup _layoutGroup;
        RectTransform _layoutGroupRect;

        void _OnValidateUI()
        {
            foreach (OrientationLayoutStyle layoutStyle in _layoutStyleList)
            {
                Assert.IsNotNull(layoutStyle);
            }

            Assert.IsNotNull(_layoutStyleFirst);
            Assert.IsNotNull(_layoutStyleSecond);
            Assert.IsNotNull(_handlePrefab);
        }

        protected override void Awake()
        {
            base.Awake();

            _OnValidateUI();

            _layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
            _layoutGroupRect = _layoutGroup.transform as RectTransform;

            if (_layoutGroup.spacing < OrientationLayoutRuntimeAdjuster.SPACING_MIN)
            {
                LogModel.LogWarning($"Set the LayoutGroup's spacing to {OrientationLayoutRuntimeAdjuster.SPACING_MIN} or more");
            }

            for (int i = 1; i < _layoutStyleList.Count; i++)
            {
                OrientationLayoutStyle layout = _layoutStyleList[i];
                layout.Handle = Instantiate(_handlePrefab, layout.transform);
                layout.Handle.OnEndDragHandle += _OnEndDragHandle;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            foreach (OrientationLayoutStyle layout in _layoutStyleList)
            {
                layout.SetHandleFromLayoutStyle();
            }
        }

        void _OnEndDragHandle()
        {
            foreach (OrientationLayoutStyle layout in _layoutStyleList)
            {
                layout.OverwriteLayoutByCurrentSize(_layoutStyleList.Count);
            }

        }

        public void SetHandleTargetLayouts()
        {
            List<OrientationLayoutStyle> activeLayoutList =
                _layoutStyleList.Where(x => x.gameObject.activeInHierarchy).ToList();

            bool isFirst = true;
            for (int i = 0; i < activeLayoutList.Count - 1; i++)
            {
                OrientationLayoutStyle former = activeLayoutList[i];
                OrientationLayoutStyle latter = activeLayoutList[i + 1];

                if (isFirst)
                {
                    if (former.Handle != null)
                    {
                        former.Handle.gameObject.SetActive(false);
                    }

                    isFirst = false;
                }

                Vector2 rectSize = _layoutGroupRect.sizeDelta;
                latter.Handle.gameObject.SetActive(true);
                latter.Handle.Initialize(former, latter, _layoutGroup,
                                          rectSize.x * _minWidthRatio, rectSize.y * _minHeightRatio);
            }
        }
    }
}
