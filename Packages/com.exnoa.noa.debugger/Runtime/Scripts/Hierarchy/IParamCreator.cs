using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace NoaDebugger
{
    interface IParamCreator
    {
        List<GameObjectDetailEntry> CreateSubParameter(Object obj, Component component, int depth);

        string GetValue(Object obj, Object component);
    }
}
