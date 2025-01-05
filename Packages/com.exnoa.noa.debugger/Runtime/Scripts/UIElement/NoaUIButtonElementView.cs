using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class NoaUIButtonElementView : NoaUIElementViewBase<NoaUIButtonElement>
    {
        [SerializeField]
        Button _button;
        [SerializeField]
        TextMeshProUGUI _labelText;

        protected override void OnInitialize(NoaUIButtonElement element)
        {
            _labelText.text = element.Label;
            if (element.OnClick == null)
            {
                _button.onClick.RemoveAllListeners();
                LogModel.LogWarning($"Warning: The OnClick event for the button with key '{element.Key}' is not assigned.");
            }
            else
            {
                _button.onClick.RemoveListener(element.OnClick);
                _button.onClick.AddListener(element.OnClick);
            }
        }
    }
}
