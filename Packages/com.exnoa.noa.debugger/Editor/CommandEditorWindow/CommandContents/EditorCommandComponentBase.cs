#if NOA_DEBUGGER
using UnityEditor;
using UnityEngine;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    abstract class EditorCommandComponentBase<TCommand> : IEditorCommandComponent where TCommand : ICommand
    {
        protected TCommand _command;

        protected virtual bool DontDisabled => false;

        protected EditorCommandComponentBase(TCommand command)
        {
            _command = command;
        }

        protected abstract void _Draw();
        public void Draw(float windowWidth)
        {
            if (!_command.IsVisible)
            {
                return;
            }

            using (new EditorGUI.DisabledScope(!_command.IsInteractable && !DontDisabled))
            {
                var tmp = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = GetLabelWidth(windowWidth);
                _Draw();
                EditorGUIUtility.labelWidth = tmp;
            }
        }

        public void DrawDetail()
        {
            EditorGUILayout.LabelField(_command.DisplayName, EditorStyles.boldLabel);

            foreach (var item in _command.CreateDetailContext())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(item.Key);

                EditorGUILayout.SelectableLabel(
                    item.Value, EditorStyles.label, GUILayout.Height(EditorGUIUtility.singleLineHeight));

                EditorGUILayout.EndHorizontal();
            }

            if (!string.IsNullOrEmpty(_command.GetDetailSuffix()))
            {
                GUILayout.Label(_command.GetDetailSuffix());
            }

            EditorBaseStyle.DrawUILine();
        }

        float GetLabelWidth(float windowWidth)
        {
            return windowWidth * 0.45f;
        }
    }
}
#endif
