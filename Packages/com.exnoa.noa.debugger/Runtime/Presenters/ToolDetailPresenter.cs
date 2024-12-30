using System;
using UnityEngine;

namespace NoaDebugger
{
    sealed class ToolDetailPresenter : NoaDebuggerToolBase, INoaDebuggerTool
    {
        [Header("MainView")]
        [SerializeField]
        ToolDetailView _mainViewPrefab;
        ToolDetailView _mainView;

        ToolDetailModel _model;

        [NonSerialized]
        public bool _isDeviceOrientationChanged;

        public ToolNotificationStatus NotifyStatus => ToolNotificationStatus.None;

        public void Init()
        {
            _model = new ToolDetailModel();
        }

        public sealed class ToolDetailMenuInfo : IMenuInfo
        {
            public string Name => "ToolDetail";
            public string MenuName => "Noa Debugger";
            public int SortNo => 99;
        }

        ToolDetailMenuInfo _toolDetailMenuInfo;

        public IMenuInfo MenuInfo()
        {
            if (_toolDetailMenuInfo == null)
            {
                _toolDetailMenuInfo = new ToolDetailMenuInfo();
            }

            return _toolDetailMenuInfo;
        }

        public void ShowView(Transform parent)
        {
            if (_mainView == null)
            {
                _mainView = GameObject.Instantiate(_mainViewPrefab, parent);
                _model = new ToolDetailModel();
            }

            _UpdateAllView();
            _mainView.gameObject.SetActive(true);
        }

        public ToolDetailViewLinker Linker
        {
            get
            {
                var toolDetailInformation = _model.ToolDetailInformation;
                return new ToolDetailViewLinker()
                {
                    _copyright = toolDetailInformation._copyright,
                    _operatingEnv = toolDetailInformation._operatingEnv,
                };
            }
        }

        void _UpdateAllView()
        {
            if (_mainView == null)
            {
                return;
            }

            _mainView.Show(Linker);
        }

        public void AlignmentUI(bool isReverse)
        {
            _mainView.AlignmentUI(isReverse);
        }


        public ToolPinStatus GetPinStatus()
        {
            return ToolPinStatus.None;
        }

        public void TogglePin(Transform parent)
        {
        }

        public void InitFloatingWindow(Transform parent)
        {
        }


        void _OnHidden()
        {
            if (_mainView != null)
            {
                _mainView.Hide();
            }
        }

        public void OnHidden()
        {
            _OnHidden();
        }

        public void OnToolDispose()
        {
            _OnHidden();
            _model = null;
            _mainViewPrefab = default;
            _mainView = default;
            _toolDetailMenuInfo = default;
        }

        void OnDestroy()
        {
            _mainViewPrefab = default;
            _mainView = default;
            _model = default;
            _toolDetailMenuInfo = default;
        }
    }
}
