using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class LogPanel : MonoBehaviour
    {
        [SerializeField]
        GameObject _select;

        [SerializeField]
        Image _logType;

        [SerializeField]
        TextMeshProUGUI _logString;

        [SerializeField]
        TextMeshProUGUI _stackTraceString;

        [SerializeField]
        LongTapButton _selfButton;

        [SerializeField]
        GameObject _logCounterRoot;

        [SerializeField]
        TextMeshProUGUI _logCounter;

        [SerializeField]
        Image _backGround;

        int _index;

        UnityAction<int> _onClick;
        UnityAction<int> _onPointerDown;
        UnityAction _onLongTap;
        UnityAction<int> _onDelete;

        void _OnValidateUI()
        {
            Assert.IsNotNull(_select);
            Assert.IsNotNull(_logType);
            Assert.IsNotNull(_logString);
            Assert.IsNotNull(_stackTraceString);
            Assert.IsNotNull(_selfButton);
            Assert.IsNotNull(_backGround);
        }

        void Awake()
        {
            _OnValidateUI();

            _selfButton.onClick.RemoveAllListeners();
            _selfButton.onClick.AddListener(_OnClick);
            _selfButton._onPointerDown.RemoveAllListeners();
            _selfButton._onPointerDown.AddListener(_OnPointerDown);
            _selfButton._onLongTap.RemoveAllListeners();
            _selfButton._onLongTap.AddListener(_OnLongTap);
        }

        public void Draw(LogViewLinker.LogPanelInfo logInfo)
        {
            _index = logInfo._index;

            _select.SetActive(logInfo._isSelect);

            if (logInfo._viewIndex % 2 == 0)
            {
                _backGround.color = NoaDebuggerDefine.BackgroundColors.LogBright;
            }
            else
            {
                _backGround.color = NoaDebuggerDefine.BackgroundColors.LogDark;
            }

            _logType.color = _GetLogTypeColor(logInfo._logType);
            _logString.text = NoaDebuggerText.GetHasFontAssetString(_logString.font, logInfo._logString);
            if (!string.IsNullOrEmpty(logInfo._stackTraceString))
            {
                _stackTraceString.gameObject.SetActive(true);
                _stackTraceString.text = NoaDebuggerText.GetHasFontAssetString(_logString.font, logInfo._stackTraceString);
            }
            else
            {
                _stackTraceString.gameObject.SetActive(false);
            }

            _onClick = logInfo._onClick;
            _onPointerDown = logInfo._onPointerDown;
            _onLongTap = logInfo._onLongTap;

            if (logInfo._numberOfMatchingLogs > 1)
            {
                _logCounter.text = logInfo._numberOfMatchingLogs >= NoaDebuggerDefine.MaxNumberOfMatchingLogsToDisplay ? $"{NoaDebuggerDefine.MaxNumberOfMatchingLogsToDisplay}+" : logInfo._numberOfMatchingLogs.ToString();
                _logCounterRoot.SetActive(true);
            }
            else
            {
                _logCounterRoot.SetActive(false);
            }
        }

        void _OnClick()
        {
            _onClick?.Invoke(_index);
        }

        void _OnPointerDown()
        {
            _onPointerDown?.Invoke(_index);
        }

        void _OnLongTap()
        {
            _onLongTap?.Invoke();
        }

        void _OnDelete()
        {
            _onDelete?.Invoke(_index);
        }

        Color _GetLogTypeColor(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    return NoaDebuggerDefine.LogColors.LogError;
                case LogType.Warning:
                    return NoaDebuggerDefine.LogColors.LogWarning;
                case LogType.Log:
                    return NoaDebuggerDefine.LogColors.LogMessage;
            }

            return NoaDebuggerDefine.LogColors.LogMessage;
        }

        void OnDestroy()
        {
            _select = default;
            _logType = default;
            _logString = default;
            _stackTraceString = default;
            _selfButton = default;
            _backGround = default;
            _onClick = default;
            _onPointerDown = default;
            _onLongTap = default;
            _onDelete = default;
        }
    }
}
