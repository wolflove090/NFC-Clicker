using System;
using System.Collections.Generic;
using UnityEngine;

namespace NoaDebugger
{
    sealed class DeviceOrientationManager : MonoBehaviour
    {
        static DeviceOrientationManager _instance;
        Dictionary<string, System.Action<bool>> _actionDic = new Dictionary<string, System.Action<bool>>();
        List<string> _deleteActionKeyList = new List<string>();
        Orientation _orientation = Orientation.None;

        enum Orientation
        {
            None = 0,
            Portrait = 1,
            Landscape = 2,
        }

        public static bool IsPortrait
        {
            get
            {
                if (_instance == null || _instance._orientation == Orientation.None)
                {
                    Orientation orientation = Screen.width < Screen.height ? Orientation.Portrait : Orientation.Landscape;
                    return orientation == Orientation.Portrait;
                }

                return _instance._orientation == Orientation.Portrait;
            }
        }

        public static void Init(Transform parent)
        {
            var obj = new GameObject("NoaDebuggerOrientationManager");
            obj.transform.parent = parent;
            var manager = obj.AddComponent<DeviceOrientationManager>();
            _instance = manager;
        }

        public static bool ContainsKey(string key)
        {
            if (_instance == null)
            {
                LogModel.DebugLogWarning("Instance has not been created.");
                return false;
            }

            return _instance._actionDic.ContainsKey(key);
        }

        public static void SetAction(string key, System.Action<bool> action)
        {
            if (_instance == null)
            {
                LogModel.DebugLogWarning("Instance has not been created.");
                return;
            }

            _instance._SetAction(key, action);
        }
        void _SetAction(string key, System.Action<bool> action)
        {
            _actionDic.Add(key, action);
        }

        public static void DeleteAction(string key)
        {
            if (_instance == null)
            {
                LogModel.DebugLogWarning("Instance has not been created.");
                return;
            }

            _instance._DeleteAction(key);
        }

        void _DeleteAction(string key)
        {
            if (_actionDic.ContainsKey(key))
            {
                _deleteActionKeyList.Add(key);
            }
        }

        void _WatchDeviceOrientation()
        {
            Orientation orientation = Screen.width < Screen.height ? Orientation.Portrait : Orientation.Landscape;
            if (orientation != _orientation)
            {
                _orientation = orientation;

                foreach (var kv in _actionDic)
                {
                    try
                    {
                        var action = kv.Value;
                        action?.Invoke(IsPortrait);
                    }
                    catch (Exception e)
                    {
                        LogModel.CollectNoaDebuggerErrorLog($"{e.Message},{kv.Key}", e.StackTrace);
                        NoaDebuggerManager.DetectError();
                        throw new Exception(e.Message, e);
                    }
                }
            }

            foreach (var deleteKey in _deleteActionKeyList)
            {
                if (_actionDic.ContainsKey(deleteKey))
                {
                    _actionDic.Remove(deleteKey);
                }
            }
            _deleteActionKeyList.Clear();
        }

        void Update()
        {
            _WatchDeviceOrientation();
        }

        void OnDestroy()
        {
            DeviceOrientationManager._instance = null;
        }
    }
}
