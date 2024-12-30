using System;

namespace NoaDebugger
{
    /// <summary>
    /// Specifies the increase amount for drag operations
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandIncrementAttribute : Attribute
    {
        readonly public object _increment;

        public CommandIncrementAttribute(object increment)
        {
            _increment = increment;
        }
    }
}
