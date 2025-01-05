#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;

namespace NoaDebugger
{
    sealed class FloatPropertyEditorCommandComponent : EditorCommandComponentBase<FloatPropertyCommand>
    {
        public FloatPropertyEditorCommandComponent(FloatPropertyCommand command) : base(command) { }
        protected override void _Draw()
        {
            float changed = EditorGUILayout.FloatField(_command.DisplayName, _command.GetValue());
            if (changed != _command.GetValue())
            {
                _command.SetValue(changed);
            }
        }
    }
}
#endif
