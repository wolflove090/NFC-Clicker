using UnityEngine;

namespace NoaDebugger
{
    [RequireComponent(typeof(NoaDebuggerInputField))]
    sealed class NoaInputFieldEventConfigurator : MonoBehaviour
    {
        [SerializeField]
        bool _activateOnPointerDown = true;

        [SerializeField]
        GameObject _dragTarget;

        NoaDebuggerInputField _inputField;

        void Start()
        {
            var inputField = GetComponent<NoaDebuggerInputField>();
            inputField.SetActivateOnPointerDown(_activateOnPointerDown);
            inputField.SetDragTarget(_dragTarget);
        }
    }
}

