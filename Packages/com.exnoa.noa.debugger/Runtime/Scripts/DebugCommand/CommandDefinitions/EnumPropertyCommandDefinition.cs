using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class EnumPropertyCommandDefinition : MutablePropertyCommandDefinition<Enum>
    {
        Type EnumType { get; }

        public EnumPropertyCommandDefinition(
            string categoryName, string displayName, Func<Enum> getter, Action<Enum> setter, Type enumType,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes)
        {
            EnumType = enumType;
        }

        internal override ICommand CreateCommand()
        {
            return new EnumPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, EnumType, Attributes).Build();
        }
    }
}
