using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class GameObjectDetailPanel : HierarchyPanelBase<GameObjectDetailPanelData>
    {
        const string NAME_VALUE_FORMAT = "{0} :    {1}";
        const string NAME_ONLY_FORMAT = "{0}";

        [SerializeField, Header("GameObjectDetailPanel Element")] TextMeshProUGUI _keyValueLabel;
        [SerializeField] Button _headerToggle; 

        protected override void _Draw(GameObjectDetailPanelData panelData)
        {
            string labelFormat = NAME_VALUE_FORMAT;
            if (panelData._hasChildren)
            {
                labelFormat = NAME_ONLY_FORMAT;
            }
            _keyValueLabel.text = string.Format(labelFormat, panelData._itemName, panelData._itemValue);

            _headerToggle.onClick.RemoveAllListeners();
            _headerToggle.onClick.AddListener(_OnToggleButton);
            _headerToggle.interactable = panelData._hasChildren;
        }

        protected override float _GetLabelLength()
        {
            return _keyValueLabel.preferredWidth;
        }
    }

    sealed class GameObjectDetailPanelData : HierarchyPanelDataBase
    {
        public string _itemName;
        public string _itemValue;

        public GameObjectDetailPanelData(GameObjectDetailEntry entry, int depth, System.Action onToggle)
        {
            _itemName = entry._name;
            _itemValue = entry._value;
            _depth = depth;
            _isOpen = entry._isOpen;
            _hasChildren = entry._subDetailList.Count > 0;
            _toggleOpen = onToggle;
        }
    }
}
