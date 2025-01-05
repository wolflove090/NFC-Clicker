using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace NoaDebugger
{
    sealed class NoaDebuggerManager
    {
        static readonly string PrefabPath = "Prefabs/NoaDebuggerRoot";
        static readonly string ErrorObserverKey = "NoaDebuggerErrorObserver";
        static readonly string MonitorFontOnUpdateKey = "NoaDebuggerManagerMonitorFontOnUpdate";

        static NoaDebugger _noaDebugger;
        static NoaDebuggerManager _instance;

        bool _buttonVisibleSetting = true;
        bool _isError;

        bool _isInitializedOnce = false;

        NoaDebuggerButton _noaDebuggerButton;
        bool _isDebuggerButtonActive;

        Action<int> _onShow;

        Action<int> _onHide;

        Action<int, bool> _onMenuChanged;

        public static Action<int> OnShow
        {
            get => NoaDebuggerManager._instance._onShow;
            set => _instance._onShow = value;
        }

        public static Action<int> OnHide
        {
            get => NoaDebuggerManager._instance._onHide;
            set => _instance._onHide = value;
        }

        public static Action<int, bool> OnMenuChanged
        {
            get => NoaDebuggerManager._instance._onMenuChanged;
            set => _instance._onMenuChanged = value;
        }

        public static Transform RootTransform =>
            NoaDebuggerManager.IsDebuggerInitialized()
                ? NoaDebuggerManager._noaDebugger.transform
                : null;

        public static bool IsWorldSpaceRenderingEnabled =>
            NoaDebuggerManager.IsDebuggerInitialized() && NoaDebuggerManager._noaDebugger.IsWorldSpaceRenderingEnabled;

        public static bool IsDebuggerVisible =>
            NoaDebuggerManager.IsDebuggerInitialized() && NoaDebuggerManager._noaDebugger.IsDebuggerVisible;

        public static bool IsTriggerButtonVisible =>
            NoaDebuggerManager._instance._noaDebuggerButton != null &&
            NoaDebuggerManager._instance._noaDebuggerButton.IsActive();

        public static bool IsFloatingWindowVisible => NoaDebugger.IsFloatingWindowVisible();

#if NOA_DEBUGGER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void _AutoInitialize()
        {
            NoaDebuggerManager._instance = new NoaDebuggerManager();
            NoaDebuggerSettings settings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings();
            NoaDebuggerText.Init(settings);

            if (settings.AutoInitialize)
            {
                InitializeDebugger();
            }
        }
#endif
        static void _Instantiate()
        {
            if (NoaDebuggerManager.IsDebuggerInitialized())
            {
                return;
            }

            var prefab = Resources.Load<GameObject>(PrefabPath);
            GameObject obj = GameObject.Instantiate(prefab);
            obj.name = NoaDebuggerDefine.RootObjectName;
            GameObject.DontDestroyOnLoad(obj);
            NoaDebuggerManager._noaDebugger = obj.GetComponent<NoaDebugger>();

            obj.AddComponent<NoaDebuggerDestroyReceiver>();

            NoaDebuggerManager._instance._Init();
            NoaDebuggerManager._instance._isInitializedOnce = true;
        }

        void _Init()
        {
            NoaDebuggerManager._noaDebugger.Init();

            NoaDebuggerManager._noaDebugger.OnShow = NoaDebuggerManager.RunOnShow;
            NoaDebuggerManager._noaDebugger.OnHide = NoaDebuggerManager.RunOnHide;
            NoaDebuggerManager._noaDebugger.OnMenuChanged = NoaDebuggerManager.RunOnMenuChanged;

            Application.logMessageReceived += _HandleException;

            UpdateManager.SetAction(NoaDebuggerManager.ErrorObserverKey, _ErrorObserver);

            Transform buttonObj = NoaDebuggerManager._noaDebugger.transform.Find("Root/NoaDebuggerButton");
            _noaDebuggerButton = buttonObj.GetComponent<NoaDebuggerButton>();
            _noaDebuggerButton.Init();
        }

        void _ErrorObserver()
        {
            if (_isError)
            {
                NoaDebuggerManager._CloseNoaDebugger();

                var linker = new ToastViewLinker
                {
                    _label = NoaDebuggerDefine.ShowErrorText,
                };

                NoaDebugger.ShowToast(linker);
                _isError = false;
            }
        }

        void _HandleException(string logString, string stackTrace, UnityEngine.LogType type)
        {
            if (type != UnityEngine.LogType.Exception)
            {
                return;
            }

            if (!NoaDebugger.IsInternalError(stackTrace))
            {
                return;
            }

            _isError = true;

            LogModel.CollectNoaDebuggerErrorLog(logString, stackTrace);
        }

        static void _CloseNoaDebugger()
        {
            SetActiveTriggerButton(true);
            NoaDebuggerManager._noaDebugger.CloseNoaDebugger();
        }

        public static void OnChangeNotificationStatus<TPresenter>(ToolNotificationStatus notifyStatus)
            where TPresenter : INoaDebuggerTool
        {
            if (notifyStatus == ToolNotificationStatus.Error && _instance._noaDebuggerButton != null)
            {
                _instance._noaDebuggerButton.PlayOnErrorAnimation();
            }

            NoaDebugger.OnChangeNotificationStatus<TPresenter>();
        }

        public static void DetectError()
        {
            if (_instance == null)
            {
                return;
            }

            _instance._isError = true;
        }

        public static void InitializeDebugger()
        {
            if (NoaDebuggerManager.IsDebuggerInitialized())
            {
                return;
            }

            GameObject obj = new GameObject("InitializeNoaDebuggerBehaviour");
            obj.AddComponent<InitializeNoaDebuggerBehaviour>();
        }

        public static void ShowDebugger(int? index = null, bool? isCustomMenu = null)
        {
            if (NoaDebugger.IsShowNormalView)
            {
                return;
            }

            bool isInit = NoaDebuggerManager.IsDebuggerInitialized();

            if (!isInit)
            {
                NoaDebuggerManager.InitializeDebugger();
            }

            if (NoaDebuggerManager._instance._isError)
            {
                return;
            }

            GlobalCoroutine.Run(NoaDebuggerManager._ShowDebugger(index, isCustomMenu, isInit));
        }

        static IEnumerator _ShowDebugger(int? index, bool? isCustomMenu, bool isInit)
        {
            if (!isInit)
            {
                yield return null;
            }

            if (index != null && isCustomMenu != null)
            {
                NoaDebugger.ShowNoaDebugger(index.Value, isCustomMenu.Value);
            }
            else
            {
                NoaDebugger.ShowNoaDebuggerLastActiveTool();
            }
        }

        public static void HideDebugger()
        {
            if (NoaDebuggerManager._noaDebugger == null)
            {
                return;
            }

            NoaDebuggerManager._CloseNoaDebugger();
        }

        public static void SetDebuggerActive(bool isActive)
        {
            NoaDebugger.SetActiveNormalView(isActive);
        }

        public static void SetFloatingWindowActive(bool isActive)
        {
            NoaDebugger.SetFloatingWindowVisibleSetting(isActive);
            NoaDebugger.SetActiveFloatingWindow(isActive);
        }

        public static void SetActiveTriggerButton(bool isActive)
        {
            if (NoaDebuggerManager._instance == null
                || NoaDebuggerManager._instance._noaDebuggerButton == null
                || NoaDebuggerManager._noaDebugger.IsWorldSpaceRenderingEnabled)
            {
                return;
            }

            if (!NoaDebuggerManager._instance._buttonVisibleSetting && !NoaDebuggerManager._instance._isError)
            {
                NoaDebuggerManager._instance._noaDebuggerButton.SetActive(false);

                return;
            }

            NoaDebuggerManager._instance._noaDebuggerButton.SetActive(isActive);
        }

        public static void SetTriggerButtonActive(bool isActive)
        {
            NoaDebuggerManager._instance._buttonVisibleSetting = isActive;
            SetActiveTriggerButton(isActive);
        }

        public static void SetFont(TMP_FontAsset fontAsset, Material fontMaterial, float fontSizeRate)
        {
            if (fontAsset != null && fontMaterial == null)
            {
                fontMaterial = fontAsset.material;
            }
            NoaDebuggerText.ChangeFont(fontAsset, fontMaterial, fontSizeRate);
            NoaDebuggerManager._ApplyFontToInstantiatedObjects();

            UpdateManager.AddOrOverwriteAction(NoaDebuggerManager.MonitorFontOnUpdateKey, NoaDebuggerManager._MonitorFontOnUpdate);
        }

        static void _MonitorFontOnUpdate()
        {
            if (!NoaDebuggerText.HasFontAsset)
            {
                NoaDebuggerText.ResetFont();
                NoaDebuggerManager._ApplyFontToInstantiatedObjects();

                UpdateManager.DeleteAction(NoaDebuggerManager.MonitorFontOnUpdateKey);
            }
        }

        static void _ApplyFontToInstantiatedObjects()
        {
            if (NoaDebuggerManager._noaDebugger != null)
            {
                NoaDebuggerText[] allTexts = NoaDebuggerManager._noaDebugger.GetComponentsInChildren<NoaDebuggerText>(includeInactive:true);

                foreach (NoaDebuggerText text in allTexts)
                {
                    text.ApplyFont(isForce:true);
                }
            }
        }

        public static void EnableWorldSpaceRendering(Camera worldCamera = null)
        {
            if (!NoaDebuggerManager.IsDebuggerInitialized())
            {
                return;
            }

            NoaDebuggerManager._instance._isDebuggerButtonActive =
                NoaDebuggerManager._instance._noaDebuggerButton.gameObject.activeSelf;

            SetActiveTriggerButton(false);

            NoaDebuggerManager._noaDebugger.EnableWorldSpaceRendering(worldCamera);
        }

        public static void DisableWorldSpaceRendering()
        {
            NoaDebuggerManager._noaDebugger.DisableWorldSpaceRendering();

            SetActiveTriggerButton(NoaDebuggerManager._instance._isDebuggerButtonActive);
        }

        public static void DestroyDebugger()
        {
            if (NoaDebuggerManager.IsDebuggerInitialized())
            {
                UpdateManager.DeleteAction(NoaDebuggerManager.ErrorObserverKey);
                OnHide = null;

                NoaDebuggerManager._noaDebugger._DestroyNoaDebugger();
                Application.logMessageReceived -= _instance._HandleException;

                GameObject.Destroy(NoaDebuggerManager._noaDebugger.gameObject);
                NoaDebuggerManager._instance._noaDebuggerButton.Dispose();
                NoaDebuggerManager._instance._noaDebuggerButton = null;
                NoaDebuggerManager._noaDebugger.Dispose();
                NoaDebuggerManager._noaDebugger = null;
            }
        }

        public static bool IsDebuggerInitialized()
        {
            return NoaDebuggerManager._noaDebugger != null && NoaDebuggerManager._noaDebugger.IsInitialized;
        }

        public static bool IsError()
        {
            return NoaDebuggerManager._instance._isError;
        }

        static void RunOnShow(int index)
        {
            OnShow?.Invoke(index);
        }

        static void RunOnHide(int index)
        {
            OnHide?.Invoke(index);
        }

        static void RunOnMenuChanged(int index, bool isCustomMenu)
        {
            OnMenuChanged?.Invoke(index, isCustomMenu);
        }

        public static void TakeScreenshot(Action<byte[]> callback)
        {
            GlobalCoroutine.Run(NoaDebuggerManager.TakeScreenshotCoroutine(callback));
        }

        static IEnumerator TakeScreenshotCoroutine(Action<byte[]> callback)
        {
            yield return new WaitForEndOfFrame();
            callback?.Invoke(NoaDebuggerManager.TakeScreenshotInternal());
        }

        static byte[] TakeScreenshotInternal()
        {
            var screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenShot.Apply();
            var data = screenShot.EncodeToPNG();
            GameObject.Destroy(screenShot);
            NoaDebugger.ShowToast(new ToastViewLinker {_label = NoaDebuggerDefine.TakeScreenshotText});
            return data;
        }

        public static void RegisterUIElement(INoaUIElement element)
        {
            if (!NoaDebuggerManager.IsDebuggerInitialized())
            {
                return;
            }

            NoaDebuggerManager._noaDebugger.NoaUIElementManager.RegisterUIElement(element);
        }

        public static void UnregisterUIElement(string key)
        {
            if (!NoaDebuggerManager.IsDebuggerInitialized())
            {
                return;
            }

            NoaDebuggerManager._noaDebugger.NoaUIElementManager.UnregisterUIElement(key);
        }

        public static void UnregisterAllUIElements()
        {
            if (!NoaDebuggerManager.IsDebuggerInitialized())
            {
                return;
            }

            NoaDebuggerManager._noaDebugger.NoaUIElementManager.UnregisterAllUIElements();
        }

        public static bool IsUIElementRegistered(string key = null)
        {
            return NoaDebuggerManager._noaDebugger.NoaUIElementManager.IsUIElementRegistered(key);
        }

        public static void SetUIElementVisibility(string key, bool visible)
        {
            NoaDebuggerManager._noaDebugger.NoaUIElementManager.SetUIElementVisibility(key, visible);
        }

        public static void SetAllUIElementsVisibility(bool visible)
        {
            NoaDebuggerManager._noaDebugger.NoaUIElementManager.SetAllUIElementsVisibility(visible);
        }

        public static bool IsUIElementVisible(string key = null)
        {
            return NoaDebuggerManager._noaDebugger.NoaUIElementManager.IsUIElementVisible(key);
        }

        class InitializeNoaDebuggerBehaviour : MonoBehaviour
        {
            void Awake()
            {
                StartCoroutine(_Init());
            }

            IEnumerator _Init()
            {
                if (NoaDebuggerManager._instance._isInitializedOnce)
                {
                    yield return null;
                }

                NoaDebuggerManager._Instantiate();
                GameObject.Destroy(gameObject);
            }
        }

        class NoaDebuggerDestroyReceiver : MonoBehaviour
        {
            void OnDestroy()
            {
                DestroyDebugger();
            }
        }
    }
}
