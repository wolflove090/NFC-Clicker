using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger.DebugCommand
{
    class DebugCommandModel
    {
        static readonly string DebugCommandModelOnUpdate = "DebugCommandModelOnUpdate";

        class CommandCategory
        {
            public string _displayName = null;

            public Dictionary<string, CommandGroupData> _groupedCommands = new();

            public readonly Dictionary<string, CommandGroupData> _taggedCommands = new();

            public readonly Dictionary<string, TaggedCommandState> _taggedCommandStates = new();

            public object _categoryInstance = null;
        }

        class TaggedCommandState
        {
            public bool IsInteractable { get; set; } = true;

            public bool IsVisible { get; set; } = true;
        }

        readonly Dictionary<string, CommandCategory> _categories = new();

        bool _shouldRefreshFirst = true;

        public string[] CategoryNames => _categories.Keys.ToArray();

        public string[] CategoryDisplayNames => _categories.Select(x => x.Value._displayName).ToArray();

        public UnityAction OnAutoRefresh;

        public UnityAction<bool, bool> OnAutoRefreshStateChanged;

        float _autoRefreshTimer;

        public DebugCommandModel()
        {
            DebugCommandRegister.OnAddCategory += _OnAddCategory;
            _HandleOnUpdate(IsAutoRefresh());
        }

        public void Dispose()
        {
            _categories.Clear();
            DebugCommandRegister.OnAddCategory -= _OnAddCategory;
            DebugCommandRegister.Dispose();
        }


        void _OnAddCategory()
        {
            _AddMemberInfoCommands();
            _AddDynamicCommands();
        }


        public Dictionary<string, CommandGroupData> GetGroupsForCategory(string categoryName)
        {
            if (_categories.Count <= 0 || !_categories.ContainsKey(categoryName))
            {
                return null;
            }

            return _categories[categoryName]._groupedCommands;
        }

        public object GetInstance(string categoryName)
        {
            return _categories.TryGetValue(categoryName, out CommandCategory category)
                ? category._categoryInstance
                : null;
        }

        public void RefreshCategoryFirst()
        {
            if (_shouldRefreshFirst)
            {
                _shouldRefreshFirst = false;

                _AddMemberInfoCommands();
                _AddDynamicCommands();
            }
        }

        public void SetInteractable(string categoryName, string commandTag, bool isInteractable)
        {
            if (!_categories.TryGetValue(categoryName, out CommandCategory category)
                || !category._taggedCommandStates.TryGetValue(commandTag, out TaggedCommandState state))
            {
                Debug.LogError($"{commandTag}: Tag does not exist in the category '{categoryName}'.");

                return;
            }

            state.IsInteractable = isInteractable;

            foreach (ICommand command in category._taggedCommands[commandTag]._commandList)
            {
                command.IsInteractable = isInteractable;
            }
        }

        public bool IsInteractable(string categoryName, string commandTag)
        {
            if (_categories.TryGetValue(categoryName, out CommandCategory category)
                && category._taggedCommandStates.TryGetValue(commandTag, out TaggedCommandState state))
            {
                return state.IsInteractable;
            }

            Debug.LogError($"{commandTag}: Tag does not exist in the category '{categoryName}'.");

            return false;
        }

        public void SetVisible(string categoryName, string commandTag, bool isVisible)
        {
            if (!_categories.TryGetValue(categoryName, out CommandCategory category)
                || !category._taggedCommandStates.TryGetValue(commandTag, out TaggedCommandState state))
            {
                Debug.LogError($"{commandTag}: Tag does not exist in the category '{categoryName}'.");

                return;
            }

            state.IsVisible = isVisible;

            foreach (ICommand command in category._taggedCommands[commandTag]._commandList)
            {
                command.IsVisible = isVisible;
            }
        }

        public bool IsVisible(string categoryName, string commandTag)
        {
            if (_categories.TryGetValue(categoryName, out CommandCategory category)
                && category._taggedCommandStates.TryGetValue(commandTag, out TaggedCommandState state))
            {
                return state.IsVisible;
            }

            Debug.LogError($"{commandTag}: Tag does not exist in the category '{categoryName}'.");

            return false;
        }

        public bool IsAutoRefresh()
        {
            return NoaPrefs.GetBoolean(NoaDebuggerPrefsDefine.PrefsKeyDebugCommandAutoRefresh, false);
        }

        public void UpdateAutoRefresh(bool isAutoRefresh, bool isFloatingWindow)
        {
            NoaPrefs.SetBoolean(NoaDebuggerPrefsDefine.PrefsKeyDebugCommandAutoRefresh, isAutoRefresh);
            _HandleOnUpdate(isAutoRefresh);
            OnAutoRefreshStateChanged?.Invoke(isAutoRefresh, isFloatingWindow);
        }


        void _HandleOnUpdate(bool isAutoRefresh)
        {
            string key = DebugCommandModelOnUpdate;

            if (isAutoRefresh)
            {
                if (UpdateManager.ContainsKey(key))
                {
                    return;
                }

                UpdateManager.SetAction(key, _OnUpdate);
            }

            else
            {
                _autoRefreshTimer = 0;
                UpdateManager.DeleteAction(key);
            }
        }

        void _OnUpdate()
        {
            _autoRefreshTimer += Time.deltaTime;

            if (_autoRefreshTimer > NoaDebuggerDefine.DebugCommandAutoRefreshInterval)
            {
                OnAutoRefresh?.Invoke();
                _autoRefreshTimer = 0;
            }
        }

        void _AddMemberInfoCommands()
        {
            Dictionary<string, CommandCategory> oldCategories = _categories.ToDictionary(
                entry => entry.Key,
                entry => entry.Value);

            _categories.Clear();
            var categories = DebugCommandRegister.CategoryTypes.OrderBy(category => category.Value._order);

            foreach ((string categoryName, DebugCommandRegister.DebugCategory category) in categories)
            {
                Type categoryType = category._type;

                if (categoryType.GetConstructor(Type.EmptyTypes) == null)
                {
                    Debug.LogWarning($"{categoryType}: Debug-category class must have a parameterless constructor.");

                    continue;
                }

                if (oldCategories.TryGetValue(categoryName, out CommandCategory existingCategory))
                {
                    _categories.Add(categoryName, existingCategory);

                    continue;
                }

                const BindingFlags memberBindingFlags = BindingFlags.Instance | BindingFlags.Public;
                MethodInfo[] methods = categoryType.GetMethods(memberBindingFlags);
                object categoryInstance = Activator.CreateInstance(categoryType);
                List<ICommand> commands = new();

                foreach (MethodInfo method in methods)
                {
                    if (!method.Equals(method.GetBaseDefinition()) 
                        || method.DeclaringType != categoryType 
                        || method.GetParameters().Length != 0) 
                    {
                        continue;
                    }

                    const string getterPrefix = "get_";
                    ICommand command = null;

                    if (!method.IsSpecialName)
                    {
                        command = CommandFactory.CreateCommand(categoryInstance, method, categoryName);
                    }
                    else if (method.Name.StartsWith(getterPrefix))
                    {
                        string propertyName = method.Name[getterPrefix.Length..];
                        PropertyInfo property = categoryType.GetProperty(propertyName);

                        if (property != null)
                        {
                            command = CommandFactory.CreateCommand(categoryInstance, property, categoryName);
                        }
                    }

                    if (command != null)
                    {
                        commands.Add(command);
                    }
                }

                CommandCategory commandCategory = DebugCommandModel._CreateCommandCategory(commands, category._displayName, categoryInstance);
                _categories.Add(categoryName, commandCategory);
            }
        }

        void _AddDynamicCommands()
        {
            List<string> duplicatedCategories = null;

            foreach ((string categoryName, HashSet<CommandDefinition> commandDefinitions)
                     in DebugCommandRegister.DebugCommandDefinitionRegister.CommandDefinitions)
            {
                if (_categories.ContainsKey(categoryName))
                {
                    duplicatedCategories ??= new List<string>();
                    duplicatedCategories.Add(categoryName);
                    Debug.LogWarning($"{categoryName}: Category name is already registered.");

                    continue;
                }

                List<ICommand> commands = commandDefinitions
                                          .Select(commandDefinition => commandDefinition.CreateCommand())
                                          .Where(command => command != null)
                                          .ToList();

                CommandCategory commandCategory = DebugCommandModel._CreateCommandCategory(commands, categoryName);
                _categories.Add(categoryName, commandCategory);
            }

            if (duplicatedCategories != null)
            {
                foreach (string categoryName in duplicatedCategories)
                {
                    DebugCommandRegister.DebugCommandDefinitionRegister.RemoveCategory(categoryName);
                }
            }
        }

        static CommandCategory _CreateCommandCategory(IEnumerable<ICommand> commands, string displayName, object instance = null)
        {
            var category = new CommandCategory
            {
                _displayName = displayName,
                _categoryInstance = instance
            };

            foreach (ICommand command in commands)
            {
                string groupName = command.GroupName;
                int? groupOrder = command.GroupOrder;
                Dictionary<string, CommandGroupData> groupList = category._groupedCommands;

                if (groupList.ContainsKey(groupName))
                {
                    groupList[groupName]._commandList.Add(command);
                }
                else
                {
                    groupList.Add(groupName, new CommandGroupData()
                    {
                        _commandList = new List<ICommand> {command}
                    });
                }

                if (groupOrder != null)
                {
                    groupList[groupName]._order = groupOrder.Value;
                }

                string tagName = command.TagName;

                if (!string.IsNullOrEmpty(tagName))
                {
                    Dictionary<string, CommandGroupData> tagList = category._taggedCommands;

                    if (tagList.ContainsKey(tagName))
                    {
                        tagList[tagName]._commandList.Add(command);
                    }
                    else
                    {
                        tagList.Add(tagName, new CommandGroupData()
                        {
                            _commandList = new List<ICommand> {command}
                        });
                        category._taggedCommandStates.Add(tagName, new TaggedCommandState());
                    }
                }
            }

            var newGroup = new Dictionary<string, CommandGroupData>(category._groupedCommands.Count);

            foreach (KeyValuePair<string, CommandGroupData> group in category._groupedCommands)
            {
                List<ICommand> orderCommandList =
                    category._groupedCommands[group.Key]._commandList.OrderBy(command => command.Order).ToList();

                newGroup.Add(group.Key, new CommandGroupData()
                {
                    _order = group.Value._order,
                    _commandList = orderCommandList
                });
            }

            category._groupedCommands = newGroup;

            return category;
        }
    }

    class CommandGroupData
    {
        public bool _isCollapsed = false;

        public int? _order = null;

        public List<ICommand> _commandList;
    }
}
