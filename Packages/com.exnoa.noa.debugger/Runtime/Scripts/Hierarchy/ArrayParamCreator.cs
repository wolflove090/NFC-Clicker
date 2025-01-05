using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace NoaDebugger
{
    sealed class ArrayParamCreator : IParamCreator
    {
        public List<GameObjectDetailEntry> CreateSubParameter(Object obj, Component component, int depth)
        {
            List<GameObjectDetailEntry> ret = new List<GameObjectDetailEntry>();
            if (depth >= NoaDebuggerSettingsManager.GetNoaDebuggerSettings().HierarchyLevels)
            {
                return ret;
            }

            ++depth;

            if (obj is Array)
            {
                ret = _GetGameObjectDetailEntryFromArray((Array) obj, component, depth);
            }
            else if (obj is IList)
            {
                ret = _GetGameObjectDetailEntryFromList((IList)obj, component, depth);
            }

            return ret;
        }

        public string GetValue(Object obj, Object component)
        {
            return null;
        }

        List<GameObjectDetailEntry> _GetGameObjectDetailEntryFromArray (Array array, Component component, int depth)
        {
            List<GameObjectDetailEntry> ret = new List<GameObjectDetailEntry>();

            for(int i = 0; i< array.Length; i++)
            {
                var value = array.GetValue(i);
                if (value == null)
                {
                    continue;
                }
                var valueType = value.GetType();

                IParamCreator creator = ParamCreatorFactory.ParamCreatorByType(valueType);

                List<GameObjectDetailEntry> subParam = creator.CreateSubParameter(value, component, depth);

                GameObjectDetailEntry tmp = new GameObjectDetailEntry()
                {
                    _name = $"Element {i}",
                    _value = (typeof(PrimitiveParamCreator) == creator.GetType()) ? value.ToString() : null,
                    _isOpen = false,
                    _subDetailList = subParam
                };

                ret.Add(tmp);
            }

            return ret;
        }

        List<GameObjectDetailEntry> _GetGameObjectDetailEntryFromList (IList list, Component component, int depth)
        {
            List<GameObjectDetailEntry> ret = new List<GameObjectDetailEntry>();

            for(int i = 0; i< list.Count; i++)
            {
                var value = list[i];
                if (value == null)
                {
                    continue;
                }
                var valueType = value.GetType();

                IParamCreator creator = ParamCreatorFactory.ParamCreatorByType(valueType);

                List<GameObjectDetailEntry> subParam = creator.CreateSubParameter(value, component, depth);

                GameObjectDetailEntry tmp = new GameObjectDetailEntry()
                {
                    _name = $"Element {i}",
                    _value = (typeof(PrimitiveParamCreator) == creator.GetType()) ? value.ToString() : null,
                    _isOpen = false,
                    _subDetailList = subParam
                };

                ret.Add(tmp);
            }

            return ret;
        }
    }
}
