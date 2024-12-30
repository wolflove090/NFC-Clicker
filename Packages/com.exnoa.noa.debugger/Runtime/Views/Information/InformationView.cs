using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class InformationView : NoaDebuggerToolViewBase<InformationViewLinker>
    {
        [Header("Tab menu")]
        [SerializeField]
        ToggleButtonBase _systemButton;

        [SerializeField]
        ToggleButtonBase _unityButton;

        [SerializeField]
        Button _reloadButton;

        [SerializeField]
        Button _downloadButton;

        [Header("Tab view")]
        [SerializeField]
        SystemInformationView _systemInfoView;

        [SerializeField]
        UnityInformationView _unityInfoView;

        public enum ToggleTabType
        {
            System,
            Unity,
        }

        public event UnityAction<ToggleTabType> OnClickTab;

        public event UnityAction OnClickReload;

        public event UnityAction OnClickDownload;

        protected override void _Init()
        {
            _systemButton._onClick.RemoveListener(_OnClickSystemTab);
            _unityButton._onClick.RemoveListener(_OnClickUnityTab);
            _reloadButton.onClick.RemoveListener(_OnClickReload);
            _downloadButton.onClick.RemoveListener(_OnClickDownload);

            _systemButton._onClick.AddListener(_OnClickSystemTab);
            _unityButton._onClick.AddListener(_OnClickUnityTab);
            _reloadButton.onClick.AddListener(_OnClickReload);
            _downloadButton.onClick.AddListener(_OnClickDownload);
        }

        protected override void _OnShow(InformationViewLinker linker)
        {
            switch (linker._tabType)
            {
                case ToggleTabType.System:
                    _systemButton.Init(true);
                    _unityButton.Init(false);
                    _unityInfoView.Hide();
                    _systemInfoView.Show(linker._systemInformationViewLinker);
                    break;

                case ToggleTabType.Unity:
                    _systemButton.Init(false);
                    _unityButton.Init(true);
                    _systemInfoView.Hide();
                    _unityInfoView.Show(linker._unityInformationViewLinker);
                    break;
            }
        }

        void _OnClickSystemTab(bool isOn)
        {
            if (isOn)
            {
                OnClickTab?.Invoke(ToggleTabType.System);
            }
        }

        void _OnClickUnityTab(bool isOn)
        {
            if (isOn)
            {
                OnClickTab?.Invoke(ToggleTabType.Unity);
            }
        }

        void _OnClickReload()
        {
            OnClickReload?.Invoke();
        }

        void _OnClickDownload()
        {
            OnClickDownload?.Invoke();
        }
    }

    sealed class InformationViewLinker : ViewLinkerBase
    {
        public SystemInformationViewLinker _systemInformationViewLinker;

        public UnityInformationViewLinker _unityInformationViewLinker;

        public InformationView.ToggleTabType _tabType;
    }
}
