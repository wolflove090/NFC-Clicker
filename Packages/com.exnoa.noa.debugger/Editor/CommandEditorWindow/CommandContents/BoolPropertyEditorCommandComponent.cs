#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class BoolPropertyEditorCommandComponent : EditorCommandComponentBase<BoolPropertyCommand>
    {
        public BoolPropertyEditorCommandComponent(BoolPropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            bool changed = EditorGUILayout.Toggle(_command.DisplayName, _command.GetValue());
            if (changed != _command.GetValue())
            {
                _command.SetValue(changed);
            }
        }
    }
}
#endif
