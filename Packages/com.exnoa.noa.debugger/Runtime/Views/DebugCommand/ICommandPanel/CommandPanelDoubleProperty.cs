namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelDoubleProperty : CommandPanelFloatingPointNumberPropertyBase<DoublePropertyCommand, double>
    {
        public CommandPanelDoubleProperty(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnEndInputEdit(string text)
        {
            double value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
