using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace NoaDebugger
{
    class DebugCommandFloatingWindowView : FloatingWindowViewBase<DebugCommandFloatingWindowViewLinker>
    {
        [Header("Default")]
        [SerializeField] DebugCommandScrollAreaView _scrollArea;
        [SerializeField] DebugCommandCategoryView _defaultViewCategorySelect;
        [SerializeField] LongTapToggleColorButton _refreshButton;

        [Header("Small")]
        [SerializeField] DebugCommandCategoryView _smallView;


        public event UnityAction<int> OnSelectCategory;

        public event UnityAction<bool,DebugCommandFloatingWindowViewLinker> OnTapRefreshButton;

        public event UnityAction<DebugCommandFloatingWindowViewLinker> OnLongTapRefreshButton;


        DebugCommandFloatingWindowViewLinker _linker;


        void _OnValidateUI()
        {
            Assert.IsNotNull(_scrollArea);
            Assert.IsNotNull(_defaultViewCategorySelect);
            Assert.IsNotNull(_smallView);
            Assert.IsNotNull(_refreshButton);
        }

        protected override void _Init()
        {
            base._Init();
            _OnValidateUI();
            _defaultViewCategorySelect.OnSelectCategory += _OnCategoryTab;
            _smallView.OnSelectCategory += _OnCategoryTab;

            _refreshButton.OnClick.RemoveAllListeners();
            _refreshButton.OnClick.AddListener(_OnTapRefreshButton);
            _refreshButton.OnToggle.RemoveAllListeners();
            _refreshButton.OnToggle.AddListener(_OnLongTapRefreshButton);
        }

        protected override void _OnShow(DebugCommandFloatingWindowViewLinker linker)
        {
            _linker = linker;
            DebugCommandViewLinker viewLinker = linker._viewLinker;
            if (viewLinker.IsMatchUpdateTarget(CommandViewUpdateTarget.Buttons))
            {
                bool isAutoRefresh = linker._viewLinker != null && linker._viewLinker._isAutoRefresh;
                _refreshButton.Init(isAutoRefresh);
            }

            if (linker._refreshCommandPanelsOnly)
            {
                _scrollArea.RefreshCommandPanels();
            }
            else
            {
                _scrollArea.Show(linker._viewLinker);

                if (viewLinker.IsMatchUpdateTarget(CommandViewUpdateTarget.CategorySelect))
                {
                    _defaultViewCategorySelect.Show(linker._viewLinker);
                    _smallView.Show(linker._viewLinker);
                }
            }
        }


        void _OnCategoryTab(int index)
        {
            OnSelectCategory?.Invoke(index);
            _scrollArea.ResetScrollPosition();
        }

        void _OnTapRefreshButton()
        {
            OnTapRefreshButton?.Invoke(_refreshButton.IsOn, _linker);
        }

        void _OnLongTapRefreshButton(bool isToggleOn)
        {
            OnLongTapRefreshButton?.Invoke(_linker);
        }
    }


    class DebugCommandFloatingWindowViewLinker : ViewLinkerBase
    {
        public DebugCommandViewLinker _viewLinker;

        public bool _refreshCommandPanelsOnly;

        public UnityAction<bool> _onLongTapRefreshButton;
    }
}
