using System;

namespace NoaDebugger
{
    /// <summary>
    /// Specifies the description text of the debug commands
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class CommandDescriptionAttribute : Attribute
    {
        readonly public string _text;

        public CommandDescriptionAttribute(string text)
        {
            _text = text;
        }
    }
}
