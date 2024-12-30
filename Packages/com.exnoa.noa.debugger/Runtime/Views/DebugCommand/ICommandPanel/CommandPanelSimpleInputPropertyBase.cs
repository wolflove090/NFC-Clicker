namespace NoaDebugger.DebugCommand
{
    abstract class CommandPanelSimpleInputPropertyBase<TCommand> : CommandPanelBase<TCommand>
        where TCommand : CommandBase
    {
        protected CommandPanelSimpleInputPropertyBase(DebugCommandPanel panel) : base(panel)
        {
            panel._actions.SetActive(true);

            panel._inputRoot.gameObject.SetActive(true);
            panel._inputField.gameObject.SetActive(true);
            panel._inputField._onEndEdit.RemoveAllListeners();
            panel._inputField._onEndEdit.AddListener(OnEndInputEdit);

            _panel._displayName.alignment = GetDisplayNameAlignmentFromDisplayFormat();
        }

        public override void Refresh()
        {
            if (!_panel._inputField.IsInputMode)
            {
                _panel._inputField.Text = GetValueString();
            }
        }

        protected abstract string GetValueString();

        protected virtual void OnEndInputEdit(string text) { }
    }
}
