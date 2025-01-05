using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class BytePropertyCommandDefinition : MutablePropertyCommandDefinition<byte>
    {
        public BytePropertyCommandDefinition(
            string categoryName, string displayName, Func<byte> getter, Action<byte> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new BytePropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
