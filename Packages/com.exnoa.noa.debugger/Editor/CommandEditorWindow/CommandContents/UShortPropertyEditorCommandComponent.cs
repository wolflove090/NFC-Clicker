#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class UShortPropertyEditorCommandComponent : EditorCommandComponentBase<UShortPropertyCommand>
    {
        public UShortPropertyEditorCommandComponent(UShortPropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            int changed = EditorGUILayout.IntField(_command.DisplayName, _command.GetValue());
            ushort changedValue = _command.ValidateValueForFluctuation(_command.GetValue(), changed - _command.GetValue());
            if (changedValue != _command.GetValue())
            {
                _command.SetValue(changedValue);
            }
        }
    }
}
#endif
