using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace NoaDebugger
{
    sealed class ClassParamCreator : IParamCreator
    {
        public List<GameObjectDetailEntry> CreateSubParameter(Object obj, Component component, int depth)
        {
            List<GameObjectDetailEntry> ret = new List<GameObjectDetailEntry>();
            if (depth >= NoaDebuggerSettingsManager.GetNoaDebuggerSettings().HierarchyLevels)
            {
                return ret;
            }
            ++depth;

            var type = obj.GetType();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var fieldInfo in fieldInfos)
            {
                if (!HierarchyModelHelper.IsViewFieldInfo(fieldInfo))
                {
                    continue;
                }

                var valueType = fieldInfo.FieldType;
                IParamCreator creator = ParamCreatorFactory.ParamCreatorByType(valueType);

                var detailValue = creator.GetValue(fieldInfo, obj);
                var value = detailValue ?? fieldInfo.GetValue(obj);

                List<GameObjectDetailEntry> subParam = new List<GameObjectDetailEntry>();
                if (value != null)
                {
                    subParam = creator.CreateSubParameter(value, component, depth);
                }

                GameObjectDetailEntry tmp = new GameObjectDetailEntry()
                {
                    _name = fieldInfo.Name,
                    _value = detailValue,
                    _isOpen = false,
                    _subDetailList = subParam
                };

                ret.Add(tmp);
            }

            return ret;
        }

        public string GetValue(Object obj, Object component)
        {
            return null;
        }
    }
}
