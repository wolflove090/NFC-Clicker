using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NoaDebugger
{
    sealed class FloatingWindowPresenter<TVIEW, TLINKER>
        where TVIEW : FloatingWindowViewBase<TLINKER>
        where TLINKER : ViewLinkerBase
    {
        static readonly string KeyPrefix = "FloatingWindowOnDeviceOrientation";
        readonly string _deviceOrientationKey;
        readonly string _prefsKey;

        readonly TVIEW _landscapeFloatingWindowPrefab;
        readonly TVIEW _portraitFloatingWindowPrefab;
        TVIEW _floatingWindow;

        FloatingWindowInfo _floatingWindowInfo;

        string _toolName;

        public bool IsActive
        {
            get
            {
                return _floatingWindowInfo._isActive;
            }
        }

        public event UnityAction<TVIEW> OnInitAction;

        public FloatingWindowPresenter(TVIEW windowPrefab, string prefsKey, string toolName)
        {
            _deviceOrientationKey = $"{FloatingWindowPresenter<TVIEW, TLINKER>.KeyPrefix}-{GetHashCode()}";
            _prefsKey = prefsKey;
            _landscapeFloatingWindowPrefab = windowPrefab;
            _portraitFloatingWindowPrefab = windowPrefab; 
            _toolName = toolName;

            _floatingWindowInfo = new FloatingWindowInfo(_prefsKey);
            DeviceOrientationManager.SetAction(_deviceOrientationKey, _OnDeviceOrientation);
        }

        public FloatingWindowPresenter(TVIEW landscapeWindowPrefab, TVIEW portraitWindowPrefab, string prefsKey, string toolName) : this(landscapeWindowPrefab, prefsKey, toolName)
        {
            _portraitFloatingWindowPrefab = portraitWindowPrefab;
        }

        public GameObject InstantiateWindow(Transform parent)
        {
            if (_floatingWindow == null)
            {
                Vector2 windowScreenPos =
                    DeviceOrientationManager.IsPortrait ? _floatingWindowInfo.ScreenPositionPortrait : _floatingWindowInfo.ScreenPositionLandscape;
                TVIEW targetPrefab = DeviceOrientationManager.IsPortrait
                    ? _portraitFloatingWindowPrefab
                    : _landscapeFloatingWindowPrefab;

                _floatingWindow = GameObject.Instantiate(targetPrefab, parent);
                _floatingWindow.SetScreenPos(windowScreenPos);
                _floatingWindow.SetState(_floatingWindowInfo._isShowDefaultInfo);
                _floatingWindow.ToolName = _toolName;
                _floatingWindow.OnToggleStateChange += _OnToggleStateChangeToFloatingWindow;
                _floatingWindow.OnDrag += _OnDragFloatingWindow;
                _floatingWindow.OnDragEnd += _OnDragEndToFloatingWindow;
                _floatingWindow.OnClose += _OnCloseToFloatingWindow;
                if (_floatingWindow is DebugCommandFloatingWindowView)
                {
                    DebugCommandFloatingWindowView debugCommandFloatingWindowView = _floatingWindow as DebugCommandFloatingWindowView;
                    if (debugCommandFloatingWindowView is not null)
                    {
                        debugCommandFloatingWindowView.OnTapRefreshButton += _OnRefreshFloatingWindow;
                        debugCommandFloatingWindowView.OnLongTapRefreshButton += _OnLongTapRefreshButton;
                    }
                }

                OnInitAction?.Invoke(_floatingWindow);
            }

            return _floatingWindow.gameObject;
        }

        public void ShowWindowView(TLINKER linker)
        {
            if (_floatingWindow != null)
            {
                _floatingWindow.Show(linker);
            }
        }

        public GameObject ToggleActive(Transform parent)
        {
            _floatingWindowInfo._isActive = !_floatingWindowInfo._isActive;

            var isWindowDrawing = _floatingWindowInfo._isActive;
            GameObject result = null;
            if (isWindowDrawing)
            {
                result =  InstantiateWindow(parent);
            }
            else
            {
                DestroyWindow();
            }

            return result;
        }

        void _OnToggleStateChangeToFloatingWindow(bool isShowDefault)
        {
            _floatingWindowInfo._isShowDefaultInfo = isShowDefault;
        }

        void _OnDragFloatingWindow(Vector2 screenPos)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        void _OnDragEndToFloatingWindow(Vector2 screenPos)
        {
            if (DeviceOrientationManager.IsPortrait)
            {
                _floatingWindowInfo.ScreenPositionPortrait = screenPos;
            }
            else
            {
                _floatingWindowInfo.ScreenPositionLandscape = screenPos;
            }

            EventSystem.current.SetSelectedGameObject(null);
        }

        void _OnCloseToFloatingWindow()
        {
            _floatingWindow.gameObject.SetActive(false);
        }

        void _OnRefreshFloatingWindow(bool isAutoRefresh, DebugCommandFloatingWindowViewLinker linker)
        {
            if (isAutoRefresh)
            {
                linker._onLongTapRefreshButton?.Invoke(false);
            }
            else
            {
                DebugCommandRegister.RefreshProperty();
            }
        }

        void _OnLongTapRefreshButton(DebugCommandFloatingWindowViewLinker linker)
        {
            linker?._onLongTapRefreshButton?.Invoke(true);
        }

        void _OnDeviceOrientation(bool isPortrait)
        {
            if (_floatingWindow == null)
            {
                return;
            }

            bool windowActive = _floatingWindow.gameObject.activeSelf;
            Transform parent = _floatingWindow.transform.parent;
            GameObject.Destroy(_floatingWindow.gameObject);
            _floatingWindow = null;
            InstantiateWindow(parent);
            _floatingWindow.gameObject.SetActive(windowActive);
        }

        void DestroyWindow()
        {
            if (_floatingWindow != null)
            {
                GameObject.Destroy(_floatingWindow.gameObject);
                _floatingWindow = null;
                _floatingWindowInfo.Reset();
            }
        }
    }
}
