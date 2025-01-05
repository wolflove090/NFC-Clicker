#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class IntPropertyEditorCommandComponent : EditorCommandComponentBase<IntPropertyCommand>
    {
        public IntPropertyEditorCommandComponent(IntPropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            int changed = EditorGUILayout.IntField(_command.DisplayName, _command.GetValue());
            if (changed != _command.GetValue())
            {
                _command.SetValue(changed);
            }
        }
    }
}
#endif
