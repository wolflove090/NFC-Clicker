using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class HierarchyFloatingWindowView : FloatingWindowViewBase<HierarchyViewLinker>
    {
        [SerializeField, Header("Counter")] TextMeshProUGUI _sceneCountLabel;
        [SerializeField]
        public HierarchyTreeView _hierarchyTreeView;
        [SerializeField] TextMeshProUGUI _objectCountLabel;

        [SerializeField] NoaDebuggerDisableButton _refreshButton;

        HierarchyPanelScrollHelper<HierarchyPanel, HierarchyPanelData> _scrollHelper;

        public event System.Action OnRefreshHierarchy;

        void _OnValidateUI()
        {
            Assert.IsNotNull(_sceneCountLabel);
            Assert.IsNotNull(_objectCountLabel);
            Assert.IsNotNull(_refreshButton);
        }

        protected override void _OnShow(HierarchyViewLinker linker)
        {
            HierarchyViewData hierarchyData = linker._hierarchyViewData;

            _hierarchyTreeView.Show(hierarchyData);

            _OnValidateUI();

            _refreshButton.onClick.RemoveAllListeners();
            _refreshButton.onClick.AddListener(_OnRefresh);

            _sceneCountLabel.text = hierarchyData._sceneNum.ToString();
            _objectCountLabel.text = hierarchyData._objectNum.ToString();
        }

        void _OnRefresh()
        {
            OnRefreshHierarchy?.Invoke();
        }
    }
}
