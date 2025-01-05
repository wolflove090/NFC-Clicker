using System;

namespace NoaDebugger.DebugCommand
{
    sealed class BoolPropertyCommandBuilder : MutablePropertyCommandBuilderBase<bool>
    {
        public BoolPropertyCommandBuilder(
            string categoryName, string displayName, Func<bool> getter, Action<bool> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override ICommand BuildCommand()
        {
            return new BoolPropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey);
        }
    }
}
