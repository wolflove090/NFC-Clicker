using System;
using TMPro;
using UnityEngine;

namespace NoaDebugger
{
    sealed class NoaUITextElementView : NoaUIElementViewBase<NoaUITextElement>
    {
        [SerializeField]
        TextMeshProUGUI _textComponent;

        float _nextUpdateTime;

        protected override void OnInitialize(NoaUITextElement element)
        {
            _nextUpdateTime = Time.time;
            _UpdateValue();
        }

        protected override void OnUpdate()
        {
            if (Time.time >= _nextUpdateTime)
            {
                _UpdateValue();
                _nextUpdateTime = Time.time + _element.UpdateInterval;
            }
        }

        void _UpdateValue()
        {
            var value = _element.UpdateValue();
            gameObject.SetActive(!string.IsNullOrEmpty(value));

            if (_textComponent != null)
            {
                _textComponent.text = value;
            }
        }
    }
}
