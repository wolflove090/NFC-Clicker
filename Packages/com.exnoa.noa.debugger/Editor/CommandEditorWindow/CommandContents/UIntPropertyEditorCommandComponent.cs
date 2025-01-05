#if NOA_DEBUGGER
using System;
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class UIntPropertyEditorCommandComponent : EditorCommandComponentBase<UIntPropertyCommand>
    {
        public UIntPropertyEditorCommandComponent(UIntPropertyCommand command) : base(command) { }

        protected override void _Draw()
        {
            long changed = EditorGUILayout.LongField(_command.DisplayName, _command.GetValue());
            string changedString = changed.ToString();
            uint changedValue = _command.FromString(changedString);
            if (changedValue != _command.GetValue())
            {
                _command.SetValue(changedValue);
            }
        }
    }
}
#endif
