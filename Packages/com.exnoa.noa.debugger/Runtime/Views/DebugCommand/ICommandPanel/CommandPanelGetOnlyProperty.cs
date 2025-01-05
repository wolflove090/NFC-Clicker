using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelGetOnlyProperty : CommandPanelBase<GetOnlyPropertyCommand>
    {
        readonly LayoutElement _layoutElement = null;
        readonly float _horizontalPadding = 0f;

        public CommandPanelGetOnlyProperty(DebugCommandPanel panel, float maxContentWidth) : base(panel)
        {
            UpdateData(panel._command);

            panel._actions.SetActive(true);
            panel._valueRoot.SetActive(true);

            _panel._displayName.alignment = GetDisplayNameAlignmentFromDisplayFormat();

            _layoutElement = panel._valueRoot.GetComponent<LayoutElement>();

            if (panel._valueRoot.transform is RectTransform parentRect &&
                panel._valueText.transform is RectTransform textRect)
            {
                _horizontalPadding = parentRect.rect.width - textRect.rect.width;
            }

            Refresh();
            OnUpdateWidth(maxContentWidth);
        }

        public override void OnUpdateWidth(float maxContentWidth)
        {
            TMP_Text text = _panel._valueText;
            SetPreferredWidthFromText(_layoutElement, _horizontalPadding, text, maxContentWidth);

            text.rectTransform.localPosition = Vector3.zero;
        }

        public override void Refresh()
        {
            _panel._valueText.text = _command.GetValue();

            _panel._grayOut.SetActive(false);
        }
    }
}
