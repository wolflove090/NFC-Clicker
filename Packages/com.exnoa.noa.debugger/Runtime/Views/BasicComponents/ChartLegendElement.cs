using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class ChartLegendElement : MonoBehaviour
    {
        [SerializeField]
        NoaDebuggerText _label;

        [SerializeField]
        Image _icon;

        void Awake()
        {
            Assert.IsNotNull(_label);
            Assert.IsNotNull(_icon);
        }

        public string Text
        {
            get => _label.text;
            set => _label.text = value;
        }

        public Color Color
        {
            get => _icon.color;
            set => _icon.color = value;
        }

        void OnDestroy()
        {
            _label = default;
            _icon = default;
        }
    }
}
