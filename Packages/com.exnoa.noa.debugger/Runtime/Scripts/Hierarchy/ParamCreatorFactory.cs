using System;
using System.Collections.Generic;
using UnityEngine;

namespace NoaDebugger
{
    sealed class ParamCreatorFactory
    {
        static public IParamCreator ParamCreatorByType(Type type)
        {
            IParamCreator creator;

            if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                creator = new ArrayParamCreator();
            }
            else if (type.IsValueType && !type.IsPrimitive && !type.IsEnum)
            {
                creator = new StructParamCreator();
            }
            else if (typeof(String) == type)
            {
                creator = new PrimitiveParamCreator();
            }
            else if (type.IsClass)
            {
                creator = new ClassParamCreator();
            }
            else
            {
                creator = new PrimitiveParamCreator();
            }

            return creator;
        }
    }
}
