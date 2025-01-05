using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelFloatProperty : CommandPanelFloatingPointNumberPropertyBase<FloatPropertyCommand, float>
    {
        public CommandPanelFloatProperty(DebugCommandPanel panel) : base(panel)
        {
            Refresh();
        }

        protected override void OnEndInputEdit(string text)
        {
            float value = _command.FromString(text);
            _command.SetValue(value);
            Refresh();
        }
    }
}
