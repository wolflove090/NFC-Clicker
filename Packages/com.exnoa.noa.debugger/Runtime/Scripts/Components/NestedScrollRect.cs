using UnityEngine.UI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NoaDebugger
{
    sealed class NestedScrollRect : ScrollRect
    {
        bool _routeToParent = false;

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            DoParentEventSystemHandler<IInitializePotentialDragHandler>(
                transform, parent => parent.OnInitializePotentialDrag(eventData));
            base.OnInitializePotentialDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_routeToParent)
            {
                DoParentEventSystemHandler<IDragHandler>(transform, parent => parent.OnDrag(eventData));
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
            {
                _routeToParent = true;
            }
            else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
            {
                _routeToParent = true;
            }
            else
            {
                _routeToParent = false;
            }
            if (_routeToParent)
            {
                DoParentEventSystemHandler<IBeginDragHandler>(transform, parent => parent.OnBeginDrag(eventData));
            }
            else
            {
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_routeToParent)
            {
                DoParentEventSystemHandler<IEndDragHandler>(transform, parent => parent.OnEndDrag(eventData));
            }
            else
            {
                base.OnEndDrag(eventData);
            }
            _routeToParent = false;
        }

        static void DoParentEventSystemHandler<T>(Transform transform, Action<T> action)
            where T : IEventSystemHandler
        {
            Transform parent = transform.parent;
            while (parent != null)
            {
                foreach (Component component in parent.GetComponents<Component>())
                {
                    if (component is T)
                    {
                        action((T)(IEventSystemHandler)component);
                    }
                }
                parent = parent.parent;
            }
        }
    }
}
