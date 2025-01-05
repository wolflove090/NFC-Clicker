#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class ULongPropertyEditorCommandComponent : EditorCommandComponentBase<ULongPropertyCommand>
    {
        public ULongPropertyEditorCommandComponent(ULongPropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            string beforeValue = _command.GetValue().ToString();
            string changed = EditorGUILayout.TextField(_command.DisplayName, beforeValue);
            if (changed != beforeValue)
            {
                ulong value = _command.FromString(changed);
                _command.SetValue(value);
            }
        }
    }
}
#endif
