namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelDecimalProperty : CommandPanelFloatingPointNumberPropertyBase<DecimalPropertyCommand, decimal>
    {
        public CommandPanelDecimalProperty(DebugCommandPanel panel, float maxContentWidth) : base(panel)
        {
            Refresh();
            OnUpdateWidth(maxContentWidth);
        }

        protected override void OnEndInputEdit(string text)
        {
            decimal value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
