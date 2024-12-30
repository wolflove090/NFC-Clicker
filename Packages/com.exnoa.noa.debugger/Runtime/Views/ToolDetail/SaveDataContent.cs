using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class SaveDataContent : MonoBehaviour
    {
        [SerializeField] Transform _keyRoot;
        [SerializeField] SaveDataKeyPanel _keyPanel;
        [SerializeField] Button _allClearButton;

        List<SaveDataKeyPanel> _keyPanels = new List<SaveDataKeyPanel>();

        void _OnValidate()
        {
            Assert.IsNotNull(_keyRoot);
            Assert.IsNotNull(_keyPanel);
            Assert.IsNotNull(_allClearButton);
        }

        public void Show(string[] targetKeys)
        {
            _DestroyPanels();

            for(int i = 0; i < targetKeys.Length; i++)
            {
                string key = targetKeys[i];
                SaveDataKeyPanel panel = GameObject.Instantiate(_keyPanel, _keyRoot);
                panel.OnRemove += _RemoveAt;
                bool isShowBackground = i % 2 == 0; 
                panel.Show(key, isShowBackground);
                _keyPanels.Add(panel);
            }

            _allClearButton.onClick.RemoveAllListeners();
            _allClearButton.onClick.AddListener(_RemoveAllKeys);
        }

        void _RemoveAt(SaveDataKeyPanel panel)
        {
            NoaDebuggerPrefs.DeleteAt(panel.Key);
            _keyPanels.Remove(panel);
            GameObject.Destroy(panel.gameObject);

            _RefreshPanels();

            NoaDebugger.ShowToast(new ToastViewLinker(){_label = NoaDebuggerDefine.DeleteSaveDataText});
        }

        void _RemoveAllKeys()
        {
            foreach(SaveDataKeyPanel panel in _keyPanels)
            {
                NoaDebuggerPrefs.DeleteAt(panel.Key);
            }

            NoaDebugger.ShowToast(new ToastViewLinker(){_label = NoaDebuggerDefine.DeleteSaveDataText});
            _DestroyPanels();
        }

        void _RefreshPanels()
        {
            for (int i = 0; i < _keyPanels.Count; i++)
            {
                SaveDataKeyPanel panel = _keyPanels[i];
                bool isShowBackground = i % 2 == 0; 
                panel.Refresh(isShowBackground);
            }
        }

        void _DestroyPanels()
        {
            foreach(SaveDataKeyPanel panel in _keyPanels)
            {
                GameObject.Destroy(panel.gameObject);
            }
            _keyPanels.Clear();
        }

        void OnDestroy()
        {
            _keyRoot = default;
            _keyPanel = default;
            _allClearButton = default;
            _keyPanels = default;
        }
    }
}
