using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NoaDebugger
{
    class DialogBase<T> : ViewBase<T>
            where T : DialogBaseViewLinker
    {
        [SerializeField]
        [Tooltip("(Required) The dialog view.")]
        GameObject _targetView;

        [SerializeField]
        [Tooltip("(Required) The heading text.")]
        TextMeshProUGUI _caption;

        [SerializeField]
        [Tooltip("(Required) The button to close the dialog.")]
        Button _closeButton;

        [SerializeField]
        [Tooltip("An alternative button to close the dialog. Specify if you want to close the dialog with a button other than the close button.")]
        Button _alternativeCloseButton;

        protected override void _Init()
        {
            Assert.IsNotNull(_targetView);
            Assert.IsNotNull(_caption);
            Assert.IsNotNull(_closeButton);

            base._Init();

            _closeButton.onClick.AddListener(Hide);
            if (_alternativeCloseButton)
            {
                _alternativeCloseButton.onClick.AddListener(Hide);
            }
        }

        protected override void _OnShow(T linker)
        {
            base._OnShow(linker);

            _caption.text = linker._caption;
            _closeButton.onClick.AddListener(Hide);
            _targetView.SetActive(true);
        }

        protected override void _OnHide()
        {
            _targetView.SetActive(false);
            _closeButton.onClick.RemoveAllListeners();

            base._OnHide();
        }
    }

    class DialogBaseViewLinker : ViewLinkerBase
    {
        public string _caption;
    }
}
