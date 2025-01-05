#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;

namespace NoaDebugger
{
    sealed partial class MemoryModel
    {
        [DllImport("__Internal", EntryPoint = "NoaDebuggerGetCurrentMemoryByte")]
        static extern long _NoaDebuggerGetCurrentMemoryByte();

        static public partial long? _GetCurrentMemoryByte()
        {
            return _NoaDebuggerGetCurrentMemoryByte();
        }
    }
}
#endif
