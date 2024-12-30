using System;
using System.Collections.Generic;
using UnityEngine;

namespace NoaDebugger
{
    sealed class HierarchyPresenter : NoaDebuggerToolBase, INoaDebuggerTool
    {
        [Header("MainView")]
        [SerializeField]
        HierarchyView _mainViewPrefab;
        HierarchyView _mainView;

        [SerializeField, Header("FloatingWindowView")]
        HierarchyFloatingWindowView _floatingWindowPrefab;
        FloatingWindowPresenter<HierarchyFloatingWindowView, HierarchyViewLinker> _floatingWindowPresenter;

        HierarchyModel _hierarchyModel;
        HierarchyModel.HierarchyInformation _hierarchyInfo;

        List<GameObjectDetailEntry> _componentPropertyList = new List<GameObjectDetailEntry>();
        HierarchyGameObjectEntry _selectGameObjectEntry;
        bool _isLockSelectGameObject;
        private bool _isSelectGameObjectPropertyChanged;

        class HierarchyMenuInfo : IMenuInfo
        {
            public string Name => "Hierarchy";
            public string MenuName => "Hierarchy";
            public int SortNo => NoaDebuggerDefine.HIERARCHY_MENU_SORT_NO;
        }
        HierarchyMenuInfo _menuInfo;

        public IMenuInfo MenuInfo()
        {
            if (_menuInfo == null)
            {
                _menuInfo = new HierarchyMenuInfo();
            }

            return _menuInfo;
        }

        public ToolNotificationStatus NotifyStatus => ToolNotificationStatus.None;

        public void Init()
        {
            _hierarchyModel = new HierarchyModel();

            _floatingWindowPresenter =
                new FloatingWindowPresenter<HierarchyFloatingWindowView, HierarchyViewLinker>(
                    _floatingWindowPrefab, NoaDebuggerPrefsDefine.PrefsKeyIsHierarchyWindowInfo, MenuInfo().Name);
            _floatingWindowPresenter.OnInitAction += _InitFloatingWindow;
        }


        public void ShowView(Transform parent)
        {
            if (_mainView == null)
            {
                _mainView = GameObject.Instantiate(_mainViewPrefab, parent);
                _InitView(_mainView);
            }

            _RefreshHierarchy();
            _mainView.gameObject.SetActive(true);
        }

        void _InitView(HierarchyView view)
        {
            view._hierarchyTree.OnRefreshHierarchy += _RefreshHierarchy;
            view._gameObjectDetail.OnDetailLock += _OnDetailLock;
            view._gameObjectDetail.OnSwitchSelectedObjectActive += _OnSwitchSelectedObjectActive;
        }

        void _HiddenView()
        {
            if (_mainView == null)
            {
                return;
            }
            _mainView.gameObject.SetActive(false);
        }

        void _UpdateView()
        {
            if (_hierarchyInfo == null)
            {
                (_hierarchyInfo, _selectGameObjectEntry) = _hierarchyModel.GetHierarchy();
            }

            HierarchyViewLinker linker = _CreateLinker();

            if (_mainView != null)
            {
                _mainView.Show(linker);
            }

            if (_floatingWindowPresenter.IsActive)
            {
                _floatingWindowPresenter.ShowWindowView(linker);
            }
        }

        void _RefreshHierarchy()
        {
            _hierarchyInfo = null;
            _UpdateView();
        }

        public void _OnHierarchyToggleOpen(int hash, bool isOpen)
        {
            if (isOpen)
            {
                _hierarchyModel.AddOpenObjectHash(hash);
            }
            else
            {
                _hierarchyModel.RemoveOpenObjectHash(hash);
            }
        }

        void _OnDetailLock(bool isLock)
        {
            _isLockSelectGameObject = isLock;
        }

        void _OnSwitchSelectedObjectActive(bool isActive)
        {
            if (_selectGameObjectEntry == null || _selectGameObjectEntry._gameObject == null)
            {
                return;
            }

            _isSelectGameObjectPropertyChanged = _selectGameObjectEntry._gameObject.activeSelf != isActive;
            _selectGameObjectEntry._gameObject.SetActive(isActive);
            _RefreshHierarchy();
        }

        void _SelectGameObject(HierarchyGameObjectEntry target)
        {
            if (_isLockSelectGameObject)
            {
                _UpdateView();
                return;
            }

            if (_selectGameObjectEntry != null)
            {
                _selectGameObjectEntry._isSelected = false;
            }

            _componentPropertyList = new List<GameObjectDetailEntry>();
            _selectGameObjectEntry = target;
            _selectGameObjectEntry._isSelected = true;
            _hierarchyModel.SetSelectObjectHashCode(target._hashCode);
            _UpdateView();
        }

