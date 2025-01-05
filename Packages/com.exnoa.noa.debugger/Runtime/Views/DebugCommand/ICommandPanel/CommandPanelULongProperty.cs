namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelULongProperty : CommandPanelUnsignedNumberPropertyBase<ULongPropertyCommand, ulong>
    {
        public CommandPanelULongProperty(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnEndInputEdit(string text)
        {
            ulong value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
