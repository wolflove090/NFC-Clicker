namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelLongProperty : CommandPanelSignedNumberPropertyBase<LongPropertyCommand, long>
    {
        public CommandPanelLongProperty(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnEndInputEdit(string text)
        {
            long value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
