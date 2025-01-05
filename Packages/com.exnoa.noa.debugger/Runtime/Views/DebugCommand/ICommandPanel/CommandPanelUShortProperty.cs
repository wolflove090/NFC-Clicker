namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelUShortProperty : CommandPanelUnsignedNumberPropertyBase<UShortPropertyCommand, ushort>
    {
        public CommandPanelUShortProperty(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnEndInputEdit(string text)
        {
            ushort value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
