using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger.DebugCommand
{
    class CommandScroll : ScrollRect
    {
        [Header("CommandScroll")]
        [SerializeField]
        bool _isFloatingWindow;

        [SerializeField]
        float _groupHeaderHeight;

        [SerializeField]
        VerticalLayoutGroup _groupLayout;

        [SerializeField]
        CommandGroupPanel _groupPanelPrefab;

        ContentPanelPool _groupPanelPool;
        List<CommandGroupPanel> _groupPanelComponents;
        List<int> _groupPanelPoolIndexes;

        public bool _canMoveScroll = true;


        bool _isInit;

        void _Init()
        {
            if (_isInit)
            {
                return;
            }

            _isInit = true;
            _groupPanelPool = new ContentPanelPool(_groupPanelPrefab.gameObject, content);
            _groupPanelComponents = new List<CommandGroupPanel>();
            _groupPanelPoolIndexes = new List<int>();
            _canMoveScroll = true;
        }


        public void Reset(GroupPanelInfo[] groups)
        {
            if (!_isInit)
            {
                _Init();
            }

            _DestroyPanels();
            _InstantiateGroups(groups);
        }

        new void LateUpdate()
        {
            base.LateUpdate();

            if (_groupPanelComponents == null || _groupPanelComponents.Count <= 0)
            {
                return;
            }

            bool isNeedAlign = false;

            foreach (CommandGroupPanel group in _groupPanelComponents)
            {
                if (group.IsNeedAlign)
                {
                    isNeedAlign = true;

                    break;
                }
            }

            if (!isNeedAlign)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();

            foreach (CommandGroupPanel group in _groupPanelComponents)
            {
                group.AlignmentPanels();
            }

            _groupLayout.SetLayoutVertical();

            if (_groupLayout.transform is RectTransform rect)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }

            ResetScrollPosition();
        }

        public void UpdatePanelsStatus(DebugCommandViewLinker linker)
        {
            Action<CommandGroupPanel, GroupPanelInfo> updateGroup = null;

            if (linker.IsMatchUpdateTarget(CommandViewUpdateTarget.GroupHeader))
            {
                updateGroup += (panel, info) =>
                               {
                                   panel.RefreshHeader(info, _isFloatingWindow, _groupHeaderHeight);
                               };
            }

            if (linker.IsMatchUpdateTarget(CommandViewUpdateTarget.CommandStatus))
            {
                updateGroup += (panel, info) =>
                               {
                                   panel.RefreshPanelsStatus(info);
                               };
            }

            if (updateGroup == null)
            {
                return;
            }

            for (int i = 0; i < _groupPanelComponents.Count; i++)
            {
                GroupPanelInfo group = linker._groups[i];
                CommandGroupPanel panel = _groupPanelComponents[i];
                updateGroup.Invoke(panel, group);
            }
        }

        public void RefreshPanels()
        {
            if (_groupPanelComponents != null)
            {
                foreach (CommandGroupPanel group in _groupPanelComponents)
                {
                    group.RefreshPanels();
                }
            }
        }

        public void ResetScrollPosition()
        {
            normalizedPosition = new Vector2(0, 1);
        }


        public override void OnDrag(PointerEventData eventData)
        {
            if (!_canMoveScroll)
            {
                return;
            }

            base.OnDrag(eventData);
        }


        void _InstantiateGroups(GroupPanelInfo[] groups)
        {
            for (int i = 0; i < groups.Length; i++)
            {
                GroupPanelInfo group = groups[i];
                (GameObject obj, int index) objectAndIndex = _groupPanelPool.GetObjectAndIndex();
                CommandGroupPanel component = objectAndIndex.obj.GetComponent<CommandGroupPanel>();
                bool isLast = i == groups.Length - 1;
                component.InstantiateGroup(group, _isFloatingWindow, _groupHeaderHeight, isLast, this);
                _groupPanelComponents.Add(component);
                _groupPanelPoolIndexes.Add(objectAndIndex.index);
            }
        }


        void _DestroyPanels()
        {
            if (_groupPanelComponents != null)
            {
                foreach (CommandGroupPanel group in _groupPanelComponents)
                {
                    group.DestroyPanels();
                }

                _groupPanelComponents.Clear();
            }

            if (_groupPanelPoolIndexes != null)
            {
                foreach (int index in _groupPanelPoolIndexes)
                {
                    _groupPanelPool.ReturnObject(index);
                }

                _groupPanelPoolIndexes.Clear();
            }
        }

        public void Dispose()
        {
            _groupLayout = default;
            _groupPanelPrefab = default;
            _groupPanelPool = default;
            _groupPanelComponents = default;
            _groupPanelPoolIndexes = default;
        }
    }
}
