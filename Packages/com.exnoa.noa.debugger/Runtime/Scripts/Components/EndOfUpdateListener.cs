using System;
using UnityEngine;

namespace NoaDebugger
{
    sealed class EndOfUpdateListener : MonoBehaviour
    {
        public event Action OnLateUpdate;

        void LateUpdate() => OnLateUpdate?.Invoke();
    }
}
