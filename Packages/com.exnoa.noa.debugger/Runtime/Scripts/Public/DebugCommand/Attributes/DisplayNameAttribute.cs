using System;

namespace NoaDebugger
{
    /// <summary>
    /// Specifies the display name of the debug commands
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class DisplayNameAttribute : Attribute
    {
        readonly public string _name;

        public DisplayNameAttribute(string name)
        {
            _name = name;
        }
    }
}
