using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class HandleMethodCommandDefinition : CommandDefinition
    {
        Func<MethodHandler> Method { get; }

        public HandleMethodCommandDefinition(
            string categoryName, string displayName, Func<MethodHandler> method, Attribute[] attributes = null)
            : base(categoryName, displayName, attributes)
        {
            Method = method;
        }

        internal override ICommand CreateCommand()
        {
            return new HandleMethodCommandBuilder(CategoryName, DisplayName, Method, Attributes).Build();
        }
    }
}
