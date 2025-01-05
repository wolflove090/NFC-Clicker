namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelMethod : CommandPanelMethodBase<MethodCommand>
    {
        public CommandPanelMethod(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnClick()
        {
            _command.Invoke();
        }
    }
}
