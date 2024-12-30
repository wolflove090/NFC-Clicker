using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class DebugCommandCategoryView : ViewBase<DebugCommandViewLinker>
    {
        [SerializeField, Header("Category selection")]
        TextMeshProUGUI _categoryLabel;
        [SerializeField]
        GameObject _categorySelect;
        [SerializeField]
        NoaDebuggerDisableButton _categorySelectButton;
        [SerializeField]
        MenuTabComponent _categoryTab;
        [SerializeField]
        ScrollRect _tabScroll;

        MenuTabComponent[] _tabs;
        string[] _categoryNames;


        public event UnityAction<int> OnSelectCategory;


        void _OnValidateUI()
        {
            Assert.IsNotNull(_categoryLabel);
            Assert.IsNotNull(_categorySelect);
            Assert.IsNotNull(_categorySelectButton);
            Assert.IsNotNull(_categoryTab);
            Assert.IsNotNull(_tabScroll);
        }

        protected override void _Init()
        {
            _OnValidateUI();

            _categorySelectButton.onClick.RemoveAllListeners();
            _categorySelectButton.onClick.AddListener(_OnClickCategorySelect);

            _categorySelect.SetActive(false);
        }

        protected override void _OnShow(DebugCommandViewLinker linker)
        {
            _categoryNames = linker._categoryNames;

            string selectCategoryName = "";
            if (linker._categoryNames.Length > 0)
            {
                selectCategoryName = _categoryNames[linker._selectCategoryIndex];

                _CreateCategoryMenu(_categoryNames);

                for (int i = 0; i < _tabs.Length; i++)
                {
                    var tab = _tabs[i];
                    bool isSelect = i == linker._selectCategoryIndex;
                    tab.ChangeTabSelect(isSelect);
                    tab.ShowNoticeBadge(ToolNotificationStatus.None);
                }
                _categorySelectButton.Interactable = true;
            }
            else
            {
                selectCategoryName = "None";
                _categoryTab.gameObject.SetActive(false);
                _categorySelectButton.Interactable = false;
            }
            _categoryLabel.text = selectCategoryName;
        }

        public void AlignmentUI(bool isReverse)
        {
            Vector2 tmp = _tabScroll.content.pivot;
            tmp.y = isReverse ? 0 : 1;
            _tabScroll.content.pivot = tmp;

            if (_tabs != null)
            {
                foreach (MenuTabComponent tab in _tabs)
                {
                    tab.AlignmentUI(isReverse);
                }
            }

            GlobalCoroutine.Run(_SetScrollPosition());
        }

        IEnumerator _SetScrollPosition()
        {
            yield return null;

            _tabScroll.verticalNormalizedPosition = 1;
        }


        void _CreateCategoryMenu(string[] categoryNames)
        {
            _tabs = new MenuTabComponent[categoryNames.Length];

            for (int i = 0; i < categoryNames.Length; i++)
            {
                int index = i;

                string categoryName = categoryNames[i];
                MenuTabComponent tabC = null;
                if (i >= _tabScroll.content.childCount)
                {
                    tabC = GameObject.Instantiate(_categoryTab, _tabScroll.content);
                }
                else
                {
                    Transform tab = _tabScroll.content.GetChild(i);
                    tabC = tab.GetComponent<MenuTabComponent>();
                }

                tabC.gameObject.SetActive(true);
                tabC.name = categoryName;
                tabC._label.text = categoryName;
                tabC._tabButton.onClick.RemoveAllListeners();
                tabC._tabButton.onClick.AddListener(
                    () =>
                    {
                        _OnCategoryTab(index);
                    });
                _tabs[i] = tabC;
            }

            for (int i = categoryNames.Length; i < _tabScroll.content.childCount; i++)
            {
                _tabScroll.content.GetChild(i).gameObject.SetActive(false);
            }
        }


        void _OnClickCategorySelect()
        {
            bool isShow = !_categorySelect.activeSelf;
            _categorySelect.SetActive(isShow);

            if (isShow)
            {
                GlobalCoroutine.Run(_SetScrollPosition());
            }
        }

        void _OnCategoryTab(int index)
        {
            OnSelectCategory?.Invoke(index);
            _categorySelect.SetActive(false);
        }
    }
}
