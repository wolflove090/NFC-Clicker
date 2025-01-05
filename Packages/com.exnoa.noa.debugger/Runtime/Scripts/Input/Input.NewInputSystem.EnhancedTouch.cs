#if !ENABLE_LEGACY_INPUT_MANAGER && ENABLE_INPUT_SYSTEM
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace NoaDebugger
{
    static partial class Input
    {
        public static partial void Initialize()
        {
            EnhancedTouchSupport.Enable();
        }

        public static partial bool IsButtonUp()
        {
            return Touch.activeTouches.Count <= 0;
        }

        public static partial  bool IsButtonReleased()
        {
            return Touch.activeTouches.Count > 0
                   && Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Ended;
        }

        public static partial Vector2 GetCursorPosition()
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
    }
}
#endif
#endif
