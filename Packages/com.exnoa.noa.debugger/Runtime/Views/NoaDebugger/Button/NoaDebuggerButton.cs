using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger
{
    public enum ButtonPosition
    {
        UpperLeft, 
        UpperCenter, 
        UpperRight, 
        MiddleLeft, 
        MiddleRight, 
        LowerLeft, 
        LowerCenter, 
        LowerRight, 
    }

    public enum ButtonMovementType
    {
        Draggable,
        Fixed
    }

    sealed class NoaDebuggerButton : MonoBehaviour
    {
        static readonly string DeviceOrientationKey = "NoaDebuggerButtonDeviceOrientation";

        enum TapState
        {
            None,
            Tap,
            LongTap,
            Drag,
        }

        [SerializeField]
        DragBehaviour _dragBehaviour;

        [SerializeField]
        PointerEventComponent _bootButton;

        [SerializeField]
        Animation _bootButtonAnimation;

        [SerializeField]
        Image _bootButtonBackgroundImage;

        [SerializeField]
        CanvasGroup _bootButtonCanvasGroup;

        [SerializeField]
        Image[] _bootButtonImages;

        [SerializeField]
        PointerEventComponent _visibleButton;

        bool _isInitialized = false;

        TapState _tapState = TapState.None;

        float _pressStartTime;

        Vector2? _pressStartPos;

        bool _isShowNoaDebugger;

        bool _isErrorWhileHidden;

        NoaDebuggerSettings _noaDebuggerSettings = null;

        void _OnValidateUI()
        {
            Assert.IsNotNull(_dragBehaviour);
            Assert.IsNotNull(_bootButton);
            Assert.IsNotNull(_bootButtonAnimation);
            Assert.IsNotNull(_bootButtonBackgroundImage);
            Assert.IsNotNull(_bootButtonCanvasGroup);
            Assert.IsNotNull(_visibleButton);

            foreach (var image in _bootButtonImages)
            {
                Assert.IsNotNull(image);
            }
        }

        public void Init()
        {
            if (_isInitialized)
            {
                return;
            }

            _OnValidateUI();

            _isInitialized = true;

            _bootButton.OnPointerDownEvent += _OnPointerDown;
            _visibleButton.OnPointerClickEvent += (data) => _SetActiveUI(true);
            _UpdateSettings();

            DeviceOrientationManager.SetAction(NoaDebuggerButton.DeviceOrientationKey, _LoadPosition);
        }

        void _UpdateSettings()
        {
#if NOA_DEBUGGER
            _noaDebuggerSettings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings();

            var startButtonScale = _noaDebuggerSettings.StartButtonScale;
            transform.localScale = new Vector3(startButtonScale, startButtonScale, startButtonScale);

            _noaDebuggerSettings.ToolStartButtonAlpha = UnityEngine.Mathf.Clamp(
                _noaDebuggerSettings.ToolStartButtonAlpha,
                NoaDebuggerDefine.ToolStartButtonAlphaMin,
                NoaDebuggerDefine.ToolStartButtonAlphaMax);

            if (_noaDebuggerSettings.StartButtonMovementType == ButtonMovementType.Fixed)
            {
                gameObject.GetComponent<DragBehaviourFitWithInScreen>().enabled = false;
            }

            _UpdateButtonAlpha(_bootButtonBackgroundImage, _noaDebuggerSettings.ToolStartButtonAlpha);
            foreach (UnityEngine.UI.Image image in _bootButtonImages)
            {
                _UpdateButtonAlpha(image, _noaDebuggerSettings.ToolStartButtonAlpha);
            }

            if (_noaDebuggerSettings.ToolStartButtonAlpha == 0 && _noaDebuggerSettings.StartButtonMovementType == ButtonMovementType.Draggable)
            {
                PlayOnLocationAnimation();
            }
#endif
        }

        void _OnPointerDown(PointerEventData eventData)
        {
            _tapState = TapState.Tap;
            _dragBehaviour.CanMove = false;
            _pressStartTime = Time.realtimeSinceStartup;
            _pressStartPos = eventData.position;
            _bootButtonAnimation.Stop();
        }

        void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            _UpdateButton();

            if (_isShowNoaDebugger)
            {
                _Reset();

                NoaDebugger.ShowNoaDebuggerLastActiveTool();
            }
        }

        void _UpdateButton()
        {
            float pressingTime = Time.realtimeSinceStartup - _pressStartTime;

            _UpdateTapStateOnPressing(pressingTime);

            _UpdateButtonFromTapState(pressingTime);

            if (_isErrorWhileHidden && !_bootButtonAnimation.isPlaying)
            {
                SetActive(false);
                _isErrorWhileHidden = false;
            }
        }

        void _UpdateTapStateOnPressing(float pressingTime)
        {
            if (_tapState == TapState.Tap && pressingTime >= NoaDebuggerDefine.PressTimeSeconds)
            {
                _tapState = TapState.LongTap;
            }
            if ((_tapState is TapState.Tap or TapState.LongTap) && _pressStartPos != null)
            {
                float distance = Vector2.Distance(_pressStartPos.Value, Input.GetCursorPosition());
                if (distance >= NoaDebuggerDefine.DragThresholdDistanceOnScreen)
                {
                    _tapState = TapState.Drag;
                    _dragBehaviour.CanMove = true;
                }
            }
        }

        void _UpdateButtonFromTapState(float pressingTime)
        {
            switch (_tapState)
            {
                case TapState.None:
                    _ResetButtonColor();
                    break;

                case TapState.Tap:
                    _UpdateButtonColorOnPressing(pressingTime);
                    if (Input.IsButtonReleased())
                    {
                        _isShowNoaDebugger = true;
                        _tapState = TapState.None;
                    }
                    break;

                case TapState.LongTap:
                    _UpdateButtonColorOnPressing(pressingTime);
                    if (Input.IsButtonReleased())
                    {
                        _SetActiveUI(false);
                        _tapState = TapState.None;
                    }
                    break;

                case TapState.Drag:
                    _UpdateButtonColorOnPressing(pressingTime);
                    if (Input.IsButtonReleased())
                    {
                        _SavePosition(DeviceOrientationManager.IsPortrait);
                        _tapState = TapState.None;
                    }
                    break;
            }
        }

        void _ResetButtonColor()
        {
            if (_noaDebuggerSettings == null)
            {
                return;
            }

            bool isErrorNotice = NoaDebugger.IsErrorNotice &&
                                 _noaDebuggerSettings.ErrorNotificationType == ErrorNotificationType.Full;

            _UpdateButtonAlpha(
                _bootButtonBackgroundImage,
                _noaDebuggerSettings.ToolStartButtonAlpha,
                isErrorNotice
                    ? NoaDebuggerDefine.BackgroundColors.NoaDebuggerButtonAlert
                    : NoaDebuggerDefine.BackgroundColors.NoaDebuggerButtonDefault);
        }

        void _UpdateButtonColorOnPressing(float pressingTime)
        {
            if (_noaDebuggerSettings == null)
            {
                return;
            }

            float t = Mathf.InverseLerp(0, NoaDebuggerDefine.PressTimeSeconds, pressingTime);
            float alpha = Mathf.Lerp(_noaDebuggerSettings.ToolStartButtonAlpha, 1, t);
            _UpdateButtonAlpha(_bootButtonBackgroundImage, alpha);
        }

        void _UpdateButtonAlpha(Image image, float alpha, Color? baseColor = null)
        {
            Color nextColor = baseColor ?? image.color;
            _bootButtonCanvasGroup.alpha = alpha;
            image.color = nextColor;
        }

        void _Reset()
        {
            _tapState = TapState.None;
            _pressStartTime = 0;
            _pressStartPos = null;
            _isShowNoaDebugger = false;
        }

        void _SetActiveUI(bool active)
        {
            NoaDebuggerManager.SetFloatingWindowActive(active);
            _bootButton.gameObject.SetActive(active);
            _visibleButton.gameObject.SetActive(!active);
        }

        void _LoadPosition(bool isPortrait)
        {
            if (_noaDebuggerSettings == null)
            {
                return;
            }

            if (_noaDebuggerSettings.StartButtonMovementType == ButtonMovementType.Fixed)
            {
                _SetupButtonPosition();
                return;
            }

            transform.position = _GetPositionPrefsData(isPortrait);
        }

        void _SavePosition(bool isPortrait)
        {
            if (_noaDebuggerSettings == null || _noaDebuggerSettings.SaveStartButtonPosition == false)
            {
                return;
            }

            var key = "";
            if (isPortrait)
            {
                key = NoaDebuggerPrefsDefine.PrefsKeyStartButtonPortrait;
            }
            else
            {
                key = NoaDebuggerPrefsDefine.PrefsKeyStartButtonLandscape;
            }

            string saveString = $"{transform.position.x},{transform.position.y},{transform.position.z}";

            NoaDebuggerPrefs.SetString(key, saveString);
        }

        public void PlayOnErrorAnimation()
        {
            if (_noaDebuggerSettings.ErrorNotificationType == ErrorNotificationType.None ||
                gameObject == null)
            {
                return;
            }
            if (!gameObject.activeSelf && !NoaDebuggerManager.IsDebuggerVisible)
            {
                _isErrorWhileHidden = true;
                SetActive(true);
            }
            _bootButtonAnimation.Play("alert");
        }

        void PlayOnLocationAnimation()
        {
            if (!gameObject.activeSelf && !NoaDebuggerManager.IsDebuggerVisible)
            {
                _isErrorWhileHidden = true;
                SetActive(true);
            }
            _bootButtonAnimation.Play("locate");
        }

        Vector3 _GetPositionPrefsData(bool isPortrait)
        {
            if (!_noaDebuggerSettings.SaveStartButtonPosition)
            {
                return _SetupButtonPosition();
            }

            string key = isPortrait ? NoaDebuggerPrefsDefine.PrefsKeyStartButtonPortrait : NoaDebuggerPrefsDefine.PrefsKeyStartButtonLandscape;
            string positionString = NoaDebuggerPrefs.GetString(key, "");

            if (String.IsNullOrEmpty(positionString))
            {
                return _SetupButtonPosition();
            }

            Vector3 position = _DeserializeVector3(positionString);
            if (_IsOutOfScreen(position))
            {
                position = _SetupButtonPosition();
            }

            return position;
        }

        Vector3 _DeserializeVector3(string value)
        {
            var values = value.Split(',');
            bool canParse = true;

            if (!float.TryParse(values[0], out float x))
            {
                canParse = false;
            }
            if(!float.TryParse(values[1], out float y))
            {
                canParse = false;
            }
            if (!float.TryParse(values[2], out float z))
            {
                canParse = false;
            }

            return canParse ? new Vector3(x, y, z) : _SetupButtonPosition();
        }

        void OnRectTransformDimensionsChange()
        {
            if (_dragBehaviour.isDragging)
            {
                return;
            }

            if (_IsOutOfScreen(transform.position))
            {
                transform.position = _SetupButtonPosition();
            }
        }

        bool _IsOutOfScreen(Vector3 buttonPosition)
        {
            Vector3 leftBottom = Vector3.zero;
            Vector3 rightTop = new Vector3(Screen.width, Screen.height, 0);

            Vector3 screenMargin  = new Vector3(10, 10, 0);
            leftBottom -= screenMargin;
            rightTop += screenMargin;

            RectTransform rectTransform = transform as RectTransform;
            rectTransform.position = buttonPosition;
            Vector3[] buttonCorners = new Vector3[4];
            rectTransform.GetWorldCorners(buttonCorners);

            if (buttonCorners[2].x > rightTop.x)
            {
                return true;
            }

            if (buttonCorners[2].y > rightTop.y)
            {
                return true;
            }

            if (buttonCorners[0].x < leftBottom.x)
            {
                return true;
            }

            if (buttonCorners[0].y < leftBottom.y)
            {
                return true;
            }

            return false;
        }

        Vector3 _SetupButtonPosition()
        {
            var _buttonRectTransfrom = transform as RectTransform;

            if (_noaDebuggerSettings == null)
            {
                return _buttonRectTransfrom.position;
            }

            switch (_noaDebuggerSettings.StartButtonPosition)
            {
                case ButtonPosition.UpperLeft:
                    _buttonRectTransfrom.anchorMin = new Vector2(0, 1);
                    _buttonRectTransfrom.anchorMax = new Vector2(0, 1);
                    _buttonRectTransfrom.pivot = new Vector2(0, 1);
                    break;

                case ButtonPosition.UpperCenter:
                    _buttonRectTransfrom.anchorMin = new Vector2(0.5f, 1);
                    _buttonRectTransfrom.anchorMax = new Vector2(0.5f, 1);
                    _buttonRectTransfrom.pivot = new Vector2(0.5f, 1);
                    break;

                case ButtonPosition.UpperRight:
                    _buttonRectTransfrom.anchorMin = new Vector2(1, 1);
                    _buttonRectTransfrom.anchorMax = new Vector2(1, 1);
                    _buttonRectTransfrom.pivot = new Vector2(1, 1);
                    break;

                case ButtonPosition.MiddleRight:
                    _buttonRectTransfrom.anchorMin = new Vector2(1, 0.5f);
                    _buttonRectTransfrom.anchorMax = new Vector2(1, 0.5f);
                    _buttonRectTransfrom.pivot = new Vector2(1, 0.5f);
                    break;

                case ButtonPosition.MiddleLeft:
                    _buttonRectTransfrom.anchorMin = new Vector2(0, 0.5f);
                    _buttonRectTransfrom.anchorMax = new Vector2(0, 0.5f);
                    _buttonRectTransfrom.pivot = new Vector2(0, 0.5f);
                    _buttonRectTransfrom.anchoredPosition = new Vector2(10, 0);
                    break;

                case ButtonPosition.LowerRight:
                    _buttonRectTransfrom.anchorMin = new Vector2(1, 0);
                    _buttonRectTransfrom.anchorMax = new Vector2(1, 0);
                    _buttonRectTransfrom.pivot = new Vector2(1, 0);
                    break;

                case ButtonPosition.LowerLeft:
                    _buttonRectTransfrom.anchorMin = new Vector2(0, 0);
                    _buttonRectTransfrom.anchorMax = new Vector2(0, 0);
                    _buttonRectTransfrom.pivot = new Vector2(0, 0);
                    break;

                case ButtonPosition.LowerCenter:
                    _buttonRectTransfrom.anchorMin = new Vector2(0.5f, 0);
                    _buttonRectTransfrom.anchorMax = new Vector2(0.5f, 0);
                    _buttonRectTransfrom.pivot = new Vector2(0.5f, 0);
                    break;
            }

            _buttonRectTransfrom.anchoredPosition = new Vector2(0, 0);

            return _buttonRectTransfrom.position;
        }

        public void SetActive(bool isActive)
        {
            _ResetButtonColor();
            gameObject.SetActive(isActive);
        }

        public bool IsActive()
        {
            return _bootButton.gameObject.activeInHierarchy;
        }

        public void Dispose()
        {
            _dragBehaviour = default;
            _bootButton = default;
            _bootButtonAnimation = default;
            _bootButtonBackgroundImage = default;
            _bootButtonImages = default;
            _visibleButton = default;
            _noaDebuggerSettings = default;
        }
    }
}
