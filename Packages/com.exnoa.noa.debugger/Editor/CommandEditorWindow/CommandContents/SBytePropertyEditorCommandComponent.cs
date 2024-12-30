#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class SBytePropertyEditorCommandComponent : EditorCommandComponentBase<SBytePropertyCommand>
    {
        public SBytePropertyEditorCommandComponent(SBytePropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            int changed = EditorGUILayout.IntField(_command.DisplayName, _command.GetValue());
            sbyte changedValue = _command.ValidateValueForFluctuation(_command.GetValue(), changed - _command.GetValue());
            if (changedValue != _command.GetValue())
            {
                _command.SetValue(changedValue);
            }
        }
    }
}
#endif
