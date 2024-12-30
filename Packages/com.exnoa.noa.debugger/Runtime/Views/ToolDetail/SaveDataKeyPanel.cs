using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace NoaDebugger
{
    sealed class SaveDataKeyPanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _keyLabel;
        [SerializeField] LongTapButton _removeButton;
        [SerializeField] GameObject _background;

        public string Key{private set; get; }

        public event System.Action<SaveDataKeyPanel> OnRemove;

        void _OnValidate()
        {
            Assert.IsNotNull(_keyLabel);
            Assert.IsNotNull(_removeButton);
            Assert.IsNotNull(_background);
        }

        public void Show(string key, bool isShowBackground)
        {
            _OnValidate();

            Key = key;
            _keyLabel.text = Key;
            _removeButton._onLongTap.RemoveAllListeners();
            _removeButton._onLongTap.AddListener(_OnRemoveLongTap);

            Refresh(isShowBackground);
        }

        public void Refresh(bool isShowBackground)
        {
            _background.SetActive(isShowBackground);
        }

        void _OnRemoveLongTap()
        {
            OnRemove?.Invoke(this);
        }

        void OnDestroy()
        {
            _keyLabel = default;
            _removeButton = default;
            _background = default;
            OnRemove = default;
            Key = default;
        }
    }
}
