using System;

namespace NoaDebugger
{
    /// <summary>
    /// Specifies the tag of the command. This tag is specified when performing any operations to the command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class CommandTagAttribute : Attribute
    {
        readonly public string _tag;

        public CommandTagAttribute(string tag)
        {
            _tag = tag;
        }
    }
}
