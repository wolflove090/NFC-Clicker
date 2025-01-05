using TMPro;
using UnityEngine;

namespace NoaDebugger
{
    /// <summary>
    /// Operations of NoaDebugger can be done through this class.
    /// </summary>
    public static class NoaDebug
    {
        /// <summary>
        /// You can set callback events when displaying the tool.
        /// </summary>
        public static System.Action<int> OnShow
        {
            get => NoaDebuggerManager.OnShow;
            set => NoaDebuggerManager.OnShow = value;
        }

        /// <summary>
        /// You can set callback events when closing the tool.
        /// </summary>
        public static System.Action<int> OnHide
        {
            get => NoaDebuggerManager.OnHide;
            set => NoaDebuggerManager.OnHide = value;
        }

        /// <summary>
        /// You can set callback events when switching menus.
        /// </summary>
        public static System.Action<int, bool> OnMenuChanged
        {
            get { return NoaDebuggerManager.OnMenuChanged; }
            set { NoaDebuggerManager.OnMenuChanged = value; }
        }

        /// <summary>
        /// Returns Transform at the top level of the tool.
        /// </summary>
        public static Transform RootTransform => NoaDebuggerManager.RootTransform;

        /// <summary>
        /// Returns true if the tool is initialized.
        /// </summary>
        public static bool IsInitialized => NoaDebuggerManager.IsDebuggerInitialized();

        /// <summary>
        /// Returns true if NOA Debugger is shown on the world coordinate.
        /// </summary>
        public static bool IsWorldSpaceRenderingEnabled => NoaDebuggerManager.IsWorldSpaceRenderingEnabled;

        /// <summary>
        /// Returns true if the tool is visible.
        /// </summary>
        public static bool IsDebuggerVisible => NoaDebuggerManager.IsDebuggerVisible;

        /// <summary>
        /// Returns true if the trigger button is visible.
        /// </summary>
        public static bool IsTriggerButtonVisible => NoaDebuggerManager.IsTriggerButtonVisible;

        /// <summary>
        /// Returns true if the dedicated window is visible.
        /// </summary>
        public static bool IsFloatingWindowVisible => NoaDebuggerManager.IsFloatingWindowVisible;

        /// <summary>
        /// Initialize the tool.
        /// </summary>
        public static void Initialize()
        {
            NoaDebuggerManager.InitializeDebugger();
        }

        /// <summary>
        /// Start up the tool.
        /// Opens the last displayed tool.
        /// </summary>
        public static void Show()
        {
            NoaDebuggerManager.ShowDebugger();
        }

        /// <summary>
        /// Start up the tool.
        /// Opens the tab at the specified position by specifying the index number.
        /// If index is null, the last displayed tool will open.
        /// </summary>
        /// <param name="index">index number</param>
        /// <param name="isCustomMenu">Whether it is a CustomMenu</param>
        public static void Show(int? index, bool isCustomMenu = false)
        {
            NoaDebuggerManager.ShowDebugger(index, isCustomMenu);
        }

        /// <summary>
        /// Close the tool.
        /// </summary>
        public static void Hide()
        {
            NoaDebuggerManager.HideDebugger();
        }

        /// <summary>
        /// You can set the tool screen display / non-display.
        /// </summary>
        /// <param name="isActive">display / non-display</param>
        public static void SetDebuggerActive(bool isActive)
        {
            NoaDebuggerManager.SetDebuggerActive(isActive);
        }

        /// <summary>
        /// You can set the dedicated Window of the tool display / non-display.
        /// </summary>
        /// <param name="isActive">display / non-display</param>
        public static void SetFloatingWindowActive(bool isActive)
        {
            NoaDebuggerManager.SetFloatingWindowActive(isActive);
        }

        /// <summary>
        /// You can set the display/tracking of the tool's trigger button.
        /// </summary>
        /// <param name="isActive">display / non-display</param>
        public static void SetTriggerButtonActive(bool isActive)
        {
            NoaDebuggerManager.SetTriggerButtonActive(isActive);
        }

        /// <summary>
        /// Configure the fonts used for the tool.
        /// </summary>
        /// <param name="fontAsset">Font asset to use.</param>
        /// <param name="fontMaterial">Material to use. If omitted, the default material of the specified font asset will be applied.</param>
        /// <param name="fontSizeRate">Font size Multiplier. If omitted, it will be displayed at 1:1 scale.</param>
        public static void SetFont(TMP_FontAsset fontAsset, Material fontMaterial = null, float fontSizeRate = 1.0f)
        {
            NoaDebuggerManager.SetFont(fontAsset, fontMaterial, fontSizeRate);
        }

        /// <summary>
        /// Display tools on world coordinates.
        /// </summary>
        /// <param name="worldCamera">Specifies the camera to be used for rendering and event detection of the NOA Debugger UI. If null is specified, a camera with the MainCamera tag will be used.</param>
        public static void EnableWorldSpaceRendering(Camera worldCamera = null)
        {
            NoaDebuggerManager.EnableWorldSpaceRendering(worldCamera);
        }

        /// <summary>
        /// Display the tool on the 2D screen coordinates.
        /// </summary>
        public static void DisableWorldSpaceRendering()
        {
            NoaDebuggerManager.DisableWorldSpaceRendering();
        }

        /// <summary>
        /// Destroys the tool.
        /// </summary>
        public static void Destroy()
        {
            NoaDebuggerManager.DestroyDebugger();
        }

        /// <summary>
        /// Captures a screenshot and returns the image data through a callback function.
        /// </summary>
        /// <param name="callback">The callback function which will receive the image data as a byte array.</param>
        public static void TakeScreenshot(System.Action<byte[]> callback)
        {
            NoaDebuggerManager.TakeScreenshot(callback);
        }
    }
}
