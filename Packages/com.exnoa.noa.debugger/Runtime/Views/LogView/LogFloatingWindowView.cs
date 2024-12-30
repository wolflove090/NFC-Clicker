using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class LogFloatingWindowView : FloatingWindowViewBase<LogViewLinker>
    {
        [Header("Log scroll")]
        public LogScrollDrawer _logScrollDrawer;

        [SerializeField, Header("Log status")]
        GameObject _logStatusRoot;
        [SerializeField]
        TextMeshProUGUI _logStatus;

        [SerializeField, Header("Log switching toggle")]
        LogSwitchToggleDrawer _logSwitchToggleDrawerDefault;
        [SerializeField]
        LogSwitchToggleDrawer _logSwitchToggleDrawerSmall;

        [SerializeField, Header("Button")]
        ToggleButtonBase _recordDefault;

        [SerializeField]
        ToggleButtonBase _recordSmall;

        [SerializeField]
        Button _clearDefault;
        [SerializeField]
        Button _clearSmall;

        [SerializeField]
        NoaDebuggerDisableButton _download;

        [SerializeField, Header("Layout")]
        VerticalLayoutGroup[] _layoutGroupArray;


        public event UnityAction OnRecord;

        public event UnityAction OnClear;

        public event UnityAction OnDownload;

        public event UnityAction<LogType, bool> OnSwitchByLogType;


        void _OnValidateUI()
        {
            Assert.IsNotNull(_logScrollDrawer);
            Assert.IsNotNull(_logStatusRoot);
            Assert.IsNotNull(_logStatus);
            Assert.IsNotNull(_logSwitchToggleDrawerDefault);
            Assert.IsNotNull(_logSwitchToggleDrawerSmall);
            Assert.IsNotNull(_recordDefault);
            Assert.IsNotNull(_recordSmall);
            Assert.IsNotNull(_clearDefault);
            Assert.IsNotNull(_clearSmall);
            Assert.IsNotNull(_download);
            Assert.IsNotNull(_layoutGroupArray);
        }

        protected override void _Init()
        {
            base._Init();
            _OnValidateUI();
            _logSwitchToggleDrawerDefault.OnSwitchByLogType += _OnSwitchToggle;
            _logSwitchToggleDrawerSmall.OnSwitchByLogType += _OnSwitchToggle;

            _recordDefault._onClick.RemoveAllListeners();
            _recordDefault._onClick.AddListener(_OnRecord);

            _recordSmall._onClick.RemoveAllListeners();
            _recordSmall._onClick.AddListener(_OnRecord);

            _clearDefault.onClick.RemoveAllListeners();
            _clearDefault.onClick.AddListener(_OnClear);

            _clearSmall.onClick.RemoveAllListeners();
            _clearSmall.onClick.AddListener(_OnClear);

            _download.onClick.RemoveAllListeners();
            _download.onClick.AddListener(_OnDownload);
        }

        protected override void _OnShow(LogViewLinker linker)
        {
            GlobalCoroutine.Run(UpdateScroll(linker));

            if (string.IsNullOrEmpty(linker._logStatus))
            {
                _logStatusRoot.SetActive(false);
            }
            else
            {
                _logStatusRoot.SetActive(true);
                _logStatus.text = linker._logStatus;
            }

            _logSwitchToggleDrawerDefault.Draw(linker._logTypeToggles);
            _logSwitchToggleDrawerSmall.Draw(linker._logTypeToggles);

            _recordDefault.Init(linker._isCollecting);
            _recordSmall.Init(linker._isCollecting);

            int logSum = linker._logTypeToggles._messageNum
                         + linker._logTypeToggles._warningNum
                         + linker._logTypeToggles._errorNum;
            bool isShowDownload = logSum > 0;
            _download.Interactable = isShowDownload;
        }

        IEnumerator UpdateScroll(LogViewLinker linker)
        {
            foreach (VerticalLayoutGroup layoutGroup in _layoutGroupArray)
            {
                layoutGroup.CalculateLayoutInputVertical();
                layoutGroup.SetLayoutVertical();
            }

            yield return null;

            if (_logScrollDrawer != null)
            {
                _logScrollDrawer.Draw(linker);
            }
        }


        void _OnRecord(bool isOn)
        {
            OnRecord?.Invoke();
        }

        void _OnClear()
        {
            OnClear?.Invoke();
        }

        void _OnDownload()
        {
            OnDownload?.Invoke();
        }

        void _OnSwitchToggle(LogType type, bool isOn)
        {
            OnSwitchByLogType?.Invoke(type, isOn);
        }

        void OnDestroy()
        {
            _logScrollDrawer = default;
            _logStatusRoot = default;
            _logStatus = default;
            _logSwitchToggleDrawerDefault = default;
            _logSwitchToggleDrawerSmall = default;
            _recordDefault = default;
            _recordSmall = default;
            _clearDefault = default;
            _clearSmall = default;
            _download = default;
            _layoutGroupArray = default;
            OnRecord = default;
            OnClear = default;
            OnDownload = default;
            OnSwitchByLogType = default;
        }
    }
}
