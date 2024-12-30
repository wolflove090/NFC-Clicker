using System;

namespace NoaDebugger.DebugCommand
{
    sealed class EnumPropertyCommandBuilder : MutablePropertyCommandBuilderBase<Enum>
    {
        readonly Type _enumType;

        public EnumPropertyCommandBuilder(
            string categoryName, string displayName, Func<Enum> getter, Action<Enum> setter, Type type,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey)
        {
            _enumType = type;
        }

        protected override ICommand BuildCommand()
        {
            return new EnumPropertyCommand(
                DisplayName, Getter, Setter, _enumType, GroupName, GroupOrder, TagName, Description, Order, SaveKey);
        }
    }
}
