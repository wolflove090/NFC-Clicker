using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class StringPropertyCommandDefinition : MutablePropertyCommandDefinition<string>
    {
        public StringPropertyCommandDefinition(
            string categoryName, string displayName, Func<string> getter, Action<string> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new StringPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
