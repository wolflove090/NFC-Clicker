using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class LogScrollDrawer : MonoBehaviour
    {
        [SerializeField]
        GameObject _scrollContent;

        [SerializeField, Header("Log scroll")]
        ObjectPoolScroll _logScroll;
        [SerializeField]
        Button _scrollDown;
        [SerializeField]
        GameObject _noDataLabel;

        [SerializeField, Header("Filter")]
        TMP_InputField _filterInput;
        [SerializeField]
        Button _clearFilter;
        List<LogViewLinker.LogPanelInfo> _logInfos;

        bool _isEnable;


        public event UnityAction<string> OnChangeFilterText;

        void _OnValidateUI()
        {
            Assert.IsNotNull(_scrollContent);
            Assert.IsNotNull(_logScroll);
            Assert.IsNotNull(_scrollDown);
            Assert.IsNotNull(_noDataLabel);
            Assert.IsNotNull(_filterInput);
            Assert.IsNotNull(_clearFilter);
        }

        void Awake()
        {
            _OnValidateUI();
            _scrollDown.onClick.RemoveAllListeners();
            _scrollDown.onClick.AddListener(_OnScrollDownToBottom);
            _filterInput.onValueChanged.RemoveAllListeners();
            _filterInput.onValueChanged.AddListener(_OnChangeFilterInput);
            _clearFilter.onClick.RemoveAllListeners();
            _clearFilter.onClick.AddListener(_OnResetFilterText);
        }

        void OnEnable()
        {
            _isEnable = true;
        }

        public void Draw(LogViewLinker linker)
        {
            if (_filterInput.text != linker._filterText)
            {
                _filterInput.SetTextWithoutNotify(linker._filterText);
            }

            var logs = linker._logs;
            bool existLogs = logs != null && logs.Count > 0;
            _scrollContent.SetActive(existLogs);
            _noDataLabel.SetActive(!existLogs);

            if (existLogs)
            {
                bool scrollReset = _logScroll.verticalNormalizedPosition <= 0.01f &&
                                   !_scrollDown.gameObject.activeSelf;

                _logInfos = linker._logs;

                if (linker._forceUpdate)
                {
                    _logScroll.Init(_logInfos.Count, _OnRefreshPanel);
                }

                _logScroll.RefreshPanels();

                if (scrollReset || linker._resetScrollPos || _isEnable)
                {
                    _logScroll.verticalNormalizedPosition = 0;
                }
            }

            _isEnable = false;
        }


        void _OnScrollDownToBottom()
        {
            _logScroll.verticalNormalizedPosition = 0;
        }

        void _OnChangeFilterInput(string text)
        {
            OnChangeFilterText?.Invoke(text);
        }

        void _OnResetFilterText()
        {
            _filterInput.text = string.Empty;
        }


        void _OnRefreshPanel(int index, GameObject target)
        {
            if (index >= _logInfos.Count)
            {
                return;
            }

            var panel = target.GetComponent<LogPanel>();

            if (panel == null)
            {
                throw new Exception("Unable to fetch the LogPanel.");
            }

            if (index == _logInfos.Count - 1)
            {
                _scrollDown.gameObject.SetActive(false);
            }

            if (index <= _logInfos.Count - _logScroll.VisiblePanelCountY)
            {
                _scrollDown.gameObject.SetActive(true);
            }

            var log = _logInfos[index];
            panel.Draw(log);
        }
    }
}
