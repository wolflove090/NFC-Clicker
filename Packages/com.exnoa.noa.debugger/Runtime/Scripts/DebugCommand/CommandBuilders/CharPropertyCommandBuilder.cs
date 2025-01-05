using System;

namespace NoaDebugger.DebugCommand
{
    sealed class CharPropertyCommandBuilder : MutablePropertyCommandBuilderBase<char>
    {
        public CharPropertyCommandBuilder(
            string categoryName, string displayName, Func<char> getter, Action<char> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override ICommand BuildCommand()
        {
            return new CharPropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey);
        }
    }
}
