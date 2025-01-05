using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    public abstract class CommandDefinition
    {
        internal string CategoryName { get; }

        protected string DisplayName { get; }

        protected Attribute[] Attributes { get; }

        protected CommandDefinition(string categoryName, string displayName, Attribute[] attributes = null)
        {
            CategoryName = categoryName;
            DisplayName = displayName;
            Attributes = attributes;
        }

        internal abstract ICommand CreateCommand();
    }
}
