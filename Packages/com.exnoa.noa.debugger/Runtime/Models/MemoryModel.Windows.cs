#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System.Runtime.InteropServices;

namespace NoaDebugger
{
    sealed partial class MemoryModel
    {
#if UNITY_64
        [DllImport("noa_debugger_memory_watcher", EntryPoint = "GetWorkingSet")]
#else
        [DllImport("noa_debugger_memory_watcher_32", EntryPoint = "GetWorkingSet")]
#endif
        static extern long _GetWorkingSet();

        static public partial long? _GetCurrentMemoryByte()
        {
            return _GetWorkingSet();
        }
    }
}
#endif
