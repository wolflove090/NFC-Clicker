using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class UIntPropertyCommandDefinition : MutablePropertyCommandDefinition<uint>
    {
        public UIntPropertyCommandDefinition(
            string categoryName, string displayName, Func<uint> getter, Action<uint> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new UIntPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
