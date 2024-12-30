using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace NoaDebugger
{
    sealed class PrimitiveParamCreator : IParamCreator
    {
        public List<GameObjectDetailEntry> CreateSubParameter(Object obj, Component component, int depth)
        {
            List<GameObjectDetailEntry> ret = new List<GameObjectDetailEntry>();

            return ret;
        }

        public string GetValue(Object obj, Object component)
        {
            var infoValue = new object();
            if (obj is PropertyInfo)
            {
                var info = (PropertyInfo) obj;
                infoValue = info.GetValue(component);
            }
            else if (obj is FieldInfo)
            {
                var info = (FieldInfo) obj;
                infoValue = info.GetValue(component);
            }
            else
            {
                LogModel.ThrowException($"Cant Cast Object {obj.GetType().Name}.");
            }

            return infoValue?.ToString();
        }
    }
}
