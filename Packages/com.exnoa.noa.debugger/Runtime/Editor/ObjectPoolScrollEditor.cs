#if NOA_DEBUGGER && UNITY_EDITOR
using UnityEditor;

namespace NoaDebugger
{
    [CustomEditor(typeof(ObjectPoolScroll))]
    sealed class ObjectPoolScrollEditor : Editor
    {
    }
}
#endif
