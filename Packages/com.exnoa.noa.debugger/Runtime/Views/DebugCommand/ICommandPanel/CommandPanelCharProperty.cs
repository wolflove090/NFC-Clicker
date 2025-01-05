namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelCharProperty : CommandPanelSimpleInputPropertyBase<CharPropertyCommand>
    {
        public CommandPanelCharProperty(DebugCommandPanel panel) : base(panel)
        {
            panel._inputField.CharacterLimit = 1;

            Refresh();
        }

        protected override string GetValueString()
        {
            return _command.GetValue().ToString();
        }

        protected override void OnEndInputEdit(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _command.SetValue(default);
            }
            else
            {
                _command.SetValue(text[0]);
            }

            Refresh();
        }
    }
}
