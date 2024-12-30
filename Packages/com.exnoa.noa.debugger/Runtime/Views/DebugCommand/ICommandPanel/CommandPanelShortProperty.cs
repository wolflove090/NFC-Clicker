namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelShortProperty : CommandPanelSignedNumberPropertyBase<ShortPropertyCommand, short>
    {
        public CommandPanelShortProperty(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnEndInputEdit(string text)
        {
            short value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
