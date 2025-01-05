using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class ObjectPoolScroll : ScrollRect
    {
        [SerializeField, Header("Generate panel object")]
        GameObject _panelPrefab;

        [SerializeField, Header("Panel spacing")]
        float _panelSpace;

        [SerializeField, Header("Padding")]
        float _leftPadding;
        [SerializeField]
        float _rightPadding;
        [SerializeField]
        float _topPadding;
        [SerializeField]
        float _bottomPadding;

        int _panelNum;
        Dictionary<int, GameObject> _showingPanels = new Dictionary<int, GameObject>();
        public Dictionary<int, GameObject> ShowingPanels { get { return new Dictionary<int, GameObject>(_showingPanels); } }
        List<GameObject> _reservedPanels = new List<GameObject>();
        Vector2 _panelSize;

        bool _isInit;
        bool _isOnEnable;

        int _showTopIndex;
        int _showBottomIndex;

        const string SHOW_PANEL_NAME = "Panel";
        const string HIDE_PANEL_NAME = "Panel(Reserved)";

        UnityAction<int, GameObject> _refreshPanel;

        public int VisiblePanelCountY
        {
            get
            {
                return Mathf.FloorToInt(viewport.rect.height / _panelSize.y);
            }
        }

        public void Init(int panelNum, UnityAction<int, GameObject> refreshPanel)
        {
            if (!_isInit)
            {
                _refreshPanel = refreshPanel;
                onValueChanged.RemoveAllListeners();
                onValueChanged.AddListener(_ShowPanelsWithinScrollRange);
                _panelPrefab.SetActive(false);
                _isInit = true;
            }

            _panelNum = panelNum;
            _showTopIndex = -1;
            _showBottomIndex = -1;
            _RefreshContent();
            _ResetPanels();
            _ShowPanelsWithinScrollRange(normalizedPosition);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _isOnEnable = true;
        }

        protected override void LateUpdate ()
        {
            base.LateUpdate();

            if (_isOnEnable)
            {
                _ShowPanelsWithinScrollRange(normalizedPosition);
                _isOnEnable = false;
            }
        }

        public void RefreshPanels()
        {
            _RefreshPanels(true);
        }

        public void OverwriteContentWidth(float width, bool isPadding = true)
        {
            var padding = Vector2.zero;
            if (isPadding)
            {
                padding = new Vector2()
                {
                    x = _leftPadding + _rightPadding,
                    y = _topPadding + _bottomPadding,
                };
            }

            var contentSize = new Vector2()
            {
                x = width + padding.x,
                y = content.sizeDelta.y,
            };

            content.sizeDelta = contentSize;
        }

        void _RefreshContent()
        {
            var panelRect = _panelPrefab.GetComponent<RectTransform>();
            _PanelStretch(panelRect);

            var padding = new Vector2()
            {
                x = _leftPadding + _rightPadding,
                y = _topPadding + _bottomPadding,
            };

            var panelSizeDelta = panelRect.sizeDelta;
            _panelSize = new Vector2
            {
                x = panelSizeDelta.x,
                y = panelSizeDelta.y + _panelSpace,
            };

            var contentSize = new Vector2()
            {
                x = _panelSize.x + padding.x,
                y = _panelSize.y * _panelNum + padding.y - _panelSpace,
            };

            var contentAnchor = new Vector2(0, 1);
            content.anchorMax = contentAnchor;
            content.anchorMin = contentAnchor;
            content.pivot = contentAnchor;
            content.sizeDelta = contentSize;
        }

        void _ShowPanelsWithinScrollRange(Vector2 normalizedPos)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (!_isInit)
            {
                return;
            }

            var contentRect = content.rect;
            var viewPortRect = viewport.rect;

            var scrollAreaSize = new Vector2()
            {
                x = contentRect.width - viewPortRect.width,
                y = contentRect.height - viewPortRect.height,
            };

            Vector2 scrollPos = scrollAreaSize * normalizedPos;

            var scrollUpper = new Vector2()
            {
                x = scrollPos.x + viewPortRect.width,
                y = scrollPos.y + viewPortRect.height,
            };

            var scrollLower = new Vector2()
            {
                x = scrollPos.x - _panelSize.x,
                y = scrollPos.y - _panelSize.y,
            };

            int showTopIndex = -1;
            int showBottomIndex = -1;

            for(int i = 0; i < _panelNum; i++)
            {
                float panelPosY = _panelSize.y * (_panelNum - i - 1);

                bool isShow = true;
                isShow &= scrollUpper.y > panelPosY; 
                isShow &= scrollLower.y < panelPosY; 

                if (isShow)
                {
                    if (_showTopIndex > i || i > _showBottomIndex)
                    {
                        _ShowPanel(i);
                    }

                    if (showTopIndex == -1)
                    {
                        showTopIndex = i;
                        showBottomIndex = i;
                        _ShowPanel(i);
                    }

                    showTopIndex = showTopIndex > i ? i : showTopIndex;
                    showBottomIndex = showBottomIndex > i ? showBottomIndex : i;
                }
                else
                {
                    if (_showTopIndex <= i && i <= _showBottomIndex)
                    {
                        _HidePanel(i);
                    }
                }
            }

            _showTopIndex = showTopIndex;
            _showBottomIndex = showBottomIndex;

            _RefreshPanels();
        }

        void _ShowPanel(int index)
        {
            if (_showingPanels.ContainsKey(index))
            {
                return;
            }

            GameObject panel = null;

            if (_reservedPanels.Count > 0)
            {
                panel = _reservedPanels[0];
                _reservedPanels.RemoveAt(0);
            }
            else
            {
                panel = GameObject.Instantiate(_panelPrefab, content);
            }

            _showingPanels.Add(index, panel);
            _refreshPanel?.Invoke(index, panel);
            panel.SetActive(true);
        }

        void _HidePanel(int index)
        {
            if (!_showingPanels.ContainsKey(index))
            {
                return;
            }

            var panel = _showingPanels[index];

            _showingPanels.Remove(index);
            _reservedPanels.Add(panel);
            panel.name = ObjectPoolScroll.HIDE_PANEL_NAME;
            panel.SetActive(false);
        }

        void _PanelStretch(RectTransform panelRect)
        {
            var padding = new Vector2()
            {
                x = _leftPadding + _rightPadding,
                y = _topPadding + _bottomPadding,
            };

            var panelStretch = new Vector2()
            {
                x = viewport.rect.width - padding.x, 
                y = panelRect.rect.height,
            };
            panelRect.sizeDelta = panelStretch;
        }

        void _RefreshPanels(bool forceRefresh = false)
        {
            var panelAnchor = new Vector2(0, 1);

            foreach(var panel in _showingPanels)
            {
                var rect = panel.Value.GetComponent<RectTransform>();

                rect.gameObject.name = ObjectPoolScroll.SHOW_PANEL_NAME + panel.Key;
                rect.anchorMax = panelAnchor;
                rect.anchorMin = panelAnchor;
                rect.pivot = panelAnchor;
                _PanelStretch(rect);

                var padding = new Vector2()
                {
                    x = _leftPadding,
                    y = _topPadding,
                };

                var panelPos = new Vector2()
                {
                    x = padding.x,
                    y = -_panelSize.y * panel.Key - padding.y,
                };
                rect.transform.localPosition = panelPos;

                if (forceRefresh)
                {
                    _refreshPanel?.Invoke(panel.Key, panel.Value);
                }
            }
        }

        void _ResetPanels()
        {
            foreach(var showPanel in _showingPanels)
            {
                var panel = showPanel.Value;
                panel.SetActive(false);
                _reservedPanels.Add(showPanel.Value);
            }
            _showingPanels.Clear();
        }
    }
}
