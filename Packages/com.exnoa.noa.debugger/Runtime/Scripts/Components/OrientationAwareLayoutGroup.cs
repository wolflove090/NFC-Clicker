using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
#if NOA_DEBUGGER_DEBUG
    [AddComponentMenu("Layout/Orientation Aware Layout Group")]
#endif
    sealed class OrientationAwareLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        public enum LayoutType
        {
            Horizontal = 0,
            Vertical = 1,
        }

        public bool IsPortrait => _isPortrait = DeviceOrientationManager.IsPortrait;

        [SerializeField] public LayoutType _portraitLayoutType;
        [SerializeField] public LayoutType _landscapeLayoutType;
        [SerializeField] bool _isPortrait;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            var currentLayoutType = IsPortrait ? _portraitLayoutType : _landscapeLayoutType;

            switch (currentLayoutType)
            {
                case LayoutType.Horizontal:
                    CalcAlongAxis(0, false);
                    break;
                case LayoutType.Vertical:
                    CalcAlongAxis(0, true);
                    break;
            }
        }

        public override void CalculateLayoutInputVertical()
        {
            var currentLayoutType = IsPortrait ? _portraitLayoutType : _landscapeLayoutType;

            switch (currentLayoutType)
            {
                case LayoutType.Horizontal:
                    CalcAlongAxis(1, false);
                    break;
                case LayoutType.Vertical:
                    CalcAlongAxis(1, true);
                    break;
            }
        }

        public override void SetLayoutHorizontal()
        {
            var currentLayoutType = IsPortrait ? _portraitLayoutType : _landscapeLayoutType;

            switch (currentLayoutType)
            {
                case LayoutType.Horizontal:
                    SetChildrenAlongAxis(0, false);
                    break;
                case LayoutType.Vertical:
                    SetChildrenAlongAxis(0, true);
                    break;
            }
        }

        public override void SetLayoutVertical()
        {
            var currentLayoutType = IsPortrait ? _portraitLayoutType : _landscapeLayoutType;

            switch (currentLayoutType)
            {
                case LayoutType.Horizontal:
                    SetChildrenAlongAxis(1, false);
                    break;
                case LayoutType.Vertical:
                    SetChildrenAlongAxis(1, true);
                    break;
            }
        }
    }
}
