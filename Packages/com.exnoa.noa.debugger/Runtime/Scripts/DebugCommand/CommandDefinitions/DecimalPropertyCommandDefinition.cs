using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class DecimalPropertyCommandDefinition : MutablePropertyCommandDefinition<decimal>
    {
        public DecimalPropertyCommandDefinition(
            string categoryName, string displayName, Func<decimal> getter, Action<decimal> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new DecimalPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
