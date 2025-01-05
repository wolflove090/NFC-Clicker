#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEngine;

namespace NoaDebugger
{
    sealed class CoroutineEditorCommandComponent : EditorCommandComponentBase<CoroutineCommand>
    {
        public CoroutineEditorCommandComponent(CoroutineCommand command) : base(command) { }
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
