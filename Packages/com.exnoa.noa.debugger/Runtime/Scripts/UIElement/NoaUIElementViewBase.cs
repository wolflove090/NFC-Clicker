using System;
using UnityEngine;

namespace NoaDebugger
{
    abstract class NoaUIElementViewBase<T> : MonoBehaviour, INoaUIElementView where T : INoaUIElement
    {
        protected T _element;
        protected bool _isDisposed;

        public INoaUIElement Element => _element;
        public GameObject GameObject => gameObject == null ? null : gameObject;

        public bool IsDisposed => _isDisposed;

        public void Initialize(T element)
        {
            name = element.Key;
            _element = element;

            OnInitialize(element);
        }

        void Update()
        {
            if (_element == null)
            {
                Destroy(gameObject);
                return;
            }

            OnUpdate();
        }

        void OnDestroy()
        {
            _element = default(T);
            _isDisposed = true;

            Dispose();
        }

        protected virtual void OnInitialize(T element){}

        protected virtual void OnUpdate(){}

        protected virtual void Dispose(){}
    }
}
