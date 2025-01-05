#if UNITY_ANDROID && !UNITY_EDITOR
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NoaDebugger
{
    sealed partial class MemoryModel
    {
        static SynchronizationContext _synchronizationContext;
        static long? _currentMemByte = null;

        static long AndroidCurrentMemoryByte()
        {
            AndroidJNI.AttachCurrentThread();

            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var staticContext = new AndroidJavaClass("android.content.Context");
            using var service = staticContext.GetStatic<AndroidJavaObject>("ACTIVITY_SERVICE");
            using var activityManager = activity.Call<AndroidJavaObject>("getSystemService", service);
            var process = Process.GetCurrentProcess();
            var pidList = new int[] {process.Id};
            var memoryInfoList = activityManager.Call<AndroidJavaObject[]>("getProcessMemoryInfo", pidList);

            long totalKB = 0;
            foreach (var memoryInfo in memoryInfoList)
            {
                totalKB += memoryInfo.Call<int>("getTotalPss"); 
                memoryInfo.Dispose();
            }

            AndroidJNI.DetachCurrentThread();

            return (long) DataUnitConverterModel.KBToByte(totalKB);
        }

        async static void UpdeteCurrentMemory()
        {
            if (MemoryModel._synchronizationContext != null)
            {
                return;
            }

            MemoryModel._synchronizationContext = SynchronizationContext.Current;

            var memory = await Task.Run(() => {
                return MemoryModel.AndroidCurrentMemoryByte();
            }).ConfigureAwait(false);

            MemoryModel._synchronizationContext.Post( _ => {
                MemoryModel._currentMemByte = memory;
                MemoryModel._synchronizationContext = null;
            }, null);
        }

        static public partial long? _GetCurrentMemoryByte()
        {
            MemoryModel.UpdeteCurrentMemory();
            return MemoryModel._currentMemByte;
        }
    }
}
#endif
