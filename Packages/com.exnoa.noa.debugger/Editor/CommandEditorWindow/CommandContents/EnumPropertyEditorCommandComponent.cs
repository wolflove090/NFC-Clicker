#if NOA_DEBUGGER
using System;
using NoaDebugger.DebugCommand;
using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    sealed class EnumPropertyEditorCommandComponent : EditorCommandComponentBase<EnumPropertyCommand>
    {
        int _selectIndex;
        string _editorValue;

        public EnumPropertyEditorCommandComponent(EnumPropertyCommand command) : base(command)
        {
            RefreshEditorValue();
        }

        protected override void _Draw()
        {
            if(_editorValue != _command.GetValue())
            {
                RefreshEditorValue();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                int beforeIndex = _selectIndex;
                EditorGUILayout.LabelField(_command.DisplayName, GUILayout.Width(EditorGUIUtility.labelWidth));
                _selectIndex = EditorGUILayout.Popup(_selectIndex, _command.GetNames());

                if (beforeIndex != _selectIndex)
                {
                    _command.SetValue(_command.GetNames()[_selectIndex]);
                }
            }
        }

        void RefreshEditorValue()
        {
            _editorValue = _command.GetValue();
            _selectIndex = Array.IndexOf(_command.GetNames(), _editorValue);
        }
    }
}
#endif
