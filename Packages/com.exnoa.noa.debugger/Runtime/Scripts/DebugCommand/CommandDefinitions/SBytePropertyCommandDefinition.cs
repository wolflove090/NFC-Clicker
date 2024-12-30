using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class SBytePropertyCommandDefinition : MutablePropertyCommandDefinition<sbyte>
    {
        public SBytePropertyCommandDefinition(
            string categoryName, string displayName, Func<sbyte> getter, Action<sbyte> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new SBytePropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
