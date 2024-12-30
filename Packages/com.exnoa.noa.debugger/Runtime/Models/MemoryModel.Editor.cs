#if UNITY_EDITOR
using UnityEngine.Profiling;

namespace NoaDebugger
{
    sealed partial class MemoryModel
    {
        static public partial long? _GetCurrentMemoryByte()
        {
            return Profiler.GetTotalAllocatedMemoryLong();
        }
    }
}
#endif
