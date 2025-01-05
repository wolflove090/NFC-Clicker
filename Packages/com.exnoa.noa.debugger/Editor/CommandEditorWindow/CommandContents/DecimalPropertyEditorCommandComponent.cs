#if NOA_DEBUGGER
using System;
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class DecimalPropertyEditorCommandComponent : EditorCommandComponentBase<DecimalPropertyCommand>
    {
        public DecimalPropertyEditorCommandComponent(DecimalPropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            double changed = EditorGUILayout.FloatField(_command.DisplayName, (float)_command.GetValue());
            string changedString = changed.ToString();
            decimal changedValue = _command.FromString(changedString);
            if (changedValue != _command.GetValue())
            {
                _command.SetValue(changedValue);
            }
        }
    }
}
#endif
