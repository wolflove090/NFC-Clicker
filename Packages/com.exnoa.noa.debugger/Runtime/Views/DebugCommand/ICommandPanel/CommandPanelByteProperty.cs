namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelByteProperty : CommandPanelUnsignedNumberPropertyBase<BytePropertyCommand, byte>
    {
        public CommandPanelByteProperty(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnEndInputEdit(string text)
        {
            byte value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
