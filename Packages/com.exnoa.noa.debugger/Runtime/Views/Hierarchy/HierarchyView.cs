using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class HierarchyView : NoaDebuggerToolViewBase<HierarchyViewLinker>
    {
        public GameObjectDetailView _gameObjectDetail;
        public HierarchyTreeView _hierarchyTree;

        protected override void _OnShow(HierarchyViewLinker linker)
        {
            _hierarchyTree.Show(linker._hierarchyViewData);
            _gameObjectDetail.Show(linker._selectGameObjectDetail);
        }
    }

    sealed class HierarchyViewLinker : ViewLinkerBase
    {
        public HierarchyViewData _hierarchyViewData;

        public GameObjectDetail _selectGameObjectDetail;
    }


    struct HierarchyViewData
    {
        public List<HierarchyPanelData> _hierarchyPanelList;

        public int _sceneNum;

        public int _objectNum;
    }

    struct GameObjectDetail
    {
        public List<GameObjectDetailPanelData> _componentPanelList;

        public int _componentNum;

        public bool _isLock;

        public bool _isActive;
    }
}
