#if !UNITY_EDITOR && !UNITY_ANDROID && !UNITY_IOS && !UNITY_STANDALONE_WIN
namespace NoaDebugger
{
    sealed partial class MemoryModel
    {
        static public partial long? _GetCurrentMemoryByte()
        {
            return -1;
        }
    }
}
#endif
