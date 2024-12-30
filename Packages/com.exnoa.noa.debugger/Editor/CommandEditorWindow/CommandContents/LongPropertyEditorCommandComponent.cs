#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class LongPropertyEditorCommandComponent : EditorCommandComponentBase<LongPropertyCommand>
    {
        public LongPropertyEditorCommandComponent(LongPropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            string beforeValue = _command.GetValue().ToString();
            string changed = EditorGUILayout.TextField(_command.DisplayName, beforeValue);
            if (changed != beforeValue)
            {
                long value = _command.FromString(changed);
;                _command.SetValue(value);
            }
        }
    }
}
#endif
