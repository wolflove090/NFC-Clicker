namespace NoaDebugger.DebugCommand
{
    abstract class CommandPanelMethodBase<TCommand> : CommandPanelBase<TCommand>
        where TCommand : CommandBase
    {
        protected CommandPanelMethodBase(DebugCommandPanel panel) : base(panel)
        {
            panel._button.gameObject.SetActive(true);
            panel._button.onClick.RemoveAllListeners();
            panel._button.onClick.AddListener(OnClick);

            _panel._displayName.alignment = _alignmentOptionCenter;
        }

        protected abstract void OnClick();
    }
}
