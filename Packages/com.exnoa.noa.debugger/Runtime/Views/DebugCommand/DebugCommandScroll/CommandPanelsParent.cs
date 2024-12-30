using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger.DebugCommand
{
    [Serializable]
    sealed class CommandPanelsParent
    {
        [SerializeField] RectTransform _rootTransform;
        [SerializeField] DebugCommandPanel _panelPrefab;

        ContentPanelPool _panelPool;
        List<DebugCommandPanel> _commandComponents;
        List<int> _panelPoolIndexes;

        public RectTransform RootTransform => _rootTransform;
        public List<DebugCommandPanel> CommandComponents => _commandComponents;


        public void Init()
        {
            _panelPool = new ContentPanelPool(_panelPrefab.gameObject, _rootTransform);
            _commandComponents = new List<DebugCommandPanel>();
            _panelPoolIndexes = new List<int>();
        }


        public void RefreshPanels()
        {
            if (_commandComponents != null)
            {
                foreach (DebugCommandPanel command in _commandComponents)
                {
                    command.Refresh();
                }
            }
        }

        public void RefreshPanelsStatus(GroupPanelInfo group)
        {
            if (_commandComponents != null)
            {
                for (int i = 0; i < group._commands.Length; i++)
                {
                    DebugCommandPanel panel = _commandComponents[i];
                    ICommand command = group._commands[i];
                    panel.UpdateData(command, group._detailToggleInfo);
                }
            }
        }


        public void InstantiateCommands(GroupPanelInfo group, float maxContentWidth, CommandScroll scrollRect, Action<DebugCommandPanel> onInit = null)
        {
            ICommand[] commands = group._commands;

            foreach (var command in commands)
            {
                if (!command.IsVisible)
                {
                    continue;
                }

                (GameObject obj, int index) objectAndIndex = _panelPool.GetObjectAndIndex();
                DebugCommandPanel component = objectAndIndex.obj.GetComponent<DebugCommandPanel>();
                component.Init(command, maxContentWidth, group._detailToggleInfo, scrollRect);
                onInit?.Invoke(component);
                _commandComponents.Add(component);
                _panelPoolIndexes.Add(objectAndIndex.index);
            }

            _rootTransform.gameObject.SetActive(commands.Length > 0);
        }


        public void DestroyPanels()
        {
            if (_commandComponents != null)
            {
                _commandComponents.Clear();
            }

            if (_panelPoolIndexes != null)
            {
                foreach (int index in _panelPoolIndexes)
                {
                    _panelPool.ReturnObject(index);
                }

                _panelPoolIndexes.Clear();
            }
        }

    }
}
