using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class CharPropertyCommandDefinition : MutablePropertyCommandDefinition<char>
    {
        public CharPropertyCommandDefinition(
            string categoryName, string displayName, Func<char> getter, Action<char> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new CharPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
