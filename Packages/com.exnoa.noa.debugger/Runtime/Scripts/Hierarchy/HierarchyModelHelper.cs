using System;
using System.Reflection;
using UnityEngine;

namespace NoaDebugger
{
    sealed class HierarchyModelHelper
    {
        static public bool IsViewPropertyInfo(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                return false;
            }
            if (propertyInfo.GetGetMethod() == null && propertyInfo.GetSetMethod() == null)
            {
                return false;
            }
            if (propertyInfo.GetIndexParameters().Length > 0)
            {
                return false;
            }
            if (Attribute.IsDefined(propertyInfo, typeof(ObsoleteAttribute), true))
            {
                return false;
            }
            if (Attribute.IsDefined(propertyInfo, typeof(NonSerializedAttribute), true))
            {
                return false;
            }
            if (Attribute.IsDefined(propertyInfo, typeof(HideInInspector), true))
            {
                return false;
            }

            return true;
        }

        static public bool IsViewFieldInfo(FieldInfo fieldInfo)
        {
            Type type = fieldInfo.FieldType;
            if (Attribute.IsDefined(fieldInfo, typeof(ObsoleteAttribute), true))
            {
                return false;
            }
            if (Attribute.IsDefined(fieldInfo, typeof(NonSerializedAttribute), true))
            {
                return false;
            }
            if (Attribute.IsDefined(fieldInfo, typeof(HideInInspector), true))
            {
                return false;
            }
            if (fieldInfo.IsPublic)
            {
                if (type.IsGenericType)
                {
                    return true;
                }
                if(((type.IsValueType && !type.IsPrimitive && !type.IsEnum) || type.IsClass) && !Attribute.IsDefined(type, typeof(SerializableAttribute), true))
                {
                    if (type.Namespace != null && type.Namespace.Contains("UnityEngine"))
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
            if (Attribute.IsDefined(fieldInfo, typeof(SerializeField), true))
            {
                return true;
            }

            return false;
        }
    }
}
