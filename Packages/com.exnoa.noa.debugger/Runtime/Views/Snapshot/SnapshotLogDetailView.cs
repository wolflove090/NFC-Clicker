using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class SnapshotLogDetailView : ViewBase<SnapshotViewLinker>
    {
        [SerializeField, Header("Header")]
        GameObject _header;
        [SerializeField]
        Image _icon;
        [SerializeField, Header("Drawer")]
        ProfilerDrawerComponent _viewer;
        [SerializeField, Header("Snapshot")]
        NoaDebuggerText _label;
        [SerializeField]
        SnapshotModel.ToggleState _toggleState;
        [SerializeField]
        ScrollRect _profilingViewRoot;
        [SerializeField]
        NoaDebuggerText _emptyText;

        [SerializeField] GameObject _fps;
        [SerializeField] GameObject _memory;
        [SerializeField] GameObject _rendering;
        [SerializeField] GameObject _battery;
        [SerializeField] GameObject _thermal;

        [SerializeField]
        SnapshotAdditionalInfoView _additionalInfoView;

        List<SnapshotAdditionalInfoView> _additionalInfoViewPrev;
        List<SnapshotAdditionalInfoView> _additionalInfoViewAfter;

        [SerializeField, Header("Button")]
        Button _copyButton;

        public UnityAction<int> OnClickCopyButton { get; set; }
        SnapshotLogRecordInformation _selectedLog;

        void Awake()
        {
            Assert.IsNotNull(_header);
            Assert.IsNotNull(_icon);
            Assert.IsNotNull(_viewer);
            Assert.IsNotNull(_label);
            Assert.IsNotNull(_profilingViewRoot);
            Assert.IsNotNull(_emptyText);
            Assert.IsNotNull(_fps);
            Assert.IsNotNull(_memory);
            Assert.IsNotNull(_rendering);
            Assert.IsNotNull(_battery);
            Assert.IsNotNull(_thermal);
            Assert.IsNotNull(_additionalInfoView);
            Assert.IsNotNull(_copyButton);

            _copyButton.onClick.RemoveAllListeners();
            _copyButton.onClick.AddListener(_OnCopy);
        }

        protected override void _OnShow(SnapshotViewLinker linker)
        {
            if (linker._logList == null)
            {
                return;
            }

            bool isComparison = linker._isComparison;
            _selectedLog = linker._logList.FirstOrDefault(
                logData => logData.IsSelected ||
                           isComparison &&
                           logData.ToggleState == _toggleState);
            bool isShowProfilerInfo = _selectedLog?.Snapshot != null;
            bool isShowAdditionalInfo = _selectedLog?.AdditionalInfo?.Count > 0;
            bool showSnapshotLogDetail = isShowProfilerInfo || isShowAdditionalInfo;
            _profilingViewRoot.gameObject.SetActive(true);
            _profilingViewRoot.viewport.gameObject.SetActive(showSnapshotLogDetail);

            _label.text = _selectedLog?.Label;
            _header.SetActive(!isComparison);

            _profilingViewRoot.enabled = showSnapshotLogDetail;
            _emptyText.gameObject.SetActive(!showSnapshotLogDetail);
            if (!showSnapshotLogDetail)
            {
                _profilingViewRoot.enabled = false;
                _profilingViewRoot.horizontalScrollbar.gameObject.SetActive(false);
                _profilingViewRoot.verticalScrollbar.gameObject.SetActive(false);
                return;
            }

            _icon.gameObject.SetActive(isComparison);
            switch (_toggleState)
            {
                case SnapshotModel.ToggleState.SelectedFirst:
                    _icon.color = NoaDebuggerDefine.ImageColors.SnapshotFirstSelected;
                    break;

                case SnapshotModel.ToggleState.SelectedSecond:
                    _icon.color = NoaDebuggerDefine.ImageColors.SnapshotSecondSelected;
                    break;
            }

            _fps.SetActive(isShowProfilerInfo);
            _memory.SetActive(isShowProfilerInfo);
            _rendering.SetActive(isShowProfilerInfo);
            _battery.SetActive(isShowProfilerInfo);
            _thermal.SetActive(isShowProfilerInfo);
            if (isShowProfilerInfo)
            {
                _OnShowProfilingInfo(_selectedLog.Snapshot);
            }

            if (_additionalInfoViewPrev != null)
            {
                foreach (var additionalInfoView in _additionalInfoViewPrev)
                {
                    Destroy(additionalInfoView.gameObject);
                }

                _additionalInfoViewPrev.Clear();
            }

            if (isShowAdditionalInfo)
            {
                if (_additionalInfoViewAfter == null)
                {
                    _additionalInfoViewAfter = new List<SnapshotAdditionalInfoView>();
                }

                foreach (var additionalInfo in _selectedLog.AdditionalInfo)
                {
                    var additionalInfoView = Instantiate(_additionalInfoView, _viewer.transform);
                    var additionalInfoViewLinker = new SnapshotAdditionalInfoViewLinker()
                    {
                        _category = SnapshotPresenter.ConvertCategoryName(additionalInfo.Key),
                        _categoryItems = additionalInfo.Value.CategoryItems
                    };

                    additionalInfoView.Show(additionalInfoViewLinker);
                    _additionalInfoViewAfter.Add(additionalInfoView);
                }

                _additionalInfoViewPrev = new List<SnapshotAdditionalInfoView>(_additionalInfoViewAfter.ToArray());
                _additionalInfoViewAfter.Clear();
            }

            _copyButton.gameObject.SetActive(true);
        }

        void _OnShowProfilingInfo(ProfilerSnapshotData snapshotData)
        {
            if (snapshotData.FpsInfo != null)
            {
                _viewer.OnShowFps(snapshotData.FpsInfo);
            }

            if (snapshotData.RenderingInfo != null)
            {
                _viewer.OnShowRendering(snapshotData.RenderingInfo);
            }

            if (snapshotData.BatteryInfo != null)
            {
                _viewer.OnShowBattery(snapshotData.BatteryInfo);
            }

            if (snapshotData.MemoryInfo != null)
            {
                _viewer.OnShowMemory(snapshotData.MemoryInfo);
            }

            if (snapshotData.ThermalInfo != null)
            {
                _viewer.OnShowThermal(snapshotData.ThermalInfo);
            }
        }

        void OnDestroy()
        {
            _header = default;
            _icon = default;
            _viewer = default;
            _label = default;
            _profilingViewRoot = default;
            _emptyText = default;
            _fps = default;
            _memory = default;
            _rendering = default;
            _battery = default;
            _thermal = default;
            _additionalInfoView = default;
            _additionalInfoViewPrev = default;
            _additionalInfoViewAfter = default;
            _selectedLog = default;
            OnClickCopyButton = default;
        }

        void _OnCopy()
        {
            if (_selectedLog == null)
            {
                return;
            }
            OnClickCopyButton?.Invoke(_selectedLog.Id);

            _copyButton.gameObject.SetActive(false);
        }
    }
}
