using System;

namespace NoaDebugger
{
    /// <summary>
    /// Specifies the maximum number of input characters
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandCharacterLimitAttribute : Attribute
    {
        readonly public int _limit;

        public CommandCharacterLimitAttribute(int limit)
        {
            if (limit <= 0)
            {
                limit = 0;
            }

            _limit = limit;
        }
    }
}
