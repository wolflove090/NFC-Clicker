using System;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    [RequireComponent(typeof(ScrollRect))]
    sealed class ScrollResetter : MonoBehaviour
    {
        ScrollRect _scroll;
        bool _once;
    
        void Awake()
        {
            _scroll = GetComponent<ScrollRect>();
        }
    
        void OnEnable()
        {
            if (_once)
            {
                return;
            }
            
            ResetScroll();
            _once = true;
        }
        
        void OnDisable()
        {
            _once = false;
        }

        public void ResetScroll()
        {
            _scroll.normalizedPosition = new Vector2(0, 1f);
        }
    }
}
