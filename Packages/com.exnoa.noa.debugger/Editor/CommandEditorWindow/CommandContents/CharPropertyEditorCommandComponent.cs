#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class CharPropertyEditorCommandComponent : EditorCommandComponentBase<CharPropertyCommand>
    {
        public CharPropertyEditorCommandComponent(CharPropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            char changed = (char)EditorGUILayout.TextField(_command.DisplayName, _command.GetValue().ToString())[0];
            if (changed != _command.GetValue())
            {
                _command.SetValue(changed);
            }
        }
    }
}
#endif
