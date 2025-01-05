using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class NoaDebuggerView : ViewBase<NoaDebuggerViewLinker>
    {
        public event Action<int> ChangeToolListener;

        public event Action OnToolDetailButton;

        public event Action OnCloseButton;

        public event Action OnPinButton;

        public event Action OnChangeButton;

        [Header("Content")]
        [SerializeField] Transform _content;
        [SerializeField] Image _background;

        [Header("Button")]
        [SerializeField] Button _toolDetailButton;
        [SerializeField] Button _closeButton;
        [SerializeField] ToggleButtonBase _pinButton;
        [SerializeField] NoaDebuggerDisableButton _changeButton;

        [Header("Text")]
        [SerializeField] TextMeshProUGUI _toolName;
        [SerializeField] TextMeshProUGUI _noaDebuggerVersion;
        [SerializeField] TextMeshProUGUI _enableChangeButtonText;
        [SerializeField] TextMeshProUGUI _disableChangeButtonText;
        string ChangeButtonText
        {
            set
            {
                _enableChangeButtonText.text = value;
                _disableChangeButtonText.text = value;
            }
            get
            {
                return _enableChangeButtonText.text;
            }
        }

        [Header("Header")]
        [SerializeField] GameObject _headerSelectLine;

        [Header("Tab")]
        [SerializeField] ScrollRect _tabScroll;
        [SerializeField] MenuTabComponent _tabBase;
        MenuTabComponent[] _tabs;
        bool _createdTabs;

        [Header("Root content")]
        [SerializeField] RectTransform _rootContent;
        [Header("Side menu")]
        [SerializeField] RectTransform _sideMenu;
        [SerializeField] Button _sideMenuShowButton;
        [SerializeField] Button _sideMenuHideButton;
        [SerializeField] GameObject _sideMenuSpace;
        [SerializeField] Image _menuViewHeader;
        [SerializeField] GameObject _menuViewHeaderLine;

        [Header("UIReverseComponents")]
        [SerializeField] UIReverseComponents _reverseComponents;

        bool _isUIReversePortrait;

        protected override void _Init()
        {
            _toolDetailButton.onClick.AddListener(() => OnToolDetailButton?.Invoke());
            _closeButton.onClick.AddListener(() => OnCloseButton?.Invoke());
            _pinButton._onClick.AddListener((isOn) => OnPinButton?.Invoke());
            _changeButton.onClick.AddListener(() => OnChangeButton?.Invoke());

            var noaDebuggerSettings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings();
            noaDebuggerSettings.BackgroundAlpha = Mathf.Clamp(
                noaDebuggerSettings.BackgroundAlpha,
                NoaDebuggerDefine.CanvasAlphaMin,
                NoaDebuggerDefine.CanvasAlphaMax);
            Color backgroundColor = _background.color;
            backgroundColor.a = noaDebuggerSettings.BackgroundAlpha;
            _background.color = backgroundColor;

            _isUIReversePortrait = noaDebuggerSettings.IsUIReversePortrait;
        }

        protected override void _OnStart()
        {
            base._OnStart();
            _sideMenuShowButton.onClick.AddListener(_ShowSideMenu);
            _sideMenuHideButton.onClick.AddListener(_HideSideMenu);
        }

        protected override void _OnShow(NoaDebuggerViewLinker linker)
        {
            CreateMenu(linker._targetTools);

            INoaDebuggerTool target = linker._forceTargetTool ?? linker._targetTools[linker._activeToolIndex];

            _toolName.text = target.MenuInfo().Name;
            target.ShowView(_content);

            if (linker._isPortrait)
            {
                _OnEnableSideMenu();

                if (linker._isMenuActive)
                {
                    _ShowSideMenu();
                }
                else
                {
                    _HideSideMenu();
                }
            }
            else
            {
                _OnDisableSideMenu();
                _HideSideMenu();
            }

            _AlignmentUI(linker._isPortrait, target);

            UpdateMenu(linker);

            _headerSelectLine.SetActive(linker._forceTargetTool != null);

            ToolPinStatus pinStatus = NoaDebuggerManager.IsWorldSpaceRenderingEnabled
                ? ToolPinStatus.None
                : target.GetPinStatus();

            bool isPinButtonActive = pinStatus != ToolPinStatus.None;
            _pinButton.gameObject.SetActive(isPinButtonActive);

            bool isPinOn = pinStatus == ToolPinStatus.On;
            _pinButton.Init(isPinOn);

            NoaDebuggerInfo info = NoaDebuggerInfoManager.GetNoaDebuggerInfo();
            _noaDebuggerVersion.text = info.NoaDebuggerVersion;

            SetChangeButtonText(linker._isCustom);
        }

        void _AlignmentUI(bool isPortrait, INoaDebuggerTool target)
        {
            bool isUIReverse = isPortrait && _isUIReversePortrait;

            _reverseComponents.Alignment(isUIReverse);

            Vector2 tmp = _tabScroll.content.pivot;
            tmp.y = isUIReverse ? 0 : 1;
            _tabScroll.content.pivot = tmp;

            if (_tabs != null)
            {
                foreach (MenuTabComponent tab in _tabs)
                {
                    tab.AlignmentUI(isUIReverse);
                }
            }

            GlobalCoroutine.Run(_SetScrollPosition());

            target.AlignmentUI(isUIReverse);
        }

        IEnumerator _SetScrollPosition()
        {
            yield return null;

            _tabScroll.verticalNormalizedPosition = 1;
        }


        public void UpdateMenu(NoaDebuggerViewLinker linker)
        {
            if (_tabs != null)
            {
                for (int i = 0; i < _tabs.Length; i++)
                {
                    var tab = _tabs[i];
                    bool isSelect = i == linker._activeToolIndex;
                    tab.ChangeTabSelect(isSelect);
                    tab.ShowNoticeBadge(linker._targetTools[i].NotifyStatus);
                }
            }
        }

        void _OnChangeTool(int index)
        {
            ChangeToolListener?.Invoke(index);
        }

        public void HideContents()
        {
            foreach (Transform content in _content)
            {
                content.gameObject.SetActive(false);
            }
        }

        public void SetChangeButtonInteractable(bool interactable)
        {
            _changeButton.Interactable = interactable;
        }

        void SetChangeButtonText(bool isShowCustomTab)
        {
            if (isShowCustomTab)
            {
                ChangeButtonText = "Default Menu";
            }
            else
            {
                ChangeButtonText = "Custom Menu";
            }
        }

        void _DestroyAllMenuButton()
        {
            if (_tabs == null)
            {
                return;
            }

            foreach (MenuTabComponent menuTab in _tabs)
            {
                menuTab._tabButton.onClick.RemoveAllListeners();
                GameObject.Destroy(menuTab.gameObject);
            }
            _tabs = null;
        }

        public void CreateMenu(INoaDebuggerTool[] tools, bool forceCreate = false)
        {
            if (_createdTabs && !forceCreate)
            {
                return;
            }

            _createdTabs = true;

            if (forceCreate)
            {
                _DestroyAllMenuButton();
            }

            if (tools.Length == 0)
            {
                _tabBase.gameObject.SetActive(false);
                return;
            }

            _tabs = new MenuTabComponent[tools.Length];
            for (int i = 0; i < tools.Length; i++)
            {
                int index = i;
                var tool = tools[index];

                MenuTabComponent button = GameObject.Instantiate(_tabBase, _tabScroll.content);
                button.gameObject.SetActive(true);
                button.name = tool.MenuInfo().MenuName;
                button._label.text = tool.MenuInfo().MenuName;
                button._tabButton.onClick.AddListener(() =>
                {
                    _OnChangeTool(index);
                });
                _tabs[index] = button;
            }
            _tabBase.gameObject.SetActive(false);
        }

        public void InitTabs()
        {
            if (_tabs == null)
            {
                return;
            }
            foreach (var tab in _tabs)
            {
                tab.ChangeTabSelect(false);
            }
        }

        void _OnEnableSideMenu()
        {
            var parent = _sideMenu.parent as RectTransform;

            var layoutElement = _sideMenu.GetComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            _sideMenu.pivot = new Vector2(1, 1);
            _sideMenu.offsetMin = new Vector2(1f, 0f);
            _sideMenu.offsetMax = new Vector2(1f, 1f);

            _sideMenu.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, layoutElement.preferredWidth);
            _sideMenu.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parent.rect.height);

            _sideMenu.anchoredPosition = new Vector2(0, 0);

            _sideMenuShowButton.gameObject.SetActive(true);
            _sideMenuSpace.gameObject.SetActive(true);
            _menuViewHeader.enabled = true;
            _menuViewHeaderLine.SetActive(false);
        }

        void _OnDisableSideMenu()
        {
            var layoutElement = _sideMenu.GetComponent<LayoutElement>();
            layoutElement.ignoreLayout = false;

            _rootContent.anchoredPosition = Vector2.zero;
            _sideMenuShowButton.gameObject.SetActive(false);
            _sideMenuSpace.gameObject.SetActive(false);
            _menuViewHeader.enabled = false;
            _menuViewHeaderLine.SetActive(true);
        }

        void _ShowSideMenu()
        {
            var parent = _sideMenu.parent as RectTransform;
            _sideMenu.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parent.rect.height);
            _rootContent.anchoredPosition = new Vector2(_sideMenu.rect.width, _rootContent.anchoredPosition.y);
            _sideMenuHideButton.gameObject.SetActive(true);
        }

        void _HideSideMenu()
        {
            _rootContent.anchoredPosition = new Vector2(0, _rootContent.anchoredPosition.y);
            _sideMenuHideButton.gameObject.SetActive(false);
        }

        protected override void _OnHide()
        {
            if (_sideMenuHideButton.IsActive())
            {
                _HideSideMenu();
            }
            base._OnHide();
        }

        void OnDestroy()
        {
            ChangeToolListener = default;
            OnToolDetailButton = default;
            OnCloseButton = default;
            OnPinButton = default;
            OnChangeButton = default;
            _content = default;
            _background = default;
            _toolDetailButton = default;
            _closeButton = default;
            _pinButton = default;
            _changeButton = default;
            _toolName = default;
            _noaDebuggerVersion = default;
            _enableChangeButtonText = default;
            _disableChangeButtonText = default;
            _headerSelectLine = default;
            _tabScroll = default;
            _tabBase = default;
            _tabs = default;
            _rootContent = default;
            _sideMenu = default;
            _sideMenuShowButton = default;
            _sideMenuHideButton = default;
            _sideMenuSpace = default;
            _menuViewHeader = default;
            _menuViewHeaderLine = default;
        }
    }

    sealed class NoaDebuggerViewLinker : ViewLinkerBase
    {
        public INoaDebuggerTool[] _targetTools;
        public int _activeToolIndex;
        public bool _isCustom;
        public bool _isPortrait;

        public bool _isMenuActive = false;

        public INoaDebuggerTool _forceTargetTool;
    }
}
