#if ENABLE_LEGACY_INPUT_MANAGER || !ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace NoaDebugger
{
    static partial class Input
    {
        public static partial void Initialize() { }

        public static partial bool IsButtonUp()
        {
            return !UnityInput.GetMouseButton(0) && UnityInput.touchCount <= 0;
        }

        public static partial bool IsButtonReleased()
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            return UnityInput.touchCount > 0 && UnityInput.GetTouch(0).phase == TouchPhase.Ended;
#else
            return UnityInput.GetMouseButtonUp(0);
#endif
        }

        public static partial Vector2 GetCursorPosition()
        {
            return (UnityInput.touchCount > 1)
                ? UnityInput.GetTouch(0).position
                : UnityInput.mousePosition;
        }
    }
}
#endif
