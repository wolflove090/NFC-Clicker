using System;

namespace NoaDebugger
{
    /// <summary>
    /// Specifies the group name of the debug command
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class CommandGroupAttribute : Attribute
    {
        readonly public string _name;
        readonly public int? _order;

        public CommandGroupAttribute(string name)
        {
            _name = name;
            _order = null;
        }

        public CommandGroupAttribute(string name, int order)
        {
            _name = name;
            _order = order;
        }
    }
}
