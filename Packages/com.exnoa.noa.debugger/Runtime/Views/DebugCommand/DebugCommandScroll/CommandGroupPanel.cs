using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoaDebugger.DebugCommand
{
    sealed class CommandGroupPanel : UIBehaviour
    {
        [Header("Root")]
        [SerializeField] RectTransform _root;
        [SerializeField] VerticalLayoutGroup _rootLayout;

        [Header("Header")]
        [SerializeField] RectTransform _headerRoot;
        [SerializeField] TextMeshProUGUI _groupName;
        [SerializeField] ToggleButtonBase _collapseToggle;
        [SerializeField] ToggleButtonBase _floatingToggle;
        [SerializeField] ToggleButtonBase _detailToggle;

        [Header("CommandPanel")]
        [SerializeField] CommandPanelsParent _panelFormatCommandPanelsParent;
        [SerializeField] CommandPanelsParent _listFormatCommandPanelsParent;

        [Header("EndLine")]
        [SerializeField] GameObject _groupEndLineObject;

        [Header("ContentSpace")]
        [SerializeField] float _verticalSpace;
        [SerializeField] float _horizontalSpace;
        Vector2 _space;

        readonly Vector2 _panelsAnchor = new(0, 1);

        float MaxContentWidth => _root.rect.width;
        float ParentWidth => MaxContentWidth - _space.x;
        GroupPanelInfo _group;

        public bool IsNeedAlign { get; private set; }


        bool _isInit;

        void _Init()
        {
            if (_isInit)
            {
                return;
            }

            _isInit = true;

            _space = new Vector2()
            {
                x = _horizontalSpace,
                y = _verticalSpace,
            };

            _panelFormatCommandPanelsParent.Init();
            _listFormatCommandPanelsParent.Init();
            _collapseToggle._onClick.AddListener(_OnClickCollapseToggle);
            _floatingToggle.gameObject.SetActive(false);
            _floatingToggle._onClick.AddListener(_OnClickFloatingToggle);
            _detailToggle.gameObject.SetActive(false);
            _detailToggle._onClick.AddListener(_OnClickDetailToggle);
        }


        protected override void OnRectTransformDimensionsChange()
        {
            IsNeedAlign = true;
        }

        public void AlignmentPanels()
        {
            IsNeedAlign = false;

            CommandPanelsParent currentPanelsParent = _GetCurrentPanelsParent();

            foreach (DebugCommandPanel panel in currentPanelsParent.CommandComponents)
            {
                panel.OnUpdateWidth(MaxContentWidth);
            }

            _AlignmentPanels(
                currentPanelsParent.RootTransform,
                currentPanelsParent.CommandComponents.Select(panel => panel.transform as RectTransform).ToList(),
                isForceExpand: DebugCommandPresenter.GetCurrentFormat() == CommandDisplayFormat.List);

            _FitHeight(currentPanelsParent.RootTransform);
        }

        void _AlignmentPanels(RectTransform parent, List<RectTransform> targets, bool isForceExpand)
        {
            Vector2 basePosition = Vector2.zero;
            float panelWidthSum = 0;
            float panelHeightSum = 0;
            float lineHeight = 0;

            foreach (RectTransform target in targets)
            {
                if (target.rect.width >= MaxContentWidth || isForceExpand)
                {
                    LayoutElement layout = target.GetComponent<LayoutElement>();

                    if (layout != null)
                    {
                        layout.preferredWidth = MaxContentWidth;
                    }
                }

                Vector2 sizeDelta = target.sizeDelta;
                float width = sizeDelta.x + _space.x;
                float height = sizeDelta.y + _space.y;

                float nextPanelWidthSum = panelWidthSum + width;

                if (nextPanelWidthSum > MaxContentWidth)
                {
                    basePosition.x = 0;
                    basePosition.y -= lineHeight;
                    panelHeightSum += lineHeight;
                    panelWidthSum = 0;
                    lineHeight = 0;
                }

                target.anchorMax = _panelsAnchor;
                target.anchorMin = _panelsAnchor;
                target.pivot = _panelsAnchor;
                target.localPosition = basePosition;
                panelWidthSum += width;
                basePosition.x += width;

                if (height > lineHeight)
                {
                    lineHeight = height;
                }
            }

            panelHeightSum += lineHeight;

            Vector2 contentSize = new Vector2()
            {
                x = MaxContentWidth,
                y = panelHeightSum,
            };

            parent.sizeDelta = contentSize;
        }

        void _FitHeight(RectTransform targetPanelsParent)
        {
            Canvas.ForceUpdateCanvases();
            float rootHeight = 0;

            rootHeight += _headerRoot.rect.height;

            if (targetPanelsParent.gameObject.activeSelf)
            {
                rootHeight += targetPanelsParent.rect.height + _rootLayout.spacing;
            }

            _root.sizeDelta = new Vector2(_root.rect.width, rootHeight);
        }

        public void RefreshPanels()
        {
            _GetCurrentPanelsParent().RefreshPanels();
        }

        public void RefreshPanelsStatus(GroupPanelInfo group)
        {
            _GetCurrentPanelsParent().RefreshPanelsStatus(group);
        }

        public void RefreshHeader(GroupPanelInfo group, bool isFloatingWindow, float headerHeight)
        {
            _group = group;
            _groupName.text = group._name;

            if (isFloatingWindow)
            {
                _floatingToggle.gameObject.SetActive(false);
                _floatingToggle.Init(false);
                _detailToggle.gameObject.SetActive(false);
                _detailToggle.Init(false);
            }
            else
            {
                _floatingToggle.gameObject.SetActive(group._floatingToggleInfo._isActive);
                _floatingToggle.Init(group._floatingToggleInfo._isOn);
                _detailToggle.gameObject.SetActive(group._detailToggleInfo._isDetailMode);
                _detailToggle.Init(group.IsShowGroupDetail());
            }

            _headerRoot.sizeDelta = new Vector2(ParentWidth, headerHeight);
            _collapseToggle.Init(group._collapseToggleInfo._isOn);
        }

        CommandPanelsParent _GetCurrentPanelsParent()
        {
            return DebugCommandPresenter.GetCurrentFormat() == CommandDisplayFormat.List
                ? _listFormatCommandPanelsParent
                : _panelFormatCommandPanelsParent;
        }


        public void InstantiateGroup(GroupPanelInfo group, bool isFloatingWindow, float headerHeight, bool isLast, CommandScroll scrollRect)
        {
            if (!_isInit)
            {
                _Init();
            }

            RefreshHeader(group, isFloatingWindow, headerHeight);

            _groupEndLineObject.SetActive(!isLast);

            switch (DebugCommandPresenter.GetCurrentFormat())
            {
                case CommandDisplayFormat.Panel:
                    _listFormatCommandPanelsParent.RootTransform.gameObject.SetActive(false);
                    _panelFormatCommandPanelsParent.InstantiateCommands(group, MaxContentWidth, scrollRect);
                    break;

                case CommandDisplayFormat.List:
                    _panelFormatCommandPanelsParent.RootTransform.gameObject.SetActive(false);
                    _listFormatCommandPanelsParent.InstantiateCommands(group, MaxContentWidth, scrollRect, _SetMaxWidth);
                    break;
            }

            if (isLast)
            {
                IsNeedAlign = true;
            }

            _CollapseCommands(group._collapseToggleInfo._isOn);
        }

        void _SetMaxWidth(DebugCommandPanel target)
        {
            if (target.transform is RectTransform targetRect)
            {
                Vector2 newSize = targetRect.sizeDelta;
                newSize.x = ParentWidth;
                targetRect.sizeDelta = newSize;
            }
        }

        void _CollapseCommands(bool isCollapse)
        {
            GameObject target = _GetCurrentPanelsParent().RootTransform.gameObject;

            if (target.activeSelf == isCollapse)
            {
                _GetCurrentPanelsParent().RootTransform.gameObject.SetActive(!isCollapse);

                _FitHeight(_GetCurrentPanelsParent().RootTransform);
            }
        }


        void _OnClickCollapseToggle(bool isOn)
        {
            _group._collapseToggleInfo._isOn = isOn;
            _group._collapseToggleInfo._onChange?.Invoke(_group._name, isOn);
            _CollapseCommands(isOn);
        }

        void _OnClickFloatingToggle(bool isOn)
        {
            _group._floatingToggleInfo._isOn = isOn;
            _group._floatingToggleInfo._onChange?.Invoke(_group._name, isOn);
        }

        void _OnClickDetailToggle(bool isOn)
        {
            _group._detailToggleInfo._onSelectGroup?.Invoke(_group, isOn);
        }


        public void DestroyPanels()
        {
            _panelFormatCommandPanelsParent.DestroyPanels();
            _listFormatCommandPanelsParent.DestroyPanels();
        }
    }
}
