using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace NoaDebugger.DebugCommand
{
    sealed class CommandPanelEnumProperty : CommandPanelBase<EnumPropertyCommand>
    {
        readonly List<TMP_Dropdown.OptionData> _optionDataList;
        readonly string _longestName = "";
        readonly LayoutElement _layoutElement = null;
        readonly float _horizontalPadding = 0f;

        public CommandPanelEnumProperty(DebugCommandPanel panel, float maxContentWidth) : base(panel)
        {
            panel._actions.SetActive(true);
            panel._dropdown.gameObject.SetActive(true);
            panel._dropdown.onValueChanged.RemoveAllListeners();

            panel._displayName.alignment = GetDisplayNameAlignmentFromDisplayFormat();

            string[] names = _command.GetNames();
            _optionDataList = new List<TMP_Dropdown.OptionData>(names.Length);
            foreach(string name in names)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(name);
                _optionDataList.Add(option);

                if (name.Length > _longestName.Length)
                {
                    _longestName = name;
                }
            }

            _layoutElement = panel._dropdown.GetComponent<LayoutElement>();
            var layoutGroup = panel._dropdown.GetComponent<LayoutGroup>();
            if (layoutGroup != null)
            {
                _horizontalPadding = layoutGroup.padding.left + layoutGroup.padding.right;
            }

            OnUpdateWidth(maxContentWidth);
            Refresh();
        }

        public override void OnUpdateWidth(float maxContentWidth)
        {
            _panel._dropdown.onValueChanged.RemoveAllListeners();

            TMP_Text text = _panel._dropdown.itemText;
            text.text = _longestName; 
            SetPreferredWidthFromText(_layoutElement, _horizontalPadding, text, maxContentWidth);

            _panel._dropdown.options = _optionDataList;
            _panel._dropdown.onValueChanged.AddListener(_OnChangeDropdown);
        }

        public override void Refresh()
        {
            string valueName = _command.GetValue();
            _panel._dropdown.value = _GetEnumIndex(valueName);
        }

        int _GetEnumIndex(string value)
        {
            string[] names = _command.GetNames();
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }


        void _OnChangeDropdown(int index)
        {
            string valueName = _command.GetNames()[index];
            _command.SetValue(valueName);
        }
    }
}
