using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    [RequireComponent(typeof(Button))]
    abstract class ToggleButtonBase : MonoBehaviour
    {
        [Serializable]
        public sealed class ToggleClickEvent : UnityEvent<bool> { }

        public bool IsOn { private set; get; }

        public ToggleClickEvent _onClick;

        protected Button _component;

        [SerializeField, Tooltip("(Optional) The group for exclusive control. When selected, all other toggles in the same group will be deselected.")]
        ToggleButtonGroup _group;

        public bool Interactable
        {
            get
            {
                if (_component == null)
                {
                    _component = GetComponent<Button>();
                }

                return _component.interactable;
            }
            set
            {
                if (_component == null)
                {
                    _component = GetComponent<Button>();
                }

                _component.interactable = value;
                _Refresh();
            }
        }

        protected virtual void Awake()
        {
            _component = GetComponent<Button>();
            _Refresh();
            _component.onClick.AddListener(_Toggle);
        }

        void OnEnable()
        {
            if (_group != null)
            {
                _group.Add(this);
            }
        }

        void OnDisable()
        {
            if (_group != null)
            {
                _group.Remove(this);
            }
        }

        public void Init(bool isOn)
        {
            IsOn = isOn;
            _Refresh();
        }

        public void Clear()
        {
            IsOn = false;
            _onClick?.Invoke(IsOn);
            _Refresh();
        }

        void _Toggle()
        {
            if (IsOn && _group != null)
            {
                return;
            }

            IsOn = !IsOn;

            if (IsOn && _group != null)
            {
                _group.Select(this);
            }

            _onClick?.Invoke(IsOn);
            _Refresh();
        }

        protected abstract void _Refresh();

        void OnDestroy()
        {
            _component = default;
            _group = default;
            _onClick = default;
        }
    }
}
