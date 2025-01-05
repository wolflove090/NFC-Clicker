namespace NoaDebugger.DebugCommand
{
    interface ICommandPanel
    {
        public void OnUpdateWidth(float maxContentWidth);

        public void UpdateData(ICommand command);

        public void Refresh();
    }
}
