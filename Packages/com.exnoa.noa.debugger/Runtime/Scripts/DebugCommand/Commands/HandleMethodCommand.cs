using System;
using System.Collections;
using System.Reflection;
using UnityEngine.Events;

namespace NoaDebugger.DebugCommand
{
    sealed class HandleMethodCommand : CommandBase, ICommand
    {
        public bool IsInteractable
        {
            get => _isInteractable && !_isWaiting;
            set => _isInteractable = value;
        }

        public bool IsVisible { get; set; } = true;

        protected override string TypeName => "Handle Method";

        readonly Func<MethodHandler> _method;
        bool _isInteractable = true;
        bool _isWaiting;
        UnityAction _completeCallback;
        MethodHandler _handler;

        public HandleMethodCommand(
            string displayName, Func<MethodHandler> method, string groupName = null, int? groupOrder = null,
            string tagName = null, string description = null, int? order = null)
            : base(displayName, groupName, groupOrder, tagName, description, order)
        {
            _method = method;
        }

        public void Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Invoke(UnityAction onComplete)
        {
            try
            {
                _completeCallback = onComplete;
                _isWaiting = true;
                _handler = _method();
                GlobalCoroutine.Run(_WatchHandler());
            }
            catch (TargetInvocationException te)
            {
                if (te.InnerException == null)
                {
                    throw;
                }

                throw te.InnerException;
            }
        }

        IEnumerator _WatchHandler()
        {
            while (_handler != null && !_handler.IsDone)
            {
                yield return null;
            }

            _OnComplete();
        }

        void _OnComplete()
        {
            _isWaiting = false;
            _handler = null;
            _completeCallback?.Invoke();
        }
    }
}
