#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class DoublePropertyEditorCommandComponent : EditorCommandComponentBase<DoublePropertyCommand>
    {
        public DoublePropertyEditorCommandComponent(DoublePropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            string beforeValue = _command.GetValue().ToString();
            string changed = EditorGUILayout.TextField(_command.DisplayName, beforeValue);
            if (changed != beforeValue)
            {
                double value = _command.FromString(changed);
                _command.SetValue(value);
            }
        }
    }
}
#endif
