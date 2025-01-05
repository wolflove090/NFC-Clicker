using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class ThermalModel : ModelBase
    {
        static readonly string ThermalModelOnUpdate = "ThermalModelOnUpdate";
        static readonly float ThermalCheckIntervalSec = 0.5f;

        float _timeSinceThermalCheck = ThermalModel.ThermalCheckIntervalSec;

#pragma warning disable 0414
        float _totalTemperature;
        int _temperatureCount;
#pragma warning restore

        public ThermalInfo ThermalInfo { get; private set; }

        public UnityAction OnThermalInfoChanged { get; set; }

        public ThermalModel()
        {
            _ResetThermalInfo();
            var isProfiling = NoaDebuggerPrefs.GetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsThermalProfiling, true);
            ThermalInfo.SetIsProfiling(isProfiling);
            _HandleOnUpdate(isProfiling);
        }

        public void Dispose()
        {
            UpdateManager.DeleteAction(ThermalModelOnUpdate);
        }

        void _OnUpdate()
        {
            if (!ThermalInfo.IsProfiling)
            {
                return;
            }
            _timeSinceThermalCheck += Time.unscaledDeltaTime;
            if (_timeSinceThermalCheck < ThermalModel.ThermalCheckIntervalSec)
            {
                return;
            }
            ThermalInfo.StartProfiling();
            _timeSinceThermalCheck = 0;

            _OnUpdateThermalInfo();
            OnThermalInfoChanged?.Invoke();
        }

        void _ResetThermalInfo()
        {
            bool isProfiling = ThermalInfo != null ? ThermalInfo.IsProfiling : true;
            ThermalInfo = new ThermalInfo();
            ThermalInfo.SetIsProfiling(isProfiling);
            _timeSinceThermalCheck = ThermalModel.ThermalCheckIntervalSec;
            _totalTemperature = 0;
            _temperatureCount = 0;
        }

        public void ChangeProfilingState(bool isProfiling)
        {
            ThermalInfo.SetIsProfiling(isProfiling);

            NoaDebuggerPrefs.SetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsThermalProfiling, isProfiling);
            _HandleOnUpdate(isProfiling);
        }

        void _HandleOnUpdate(bool isProfiling)
        {
            if (!ThermalModel.CanProfiling())
            {
                return;
            }
            string key = ThermalModel.ThermalModelOnUpdate;

            if (isProfiling)
            {
                if (UpdateManager.ContainsKey(key))
                {
                    return;
                }
                _ResetThermalInfo();

                UpdateManager.SetAction(key, _OnUpdate);
            }

            else
            {
                UpdateManager.DeleteAction(key);
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        float GetCurrentTemperature()
        {
            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

            using AndroidJavaObject intentFilter =
                new AndroidJavaObject("android.content.IntentFilter", "android.intent.action.BATTERY_CHANGED");
            using AndroidJavaObject intent = context.Call<AndroidJavaObject>("registerReceiver", null, intentFilter);

            int temperature = intent.Call<int>("getIntExtra", "temperature", -1);

            return (float) temperature / 10;
        }

        void _OnUpdateThermalInfo()
        {
            float currentTemp = GetCurrentTemperature();
            _totalTemperature += currentTemp;
            _temperatureCount++;

            var averageTemp = _totalTemperature / _temperatureCount;
            averageTemp = (float) Math.Round(averageTemp, 2);

            ThermalInfo.RefreshTemperature(currentTemp, averageTemp);
            ThermalInfo.RefreshThermalStatus(ThermalStatus.Nominal);
        }
#elif UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal", EntryPoint = "getThermalState")]
        static extern int GetCurrentThermalState();

        void _OnUpdateThermalInfo()
        {
            int currentProcessState = GetCurrentThermalState();
            var currentProfilerThermalState = _ToProfilerThermalStatus(currentProcessState);

            ThermalInfo.RefreshThermalStatus(currentProfilerThermalState);
        }

        ThermalStatus _ToProfilerThermalStatus(int processInfoThermalState)
        {
            switch (processInfoThermalState)
            {
                case 0:
                    return ThermalStatus.Nominal;

                case 1:
                    return ThermalStatus.Fair;

                case 2:
                    return ThermalStatus.Serious;

                case 3:
                    return ThermalStatus.Critical;

                default:
                    return ThermalStatus.Unknown;
            }
        }
#else
        void _OnUpdateThermalInfo()
        {
            ThermalInfo.RefreshTemperature(-1, 0);
            ThermalInfo.RefreshThermalStatus(ThermalStatus.Unknown);
        }
#endif

        public static bool CanProfiling()
        {
            bool canProfiling = Application.platform == RuntimePlatform.Android ||
                                Application.platform == RuntimePlatform.IPhonePlayer;
            return canProfiling;
        }
    }
}
