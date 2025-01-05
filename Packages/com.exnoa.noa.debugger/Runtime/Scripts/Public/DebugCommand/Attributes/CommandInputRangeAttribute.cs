using System;

namespace NoaDebugger
{
    /// <summary>
    /// Specifies input range
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandInputRangeAttribute : Attribute
    {
        readonly public object _min;
        readonly public object _max;

        public CommandInputRangeAttribute(object min, object max)
        {
            _min = min;
            _max = max;
        }
    }
}
