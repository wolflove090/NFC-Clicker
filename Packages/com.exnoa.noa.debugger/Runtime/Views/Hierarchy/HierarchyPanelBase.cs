using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    class HierarchyPanelBase<T> : MonoBehaviour
        where T : HierarchyPanelDataBase
    {
        const int PADDING_PER_DEPTH = 10;
        readonly Color DefaultColor = new Color32(0xFF, 0xFF, 0xFF, 0xCC);

        [SerializeField, Header("Basic Element")] Image _background;
        [SerializeField] LayoutElement _layoutElement;
        [SerializeField] LayoutGroup _depthPaddingTarget;
        [SerializeField] Button _toggle;
        [SerializeField] RectTransform _toggleIcon;

        RectTransform _rectTransform;
        protected T _data;

        public void Draw(T panelData)
        {
            _data = panelData;
            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }

            _background.color = panelData._depth == 0 ? DefaultColor : NoaDebuggerDefine.ImageColors.Clear;
            _depthPaddingTarget.padding.left = PADDING_PER_DEPTH * panelData._depth;

            _toggle.gameObject.SetActive(panelData._hasChildren);
            _toggle.onClick.RemoveAllListeners();
            _toggle.onClick.AddListener(() => {
                _OnToggleButton();
                _data._onUpdateView?.Invoke();
            });
            _RefreshToggleIcon();

            _Draw(panelData);

            SetPanelWidth(_rectTransform.rect.width);
        }

        protected virtual void _Draw(T panelData) { }

        public float GetPanelWidth()
        {
            return _rectTransform.sizeDelta.x;
        }

        public float GetLabelLength()
        {
            return _GetLabelLength();
        }

        protected virtual float _GetLabelLength()
        {
            return 0;
        }

        public void SetPanelWidth(float width)
        {
            _layoutElement.minWidth = width;
        }

        void _RefreshToggleIcon()
        {
            Vector3 newRotation = _toggleIcon.eulerAngles;
            newRotation.z = _data._isOpen ? 0 : 90;
            _toggleIcon.eulerAngles = newRotation;
        }

        protected void _OnToggleButton()
        {
            _data._toggleOpen?.Invoke();
            _RefreshToggleIcon();
        }
    }
}
