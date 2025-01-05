using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NoaDebugger
{
    class UIBehaviourComponent : UIBehaviour
    {
        public event UnityAction OnRectTransformDimensionsChanged;

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            OnRectTransformDimensionsChanged?.Invoke();
        }
    }
}
