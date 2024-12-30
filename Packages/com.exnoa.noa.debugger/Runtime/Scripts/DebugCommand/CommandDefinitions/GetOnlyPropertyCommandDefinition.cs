using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class GetOnlyPropertyCommandDefinition : CommandDefinition
    {
        Func<object> Getter { get; }

        public GetOnlyPropertyCommandDefinition(
            string categoryName, string displayName, Func<object> getter, Attribute[] attributes = null)
            : base(categoryName, displayName, attributes)
        {
            Getter = getter;
        }

        internal override ICommand CreateCommand()
        {
            return new GetOnlyPropertyCommandBuilder(CategoryName, DisplayName, Getter, Attributes).Build();
        }
    }
}