        public void AlignmentUI(bool isReverse)
        {
            _mainView.AlignmentUI(isReverse);
        }


        public ToolPinStatus GetPinStatus()
        {
            return _floatingWindowPresenter.IsActive ? ToolPinStatus.On : ToolPinStatus.Off;
        }

        public void TogglePin(Transform parent)
        {
            _floatingWindowPresenter.ToggleActive(parent);
        }

        public void InitFloatingWindow(Transform parent)
        {
            var isWindowDrawing = _floatingWindowPresenter.IsActive;
            if (isWindowDrawing)
            {
                _floatingWindowPresenter.InstantiateWindow(parent);
            }
        }

        void _InitFloatingWindow(HierarchyFloatingWindowView window)
        {
            window.OnRefreshHierarchy += _RefreshHierarchy;
            window._hierarchyTreeView.OnRefreshHierarchy += _RefreshHierarchy;
            _UpdateView();
        }


        public void OnHidden()
        {
            _HiddenView();
        }

        public void OnToolDispose()
        {
            _HiddenView();
            _hierarchyModel = null;
            _hierarchyInfo = null;
            _componentPropertyList = null;
            _selectGameObjectEntry = null;
        }


        HierarchyViewLinker _CreateLinker()
        {

            HierarchyViewData hierarchyViewData = new HierarchyViewData();
            hierarchyViewData._sceneNum = _hierarchyInfo._hierarchySceneList.Count;
            hierarchyViewData._objectNum = _hierarchyInfo._objectNum;
            hierarchyViewData._hierarchyPanelList = _CreateHierarchyData();


            GameObjectDetail gameObjectDetail = new GameObjectDetail();

            if (_selectGameObjectEntry != null && _componentPropertyList.Count == 0 && _selectGameObjectEntry._gameObject != null)
            {
                _componentPropertyList = _hierarchyModel.GetGameObjectDetailIEntryList(_selectGameObjectEntry._gameObject);
            }

            gameObjectDetail._componentNum = _componentPropertyList != null ? _componentPropertyList.Count - 1 : 0;
            gameObjectDetail._isLock = _isLockSelectGameObject;
            bool isActive = false;

            if (_selectGameObjectEntry != null && _selectGameObjectEntry._gameObject != null)
            {
                isActive = _selectGameObjectEntry._gameObject.activeSelf;
                if (_isSelectGameObjectPropertyChanged && _componentPropertyList != null && _componentPropertyList.Count > 0)
                {
                    bool isComponentDetailOpened = _componentPropertyList[0]._isOpen;
                    _componentPropertyList = _hierarchyModel.GetGameObjectDetailIEntryList(_selectGameObjectEntry._gameObject);
                    _componentPropertyList[0]._isOpen = isComponentDetailOpened;
                    _isSelectGameObjectPropertyChanged = false;
                }
            }

            gameObjectDetail._isActive = isActive;
            gameObjectDetail._componentPanelList = _CreateComponentPanelData();


            return new HierarchyViewLinker()
            {
                _hierarchyViewData = hierarchyViewData,
                _selectGameObjectDetail = gameObjectDetail,
            };
        }

        List<HierarchyPanelData> _CreateHierarchyData()
        {
            HierarchyGameObjectEntryCallback callback = new HierarchyGameObjectEntryCallback()
            {
                _onSelect = _SelectGameObject,
                _onToggleOpen = _OnHierarchyToggleOpen,
                _onUpdateView = _UpdateView,
            };

            var list = new List<HierarchyPanelData>();
            foreach (var hierarchy in _hierarchyInfo._hierarchySceneList)
            {
                list.AddRange(hierarchy.GetPanelDataListWithChildren(callback));
            }

            return list;
        }

        List<GameObjectDetailPanelData> _CreateComponentPanelData()
        {
            GameObjectDetailEntryCallback callback = new GameObjectDetailEntryCallback()
            {
                _onUpdateView = _UpdateView,
            };

            var list = new List<GameObjectDetailPanelData>();
            foreach (var component in _componentPropertyList)
            {
                list.AddRange(component.GetPanelDataListWithSubDetail(callback));
            }

            return list;
        }

        void OnDestroy()
        {
            _mainViewPrefab = default;
            _mainView = default;
            _floatingWindowPrefab = default;
            _floatingWindowPresenter = default;
            _hierarchyModel = default;
            _hierarchyInfo = default;
            _componentPropertyList = default;
            _selectGameObjectEntry = default;
            _menuInfo = default;
        }
    }
}
