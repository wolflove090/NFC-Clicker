using System;

namespace NoaDebugger
{
    /// <summary>
    /// Specifies the order of debug commands
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class CommandOrderAttribute : Attribute
    {
        readonly public int _order;

        public CommandOrderAttribute(int order)
        {
            _order = order;
        }
    }
}
