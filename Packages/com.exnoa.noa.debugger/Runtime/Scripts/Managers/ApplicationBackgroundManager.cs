using UnityEngine;
using System;
using System.Collections.Generic;

namespace NoaDebugger
{
    sealed class ApplicationBackgroundManager : MonoBehaviour
    {
        public enum SameKeyActionType
        {
            None,

            Add,

            Override,

            Error,
        }

        static ApplicationBackgroundManager _instance;

        Dictionary<string, Action<bool>> _actionDic = new Dictionary<string, Action<bool>>();

        public bool IsBackground { get; private set; } = false;

        void Awake()
        {
            _instance = this;
        }

        public static void Instantiate(Transform parent)
        {
            var obj = new GameObject("NoaDebuggerBackgroundManager");
            obj.gameObject.transform.parent = parent;
            var manager = obj.AddComponent<ApplicationBackgroundManager>();
            _instance = manager;
        }

        public static void SetAction(string key, Action<bool> action, SameKeyActionType type = SameKeyActionType.None)
        {
            if (_instance == null)
            {
                LogModel.DebugLogWarning("Instance has not been created.");
                return;
            }

            _instance._SetAction(key, action, type);
        }

        void _SetAction(string key, Action<bool> action, SameKeyActionType type)
        {
            if (_actionDic.ContainsKey(key))
            {
                switch (type)
                {
                    case SameKeyActionType.Add:
                        _actionDic[key] += action;
                        break;
                    case SameKeyActionType.Override:
                        _actionDic[key] = action;
                        break;
                    case SameKeyActionType.Error:
                        Debug.LogError("Add action with same key on ApplicationBackgroundManager.");
                        break;
                }
                return;
            }

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
                _actionDic.Remove(key);
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            _ChangeBackgroundStatus(pauseStatus);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            _ChangeBackgroundStatus(!hasFocus);
        }

        void _ChangeBackgroundStatus(bool isBackground)
        {
            if (isBackground == IsBackground)
            {
                return;
            }
            IsBackground = isBackground;

            foreach (var kv in _actionDic)
            {
                try
                {
                    var action = kv.Value;
                    action?.Invoke(isBackground);
                }
                catch (Exception e)
                {
                    LogModel.CollectNoaDebuggerErrorLog(e.Message, e.StackTrace);
                    NoaDebuggerManager.DetectError();
                    throw new Exception(e.Message, e);
                }
            }
        }

        void OnDestroy()
        {
            ApplicationBackgroundManager._instance = null;
        }
    }
}

