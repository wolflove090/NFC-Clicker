#if NOA_DEBUGGER && UNITY_EDITOR
using UnityEditor;

namespace NoaDebugger.DebugCommand
{
    [CustomEditor(typeof(CommandScroll))]
    sealed class CommandScrollEditor : Editor
    {
    }  
}
#endif
