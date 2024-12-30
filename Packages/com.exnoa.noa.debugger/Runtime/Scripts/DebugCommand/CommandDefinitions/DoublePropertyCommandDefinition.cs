using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class DoublePropertyCommandDefinition : MutablePropertyCommandDefinition<double>
    {
        public DoublePropertyCommandDefinition(
            string categoryName, string displayName, Func<double> getter, Action<double> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new DoublePropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
