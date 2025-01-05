using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Class that provides a custom menu.
    /// </summary>
    public abstract class NoaCustomMenuBase : INoaDebuggerTool
    {
        GameObject _viewObj;
        GameObject _mainView;

        /// <summary>
        /// Fixed to None as notification functionality will not be used.
        /// </summary>
        ToolNotificationStatus INoaDebuggerTool.NotifyStatus => ToolNotificationStatus.None;

        /// <summary>
        /// Initialization process.
        /// </summary>
        void INoaDebuggerTool.Init()
        {
            _viewObj = Resources.Load<GameObject>(ViewPrefabPath);
            OnInitialize();
        }

        /// <summary>
        /// Menu information class of the provider class.
        /// </summary>
        internal class CustomMenuInfo : IMenuInfo
        {
            public string Name => MenuName;
            public string MenuName { get; }

            public int SortNo => NoaDebuggerDefine.CUSTOM_MENU_SORT_NO;

            /// <summary>
            /// Initialization process.
            /// </summary>
            /// <param name="menuName">Menu display name.</param>
            public CustomMenuInfo(string menuName)
            {
                MenuName = menuName;
            }

            /// <summary>
            /// Initialization process (Made becuase Default was needed. Not used)
            /// It's necessary as a setup because it's processed in NoaDebuggerSettings.GetIMenuInfoList
            /// </summary>
            public CustomMenuInfo() { }
        }

        /// <summary>
        /// Holds the menu information.
        /// </summary>
        CustomMenuInfo _customMenuInfo;

        /// <summary>
        /// Menu information.
        /// </summary>
        IMenuInfo INoaDebuggerTool.MenuInfo()
        {
            return _customMenuInfo ??= new CustomMenuInfo(MenuName);
        }

        /// <summary>
        /// Refer to the summary in the inherited parent.
        /// </summary>
        void INoaDebuggerTool.ShowView(Transform parent)
        {
            if (_viewObj == null)
            {
                Debug.LogWarning($"Prefab Not Found:{this.GetType().Name}:{ViewPrefabPath}");
                return;
            }

            if (_mainView == null)
            {
                _mainView = Object.Instantiate(_viewObj, parent);
            }

            _ShowView(_mainView);
        }

        /// <summary>
        /// Display process for the specified view.
        /// </summary>
        /// <param name="view">Specify the view to be displayed.</param>
        void _ShowView(GameObject view)
        {
            _mainView.gameObject.SetActive(true);
            OnShow(view);
        }

        /// <summary>
        /// Retrieve the display information for the pin button.
        /// </summary>
        /// <return>Returns the display information for the pin button.</return>
        ToolPinStatus INoaDebuggerTool.GetPinStatus()
        {
            return ToolPinStatus.None;
        }

        /// <summary>
        /// Change the state of the pin button.
        /// </summary>
        void INoaDebuggerTool.TogglePin(Transform parent)
        {
        }

        /// <summary>
        /// Initial generation process of the dedicated window.
        /// </summary>
        void INoaDebuggerTool.InitFloatingWindow(Transform parent)
        {
        }

        /// <summary>
        /// Alignment process of the menu UI.
        /// </summary>
        void INoaDebuggerTool.AlignmentUI(bool isReverse)
        {
        }

        /// <summary>
        /// Hide the view.
        /// </summary>
        void INoaDebuggerTool.OnHidden()
        {
            OnHide();

            if (_mainView != null)
            {
                _mainView.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Refer to the summary in the inherited parent.
        /// </summary>
        void INoaDebuggerTool.OnToolDispose()
        {
            OnDispose();

            if (_viewObj == null)
            {
                return;
            }

            _viewObj = null;
        }

        /// <summary>
        /// Callback when the tool is displayed.
        /// </summary>
        /// <param name="view">The GameObject that will be displayed is entered.</param>
        protected virtual void OnShow(GameObject view) { }

        /// <summary>
        /// Callback when the tool is Hide.
        /// </summary>
        protected virtual void OnHide() { }

        /// <summary>
        /// Initialization process.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Disabling process.
        /// </summary>
        protected virtual void OnDispose() { }

        /// <summary>
        /// Path of the Prefab to be displayed.
        /// </summary>
        protected abstract string ViewPrefabPath { get; }

        /// <summary>
        /// Name of the menu to be displayed.
        /// </summary>
        protected abstract string MenuName { get; }
    }
}
