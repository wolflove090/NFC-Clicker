using System.Collections.Generic;
using UnityEngine;

namespace NoaDebugger
{
    sealed class HierarchyGameObjectEntry
    {
        public int _hashCode;

        public string _name;

        public bool _isActive;

        public bool _isOpen;

        public bool _isSelected;

        public HierarchyGameObjectEntryCallback _callback;

        public GameObject _gameObject;

        public List<HierarchyGameObjectEntry> _children = new List<HierarchyGameObjectEntry>();

        public List<HierarchyPanelData> GetPanelDataListWithChildren(HierarchyGameObjectEntryCallback callback)
        {
            var list = new List<HierarchyPanelData>();
            _AddPanelData(list, callback);

            return list;
        }

        void _AddPanelData(List<HierarchyPanelData> list, HierarchyGameObjectEntryCallback callback, int depth = 0)
        {
            _callback = callback;

            HierarchyPanelData panelData = new HierarchyPanelData(this, depth, _OnToggleOpen);
            list.Add(panelData);

            if (_isOpen && _children != null)
            {
                foreach(var child in _children)
                {
                    child._AddPanelData(list, callback, depth + 1);
                }
            }
        }

        void _OnToggleOpen()
        {
            SwitchToggle();
        }

        public void SwitchToggle()
        {
            _isOpen = !_isOpen;
            _callback._onToggleOpen?.Invoke(_hashCode, _isOpen);
        }
    }

    sealed class HierarchyGameObjectEntryCallback
    {
        public System.Action<HierarchyGameObjectEntry> _onSelect;

        public System.Action<int, bool> _onToggleOpen;

        public System.Action _onUpdateView;
    }
}
