namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelSByteProperty : CommandPanelSignedNumberPropertyBase<SBytePropertyCommand, sbyte>
    {
        public CommandPanelSByteProperty(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnEndInputEdit(string text)
        {
            sbyte value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
