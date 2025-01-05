using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class ULongPropertyCommandDefinition : MutablePropertyCommandDefinition<ulong>
    {
        public ULongPropertyCommandDefinition(
            string categoryName, string displayName, Func<ulong> getter, Action<ulong> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new ULongPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
