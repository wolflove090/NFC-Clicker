using System;

namespace NoaDebugger
{
    /// <summary>
    /// Specifies the key for storing changed value
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SaveOnUpdateAttribute : Attribute { }
}
