using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class ShortPropertyCommandDefinition : MutablePropertyCommandDefinition<short>
    {
        public ShortPropertyCommandDefinition(
            string categoryName, string displayName, Func<short> getter, Action<short> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new ShortPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
