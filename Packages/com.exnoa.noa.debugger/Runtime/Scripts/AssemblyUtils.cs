using System;
using System.Linq;
using System.Reflection;

namespace NoaDebugger
{
    static class AssemblyUtils
    {
        public static Type[] GetInterfaces<T>()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(c => c.GetInterfaces().Any(t => t == typeof(T))).ToArray();
        }
        
        public static T[] CreateInterfaceInstances<T>() where T : class
        {
            return GetInterfaces<T>().Select(c => Activator.CreateInstance(c) as T).ToArray();
        }
    }

}
