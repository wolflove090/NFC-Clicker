using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class ConsoleLogDetailView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _logDetailText;

        [SerializeField]
        Button _copyButton;

        UnityAction _onClickCopyButton;

        void Awake()
        {
            Assert.IsNotNull(_logDetailText);
            Assert.IsNotNull(_copyButton);

            _copyButton.onClick.RemoveAllListeners();
            _copyButton.onClick.AddListener(_OnCopy);
        }

        public void SetLogDetailText(string text)
        {
            _logDetailText.text = NoaDebuggerText.GetHasFontAssetString(_logDetailText.font, text);
        }

        public void SetCopyButton(UnityAction onClick)
        {
            _onClickCopyButton = onClick;
            _copyButton.gameObject.SetActive(true);
        }

        void _OnCopy()
        {
            _onClickCopyButton?.Invoke();

            _copyButton.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            _logDetailText = default;
            _copyButton = default;
            _onClickCopyButton = default;
        }
    }
}
