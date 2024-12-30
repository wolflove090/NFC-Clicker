using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NoaDebugger
{
    sealed class HierarchyPanelScrollHelper<TPanel, TData>
        where TPanel : HierarchyPanelBase<TData>
        where TData : HierarchyPanelDataBase
    {

        ObjectPoolScroll _scroll;
        List<TData> _dataList = new List<TData>();

        bool _isRunningSetPanelWidthCoroutine;
        float _maxLabelLength;

        public HierarchyPanelScrollHelper(ObjectPoolScroll scroll)
        {
            _scroll = scroll;
        }

        public void RefreshScroll(List<TData> dataList)
        {
            _dataList = dataList;
            _scroll.Init(_dataList.Count, _OnRefreshPanel);
            _scroll.RefreshPanels();

            GlobalCoroutine.Run(_WaitAfterSetPanelWidth());
        }

        IEnumerator _WaitAfterSetPanelWidth()
        {
            if(_isRunningSetPanelWidthCoroutine)
            {
                 yield break;
            }
            _isRunningSetPanelWidthCoroutine = true;

            yield return null;

            if (_scroll == null)
            {
                yield break;
            }

            float viewportWidth = _scroll.viewport.rect.width;
            float maxWidth = viewportWidth;
            float maxLabelLength = 0;

            TPanel[] hierarchyPanels =
                _scroll.ShowingPanels.Values.Select((showPanel) => showPanel.GetComponent<TPanel>()).ToArray();

            foreach (TPanel panel in hierarchyPanels)
            {
                if (panel == null)
                {
                    continue;
                }

                float width = panel.GetPanelWidth();
                if (width > maxWidth)
                {
                    maxWidth = width;
                    maxLabelLength = panel.GetLabelLength();
                }
            }

            if (maxWidth == viewportWidth)
            {
                _scroll.OverwriteContentWidth(maxWidth, false);
            }
            else
            {
                _scroll.OverwriteContentWidth(maxWidth);
            }
            foreach (TPanel panel in hierarchyPanels)
            {
                if (panel == null)
                {
                    continue;
                }

                panel.SetPanelWidth(maxWidth);
            }

            _maxLabelLength = maxLabelLength;
            _isRunningSetPanelWidthCoroutine = false;
        }

        void _OnRefreshPanel(int index, GameObject target)
        {
            TPanel panel = target.GetComponent<TPanel>();
            if (panel == null)
            {
                return;
            }

            panel.Draw(_dataList[index]);

            if (_maxLabelLength < panel.GetLabelLength())
            {
                GlobalCoroutine.Run(_WaitAfterSetPanelWidth());
            }
        }
    }
}
