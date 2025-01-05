using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class LongPropertyCommandDefinition : MutablePropertyCommandDefinition<long>
    {
        public LongPropertyCommandDefinition(
            string categoryName, string displayName, Func<long> getter, Action<long> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new LongPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
