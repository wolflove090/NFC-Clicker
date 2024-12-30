using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class MethodCommandDefinition : CommandDefinition
    {
        public Action Method { get; }

        public MethodCommandDefinition(
            string categoryName, string displayName, Action method, Attribute[] attributes = null)
            : base(categoryName, displayName, attributes)
        {
            Method = method;
        }

        internal override ICommand CreateCommand()
        {
            return new MethodCommandBuilder(CategoryName, DisplayName, Method, Attributes).Build();
        }
    }
}
