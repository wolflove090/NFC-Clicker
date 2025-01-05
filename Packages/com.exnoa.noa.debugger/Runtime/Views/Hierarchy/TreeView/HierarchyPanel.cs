using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    class HierarchyPanel : HierarchyPanelBase<HierarchyPanelData>
    {
        [SerializeField, Header("HierarchyPanel Element")] TextMeshProUGUI _nameLabel;
        [SerializeField] protected Button _selectButton;
        [SerializeField] GameObject _selectIcon;

        protected override void _Draw(HierarchyPanelData panelData)
        {
            _nameLabel.text = panelData._gameObjectName;
            _nameLabel.color = panelData._isActive ? Color.white : Color.gray;

            _selectButton.onClick.RemoveAllListeners();
            _selectButton.onClick.AddListener(_OnToggleButton);
            _selectButton.onClick.AddListener(_OnSelect);
            _selectIcon.SetActive(panelData._isSelected);
        }

        protected override float _GetLabelLength()
        {
            return _nameLabel.preferredWidth;
        }

        protected void _OnSelect()
        {
            _data._onSelect?.Invoke(_data._entry);
        }
    }

    class HierarchyPanelData : HierarchyPanelDataBase
    {
        public string _gameObjectName;
        public HierarchyGameObjectEntry _entry;
        public System.Action<HierarchyGameObjectEntry> _onSelect;

        public HierarchyPanelData(HierarchyGameObjectEntry entry, int depth, System.Action onToggle)
        {
            _gameObjectName = entry._name;
            _entry = entry;
            _depth = depth;
            _isActive = entry._isActive;
            _isOpen = entry._isOpen;
            _isSelected = entry._isSelected;
            _hasChildren = entry._children != null && entry._children.Count > 0;
            _toggleOpen = onToggle;
            _onSelect = entry._callback._onSelect;
            _onUpdateView = entry._callback._onUpdateView;
        }
    }
}
