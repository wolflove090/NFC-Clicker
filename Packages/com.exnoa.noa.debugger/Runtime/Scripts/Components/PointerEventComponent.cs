using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NoaDebugger
{
    sealed class PointerEventComponent :
        MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public event UnityAction<PointerEventData> OnPointerDownEvent;
        public event UnityAction<PointerEventData> OnPointerUpEvent;
        public event UnityAction<PointerEventData> OnPointerClickEvent;
        public event UnityAction<PointerEventData> OnPointerEnterEvent;
        public event UnityAction<PointerEventData> OnPointerExitEvent;


        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDownEvent?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPointerUpEvent?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClickEvent?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvent?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvent?.Invoke(eventData);
        }
    }
}
