#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class BytePropertyEditorCommandComponent : EditorCommandComponentBase<BytePropertyCommand>
    {
        public BytePropertyEditorCommandComponent(BytePropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            int changed = EditorGUILayout.IntField(_command.DisplayName, _command.GetValue());
            byte changedValue = _command.ValidateValueForFluctuation(_command.GetValue(), changed - _command.GetValue());
            if (changedValue != _command.GetValue())
            {
                _command.SetValue(changedValue);
            }
        }
    }
}
#endif
