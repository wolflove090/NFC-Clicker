using NoaDebugger.DebugCommand;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class DebugCommandDetailsAreaView : UIBehaviour
    {
        [SerializeField]
        GameObject _groupNameRoot;
        [SerializeField]
        TextMeshProUGUI _groupName;
        [SerializeField]
        ScrollRect _scroll;
        [SerializeField]
        ContextPanelView _panel;
        [SerializeField]
        GameObject _unselectedLabel;

        RectTransform _scrollRectTransform;
        VerticalLayoutGroup _contentLayout;
        RectTransform _verticalScrollbarRect;

        List<ContextPanelView> _instantiatedPanels = new List<ContextPanelView>();

        bool _isInit = false;

        void _OnValidateUI()
        {
            Assert.IsNotNull(_groupNameRoot);
            Assert.IsNotNull(_groupName);
            Assert.IsNotNull(_scroll);
            Assert.IsNotNull(_panel);
            Assert.IsNotNull(_unselectedLabel);
        }

        public void Init()
        {
            _OnValidateUI();

            _scrollRectTransform = _scroll.GetComponent<RectTransform>();
            _contentLayout = _scroll.content.GetComponent<VerticalLayoutGroup>();
            _verticalScrollbarRect = _scroll.verticalScrollbar.GetComponent<RectTransform>();

            _isInit = true;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (!_isInit)
            {
                return;
            }

            float lineMinWidth = _GetLineMinWidth();
            foreach (ContextPanelView panel in _instantiatedPanels)
            {
                panel.SetMinWidthForLine(lineMinWidth);
            }
        }

        public void Show(DebugCommandViewLinker linker)
        {
            _groupName.text = linker._selectedGroupForDetail;
            _groupNameRoot.SetActive(linker._isSelectGroupForDetail);

            int displayCommandsLength = linker._displayDetailCommands?.Length ?? 0;
            if (displayCommandsLength > _instantiatedPanels.Count)
            {
                int addPanelCount = displayCommandsLength - _instantiatedPanels.Count;
                for (int i = 0; i < addPanelCount; i++)
                {
                    _instantiatedPanels.Add(Instantiate(_panel, _scroll.content));
                }
            }

            float lineMinWidth = _GetLineMinWidth();

            for (int i = 0; i < _instantiatedPanels.Count; i++)
            {
                ContextPanelView currentPanel = _instantiatedPanels[i];

                if (linker._displayDetailCommands != null && i < displayCommandsLength)
                {
                    ICommand currentCommand = linker._displayDetailCommands[i];
                    currentPanel.SetText(
                        currentCommand.DisplayName,
                        currentCommand.CreateDetailContext(),
                        suffix: currentCommand.GetDetailSuffix(),
                        missingValue: "None");
                    currentPanel.SetMinWidthForLine(lineMinWidth);
                    currentPanel.gameObject.SetActive(true);
                }
                else
                {
                    currentPanel.gameObject.SetActive(false);
                }
            }

            _unselectedLabel.SetActive(linker._displayDetailCommands == null);

            gameObject.SetActive(true);
        }

        float _GetLineMinWidth()
        {
            return _scrollRectTransform.rect.width -
                   (_verticalScrollbarRect.rect.width - _scroll.verticalScrollbarSpacing) -
                   (_contentLayout.padding.left + _contentLayout.padding.right);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            foreach (ContextPanelView panel in _instantiatedPanels)
            {
                panel.gameObject.SetActive(false);
            }
        }
    }
}
