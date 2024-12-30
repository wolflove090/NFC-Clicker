using System;
using UnityEngine;
using NoaDebugger.DebugCommand;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class DebugCommandView : NoaDebuggerToolViewBase<DebugCommandViewLinker>
    {
        [Header("Header")]
        [SerializeField] ToggleButtonBase _displayDetailsToggle;
        [SerializeField] LongTapToggleColorButton _refreshButton;

        [Header("Scroll")]
        [SerializeField] DebugCommandScrollAreaView _scrollArea;

        [SerializeField] GameObject _explanationLabel;

        [Header("Detail")]
        [SerializeField] DebugCommandDetailsAreaView _detailsArea;

        [Header("Category")]
        [SerializeField] DebugCommandCategoryView _categorySelect;


        public event UnityAction<int> OnSelectCategory;

        public event UnityAction<bool> OnDisplayDetailsToggle;

        public event UnityAction<bool> OnTapRefreshButton;

        public event UnityAction<bool> OnLongTapRefreshButton;


        void _OnValidateUI()
        {
            Assert.IsNotNull(_displayDetailsToggle);
            Assert.IsNotNull(_refreshButton);
            Assert.IsNotNull(_scrollArea);
            Assert.IsNotNull(_explanationLabel);
            Assert.IsNotNull(_detailsArea);
            Assert.IsNotNull(_categorySelect);
        }

        protected override void _Init()
        {
            _OnValidateUI();
            _categorySelect.OnSelectCategory += _OnCategoryTab;
            _displayDetailsToggle._onClick.RemoveAllListeners();
            _displayDetailsToggle._onClick.AddListener(_OnDisplayDetailsToggle);
            _refreshButton.OnClick.RemoveAllListeners();
            _refreshButton.OnClick.AddListener(_OnTapRefreshButton);
            _refreshButton.OnToggle.RemoveAllListeners();
            _refreshButton.OnToggle.AddListener(_OnLongTapRefreshButton);
            _detailsArea.Init();
        }

        protected override void _OnShow(DebugCommandViewLinker linker)
        {
            if(linker.IsMatchUpdateTarget(CommandViewUpdateTarget.CategorySelect))
            {
                _categorySelect.Show(linker);
            }

            if(linker.IsMatchUpdateTarget(CommandViewUpdateTarget.DetailArea))
            {
                if (linker._isDetailMode)
                {
                    _detailsArea.Show(linker);
                }
                else
                {
                    _detailsArea.Hide();
                }
            }

            _scrollArea.Show(linker);

            if (linker.IsMatchUpdateTarget(CommandViewUpdateTarget.GroupHeader))
            {
                bool isShowExplanation = linker._isActiveFloatingWindow &&
                                         linker._categoryNames is {Length: > 0} &&
                                         linker._groups is {Length: > 0};

                _explanationLabel.SetActive(isShowExplanation);
            }

            if (linker.IsMatchUpdateTarget(CommandViewUpdateTarget.Buttons))
            {
                _displayDetailsToggle.Init(linker._isDetailMode);

                _refreshButton.Init(linker._isAutoRefresh);
            }
        }

        public override void AlignmentUI(bool isReverse)
        {
            base.AlignmentUI(isReverse);

            _scrollArea.AlignmentUI(isReverse);
            _categorySelect.AlignmentUI(isReverse);
        }


        public void RefreshCommandPanels()
        {
            _scrollArea.RefreshCommandPanels();
        }


        void _OnCategoryTab(int index)
        {
            OnSelectCategory?.Invoke(index);
            _scrollArea.ResetScrollPosition();
        }

        void _OnDisplayDetailsToggle(bool isOn)
        {
            OnDisplayDetailsToggle?.Invoke(isOn);
        }

        void _OnTapRefreshButton()
        {
            OnTapRefreshButton?.Invoke(_refreshButton.IsOn);
        }

        void _OnLongTapRefreshButton(bool isToggleOn)
        {
            OnLongTapRefreshButton?.Invoke(false);
        }
    }


    class DebugCommandViewLinker : ViewLinkerBase
    {
        public string[] _categoryNames;

        public GroupPanelInfo[] _groups;

        public int _selectCategoryIndex;

        public bool _isActiveFloatingWindow;

        public bool _isDetailMode = false;

        public bool _isSelectGroupForDetail = false;

        public string _selectedGroupForDetail = null;

        public ICommand[] _displayDetailCommands = null;

        public bool _isAutoRefresh;

        public CommandViewUpdateTarget _updateTarget = CommandViewUpdateTarget.All;

        public bool IsMatchUpdateTarget(CommandViewUpdateTarget target)
        {
            return (_updateTarget & target) == target;
        }
    }

    [Flags]
    enum CommandViewUpdateTarget
    {
        CategorySelect = 1 << 0,

        GroupHeader = 1 << 1,

        CommandStatus = 1 << 2,

        CommandPanel = 1 << 3,

        DetailArea = 1 << 4,

        Buttons = 1 << 5,

        All = CategorySelect | GroupHeader | CommandStatus | CommandPanel | DetailArea | Buttons,
    }
}
