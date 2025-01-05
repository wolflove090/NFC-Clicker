using UnityEngine;

namespace NoaDebugger
{
    sealed class FloatingHierarchyPanel : HierarchyPanel
    {
        protected override void _Draw(HierarchyPanelData panelData)
        {
            base._Draw(panelData);

            _selectButton.onClick.RemoveAllListeners();
            _selectButton.onClick.AddListener(_OnSelectButton);
        }

        void _OnSelectButton()
        {
            if (_data._hasChildren)
            {
                _data._entry.SwitchToggle();
            }

            _OnSelect();
            if (_data._entry._gameObject == null)
            {
                _data._entry._callback._onUpdateView();
            }

        }
    }
}
