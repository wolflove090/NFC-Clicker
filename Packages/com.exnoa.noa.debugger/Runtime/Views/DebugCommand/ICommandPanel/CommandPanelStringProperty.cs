using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelStringProperty : CommandPanelSimpleInputPropertyBase<StringPropertyCommand>
    {
        readonly LayoutElement _layoutElement = null;
        readonly float _horizontalPadding = 0f;

        public CommandPanelStringProperty(DebugCommandPanel panel, float maxContentWidth) : base(panel)
        {
            panel._inputField.CharacterLimit = _command.CharacterLimit;

            _layoutElement = panel._inputRoot.GetComponent<LayoutElement>();
            var layoutGroup = panel._inputRoot.GetComponent<LayoutGroup>();
            if (layoutGroup != null)
            {
                _horizontalPadding = layoutGroup.padding.left + layoutGroup.padding.right;
            }

            Refresh();
            OnUpdateWidth(maxContentWidth);
        }

        public override void OnUpdateWidth(float maxContentWidth)
        {
            TMP_Text text = _panel._inputField.TextComponent;
            SetPreferredWidthFromText(_layoutElement, _horizontalPadding, text, maxContentWidth);

            text.rectTransform.localPosition = Vector3.zero;
        }

        protected override string GetValueString()
        {
            return _command.GetValue();
        }

        protected override void OnEndInputEdit(string text)
        {
            _command.SetValue(text);
            Refresh();
        }
    }
}
