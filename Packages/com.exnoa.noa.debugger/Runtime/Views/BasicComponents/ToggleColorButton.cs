using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class ToggleColorButton : ToggleButtonBase
    {
        [SerializeField]
        Color _off = NoaDebuggerDefine.ImageColors.Default;
        [SerializeField]
        Color _on = NoaDebuggerDefine.ImageColors.Default;
        [SerializeField]
        Color _disable = NoaDebuggerDefine.ImageColors.Disabled;
        [SerializeField]
        bool _isUseDisable = false;
        [SerializeField]
        Graphic[]  _targetGraphics;

        protected override void Awake()
        {
            foreach (Graphic graphic in _targetGraphics)
            {
                Assert.IsNotNull(graphic);
            }
            base.Awake();
        }

        protected override void _Refresh()
        {
            Color newColor;
            if (_isUseDisable && !Interactable)
            {
                newColor = _disable;
            }
            else
            {
                newColor = IsOn ? _on : _off;
            }

            foreach (Graphic graphic in _targetGraphics)
            {
                graphic.color = newColor;
            }
        }
    }
}
