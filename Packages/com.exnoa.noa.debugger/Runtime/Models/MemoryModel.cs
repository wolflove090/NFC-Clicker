using System;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed partial class MemoryModel : ModelBase
    {
        public static readonly int UpdateIntervalFrames = 20;

        static readonly string MemoryModelOnUpdate = "MemoryModelOnUpdate";

        static public partial long? _GetCurrentMemoryByte();

        int _framesSinceLastUpdate = MemoryModel.UpdateIntervalFrames;
        long _totalMemoryUsageMB;
        int _memoryCheckCount;

        public MemoryInfo MemoryInfo { get; private set; }

        public UnityAction OnMemoryInfoChanged { get; set; }

        public MemoryModel()
        {
            MemoryInfo = new MemoryInfo();
            bool isProfiling = NoaDebuggerPrefs.GetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsMemoryProfiling, true);
            bool isGraphShowing = NoaDebuggerPrefs.GetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsMemoryGraphShowing, true);
            MemoryInfo.ToggleProfiling(isProfiling);
            MemoryInfo.ToggleGraphShowing(isGraphShowing);
            _HandleOnUpdate(isProfiling);
        }

        public void Dispose()
        {
            UpdateManager.DeleteAction(MemoryModelOnUpdate);
        }

        void _OnUpdate()
        {
            if (!MemoryInfo.IsProfiling)
            {
                return;
            }

            if (++_framesSinceLastUpdate < MemoryModel.UpdateIntervalFrames)
            {
                return;
            }

            MemoryInfo.StartProfiling();

            _framesSinceLastUpdate = 0;

            long? currentMemByte = MemoryModel._GetCurrentMemoryByte();
            if (null == currentMemByte)
            {
                return;
            }
            float currentMemMB = MemoryModel.GetRoundedMemoryMB(currentMemByte.Value);

            int addMemoryUsage = Mathf.FloorToInt(currentMemMB * 100);
            _totalMemoryUsageMB += addMemoryUsage;
            _memoryCheckCount++;

            if (currentMemByte == -1)
            {
                currentMemMB = -1;
            }

            float maxMemMB = Mathf.Max(currentMemMB, MemoryInfo.MaxMemoryMB);

            float minMemMB = Mathf.Min(currentMemMB, MemoryInfo.MinMemoryMB);

            float averageMemMB = (float)_totalMemoryUsageMB / _memoryCheckCount;
            averageMemMB /= 100;
            averageMemMB = (float)Math.Round(averageMemMB, 2);

            MemoryInfo.Refresh(currentMemMB, maxMemMB, minMemMB, averageMemMB);
            OnMemoryInfoChanged?.Invoke();
        }

        void _ResetMemoryInfo()
        {
            MemoryInfo.ResetProfiledValue();
            _framesSinceLastUpdate = MemoryModel.UpdateIntervalFrames;
            _totalMemoryUsageMB = 0;
            _memoryCheckCount = 0;
        }

        public void ChangeProfilingState(bool isProfiling)
        {
            MemoryInfo.ToggleProfiling(isProfiling);

            NoaDebuggerPrefs.SetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsMemoryProfiling, isProfiling);
            _HandleOnUpdate(isProfiling);
        }

        public void ChangeGraphShowingState(bool isGraphShowing)
        {
            MemoryInfo.ToggleGraphShowing(isGraphShowing);

            NoaDebuggerPrefs.SetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsMemoryGraphShowing, isGraphShowing);
        }

        void _HandleOnUpdate(bool isProfiling)
        {
            if (!MemoryModel.CanProfiling())
            {
                return;
            }

            string key = MemoryModel.MemoryModelOnUpdate;

            if (isProfiling)
            {
                if (UpdateManager.ContainsKey(key))
                {
                    return;
                }

                _ResetMemoryInfo();
                UpdateManager.SetAction(key, _OnUpdate);
            }
            else
            {
                UpdateManager.DeleteAction(key);
            }
        }

        public static bool CanProfiling()
        {
            bool canProfiling = Application.platform == RuntimePlatform.Android ||
                                Application.platform == RuntimePlatform.IPhonePlayer ||
                                Application.platform == RuntimePlatform.WindowsPlayer ||
                                Application.platform == RuntimePlatform.OSXEditor ||
                                Application.platform == RuntimePlatform.WindowsEditor;

            return canProfiling;
        }

        static float GetRoundedMemoryMB(long memByte)
        {
            return (float)Math.Round(DataUnitConverterModel.ByteToMB(memByte), 2);
        }
    }
}
