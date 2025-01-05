namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelIntProperty : CommandPanelSignedNumberPropertyBase<IntPropertyCommand, int>
    {
        public CommandPanelIntProperty(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnEndInputEdit(string text)
        {
            int value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
