using UnityEngine;
using UnityEngine.Assertions;

namespace NoaDebugger
{
    sealed class ToggleObjectButton : ToggleButtonBase
    {
        [SerializeField, Tooltip("(Required) The appearance when the toggle is off.")]
        GameObject _off;
        [SerializeField, Tooltip("(Required) The appearance when the toggle is on.")]
        GameObject _on;
        [SerializeField, Tooltip("(Optional) The appearance when the toggle button is disabled.")]
        GameObject _disable;

        protected override void Awake()
        {
            Assert.IsNotNull(_on);
            Assert.IsNotNull(_off);
            base.Awake();
        }

        protected override void _Refresh()
        {
            _on.SetActive(IsOn);
            _off.SetActive(!IsOn);

            if(_disable)
            {
                _on.SetActive(IsOn && Interactable);
                _off.SetActive(!IsOn && Interactable);

                _disable.SetActive(!Interactable);
            }
        }
    }
}
