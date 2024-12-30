using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace NoaDebugger
{
    sealed class EventSystemManager : MonoBehaviour
    {
        static readonly string EventSystemName = "EventSystem(Created by NOA Debugger)";
        private GameObject _createdEventSystem;

        public static void Init(Transform parent)
        {
            var obj = new GameObject(nameof(EventSystemManager));
            obj.transform.parent = parent;
            obj.AddComponent<EventSystemManager>();
        }

        void Awake()
        {
            NoaDebuggerSettings settings = NoaDebuggerSettingsManager.GetNoaDebuggerSettings();
            if (settings.AutoCreateEventSystem)
            {
                SceneManager.sceneLoaded += _OnLoadSceneAddOrDisabledEventSystem;
                SceneManager.sceneUnloaded += _OnUnloadSceneDisableEventSystem;
            }
        }

        void _OnLoadSceneAddOrDisabledEventSystem(Scene arg0, LoadSceneMode loadSceneMode)
        {
            if (_createdEventSystem == null)
            {
                _AddEventSystem();
            }
            else
            {
                FieldInfo fieldInfo = typeof(EventSystem).GetField("m_EventSystems", BindingFlags.NonPublic | BindingFlags.Static);
                if (fieldInfo != null)
                {
                    var eventSystems = fieldInfo.GetValue(null) as List<EventSystem>;
                    if (eventSystems != null)
                    {
                        if (eventSystems.Count == 0)
                        {
                            _createdEventSystem.SetActive(true);
                        }
                        if (eventSystems.Count > 1 && eventSystems.Exists(e => e.name == EventSystemName))
                        {
                            _createdEventSystem.SetActive(false);
                        }
                    }
                }
            }
        }

        void _OnUnloadSceneDisableEventSystem(Scene arg0)
        {
            if (_createdEventSystem != null)
            {
                _createdEventSystem.SetActive(false);
            }
        }

        void _AddEventSystem()
        {
            if (EventSystem.current == null)
            {
                _createdEventSystem = new GameObject(EventSystemName);
                _createdEventSystem.AddComponent<EventSystem>();
                _createdEventSystem.AddComponent<StandaloneInputModule>();
                _createdEventSystem.transform.parent = transform;
                LogModel.Log($"Created {EventSystemName}", _createdEventSystem);
            }
        }

        void OnDestroy()
        {
            _createdEventSystem = default;
            
        }
    }
}
