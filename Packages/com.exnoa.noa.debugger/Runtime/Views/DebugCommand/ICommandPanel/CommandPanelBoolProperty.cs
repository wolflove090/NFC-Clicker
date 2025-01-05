using UnityEngine;

namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelBoolProperty : CommandPanelBase<BoolPropertyCommand>
    {
        public CommandPanelBoolProperty(DebugCommandPanel panel) : base(panel)
        {
            panel._nameLayout.childAlignment = TextAnchor.MiddleLeft;

            panel._toggle.SetActive(true);
            panel._toggleButton.gameObject.SetActive(true);
            panel._toggleButton._onClick.RemoveAllListeners();
            panel._toggleButton._onClick.AddListener(_OnToggleChange);

            if (DebugCommandPresenter.GetCurrentFormat() == CommandDisplayFormat.List)
            {
                panel._actions.SetActive(true);
            }

            _panel._displayName.alignment = GetDisplayNameAlignmentFromDisplayFormat();

            Refresh();
        }

        public override void Refresh()
        {
            _panel._toggleButton.Init(_command.GetValue());
        }


        void _OnToggleChange(bool isOn)
        {
            _command.SetValue(isOn);
        }
    }
}
