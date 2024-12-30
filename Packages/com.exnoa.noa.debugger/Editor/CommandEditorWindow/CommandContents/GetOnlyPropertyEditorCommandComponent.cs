#if NOA_DEBUGGER
using NoaDebugger.DebugCommand;
using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    sealed class GetOnlyPropertyEditorCommandComponent : EditorCommandComponentBase<GetOnlyPropertyCommand>
    {
        protected override bool DontDisabled => true;
        public GetOnlyPropertyEditorCommandComponent(GetOnlyPropertyCommand command) : base(command) { }

        protected override void _Draw()
        {
            string key = _command.DisplayName;
            string value = _command.GetValue();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(key);
                EditorGUILayout.SelectableLabel(value, EditorStyles.label, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
        }
    }
}
#endif
