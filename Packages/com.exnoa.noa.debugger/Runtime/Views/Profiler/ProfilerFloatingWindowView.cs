using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace NoaDebugger
{
    [RequireComponent(typeof(ProfilerDrawerComponent))]
    sealed class ProfilerFloatingWindowView : FloatingWindowViewBase<ProfilerViewLinker>
    {
        [Header("SmallWindow")]
        [SerializeField]
        TextMeshProUGUI _currentFpsSmall;
        [SerializeField]
        TextMeshProUGUI _currentMemorySmall;

        [Header("DefaultWindow")]
        [SerializeField]
        GameObject _memoryMin;
        [SerializeField]
        GameObject _thermalMin;

        [SerializeField, Header("Drawer")]
        ProfilerDrawerComponent _viewer;

        [Header("FPS")]
        [SerializeField]
        ToggleButtonBase _fpsProfilingButton;

        [Header("FrameTime")]
        [SerializeField]
        ToggleButtonBase _frameTimeProfilingButton;

        [SerializeField]
        GameObject _frameTimeChartRoot;

        [Header("Memory")]
        [SerializeField]
        ToggleButtonBase _memoryProfilingButton;

        [SerializeField]
        ToggleButtonBase _memoryGraphButton;

        [SerializeField]
        GameObject _memoryChartRoot;

        [SerializeField]
        NoaDebuggerDisableButton _gcCollectButton;

        [Header("Rendering")]
        [SerializeField]
        ToggleButtonBase _renderingProfilingButton;

        [SerializeField]
        ToggleButtonBase _renderingGraphButton;

        [SerializeField]
        GameObject _renderingChartRoot;

        [SerializeField]
        ToggleButtonGroup _renderingValueOptionGroup;

        [SerializeField]
        ToggleButtonBase _renderingValueOptionSetPassCalls;

        [SerializeField]
        ToggleButtonBase _renderingValueOptionDrawCalls;

        [SerializeField]
        ToggleButtonBase _renderingValueOptionBatches;

        [SerializeField]
        ToggleButtonBase _renderingValueOptionTriangles;

        [SerializeField]
        ToggleButtonBase _renderingValueOptionVertices;

        public event UnityAction<bool> OnFpsProfilingStateChanged;
        public event UnityAction<bool> OnFrameTimeProfilingStateChanged;
        public event UnityAction<bool> OnMemoryProfilingStateChanged;
        public event UnityAction<bool> OnMemoryGraphStateChanged;
        public event UnityAction OnGcCollectExecuted;
        public event UnityAction<bool> OnRenderingProfilingStateChanged;
        public event UnityAction<bool> OnRenderingGraphStateChanged;
        public event UnityAction<RenderingGraphTarget> OnRenderingGraphSelected;

        protected override void _Init()
        {
            base._Init();
            Assert.IsNotNull(_currentFpsSmall);
            Assert.IsNotNull(_currentMemorySmall);
            Assert.IsNotNull(_memoryMin);
            Assert.IsNotNull(_thermalMin);
            Assert.IsNotNull(_viewer);
            Assert.IsNotNull(_fpsProfilingButton);
            Assert.IsNotNull(_frameTimeProfilingButton);
            Assert.IsNotNull(_frameTimeChartRoot);
            Assert.IsNotNull(_memoryProfilingButton);
            Assert.IsNotNull(_memoryGraphButton);
            Assert.IsNotNull(_memoryChartRoot);
            Assert.IsNotNull(_gcCollectButton);
            Assert.IsNotNull(_renderingProfilingButton);
            Assert.IsNotNull(_renderingGraphButton);
            Assert.IsNotNull(_renderingChartRoot);
            Assert.IsNotNull(_renderingValueOptionGroup);
            Assert.IsNotNull(_renderingValueOptionSetPassCalls);
            Assert.IsNotNull(_renderingValueOptionDrawCalls);
            Assert.IsNotNull(_renderingValueOptionBatches);
            Assert.IsNotNull(_renderingValueOptionTriangles);
            Assert.IsNotNull(_renderingValueOptionVertices);
        }

        protected override void _OnStart()
        {
            base._OnStart();

            _frameTimeChartRoot.SetActive(_frameTimeProfilingButton.IsOn);
            _memoryChartRoot.SetActive(_memoryGraphButton.IsOn && _memoryGraphButton.Interactable);
            _renderingChartRoot.SetActive(_renderingGraphButton.IsOn);

            _renderingValueOptionSetPassCalls._onClick.AddListener(_OnClickRenderingSetPassCalls);
            _renderingValueOptionDrawCalls._onClick.AddListener(_OnClickRenderingDrawCalls);
            _renderingValueOptionBatches._onClick.AddListener(_OnClickRenderingBatches);
            _renderingValueOptionTriangles._onClick.AddListener(_OnClickRenderingTriangles);
            _renderingValueOptionVertices._onClick.AddListener(_OnClickRenderingVertices);
        }

        protected override void _OnShow(ProfilerViewLinker linker)
        {
            if (linker._fpsInfo != null)
            {
                _OnShowFps(linker._fpsInfo);
            }

            if (linker._frameTimeInfo != null)
            {
                _OnShowFrameTime(linker._frameTimeInfo);
            }

            if (linker._renderingInfo != null)
            {
                _OnShowRendering(linker._renderingInfo);
            }

            if (linker._batteryInfo != null)
            {
                _viewer.OnShowBattery(linker._batteryInfo);
            }

            if (linker._memoryInfo != null)
            {
                _OnShowMemory(linker._memoryInfo);
            }

            if (linker._thermalInfo != null)
            {
                _OnShowThermal(linker._thermalInfo);
            }
        }

        void _OnShowFps(FpsUnchangingInfo info)
        {
            _viewer.OnShowFps(info);
            _currentFpsSmall.color = NoaDebuggerDefine.TextColors.Dynamic;

            if (info.IsViewHyphen)
            {
                _currentFpsSmall.text = $"{NoaDebuggerDefine.HyphenValue}";
                _currentFpsSmall.color = NoaDebuggerDefine.TextColors.Default;
            }
            else
            {
                _currentFpsSmall.text = $"{info.CurrentFps}fps({info.ElapsedTime}ms)";
            }

            _fpsProfilingButton._onClick.RemoveAllListeners();
            _fpsProfilingButton._onClick.AddListener(_OnClickFpsProfiling);
            _fpsProfilingButton.Init(info.IsProfiling);
        }

        void _OnShowFrameTime(ProfilerFrameTimeViewInformation info)
        {
            if (info._isEnabled && info._isActive)
            {
                _viewer.OnShowFrameTime(info);
            }

            _frameTimeProfilingButton._onClick.RemoveAllListeners();
            _frameTimeProfilingButton._onClick.AddListener(_OnClickFrameTimeProfiling);
            _frameTimeProfilingButton.Init(info._isEnabled);

            if (_frameTimeChartRoot.activeSelf != info._isEnabled)
            {
                _frameTimeChartRoot.SetActive(info._isEnabled);
            }
        }

        void _OnShowMemory(MemoryUnchangingInfo info)
        {
            _viewer.OnShowMemory(info);
            _currentMemorySmall.color = NoaDebuggerDefine.TextColors.Dynamic;
            _memoryMin.SetActive(true);

            _memoryProfilingButton._onClick.RemoveAllListeners();
            _memoryProfilingButton._onClick.AddListener(_OnClickMemoryProfiling);

            _memoryGraphButton._onClick.RemoveAllListeners();
            _memoryGraphButton._onClick.AddListener(_OnClickMemoryGraph);

            bool isDisable = info.CurrentMemoryMB <= -1;
            bool isInteractable = !(isDisable && !info.IsViewHyphen);
            _memoryProfilingButton.Init(info.IsProfiling);
            _memoryProfilingButton.Interactable = isInteractable;
            _memoryGraphButton.Init(info.IsGraphShowing);
            _memoryGraphButton.Interactable = isInteractable;

            if (info.IsViewHyphen)
            {
                _currentMemorySmall.text = $"{NoaDebuggerDefine.HyphenValue}";
                _currentMemorySmall.color = NoaDebuggerDefine.TextColors.Default;
                _memoryMin.SetActive(false);
            }

            else if (info.CurrentMemoryMB <= -1)
            {
                _currentMemorySmall.text = $"{NoaDebuggerDefine.MISSING_VALUE}";
                _currentMemorySmall.color = NoaDebuggerDefine.TextColors.Default;
                _memoryMin.SetActive(false);
            }
            else
            {
                _currentMemorySmall.text = $"{info.CurrentMemoryMB}MB";
            }

            bool isGraphShowing = info.IsGraphShowing && isInteractable;
            if (_memoryChartRoot.activeSelf != isGraphShowing)
            {
                _memoryChartRoot.SetActive(isGraphShowing);
            }

            _gcCollectButton.onClick.RemoveAllListeners();
            _gcCollectButton.onClick.AddListener(_OnClickGcCollect);
            _gcCollectButton.Interactable = GarbageCollector.GCMode != GarbageCollector.Mode.Disabled;
        }

        void _OnShowRendering(RenderingUnchangingInfo info)
        {
            _viewer.OnShowRendering(info);

            _renderingProfilingButton._onClick.RemoveAllListeners();
            _renderingProfilingButton._onClick.AddListener(_OnClickRenderProfiling);
            _renderingProfilingButton.Init(info.IsProfiling);

            _renderingGraphButton._onClick.RemoveAllListeners();
            _renderingGraphButton._onClick.AddListener(_OnClickRenderGraph);
            _renderingGraphButton.Init(info.IsGraphShowing);

            if (_renderingChartRoot.activeSelf != info.IsGraphShowing)
            {
                _renderingChartRoot.SetActive(info.IsGraphShowing);
            }

            _renderingValueOptionSetPassCalls.Init(info.GraphTarget == RenderingGraphTarget.SetPassCalls);
            _renderingValueOptionDrawCalls.Init(info.GraphTarget == RenderingGraphTarget.DrawCalls);
            _renderingValueOptionBatches.Init(info.GraphTarget == RenderingGraphTarget.Batches);
            _renderingValueOptionTriangles.Init(info.GraphTarget == RenderingGraphTarget.Triangles);
            _renderingValueOptionVertices.Init(info.GraphTarget == RenderingGraphTarget.Vertices);
        }

        void _OnShowThermal(ThermalUnchangingInfo info)
        {
            _thermalMin.SetActive(true);
            _viewer.OnShowThermal(info);

            if (info.IsViewHyphen || info.CurrentThermalStatus == ThermalStatus.Unknown)
            {
                _thermalMin.SetActive(false);
            }
        }

        void _OnClickFpsProfiling(bool isOn)
        {
            OnFpsProfilingStateChanged?.Invoke(isOn);
        }

        void _OnClickFrameTimeProfiling(bool isOn)
        {
            OnFrameTimeProfilingStateChanged?.Invoke(isOn);
        }

        void _OnClickMemoryProfiling(bool isOn)
        {
            OnMemoryProfilingStateChanged?.Invoke(isOn);
        }

        void _OnClickMemoryGraph(bool isOn)
        {
            OnMemoryGraphStateChanged?.Invoke(isOn);
        }

        void _OnClickGcCollect()
        {
            OnGcCollectExecuted?.Invoke();
        }

        void _OnClickRenderProfiling(bool isOn)
        {
            OnRenderingProfilingStateChanged?.Invoke(isOn);
        }

        void _OnClickRenderGraph(bool isOn)
        {
            OnRenderingGraphStateChanged?.Invoke(isOn);
        }

        void _OnClickRenderingSetPassCalls(bool isOn)
        {
            if (isOn)
            {
                OnRenderingGraphSelected?.Invoke(RenderingGraphTarget.SetPassCalls);
            }
        }

        void _OnClickRenderingDrawCalls(bool isOn)
        {
            if (isOn)
            {
                OnRenderingGraphSelected?.Invoke(RenderingGraphTarget.DrawCalls);
            }
        }

        void _OnClickRenderingBatches(bool isOn)
        {
            if (isOn)
            {
                OnRenderingGraphSelected?.Invoke(RenderingGraphTarget.Batches);
            }
        }

        void _OnClickRenderingTriangles(bool isOn)
        {
            if (isOn)
            {
                OnRenderingGraphSelected?.Invoke(RenderingGraphTarget.Triangles);
            }
        }

        void _OnClickRenderingVertices(bool isOn)
        {
            if (isOn)
            {
                OnRenderingGraphSelected?.Invoke(RenderingGraphTarget.Vertices);
            }
        }

        void OnDestroy()
        {
            _currentFpsSmall = default;
            _currentMemorySmall = default;
            _memoryMin = default;
            _thermalMin = default;
            _viewer = default;
            _fpsProfilingButton = default;
            _frameTimeProfilingButton = default;
            _frameTimeChartRoot = default;
            _memoryProfilingButton = default;
            _memoryGraphButton = default;
            _memoryChartRoot = default;
            _gcCollectButton = default;
            _renderingProfilingButton = default;
            _renderingGraphButton = default;
            _renderingChartRoot = default;
            _renderingValueOptionGroup = default;
            _renderingValueOptionSetPassCalls = default;
            _renderingValueOptionDrawCalls = default;
            _renderingValueOptionBatches = default;
            _renderingValueOptionTriangles = default;
            _renderingValueOptionVertices = default;
            OnFpsProfilingStateChanged = default;
            OnFrameTimeProfilingStateChanged = default;
            OnMemoryProfilingStateChanged = default;
            OnMemoryGraphStateChanged = default;
            OnGcCollectExecuted = default;
            OnRenderingProfilingStateChanged = default;
            OnRenderingGraphStateChanged = default;
            OnRenderingGraphSelected = default;
        }
    }
}
