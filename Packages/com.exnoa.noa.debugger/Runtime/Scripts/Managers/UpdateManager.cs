using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace NoaDebugger
{
    sealed class UpdateManager : MonoBehaviour
    {
        static UpdateManager _instance;
        ConcurrentDictionary<string, System.Action> _actionDic = new ConcurrentDictionary<string, System.Action>();
        ConcurrentBag<string> _deleteActionKeyList = new ConcurrentBag<string>();

        void Awake()
        {
            _instance = this;
        }

        static void _Instantiate()
        {
            var obj = new GameObject("NoaDebuggerLoopManager");
            var manager = obj.AddComponent<UpdateManager>();
            GameObject.DontDestroyOnLoad(obj);
            _instance = manager;
        }

        public static bool ContainsKey(string key)
        {
            if (_instance == null)
            {
                _Instantiate();
            }

            return _instance._actionDic.ContainsKey(key);
        }

        public static void SetAction(string key, System.Action action)
        {
            if (_instance == null)
            {
                _Instantiate();
            }

            _instance._SetAction(key, action);
        }
        void _SetAction(string key, System.Action action)
        {
            _actionDic.TryAdd(key, action);
        }

        public static void AddOrOverwriteAction(string key, System.Action action)
        {
            if (_instance == null)
            {
                _Instantiate();
            }

            _instance._AddOrOverwriteAction(key, action);
        }
        void _AddOrOverwriteAction(string key, System.Action action)
        {
            _actionDic[key] = action;
        }

        public static void ReplaceAction(string key, System.Action action)
        {
            if (_instance == null)
            {
                _Instantiate();
            }

            _instance._ReplaceAction(key, action);
        }
        void _ReplaceAction(string key, System.Action action)
        {
            if (_actionDic.ContainsKey(key))
            {
                _actionDic[key] = action;
            }
        }

        public static void DeleteAction(string key)
        {
            if (_instance == null)
            {
                _Instantiate();
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

        void Update()
        {
            foreach (var kv in _actionDic)
            {
                try
                {
                    var action = kv.Value;
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    LogModel.CollectNoaDebuggerErrorLog($"{e.Message} {kv.Key}", e.StackTrace);
                    NoaDebuggerManager.DetectError();
                    throw new Exception(e.Message, e);
                }
            }

            foreach (var deleteKey in _deleteActionKeyList)
            {
                if (_actionDic.ContainsKey(deleteKey))
                {
                    _actionDic.Remove(deleteKey, out _);
                }
            }
            _deleteActionKeyList.Clear();
        }
    }
}
