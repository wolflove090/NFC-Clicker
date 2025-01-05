using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class HierarchyTreeView : MonoBehaviour
    {
        [SerializeField, Header("Counter")] TextMeshProUGUI _sceneCountLabel;
        [SerializeField] TextMeshProUGUI _objectCountLabel;

        [SerializeField] ObjectPoolScroll _hierarchyScroll;
        [SerializeField] NoaDebuggerDisableButton _refreshButton;

        HierarchyPanelScrollHelper<HierarchyPanel, HierarchyPanelData> _scrollHelper;

        public event System.Action OnRefreshHierarchy;

        void _OnValidateUI()
        {
            Assert.IsNotNull(_hierarchyScroll);
            Assert.IsNotNull(_sceneCountLabel);
            Assert.IsNotNull(_objectCountLabel);
            Assert.IsNotNull(_refreshButton);

            if(_scrollHelper == null)
            {
                _scrollHelper = new HierarchyPanelScrollHelper<HierarchyPanel, HierarchyPanelData>(_hierarchyScroll);
            }
        }

        public void Show(HierarchyViewData hierarchyData)
        {
            _OnValidateUI();

            _refreshButton.onClick.RemoveAllListeners();
            _refreshButton.onClick.AddListener(_OnRefresh);

            _scrollHelper.RefreshScroll(hierarchyData._hierarchyPanelList);

            _sceneCountLabel.text = hierarchyData._sceneNum.ToString();
            _objectCountLabel.text = hierarchyData._objectNum.ToString();
        }

        void _OnRefresh()
        {
            OnRefreshHierarchy?.Invoke();
        }
    }
}
