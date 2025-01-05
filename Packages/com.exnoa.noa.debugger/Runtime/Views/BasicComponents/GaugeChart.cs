using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace NoaDebugger
{
    [RequireComponent(typeof(RectTransform))]
    sealed class GaugeChart : UIBehaviour
    {
        [SerializeField, Tooltip("Shows gauge value.")]
        RectTransform _gaugeMeter;

        [SerializeField, Tooltip("Parent of gauge meter.")]
        RectTransform _gaugeFrame;

        public float MaxValue
        {
            get => _maxValue;
            set
            {
                if (Math.Abs(_maxValue - value) <= 0.0f)
                {
                    return;
                }

                _maxValue = Mathf.Max(0.0f, value);
                _value = Mathf.Clamp(value, 0.0f, _maxValue);
                _UpdateGaugeTransform();
            }
        }

        public float Value
        {
            get => _value;
            set
            {
                if (Math.Abs(_value - value) <= 0.0f)
                {
                    return;
                }

                _value = Mathf.Clamp(value, 0.0f, _maxValue);
                _UpdateGaugeTransform();
            }
        }

        float _maxValue;
        float _value;

        protected override void Awake()
        {
            Assert.IsNotNull(_gaugeMeter);
            Assert.IsNotNull(_gaugeFrame);
        }

        protected override void OnRectTransformDimensionsChange() => _UpdateGaugeTransform();

        void _UpdateGaugeTransform()
        {
            float frameWidth = _gaugeFrame.rect.width;
            float fillRate = (MaxValue > 0) ? Value / MaxValue : 0;
            float xDelta = frameWidth - (frameWidth * fillRate);
            _gaugeMeter.sizeDelta = new Vector2(-xDelta, 0);
        }
    }
}
