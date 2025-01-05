using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger
{
    [RequireComponent(typeof(DragBehaviour))]
    class FloatingWindowViewBase<T> : ViewBase<T>, IPointerDownHandler
        where T : ViewLinkerBase
    {
        [SerializeField]
        ToggleButtonBase _toggleButton;
        [SerializeField]
        DragBehaviour _dragBehaviour;
        [SerializeField]
        Button _closeButton;

        bool _isShowDefaultInfo;

        [SerializeField]
        GameObject _defaultInfo;
        [SerializeField]
        GameObject _smallInfo;

        [SerializeField, Header("Alpha target")] Image[] _backgrounds;

        public string ToolName { get; set; }

        public event UnityAction<bool> OnToggleStateChange;

        public event UnityAction<Vector2> OnDrag;

        public event UnityAction<Vector2> OnDragEnd;

        public event UnityAction OnClose;

        void _OnValidateUI()
        {
            Assert.IsNotNull(_toggleButton);
            Assert.IsNotNull(_dragBehaviour);
            Assert.IsNotNull(_closeButton);
            Assert.IsNotNull(_defaultInfo);
            Assert.IsNotNull(_smallInfo);
        }

        protected override void _Init()
        {
            _OnValidateUI();

            _toggleButton._onClick.RemoveAllListeners();
            _toggleButton._onClick.AddListener(_OnToggleChange);
            _dragBehaviour.OnDragEnd += _OnDragEnd;
            _dragBehaviour.OnDragging += _OnDrag;

            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(_Close);

            var noaDebuggerSettings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings();
            noaDebuggerSettings.FloatingWindowAlpha = UnityEngine.Mathf.Clamp(
                noaDebuggerSettings.FloatingWindowAlpha,
                NoaDebuggerDefine.CanvasAlphaMin,
                NoaDebuggerDefine.CanvasAlphaMax);

            foreach (Image background in _backgrounds)
            {
                _SetBackgroundColor(background);
            }

            _ChangeState();
        }

        public void SetScreenPos(Vector2 screenPos)
        {
            if (screenPos == Vector2.zero)
            {
                var screenWidthThreeQuarter = Screen.width * 0.75f;
                var screenHeightQuarter = Screen.height * 0.25f;
                screenPos.x = screenWidthThreeQuarter;
                screenPos.y = screenHeightQuarter;
            }

            transform.position = screenPos;
        }

        public void SetState(bool isShowDefault)
        {
            _isShowDefaultInfo = isShowDefault;
            _ChangeState();
            _toggleButton.Init(_isShowDefaultInfo);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _ChangeWindowOrder();
        }

        void _SetBackgroundColor(Image target)
        {
            Color newColor = target.color;
            newColor.a = NoaDebuggerSettingsManager.GetNoaDebuggerSettings().FloatingWindowAlpha;
            target.color = newColor;
        }

        void _ChangeWindowOrder()
        {
            transform.SetAsLastSibling();
        }

        void _OnToggleChange(bool isOn)
        {
            _isShowDefaultInfo = isOn;

            _ChangeState();
            _ChangeWindowOrder();
            OnToggleStateChange?.Invoke(_isShowDefaultInfo);
        }

        void _ChangeState()
        {
            _defaultInfo.SetActive(_isShowDefaultInfo);
            _smallInfo.SetActive(!_isShowDefaultInfo);
        }

        void _OnDrag(Vector2 screenPos)
        {
            OnDrag?.Invoke(screenPos);
        }

        void _OnDragEnd(Vector2 screenPos)
        {
            OnDragEnd?.Invoke(screenPos);
        }

        void _Close()
        {
            OnClose?.Invoke();
        }
    }

    [Serializable]
    sealed class FloatingWindowInfo
    {
        string _playerPrefsKey;

        [SerializeField]
        bool _isActiveValue;
        public bool _isActive
        {
            get
            {
                return _isActiveValue;
            }
            set
            {
                _isActiveValue = value;
                _Save();
            }
        }

        [SerializeField]
        bool _isShowDefaultInfoVale;
        public bool _isShowDefaultInfo
        {
            get
            {
                return _isShowDefaultInfoVale;
            }
            set
            {
                _isShowDefaultInfoVale = value;
                _Save();
            }
        }

        [SerializeField]
        Vector2 _screenPositionPortraitValue;
        public Vector2 ScreenPositionPortrait
        {
            get
            {
                return _screenPositionPortraitValue;
            }
            set
            {
                _screenPositionPortraitValue = value;
                _Save();
            }
        }

        [SerializeField]
        Vector2 _screenPositionLandscapeValue;
        public Vector2 ScreenPositionLandscape
        {
            get
            {
                return _screenPositionLandscapeValue;
            }
            set
            {
                _screenPositionLandscapeValue = value;
                _Save();
            }
        }

        public FloatingWindowInfo(string prefsKey)
        {
            _playerPrefsKey = prefsKey;

            string windowInfoJson = NoaDebuggerPrefs.GetString(_playerPrefsKey, "");
            if (!String.IsNullOrEmpty(windowInfoJson))
            {
                var loadInfo = JsonUtility.FromJson<FloatingWindowInfo>(windowInfoJson);

                _isActiveValue = loadInfo._isActiveValue;
                _isShowDefaultInfoVale = loadInfo._isShowDefaultInfoVale;
                _screenPositionLandscapeValue = loadInfo._screenPositionLandscapeValue;
                _screenPositionPortraitValue = loadInfo._screenPositionPortraitValue;
            }
            else
            {
                Reset();
            }
        }

        public void Reset()
        {
            _isShowDefaultInfoVale = false;
            _isActiveValue = false;
            _screenPositionLandscapeValue = Vector2.zero;
            _screenPositionPortraitValue = Vector2.zero;

            _Save();
        }

        void _Save()
        {
            string windowInfoJson = JsonUtility.ToJson(this);
            NoaDebuggerPrefs.SetString(_playerPrefsKey, windowInfoJson);
        }
    }
}
