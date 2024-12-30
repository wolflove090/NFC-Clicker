namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelCoroutine : CommandPanelMethodBase<CoroutineCommand>
    {
        public CommandPanelCoroutine(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        public override void Refresh()
        {
            _panel._button.interactable = _command.IsInteractable;
            _panel._grayOut.SetActive(!_command.IsInteractable);
        }

        protected override void OnClick()
        {
            _command.Invoke(Refresh);
            Refresh();
        }
    }
}
