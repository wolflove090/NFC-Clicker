#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class StringPropertyEditorCommandComponent : EditorCommandComponentBase<StringPropertyCommand>
    {
        public StringPropertyEditorCommandComponent(StringPropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            string changed = EditorGUILayout.TextField(_command.DisplayName, _command.GetValue());
            if (changed != _command.GetValue())
            {
                _command.SetValue(changed);
            }
        }
    }
}
#endif
