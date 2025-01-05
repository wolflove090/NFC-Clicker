#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEngine;

namespace NoaDebugger
{
    sealed class MethodEditorCommandComponent : EditorCommandComponentBase<MethodCommand>
    {
        public MethodEditorCommandComponent(MethodCommand command) : base(command) { }
        protected override void _Draw()
        {
            if (GUILayout.Button(_command.DisplayName))
            {
                _command.Invoke();
            }
        }
    }
}
#endif
