using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace NoaDebugger
{
    [RequireComponent(typeof(RectTransform))]
    sealed class StackedAreaChartRuler : MonoBehaviour
    {
        [SerializeField]
        NoaDebuggerText _label;

        RectTransform _rectTransform;

        void Awake()
        {
            Assert.IsNotNull(_label);

            _rectTransform = GetComponent<RectTransform>();
        }

        public RectTransform RectTransform => _rectTransform;

        public string Text
        {
            get => _label.text;
            set => _label.text = value;
        }

        void OnDestroy()
        {
            _label = default;
            _rectTransform = default;
        }
    }
}
