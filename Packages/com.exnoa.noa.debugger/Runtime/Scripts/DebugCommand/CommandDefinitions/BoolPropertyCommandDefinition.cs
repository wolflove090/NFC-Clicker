using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class BoolPropertyCommandDefinition : MutablePropertyCommandDefinition<bool>
    {
        public BoolPropertyCommandDefinition(
            string categoryName, string displayName, Func<bool> getter, Action<bool> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new BoolPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
