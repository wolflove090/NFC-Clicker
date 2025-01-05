using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class BatteryModel : ModelBase
    {
        static readonly string BatteryModelOnUpdate = "BatteryModelOnUpdate";

        static readonly float BatteryCheckIntervalSec = 30f;
        static readonly int BatteryLevelAverageMinutes = 10;
        static readonly int BatteryDataLength = Mathf.FloorToInt(BatteryModel.BatteryLevelAverageMinutes / TimeUnitConverterModel.SecondsToMinutes(BatteryModel.BatteryCheckIntervalSec));

        List<BatteryLevelData> _batteryLevelDataList;
        float _timeSinceBatteryCheck = BatteryModel.BatteryCheckIntervalSec;

        public BatteryInfo BatteryInfo { get; private set; }

        public UnityAction OnBatteryInfoChanged { get; set; }

        public BatteryModel()
        {
            _ResetBatteryInfo();
            var isProfiling = NoaDebuggerPrefs.GetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsBatteryProfiling, true);
            BatteryInfo.SetIsProfiling(isProfiling);

            _HandleOnUpdate(isProfiling);
        }

        public void Dispose()
        {
            UpdateManager.DeleteAction(BatteryModelOnUpdate);
        }

        void _OnUpdate()
        {
            if (!BatteryInfo.IsProfiling)
            {
                return;
            }

            UnityEngine.BatteryStatus batteryStatus = SystemInfo.batteryStatus;

            if (BatteryInfo.Status == BatteryStatus.Charging &&
                BatteryInfo.StatusCheck(batteryStatus) != BatteryStatus.Charging)
            {
                _ResetBatteryInfo();
                BatteryInfo.StatusUpdate(batteryStatus);
                return;
            }

            if (BatteryInfo.Status != BatteryStatus.Charging &&
                BatteryInfo.StatusCheck(batteryStatus) == BatteryStatus.Charging)
            {
                _ResetBatteryInfo();
                BatteryInfo.StatusUpdate(batteryStatus);
                return;
            }

            BatteryInfo.StartProfiling();

            _timeSinceBatteryCheck += Time.unscaledDeltaTime;
            if (_timeSinceBatteryCheck < BatteryModel.BatteryCheckIntervalSec)
            {
                return;
            }

            _timeSinceBatteryCheck = 0;

            float currentLevelPercent = Mathf.Lerp(0, 100, SystemInfo.batteryLevel);

            var levelData = new BatteryLevelData()
            {
                _batteryLevelPercent = currentLevelPercent,
                _checkTime = DateTime.Now,
            };
            if (_batteryLevelDataList.Count >= BatteryModel.BatteryDataLength)
            {
                _batteryLevelDataList.RemoveAt(0);
            }
            _batteryLevelDataList.Add(levelData);

            var oldest = _batteryLevelDataList.First();
            var latest = _batteryLevelDataList.Last();

            TimeSpan elapsedTime = latest._checkTime - oldest._checkTime;
            float elapsedTimeSec = Mathf.Max(1, (float) elapsedTime.TotalSeconds); 

            if (elapsedTimeSec > TimeUnitConverterModel.MinutesToSeconds(BatteryModel.BatteryLevelAverageMinutes))
            {
                _ResetBatteryInfo();
                return;
            }

            float consumptionPerSeconds = (oldest._batteryLevelPercent - latest._batteryLevelPercent) / elapsedTimeSec;
            float consumptionPerMinute = consumptionPerSeconds * 60;

            consumptionPerMinute = (float)Math.Round(consumptionPerMinute, 3);

            var operatingTimeSec = 0;
            if (consumptionPerMinute != 0)
            {
                var operatingTimeMin = (int) (currentLevelPercent / consumptionPerMinute);
                operatingTimeSec = TimeUnitConverterModel.MinutesToSeconds(operatingTimeMin);
            }

            BatteryInfo.Refresh(batteryStatus, currentLevelPercent, consumptionPerMinute, operatingTimeSec);

            BatteryInfo.StatusUpdate(batteryStatus);
            OnBatteryInfoChanged?.Invoke();
        }

        void _ResetBatteryInfo()
        {
            bool isProfiling = BatteryInfo != null ? BatteryInfo.IsProfiling : true;
            BatteryInfo = new BatteryInfo();
            BatteryInfo.SetIsProfiling(isProfiling);

            _batteryLevelDataList = new List<BatteryLevelData>(BatteryModel.BatteryDataLength);
            _timeSinceBatteryCheck = BatteryModel.BatteryCheckIntervalSec;
        }

        public void ChangeBatteryProfilingState(bool isProfiling)
        {
            BatteryInfo.SetIsProfiling(isProfiling);

            NoaDebuggerPrefs.SetBoolean(NoaDebuggerPrefsDefine.PrefsKeyIsBatteryProfiling, (isProfiling));
            _HandleOnUpdate(isProfiling);
        }

        void _HandleOnUpdate(bool isProfiling)
        {
            if (!BatteryModel.CanProfiling())
            {
                return;
            }

            string key = BatteryModel.BatteryModelOnUpdate;

            if (isProfiling)
            {
                if (UpdateManager.ContainsKey(key))
                {
                    return;
                }

                _ResetBatteryInfo();
                UpdateManager.SetAction(key, _OnUpdate);
            }
            else
            {
                UpdateManager.DeleteAction(key);
            }
        }

        public static bool CanProfiling()
        {
            var batteryStatus = SystemInfo.batteryStatus;
            return batteryStatus != UnityEngine.BatteryStatus.Unknown;
        }

        public sealed class BatteryLevelData
        {
            public float _batteryLevelPercent;

            public DateTime _checkTime;
        }
    }
}
