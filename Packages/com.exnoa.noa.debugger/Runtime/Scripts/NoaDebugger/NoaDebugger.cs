using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NoaDebugger
{
    [RequireComponent(typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler))]
    sealed class NoaDebugger : MonoBehaviour
    {
        static readonly string DeviceOrientationKey = "NoaDebuggerDeviceOrientation";

        static NoaDebugger _instance;

        [SerializeField]
        NoaDebuggerView _mainView;

        [SerializeField]
        Transform _presenterRoot;
        INoaDebuggerTool[] _allNoaDebuggerTools; 
        INoaDebuggerTool[] _filteredNoaDebuggerTools; 
        INoaDebuggerTool[] _noaDebuggerTools; 
        public List<string> _initializedNoaDebuggerToolNames;
        int _activeToolIndex;
        int _lastActiveToolIndex;
        bool _isSearchingToolWithoutError;
        [SerializeField]
        ToolDetailPresenter _toolDetailPresenter;

        INoaDebuggerTool[] _allCustomNoaDebuggerTools; 
        List<Type> _customMenuType;

        [SerializeField, Header("SafeArea")]
        SafeAreaAdjuster _safeAreaRoot;
        [SerializeField, Header("FloatingWindow")]
        GameObject _floatingWindowRoot;
        [SerializeField, Header("NoaDebugger")]
        GameObject _noaDebugger;
        [SerializeField]
        GameObject _noaDebuggerBg;
        [SerializeField, Header("Dialog")]
        Transform _dialogRoot;

        [SerializeField, Header("Toast")]
        Transform _toastRoot;
        [SerializeField]
        ToastView _toastPrefab;
        ToastView _toastInstance;

        [SerializeField, Header("NoaUIElement")]
        NoaUIElementManager _noaUIElementManager;

        [SerializeField, Header("Operation environments")]
        RuntimeVersionChecker _runtimeVersionChecker;

        [SerializeField, Header("Manager")]
        Transform _managerRoot;

        const int TOOL_DETAIL_ACTIVE_INDEX = -1;

        bool _isShowCustomTab;
        bool _isPortrait;
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasScaler _canvasScaler;
        bool _isActiveFloatingWindow;

        bool _floatingWindowVisibleSetting = true;

        public Action<int> OnShow { get; set; }

        public Action<int> OnHide { get; set; }

        public bool IsInitialized { get; private set; } = false;

        public bool IsWorldSpaceRenderingEnabled => _canvas != null && _canvas.renderMode == RenderMode.WorldSpace;

        public bool IsDebuggerVisible => _noaDebugger != null && _noaDebugger.gameObject.activeInHierarchy;

        Action<int, bool> _onMenuChanged;

        public NoaUIElementManager NoaUIElementManager => _noaUIElementManager;

        void Awake()
        {
            Assert.IsNotNull(_mainView);
            Assert.IsNotNull(_presenterRoot);
            Assert.IsNotNull(_toolDetailPresenter);
            Assert.IsNotNull(_safeAreaRoot);
            Assert.IsNotNull(_floatingWindowRoot);
            Assert.IsNotNull(_noaDebugger);
            Assert.IsNotNull(_noaDebuggerBg);
            Assert.IsNotNull(_dialogRoot);
            Assert.IsNotNull(_toastRoot);
            Assert.IsNotNull(_toastPrefab);
            Assert.IsNotNull(_noaUIElementManager);
            Assert.IsNotNull(_runtimeVersionChecker);
            Assert.IsNotNull(_managerRoot);

            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
            _canvasScaler = GetComponent<CanvasScaler>();
        }

        public Action<int, bool> OnMenuChanged
        {
            get { return _onMenuChanged; }
            set { _onMenuChanged = value; }
        }

        public void Init()
        {
            _Init();
        }

        void _Init()
        {
            DeviceOrientationManager.Init(_managerRoot);
            ApplicationBackgroundManager.Instantiate(_managerRoot);
            NoaDebuggerPrefsBehaviour.Initialize(_managerRoot);
            GlobalCoroutine.Init(_managerRoot);
            FrameTimeMeasurer.Instantiate(_managerRoot);
            EventSystemManager.Init(_managerRoot);
            Input.Initialize();

            _instance = this;
            _noaDebugger.SetActive(false);
            _noaDebuggerBg.SetActive(false);

            _allNoaDebuggerTools= _FindNoaDebuggerTools();
            _customMenuType = new List<Type>();
            _allCustomNoaDebuggerTools = _FindCustomNoaDebuggerTools();
            _initializedNoaDebuggerToolNames = new List<string>();
            _isShowCustomTab = false;

            string menuName = _toolDetailPresenter.MenuInfo().MenuName;
            _toolDetailPresenter.Init();
            _initializedNoaDebuggerToolNames.Add(menuName);

            _UpdateNoaDebuggerTools();

            _InitNoaDebuggerTools();

            DeviceOrientationManager.SetAction(NoaDebugger.DeviceOrientationKey, _OnDeviceOrientation);

            _mainView.ChangeToolListener += _ChangeToolButton;
            _mainView.OnToolDetailButton += _ShowToolDetail;
            _mainView.OnCloseButton += NoaDebuggerManager.HideDebugger;
            _mainView.OnPinButton += _PinTool;
            _mainView.OnChangeButton += _ChangeTab;

            _mainView.SetChangeButtonInteractable(_allCustomNoaDebuggerTools.Length > 0);

            var noaDebuggerSettings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings();
            _canvas.sortingOrder = noaDebuggerSettings.NoaDebuggerCanvasSortOrder;

            IsInitialized = true;
        }

        void _InitNoaDebuggerTools()
        {
            foreach (INoaDebuggerTool tool in _noaDebuggerTools)
            {
                string menuName = tool.MenuInfo().MenuName;
                if (_initializedNoaDebuggerToolNames.Contains(menuName))
                {
                    continue;
                }
                tool.Init();
                tool.InitFloatingWindow(_floatingWindowRoot.transform);
                _initializedNoaDebuggerToolNames.Add(menuName);
            }

            foreach (INoaDebuggerTool tool in _allCustomNoaDebuggerTools)
            {
                tool.Init();
            }
        }

        public void _DestroyNoaDebugger()
        {
            GlobalCoroutine.Dispose();

            _DisableNoaDebugger();
            _floatingWindowRoot.SetActive(false);

            DeviceOrientationManager.DeleteAction(NoaDebugger.DeviceOrientationKey);

            for (int i = 0; i < _filteredNoaDebuggerTools.Length; i++)
            {
                _filteredNoaDebuggerTools[i].OnToolDispose();
            }

            for (int i = 0; i < _allCustomNoaDebuggerTools.Length; i++)
            {
                _allCustomNoaDebuggerTools[i].OnToolDispose();
            }

            _toolDetailPresenter.OnToolDispose();
            NoaDebuggerSettingsManager.Dispose();
            NoaDebuggerInfoManager.Dispose();
        }

        void _OnDeviceOrientation(bool isPortrait)
        {
            _safeAreaRoot.Adjust();
            _isPortrait = isPortrait;
            if (NoaDebugger.IsShowNormalView)
            {
                _ChangeTool(_activeToolIndex);
            }
        }

        INoaDebuggerTool[] _FindNoaDebuggerTools()
        {
            var toolList = new List<INoaDebuggerTool>(_presenterRoot.childCount);
            foreach(Transform child in _presenterRoot)
            {
                var toolBase = child.GetComponent<NoaDebuggerToolBase>();
                if (toolBase == null)
                {
                    continue;
                }

                toolList.Add(toolBase as INoaDebuggerTool);
            }

            return toolList.ToArray();
        }

        INoaDebuggerTool[] _FindCustomNoaDebuggerTools()
        {
            var toolList = new List<INoaDebuggerTool>();
            List<CustomMenuInfo> customMenuInfos = GetCustomMenuInfo();
            foreach (CustomMenuInfo customMenuInfo in customMenuInfos)
            {
                Type t = Type.GetType(customMenuInfo._scriptName);
                if (t == null)
                {
                    continue;
                }

                bool isCommand = t.BaseType == typeof(NoaCustomMenuBase);
                if (!isCommand)
                {
                    continue;
                }

                var customPresenter = Activator.CreateInstance(t);
                var tool = customPresenter as INoaDebuggerTool;

                toolList.Add(tool);
                _customMenuType.Add(t);
            }

            return toolList.ToArray();
        }

        void _ChangeTool(int index, bool isMenuActive = false, bool isChangeMenu = false)
        {
            index = _GetToolIndexAfterValidation(index);

            var linker = new NoaDebuggerViewLinker()
            {
                _activeToolIndex = index,
                _targetTools = _noaDebuggerTools,
                _isCustom = _isShowCustomTab,
                _isPortrait = _isPortrait,
                _isMenuActive = isMenuActive,
            };

            if (index == TOOL_DETAIL_ACTIVE_INDEX)
            {
                linker._forceTargetTool = _toolDetailPresenter as INoaDebuggerTool;
            }

            if (isChangeMenu || index != _activeToolIndex)
            {
                _HideTool(_activeToolIndex);

                _onMenuChanged?.Invoke(index, _isShowCustomTab);
            }

            _activeToolIndex = index;
            _mainView.Show(linker);
        }

        void _HideTool(int index)
        {
            INoaDebuggerTool currentTool = null;
            if (index == TOOL_DETAIL_ACTIVE_INDEX)
            {
                currentTool = _toolDetailPresenter;
            }
            else if (index < _noaDebuggerTools.Length)
            {
                currentTool = _noaDebuggerTools[index];
            }

            if (currentTool != null)
            {
                currentTool.OnHidden();
            }
        }

        void _ChangeToolButton(int index)
        {
            _ChangeTool(index);
        }

        void _ShowToolDetail()
        {
            if (_activeToolIndex == TOOL_DETAIL_ACTIVE_INDEX)
            {
                return;
            }

            _HideTool(_activeToolIndex);

            _onMenuChanged?.Invoke(TOOL_DETAIL_ACTIVE_INDEX, _isShowCustomTab);

            _activeToolIndex = TOOL_DETAIL_ACTIVE_INDEX;
            _mainView.Show(new NoaDebuggerViewLinker()
            {
                _activeToolIndex = 0,
                _targetTools = _noaDebuggerTools,
                _isCustom = _isShowCustomTab,
                _isPortrait = _isPortrait,
                _forceTargetTool = _toolDetailPresenter,
            });
            _mainView.InitTabs();
        }

        public void CloseNoaDebugger()
        {
            _HideTool(_activeToolIndex);

            _SetLastActiveToolIndex();

            _noaDebugger.SetActive(false);
            _SetActiveBg(false);
            SetActiveFloatingWindow(true);

            foreach (Transform child in _floatingWindowRoot.transform)
            {
                child.gameObject.SetActive(true);
            }

            OnHide?.Invoke(_activeToolIndex);
        }

        void _SetLastActiveToolIndex()
        {
            int toolIndex;

            if (NoaDebuggerManager.IsError())
            {
                _SwitchShowTools(false);

                if (_isSearchingToolWithoutError)
                {
                    int loopIndex = (_lastActiveToolIndex + 1) % _noaDebuggerTools.Length;
                    toolIndex = loopIndex;
                }
                else
                {
                    int consoleLogIndex = _GetTargetToolIndex<ConsoleLogPresenter>();
                    toolIndex = consoleLogIndex;

                    if (_activeToolIndex == consoleLogIndex)
                    {
                        _isSearchingToolWithoutError = true;
                        toolIndex = -1; 
                    }
                }
            }
            else
            {
                _isSearchingToolWithoutError = false;
                toolIndex = _activeToolIndex;
            }

            _lastActiveToolIndex = toolIndex;
        }

        int _GetTargetToolIndex<T>() where T : INoaDebuggerTool
        {
            int index = -1;
            for(int i = 0; i < _noaDebuggerTools.Length; i++)
            {
                if (_noaDebuggerTools[i] is T)
                {
                    index = i;
                }
            }

            return index;
        }

        void _PinTool()
        {
            CurrentTool().TogglePin(_floatingWindowRoot.transform);
        }

        void _ChangeTab()
        {
            _HideTool(_activeToolIndex);

            _SwitchShowTools(!_isShowCustomTab);

            _activeToolIndex = 0;
            if (_noaDebuggerTools.Length == 0)
            {
                _activeToolIndex = -1;
            }

            _ChangeTool(_activeToolIndex, isMenuActive:true, isChangeMenu:true);
        }

        void _SwitchShowTools(bool isShowCustom)
        {
            _isShowCustomTab = isShowCustom;
            if (isShowCustom)
            {
                _SwitchCustomNoaDebuggerTools();
            }
            else
            {
                _SwitchNoaDebuggerTools();
            }

            _mainView.CreateMenu(_noaDebuggerTools, true);
        }

        void _UpdateNoaDebuggerTools()
        {
            NoaDebuggerSettingsManager.ValidateMenuSettings(AllPresenters().ToList());

            List<MenuInfo> menuSettings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings().MenuList;
            var sortedNoaDebuggerTools = new List<INoaDebuggerTool>();
            foreach (MenuInfo menuSetting in menuSettings)
            {
                INoaDebuggerTool target = _allNoaDebuggerTools.FirstOrDefault(tool => tool.MenuInfo().MenuName.Equals(menuSetting.Name));
                if (target != null)
                {
                    if (menuSetting.Enabled)
                    {
                        sortedNoaDebuggerTools.Add(target);
                    }
                    else
                    {
                        target.OnToolDispose();
                        continue;
                    }
                }
            }
            _filteredNoaDebuggerTools = sortedNoaDebuggerTools.ToArray();
            _noaDebuggerTools = _filteredNoaDebuggerTools;
        }

        void _SwitchNoaDebuggerTools()
        {
            _noaDebuggerTools = _filteredNoaDebuggerTools;
        }

        void _SwitchCustomNoaDebuggerTools()
        {
            _noaDebuggerTools = _allCustomNoaDebuggerTools;
        }

        public INoaDebuggerTool CurrentTool()
        {
            if (_noaDebuggerTools.Length == 0 || _activeToolIndex == TOOL_DETAIL_ACTIVE_INDEX)
            {
                return _toolDetailPresenter;
            }

            return _noaDebuggerTools[_activeToolIndex];
        }

        public static void ShowNoaDebugger(int index = 0, bool? isCustomMenu = false)
        {
            if (_instance == null)
            {
                return;
            }

            if (isCustomMenu != null)
            {
                NoaDebugger._instance._SwitchShowTools(isCustomMenu.Value);
            }

            _instance._noaDebugger.SetActive(true);

            NoaDebugger._instance._ChangeTool(index);
            _SetActiveBg(true);
            SetActiveFloatingWindow(false);
            NoaDebuggerManager.SetActiveTriggerButton(false);

            NoaDebuggerInfo info = NoaDebuggerInfoManager.GetNoaDebuggerInfo();
            if (info != null)
            {
                NoaDebugger._instance._runtimeVersionChecker.DoCheck(info);
            }

            NoaDebugger._instance.OnShow?.Invoke(NoaDebugger._instance._activeToolIndex);
        }

        int _GetToolIndexAfterValidation(int index)
        {
            if (_instance._noaDebuggerTools.Length == 0)
            {
                index = TOOL_DETAIL_ACTIVE_INDEX;
            }

            if (index != NoaDebugger.TOOL_DETAIL_ACTIVE_INDEX
                && (index < 0 || index >= NoaDebugger._instance._noaDebuggerTools.Length))
            {
                index = 0;
            }

            return index;
        }

        public static void ShowNoaDebuggerLastActiveTool()
        {
            ShowNoaDebugger(NoaDebugger._instance._lastActiveToolIndex, NoaDebugger._instance._isShowCustomTab);
        }

        void _DisableNoaDebugger()
        {
            _noaDebugger.SetActive(false);
            _noaDebuggerBg.SetActive(false);
            _dialogRoot.gameObject.SetActive(false);

            _HideTool(_activeToolIndex);
            _mainView.HideContents();

            CloseNoaDebugger();
        }

        public static void ShowToast(ToastViewLinker linker)
        {
            if (NoaDebugger._instance._toastInstance == null)
            {
                NoaDebugger._instance._toastInstance = GameObject.Instantiate(
                    NoaDebugger._instance._toastPrefab,
                    NoaDebugger._instance._toastRoot
                );
            }

            NoaDebugger._instance._toastInstance.Show(linker);
        }

        public static Transform GetDialogRoot()
        {
            return NoaDebugger._instance._dialogRoot;
        }

        public static TPresenter GetPresenter<TPresenter>() where TPresenter : INoaDebuggerTool
        {
            if (_instance == null)
            {
                return default(TPresenter);
            }

            if (NoaDebuggerManager.IsError())
            {
                return default(TPresenter);
            }

            INoaDebuggerTool[] toolsArray = NoaDebugger._instance._filteredNoaDebuggerTools;
            foreach (var tool in toolsArray)
            {
                if (tool is TPresenter presenter)
                {
                    return presenter;
                }
            }

            return default(TPresenter);
        }

        public static INoaDebuggerTool[] AllPresenters()
        {
            return NoaDebugger._instance._allNoaDebuggerTools;
        }

        public static INoaDebuggerTool[] AllCustomPresenters()
        {
            return NoaDebugger._instance._allCustomNoaDebuggerTools;
        }

        public static void OnChangeNotificationStatus<TPresenter>() where TPresenter : INoaDebuggerTool
        {
            if (NoaDebugger._instance == null)
            {
                return;
            }

            if (NoaDebugger._instance._noaDebugger.activeSelf &&
                NoaDebugger._instance.CurrentTool().GetType() != typeof(TPresenter))
            {
                NoaDebugger._instance._mainView.UpdateMenu(new NoaDebuggerViewLinker()
                {
                    _targetTools = NoaDebugger._instance._noaDebuggerTools,
                    _activeToolIndex = NoaDebugger._instance._activeToolIndex,
                    _isCustom = NoaDebugger._instance._isShowCustomTab,
                    _isPortrait = NoaDebugger._instance._isPortrait,
                });
            }
        }

        public static bool IsShowNormalView
        {
            get
            {
                if (NoaDebugger._instance == null)
                {
                    return false;
                }
                return NoaDebugger._instance._noaDebugger.activeSelf;
            }
        }

        public static bool IsErrorNotice
        {
            get
            {
                bool isError = false;
                isError |= NoaDebugger._instance._toolDetailPresenter.NotifyStatus == ToolNotificationStatus.Error;
                isError |= NoaDebugger._instance._filteredNoaDebuggerTools.FirstOrDefault(
                    tool => tool.NotifyStatus == ToolNotificationStatus.Error) != null;

                return isError;
            }
        }

        public static void SetActiveNormalView(bool isActive)
        {
            if (NoaDebugger._instance == null)
            {
                return;
            }

            _instance._noaDebugger.SetActive(isActive);
            _SetActiveBg(isActive);
        }

        public static void SetFloatingWindowVisibleSetting(bool isActive)
        {
            NoaDebugger._instance._floatingWindowVisibleSetting = isActive;
        }

        public static void SetActiveFloatingWindow(bool isActive)
        {
            if (NoaDebugger._instance == null || NoaDebugger._instance.IsWorldSpaceRenderingEnabled)
            {
                return;
            }

            if (!NoaDebugger._instance._floatingWindowVisibleSetting)
            {
                NoaDebugger._instance._floatingWindowRoot.SetActive(false);
                return;
            }

            NoaDebugger._instance._floatingWindowRoot.SetActive(isActive);
        }

        public static bool IsFloatingWindowVisible()
        {
            if (NoaDebugger._instance == null || NoaDebugger._instance.IsWorldSpaceRenderingEnabled)
            {
                return false;
            }

            if (NoaDebugger._instance._floatingWindowRoot == null || !NoaDebugger._instance._floatingWindowRoot.gameObject.activeInHierarchy)
            {
                return false;
            }

            bool isFloatingWindowVisible = false;
            foreach (Transform child in NoaDebugger._instance._floatingWindowRoot.transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    isFloatingWindowVisible = true;
                    break;
                }
            }

            return isFloatingWindowVisible;
        }

        static void _SetActiveBg(bool isActive)
        {
            if (NoaDebugger._instance == null || NoaDebugger._instance.IsWorldSpaceRenderingEnabled)
            {
                return;
            }

            NoaDebugger._instance._noaDebuggerBg.SetActive(isActive);
        }

        List<CustomMenuInfo> GetCustomMenuInfo()
        {
            List<CustomMenuInfo> allCustomNoaDebuggerTools = NoaDebuggerSettingsManager.GetNoaDebuggerSettings().CustomMenuList;
            return allCustomNoaDebuggerTools;
        }

        public static bool IsInternalError(string stackTrace)
        {
            const RegexOptions options = RegexOptions.Multiline;

            return Regex.IsMatch(stackTrace, NoaDebuggerDefine.InternalErrorStacktraceRegexPattern, options)
                   && !Regex.IsMatch(stackTrace, NoaDebuggerDefine.DebugCommandInvocationStacktraceRegexPattern, options);
        }

        public static bool ContainsCustomClassNameByText(string text)
        {
            if (text.Contains(nameof(NoaCustomMenuBase)))
            {
                return true;
            }

            foreach (Type customMenuType in NoaDebugger._instance._customMenuType)
            {
                if (text.Contains(customMenuType.Name))
                {
                    return true;
                }
            }

            return false;
        }

        public void EnableWorldSpaceRendering(Camera worldCamera = null)
        {
            _isActiveFloatingWindow = _floatingWindowRoot.activeSelf;
            SetActiveFloatingWindow(false);
            _SetActiveBg(false);

            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.worldCamera = worldCamera;
            _rectTransform.sizeDelta = _canvasScaler.referenceResolution;

            _OnDeviceOrientation(false);
            DeviceOrientationManager.DeleteAction(NoaDebugger.DeviceOrientationKey);
        }

        public void DisableWorldSpaceRendering()
        {
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.worldCamera = null;

            SetActiveFloatingWindow(_isActiveFloatingWindow);
            _SetActiveBg(_noaDebugger.activeSelf);

            _OnDeviceOrientation(DeviceOrientationManager.IsPortrait);

            if (!DeviceOrientationManager.ContainsKey(NoaDebugger.DeviceOrientationKey))
            {
                DeviceOrientationManager.SetAction(NoaDebugger.DeviceOrientationKey, _OnDeviceOrientation);
            }
        }

        public void Dispose()
        {
            _mainView = default;
            _presenterRoot = default;
            _allNoaDebuggerTools = default;
            _filteredNoaDebuggerTools = default;
            _noaDebuggerTools = default;
            _initializedNoaDebuggerToolNames = default;
            _toolDetailPresenter = default;
            _allCustomNoaDebuggerTools = default;
            _customMenuType = default;
            _safeAreaRoot = default;
            _floatingWindowRoot = default;
            _noaDebugger = default;
            _noaDebuggerBg = default;
            _dialogRoot = default;
            _toastRoot = default;
            _toastPrefab = default;
            _toastInstance = default;
            _runtimeVersionChecker = default;
            _managerRoot = default;
            _rectTransform = default;
            _canvas = default;
            _canvasScaler = default;
            _onMenuChanged = default;
            OnShow = default;
            OnHide = default;
            NoaDebugger._instance = null;
        }
    }
}
