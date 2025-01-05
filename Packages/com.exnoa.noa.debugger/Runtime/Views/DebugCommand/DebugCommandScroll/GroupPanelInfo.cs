using UnityEngine.Events;

namespace NoaDebugger.DebugCommand
{
    sealed class GroupPanelInfo
    {
        readonly public string _name;

        public ICommand[] _commands;

        public GroupCollapseToggleInfo _collapseToggleInfo;

        public GroupFloatingToggleInfo _floatingToggleInfo;

        public DetailToggleInfo _detailToggleInfo;

        public bool IsShowGroupDetail() => _detailToggleInfo.IsShowDetail(_name);

        public GroupPanelInfo(string name, GroupCollapseToggleInfo collapseToggleInfo,
                              GroupFloatingToggleInfo floatingToggleInfo, DetailToggleInfo detailToggleInfo)
        {
            _name = name;
            _collapseToggleInfo = collapseToggleInfo;
            _floatingToggleInfo = floatingToggleInfo;
            _detailToggleInfo = detailToggleInfo;
        }
    }

    sealed class GroupCollapseToggleInfo
    {
        public bool _isOn;

        readonly public UnityAction<string, bool> _onChange;

        public GroupCollapseToggleInfo(bool isOn, UnityAction<string, bool> onChange)
        {
            _isOn = isOn;
            _onChange = onChange;
        }
    }

    sealed class GroupFloatingToggleInfo
    {
        public bool _isOn;

        readonly public bool _isActive;

        readonly public UnityAction<string, bool> _onChange;

        public GroupFloatingToggleInfo(bool isOn, bool isActive, UnityAction<string, bool> onChange)
        {
            _isOn = isOn;
            _isActive = isActive;
            _onChange = onChange;
        }
    }

    sealed class DetailToggleInfo
    {
        readonly public bool _isDetailMode;

        readonly public string _selectedGroup;

        readonly public string _selectedCommand;

        readonly public UnityAction<GroupPanelInfo, bool> _onSelectGroup;

        readonly public UnityAction<ICommand, bool> _onSelectCommand;

        public DetailToggleInfo(
            bool isDetailMode,
            string selectedGroup,
            string selectedCommand,
            UnityAction<GroupPanelInfo, bool> onSelectGroup,
            UnityAction<ICommand, bool> onSelectCommand)
        {
            _isDetailMode = isDetailMode;
            _selectedGroup = selectedGroup;
            _selectedCommand = selectedCommand;
            _onSelectGroup = onSelectGroup;
            _onSelectCommand = onSelectCommand;
        }

        public bool IsShowDetail(string groupName, string commandName = null)
        {
            return _isDetailMode &&
                   groupName == _selectedGroup &&
                   commandName == _selectedCommand;
        }
    }
}
