using UnityEngine;

namespace NoaDebugger
{
    abstract class NoaDebuggerToolViewBase<T> : ViewBase<T> where T : ViewLinkerBase
    {
        [Header("UIReverseComponents")]
        [SerializeField] UIReverseComponents _reverseComponents;

        public virtual void AlignmentUI(bool isReverse)
        {
            _reverseComponents.Alignment(isReverse);
        }
    }
}
