using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class MenuTabComponent : MonoBehaviour
    {
        public Button _tabButton;
        public TextMeshProUGUI _label;

        [SerializeField]
        GameObject _selectLine;
        [SerializeField]
        Image _allow;
        [SerializeField]
        GameObject _badge;
        [SerializeField]
        UIReverseComponents _reverseComponents;

        void Awake()
        {
            _OnValidateUI();
        }

        void _OnValidateUI()
        {
            Assert.IsNotNull(_tabButton);
            Assert.IsNotNull(_selectLine);
            Assert.IsNotNull(_allow);
            Assert.IsNotNull(_badge);
            Assert.IsNotNull(_reverseComponents);
        }

        public void ChangeTabSelect(bool isSelect)
        {
            _selectLine.SetActive(isSelect);
            _allow.color = isSelect ? NoaDebuggerDefine.ImageColors.Default : NoaDebuggerDefine.ImageColors.Disabled;
        }

        public void ShowNoticeBadge(ToolNotificationStatus notificationStatus)
        {
            bool isShow = notificationStatus == ToolNotificationStatus.Error;
            _badge.SetActive(isShow);
        }

        void OnDestroy()
        {
            _tabButton = default;
            _label = default;
            _selectLine = default;
            _allow = default;
            _badge = default;
        }

        public void AlignmentUI(bool isReverse)
        {
            _reverseComponents.Alignment(isReverse);
        }
    }
}
