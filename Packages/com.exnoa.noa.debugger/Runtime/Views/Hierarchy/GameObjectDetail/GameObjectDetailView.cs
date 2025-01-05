using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace NoaDebugger
{
    sealed class GameObjectDetailView : MonoBehaviour
    {
        [SerializeField] ObjectPoolScroll _componentScroll;
        [SerializeField] TextMeshProUGUI _componentCountLabel;
        [SerializeField] ToggleButtonBase _detailLockToggle;
        [SerializeField] GameObject _noSelectedLabel;
        [SerializeField] ToggleButtonBase _activeSwitchToggle;

        HierarchyPanelScrollHelper<GameObjectDetailPanel, GameObjectDetailPanelData> _scrollHelper;

        public event System.Action<bool> OnDetailLock;
        public event System.Action<bool> OnSwitchSelectedObjectActive;

        void _OnValidateUI()
        {
            Assert.IsNotNull(_componentScroll);
            Assert.IsNotNull(_componentCountLabel);
            Assert.IsNotNull(_detailLockToggle);
            Assert.IsNotNull(_noSelectedLabel);

            if(_scrollHelper == null)
            {
                _scrollHelper = new HierarchyPanelScrollHelper<GameObjectDetailPanel, GameObjectDetailPanelData>(_componentScroll);
            }
        }

        public void Show(GameObjectDetail detail)
        {
            _OnValidateUI();

            _detailLockToggle._onClick.RemoveAllListeners();
            _detailLockToggle._onClick.AddListener(_OnDetailLock);
            _detailLockToggle.Init(detail._isLock);
            _activeSwitchToggle._onClick.RemoveAllListeners();
            _activeSwitchToggle._onClick.AddListener(_OnSwitchSelectedObjectActive);
            _activeSwitchToggle.Init(detail._isActive);

            if (detail._componentPanelList != null && detail._componentPanelList.Count > 0)
            {
                _noSelectedLabel.SetActive(false);
                _detailLockToggle.Interactable = true;
                _activeSwitchToggle.Interactable = true;

                _componentCountLabel.text = detail._componentNum.ToString();

                _componentScroll.gameObject.SetActive(true);
                _scrollHelper.RefreshScroll(detail._componentPanelList);
            }
            else
            {
                _noSelectedLabel.SetActive(true);
                _detailLockToggle.Init(false);
                _detailLockToggle.Interactable = false;
                _activeSwitchToggle.Interactable = false;
                _componentScroll.gameObject.SetActive(false);

                _componentCountLabel.text = NoaDebuggerDefine.HyphenValue;
            }
        }

        void _OnDetailLock(bool isLock)
        {
            OnDetailLock?.Invoke(isLock);
        }

        void _OnSwitchSelectedObjectActive(bool isActive)
        {
            OnSwitchSelectedObjectActive?.Invoke(isActive);
        }
    }
}
