#if !ENABLE_LEGACY_INPUT_MANAGER && ENABLE_INPUT_SYSTEM
#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)
using UnityEngine;
using UnityEngine.InputSystem;

namespace NoaDebugger
{
    static partial class Input
    {
        public static partial void Initialize() { }

        public static partial bool IsButtonUp()
        {
            return !Mouse.current.leftButton.isPressed;
        }

        public static partial bool IsButtonReleased()
        {
            return Mouse.current.leftButton.wasReleasedThisFrame;
        }

        public static partial Vector2 GetCursorPosition()
        {
            return Mouse.current.position.ReadValue();
        }
    }
}
#endif
#endif
