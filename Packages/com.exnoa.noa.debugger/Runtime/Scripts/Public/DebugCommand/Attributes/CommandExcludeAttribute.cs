using System;

namespace NoaDebugger
{
    /// <summary>
    /// Specification to exclude from the target of the debug command display
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class CommandExcludeAttribute : Attribute { }
}
