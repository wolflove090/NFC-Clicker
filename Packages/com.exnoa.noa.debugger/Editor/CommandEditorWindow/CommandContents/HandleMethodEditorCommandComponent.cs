#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEngine;

namespace NoaDebugger
{
    sealed class HandleMethodEditorCommandComponent : EditorCommandComponentBase<HandleMethodCommand>
    {
        public HandleMethodEditorCommandComponent(HandleMethodCommand command) : base(command) { }
        protected override void _Draw()
        {
            if (GUILayout.Button(_command.DisplayName))
            {
                _command.Invoke(null);
            }
        }
    }
}
#endif
