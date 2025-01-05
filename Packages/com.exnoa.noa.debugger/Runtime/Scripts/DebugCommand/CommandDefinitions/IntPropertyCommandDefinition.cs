using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class IntPropertyCommandDefinition : MutablePropertyCommandDefinition<int>
    {
        public IntPropertyCommandDefinition(
            string categoryName, string displayName, Func<int> getter, Action<int> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new IntPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
