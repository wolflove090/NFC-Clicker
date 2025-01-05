namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelUIntProperty : CommandPanelUnsignedNumberPropertyBase<UIntPropertyCommand, uint>
    {
        public CommandPanelUIntProperty(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnEndInputEdit(string text)
        {
            uint value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
