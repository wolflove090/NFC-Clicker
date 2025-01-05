using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class UShortPropertyCommandDefinition : MutablePropertyCommandDefinition<ushort>
    {
        public UShortPropertyCommandDefinition(
            string categoryName, string displayName, Func<ushort> getter, Action<ushort> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new UShortPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
