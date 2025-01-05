using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NoaDebugger.DebugCommand
{
    public enum CommandDisplayFormat
    {
        Panel,
        List
    }

    sealed class DebugCommandPresenter : NoaDebuggerToolBase, INoaDebuggerTool
    {
        static readonly string DebugCommandPresenterUpdate = "DebugCommandPresenterUpdate";
        static DebugCommandPresenter _instance;
        static CommandDisplayFormat _landscapeFormat;
        static CommandDisplayFormat _portraitFormat;

        Dictionary<string, string[]> _floatingRemoveGroups = new();

        [Header("MainView")]
        [SerializeField]
        DebugCommandView _mainViewPrefab;
        DebugCommandView _mainView;

        [Header("FloatingWindowView")]
        [SerializeField] DebugCommandFloatingWindowView _landscapeFloatingWindowPrefab;
        [SerializeField] DebugCommandFloatingWindowView _portraitFloatingWindowPrefab;
        FloatingWindowPresenter<DebugCommandFloatingWindowView, DebugCommandFloatingWindowViewLinker> _floatingWindowPresenter;

        DebugCommandModel _model;
        int _selectCategoryIndex = -1;

        bool _isDetailMode = false;
        string _selectedGroupForDetail;
        string _selectedCommandForDetail;
        ICommand[] _selectedCommands;

        public ToolNotificationStatus NotifyStatus => ToolNotificationStatus.None;

        public void Init()
        {
            _instance = this;
            _model = new DebugCommandModel();

            _model.OnAutoRefresh = _OnAutoRefreshView;
            _model.OnAutoRefreshStateChanged = OnAutoRefreshStateChanged;

            _floatingWindowPresenter =
                new FloatingWindowPresenter<DebugCommandFloatingWindowView, DebugCommandFloatingWindowViewLinker>(
                    _landscapeFloatingWindowPrefab,
                    _portraitFloatingWindowPrefab,
                    NoaDebuggerPrefsDefine.PrefsKeyIsDebugCommandWindowInfo,
                    name);
            _floatingWindowPresenter.OnInitAction += _InitFloatingWindow;

            _model.RefreshCategoryFirst();

            _LoadFilter();

            NoaDebuggerSettings settings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings();
            DebugCommandPresenter._landscapeFormat = settings.CommandFormatLandscape;
            DebugCommandPresenter._portraitFormat = settings.CommandFormatPortrait;
        }

        class CommandMenuInfo : IMenuInfo
        {
            public string Name => "Command";
            public string MenuName => "Command";
            public int SortNo => NoaDebuggerDefine.COMMAND_MENU_SORT_NO;
        }

        CommandMenuInfo _commandMenuInfo;

        public IMenuInfo MenuInfo()
        {
            if (_commandMenuInfo == null)
            {
                _commandMenuInfo = new CommandMenuInfo();
            }

            return _commandMenuInfo;
        }


        public void ShowView(Transform parent)
        {
            if (_mainView == null)
            {
                _mainView = GameObject.Instantiate(_mainViewPrefab, parent);
                _InitView(_mainView);
            }

            _UpdateView();
            _mainView.gameObject.SetActive(true);
        }

        void _InitView(DebugCommandView view)
        {
            view.OnSelectCategory += _OnSelectCategory;
            view.OnDisplayDetailsToggle += _OnDisplayDetailsToggle;
            view.OnTapRefreshButton += _OnRefreshView;
            view.OnLongTapRefreshButton += _OnUpdateAutoRefreshState;
        }

        void _HiddenView()
        {
            if (_mainView == null)
            {
                return;
            }
            _mainView.gameObject.SetActive(false);
        }

        void _RefreshViewOnUpdate()
        {
            _UpdateView();

            UpdateManager.DeleteAction(DebugCommandPresenter.DebugCommandPresenterUpdate);
        }

        void _UpdateView(CommandViewUpdateTarget updateTarget = CommandViewUpdateTarget.All,
                         bool isUpdateMainView = true)
        {
            string categoryName = "";
            if (_model.CategoryNames.Length > 0)
            {
                int index = _GetCategoryIndex();
                categoryName = _model.CategoryNames[index];
            }

            if (isUpdateMainView)
            {
                _UpdateMainView(categoryName, updateTarget);
            }

            _UpdateFloatingWindow(categoryName, updateTarget);
        }

        void _UpdateMainView(string category, CommandViewUpdateTarget updateTarget)
        {
            if (_mainView == null)
            {
                return;
            }

            var groupDic = _model.GetGroupsForCategory(category);
            var linker = new DebugCommandViewLinker()
            {
                _groups = _CreateGroups(category, groupDic),
                _categoryNames = _model.CategoryDisplayNames,
                _selectCategoryIndex = _selectCategoryIndex,
                _isActiveFloatingWindow = _floatingWindowPresenter.IsActive,
                _isDetailMode = _isDetailMode,
                _isSelectGroupForDetail = _selectedGroupForDetail != null && _selectedCommandForDetail == null,
                _selectedGroupForDetail = _selectedGroupForDetail,
                _displayDetailCommands = _selectedCommands,
                _isAutoRefresh = _model.IsAutoRefresh(),
                _updateTarget = updateTarget
            };
            _mainView.Show(linker);
        }

        void _UpdateFloatingWindow(string category, CommandViewUpdateTarget updateTarget)
        {
            if (!_floatingWindowPresenter.IsActive)
            {
                return;
            }

            var groupDic = _model.GetGroupsForCategory(category);

            if (_floatingRemoveGroups.ContainsKey(category))
            {
                string[] removeGroups = _floatingRemoveGroups[category];

                var newDic = new Dictionary<string, CommandGroupData>(groupDic.Count);
                foreach (var group in groupDic)
                {
                    if (!removeGroups.Contains(group.Key))
                    {
                        newDic.Add(group.Key, group.Value);
                    }
                }

                groupDic = newDic;
            }

            var linker = new DebugCommandViewLinker()
            {
                _groups = _CreateGroups(category, groupDic),
                _categoryNames = _model.CategoryDisplayNames,
                _selectCategoryIndex = _selectCategoryIndex,
                _isAutoRefresh = _model.IsAutoRefresh(),
                _updateTarget = updateTarget
            };
            var floatingLinker = new DebugCommandFloatingWindowViewLinker()
            {
                _viewLinker = linker,
                _onLongTapRefreshButton = _OnUpdateAutoRefreshState
            };
            _floatingWindowPresenter.ShowWindowView(floatingLinker);
        }

        int _GetCategoryIndex()
        {
            if (_selectCategoryIndex == -1 || _model.CategoryNames.Length <= _selectCategoryIndex)
            {
                var lastCategoryName = NoaDebuggerPrefs.GetString(NoaDebuggerPrefsDefine.PrefsKeyLastDebugCommandCategoryName, "");
                int index = Array.IndexOf(_model.CategoryNames, lastCategoryName);
                if (index == -1)
                {
                    index = 0;
                }

                _selectCategoryIndex = index;
            }

            return _selectCategoryIndex;
        }

        GroupPanelInfo[] _CreateGroups(string category, Dictionary<string, CommandGroupData> groupDic)
        {
            if (groupDic == null)
            {
                return Array.Empty<GroupPanelInfo>();
            }

            GroupPanelInfo[] groups = new GroupPanelInfo[groupDic.Count];
            int index = 0;
            foreach (var group in groupDic.OrderBy(x => x.Value._order ?? Int32.MaxValue))
            {
                bool isRemoveTarget = false;

                if (_floatingRemoveGroups.ContainsKey(category))
                {
                    if (_floatingRemoveGroups[category].Contains(group.Key))
                    {
                        isRemoveTarget = true;
                    }
                }

                GroupCollapseToggleInfo collapseToggleInfo = new GroupCollapseToggleInfo(
                    group.Value._isCollapsed, _OnSelectGroupForCollapse);
                GroupFloatingToggleInfo floatingToggleInfo = new GroupFloatingToggleInfo(!isRemoveTarget,
                    _floatingWindowPresenter.IsActive, _OnSelectGroupForFloatingWindow);
                DetailToggleInfo detailToggleInfo = new DetailToggleInfo(
                    _isDetailMode, _selectedGroupForDetail, _selectedCommandForDetail, _OnSelectGroupForDetail, _OnSelectCommandForDetail);

                GroupPanelInfo groupData = new GroupPanelInfo(group.Key, collapseToggleInfo, floatingToggleInfo, detailToggleInfo);
                groupData._commands = group.Value._commandList.ToArray();
                groups[index] = groupData;
                index++;
            }

            return groups;
        }


        void _OnSelectCategory(int index)
        {
            _selectCategoryIndex = index;
            string categoryName = _model.CategoryNames[index];
            NoaDebuggerPrefs.SetString(NoaDebuggerPrefsDefine.PrefsKeyLastDebugCommandCategoryName, categoryName);

            _isDetailMode = false;
            _ResetDetailInfo(CommandViewUpdateTarget.All);
        }

        void _OnRefreshView(bool isAutoRefresh)
        {
            if (isAutoRefresh)
            {
                _OnUpdateAutoRefreshState(false);
                _UpdateView(CommandViewUpdateTarget.Buttons);
            }
            else
            {
                DebugCommandRegister.RefreshProperty();
            }
        }

        void _OnAutoRefreshView()
        {
            DebugCommandRegister.RefreshProperty();
        }

        void _OnUpdateAutoRefreshState(bool isFloatingWindow)
        {
            _model.UpdateAutoRefresh(!_model.IsAutoRefresh(), isFloatingWindow);
        }

        void OnAutoRefreshStateChanged(bool isAutoRefresh, bool isFloatingWindow)
        {
            var linker = new DebugCommandViewLinker()
            {
                _isAutoRefresh = _model.IsAutoRefresh()
            };

            if (!isFloatingWindow)
            {
                linker._updateTarget = CommandViewUpdateTarget.Buttons;
            }

            var floatingLinker = new DebugCommandFloatingWindowViewLinker()
            {
                _viewLinker = linker,
                _refreshCommandPanelsOnly = true,
                _onLongTapRefreshButton = _OnUpdateAutoRefreshState
            };
            _floatingWindowPresenter.ShowWindowView(floatingLinker);
        }

        void _OnSelectGroupForCollapse(string group, bool isOn)
        {
            int index = _GetCategoryIndex();
            string category = _model.CategoryNames[index];
            var groups = _model.GetGroupsForCategory(category);
            groups[group]._isCollapsed = isOn;
        }

        void _OnSelectGroupForFloatingWindow(string group, bool isOn)
        {
            int index = _GetCategoryIndex();
            string category = _model.CategoryNames[index];

            if (isOn)
            {
                if (!_floatingRemoveGroups.ContainsKey(category))
                {
                    return;
                }
                string[] removeGroups = _floatingRemoveGroups[category];

                if (removeGroups.Contains(group))
                {
                    _floatingRemoveGroups[category] = removeGroups.Where(target => target != group).ToArray();
                }
            }
            else
            {
                if (_floatingRemoveGroups.ContainsKey(category))
                {
                    string[] removeGroups = _floatingRemoveGroups[category];

                    if (!removeGroups.Contains(group))
                    {
                        _floatingRemoveGroups[category] = removeGroups.Concat(new[] { group }).ToArray();
                    }
                }
                else
                {
                    _floatingRemoveGroups.Add(category, new[] { group });
                }
            }

            _SaveFilter();
        }

        void _OnDisplayDetailsToggle(bool isOn)
        {
            _isDetailMode = isOn;
            _UpdateView();
        }

        void _OnSelectGroupForDetail(GroupPanelInfo group, bool isOn)
        {
            if (isOn)
            {
                _UpdateDetailInfo(group._name, commandName:null, group._commands,
                                  CommandViewUpdateTarget.GroupHeader |
                                  CommandViewUpdateTarget.CommandStatus |
                                  CommandViewUpdateTarget.DetailArea);
            }
            else if (_selectedGroupForDetail == group._name)
            {
                _ResetDetailInfo(CommandViewUpdateTarget.GroupHeader |
                                 CommandViewUpdateTarget.DetailArea);
            }
        }

        void _OnSelectCommandForDetail(ICommand command, bool isOn)
        {
            if (isOn)
            {
                _UpdateDetailInfo(command.GroupName, command.DisplayName, new ICommand[] {command},
                                  CommandViewUpdateTarget.GroupHeader |
                                  CommandViewUpdateTarget.CommandStatus |
                                  CommandViewUpdateTarget.DetailArea);
            }
            else if (_selectedGroupForDetail == command.GroupName &&
                     _selectedCommandForDetail == command.DisplayName)
            {
                _ResetDetailInfo(CommandViewUpdateTarget.CommandStatus |
                                 CommandViewUpdateTarget.DetailArea);
            }
        }

        void _UpdateDetailInfo(string groupName, string commandName, ICommand[] commands,
                               CommandViewUpdateTarget? updateTarget)
        {
            _selectedGroupForDetail = groupName;
            _selectedCommandForDetail = commandName;
            _selectedCommands = commands;

            if (updateTarget != null)
            {
                _UpdateView(updateTarget.Value);
            }
        }

        void _ResetDetailInfo(CommandViewUpdateTarget? updateTarget)
        {
            _UpdateDetailInfo(groupName:null, commandName:null, commands:null, updateTarget);
        }

        public void AlignmentUI(bool isReverse)
        {
            _mainView.AlignmentUI(isReverse);
        }


        public ToolPinStatus GetPinStatus()
        {
            return _floatingWindowPresenter.IsActive
                ? ToolPinStatus.On
                : ToolPinStatus.Off;
        }

        public void TogglePin(Transform parent)
        {
            _floatingWindowPresenter.ToggleActive(parent);
            _UpdateView(CommandViewUpdateTarget.GroupHeader);
        }

        public void InitFloatingWindow(Transform parent)
        {
            var isWindowDrawing = _floatingWindowPresenter.IsActive;
            if (isWindowDrawing)
            {
                _floatingWindowPresenter.InstantiateWindow(parent);
            }
        }

        void _InitFloatingWindow(DebugCommandFloatingWindowView window)
        {
            window.OnSelectCategory += _OnSelectCategory;
            _UpdateView(isUpdateMainView:false);
        }


        public void OnHidden()
        {
            _OnHiddenPresenter();

            if (_floatingWindowPresenter.IsActive)
            {
                _UpdateView(isUpdateMainView:false);
            }
        }

        public void OnToolDispose()
        {
            _OnHiddenPresenter();

            DebugCommandPresenter._instance = null;
            DebugCommandPresenter._landscapeFormat = default;
            DebugCommandPresenter._portraitFormat = default;
            UpdateManager.DeleteAction(DebugCommandPresenter.DebugCommandPresenterUpdate);

            if (_model != null)
            {
                _model.Dispose();
            }
        }

        void _OnHiddenPresenter()
        {
            _HiddenView();

            _isDetailMode = false;
            _ResetDetailInfo(updateTarget:null);

            if (_mainView != null)
            {
                _mainView.gameObject.SetActive(false);
            }
        }


        public static void RefreshView()
        {
            if (DebugCommandPresenter._instance == null)
            {
                return;
            }

            if (!UpdateManager.ContainsKey(DebugCommandPresenter.DebugCommandPresenterUpdate))
            {
                UpdateManager.SetAction(
                    DebugCommandPresenter.DebugCommandPresenterUpdate,
                    DebugCommandPresenter._instance._RefreshViewOnUpdate);
            }
        }

        public static void RefreshProperty()
        {
            if (DebugCommandPresenter._instance == null)
            {
                return;
            }

            if (DebugCommandPresenter._instance._mainView != null)
            {
                DebugCommandPresenter._instance._mainView.RefreshCommandPanels();
            }

            if (DebugCommandPresenter._instance._floatingWindowPresenter.IsActive)
            {
                var viewLinker = new DebugCommandViewLinker()
                {
                    _updateTarget = CommandViewUpdateTarget.GroupHeader | CommandViewUpdateTarget.CommandStatus
                };
                var floatingLinker = new DebugCommandFloatingWindowViewLinker()
                {
                    _viewLinker = viewLinker,
                    _refreshCommandPanelsOnly = true,
                    _onLongTapRefreshButton = DebugCommandPresenter._instance._OnUpdateAutoRefreshState
                };
                DebugCommandPresenter._instance._floatingWindowPresenter.ShowWindowView(floatingLinker);
            }
        }

        public static object GetCategoryInstance(string categoryName)
        {
            return DebugCommandPresenter._instance._model.GetInstance(categoryName);
        }

        public static string[] GetCategoryNames()
        {
            if (DebugCommandPresenter._instance == null)
            {
                return null;
            }

            return DebugCommandPresenter._instance._model.CategoryNames;
        }

        public static string[] GetDisplayCategoryNames()
        {
            if (DebugCommandPresenter._instance == null)
            {
                return null;
            }

            return DebugCommandPresenter._instance._model.CategoryDisplayNames;
        }

        public static Dictionary<string, CommandGroupData> GetCategoryGroup(string categoryName)
        {
            if (DebugCommandPresenter._instance == null)
            {
                return null;
            }

            return DebugCommandPresenter._instance._model.GetGroupsForCategory(categoryName);
        }

        public static void SetCommandInteractable(string categoryName, string commandTag, bool isInteractable)
        {
            DebugCommandPresenter._instance._model.SetInteractable(categoryName, commandTag, isInteractable);
        }

        public static bool IsCommandInteractable(string categoryName, string commandTag)
        {
            return DebugCommandPresenter._instance._model.IsInteractable(categoryName, commandTag);
        }

        public static void SetCommandVisible(string categoryName, string commandTag, bool isVisible)
        {
            DebugCommandPresenter._instance._model.SetVisible(categoryName, commandTag, isVisible);
        }

        public static bool IsCommandVisible(string categoryName, string commandTag)
        {
            return DebugCommandPresenter._instance._model.IsVisible(categoryName, commandTag);
        }

        public static CommandDisplayFormat GetCurrentFormat()
        {
            return DeviceOrientationManager.IsPortrait
                ? DebugCommandPresenter._portraitFormat
                : DebugCommandPresenter._landscapeFormat;
        }


        class GroupFilter
        {
            public string _categoryName;
            public string[] _removeGroupsJson;
        }

        class GroupFilterArray
        {
            public string[] _groupFilterJson;
        }

        void _SaveFilter()
        {
            GroupFilterArray filters = new GroupFilterArray();
            filters._groupFilterJson = new string[_floatingRemoveGroups.Count];
            int index = 0;
            foreach (var target in _floatingRemoveGroups)
            {
                GroupFilter filter = new GroupFilter()
                {
                    _categoryName = target.Key,
                    _removeGroupsJson = target.Value,
                };

                string filterJson = JsonUtility.ToJson(filter);
                filters._groupFilterJson[index] = filterJson;
                index++;
            }

            string json = JsonUtility.ToJson(filters);
            NoaDebuggerPrefs.SetString(NoaDebuggerPrefsDefine.PrefsKeyDebugCommandGroupFilter, json);
        }

        void _LoadFilter()
        {
            string json = NoaDebuggerPrefs.GetString(NoaDebuggerPrefsDefine.PrefsKeyDebugCommandGroupFilter, "");
            GroupFilterArray filterArray = JsonUtility.FromJson<GroupFilterArray>(json);
            if (filterArray == null)
            {
                return;
            }

            _floatingRemoveGroups = new Dictionary<string, string[]>(filterArray._groupFilterJson.Length);
            for (int i = 0; i < filterArray._groupFilterJson.Length; i++)
            {
                string filterJson = filterArray._groupFilterJson[i];
                GroupFilter filter = JsonUtility.FromJson<GroupFilter>(filterJson);

                _floatingRemoveGroups.Add(filter._categoryName, filter._removeGroupsJson);
            }
        }

        void OnDestroy()
        {
            _floatingRemoveGroups = default;
            _mainViewPrefab = default;
            _mainView = default;
            _landscapeFloatingWindowPrefab = default;
            _portraitFloatingWindowPrefab = default;
            _floatingWindowPresenter = default;
            _model = default;
            _commandMenuInfo = default;
        }
    }
}
