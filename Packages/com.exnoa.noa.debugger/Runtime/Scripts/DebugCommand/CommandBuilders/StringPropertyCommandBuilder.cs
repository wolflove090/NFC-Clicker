using System;

namespace NoaDebugger.DebugCommand
{
    sealed class StringPropertyCommandBuilder : MutablePropertyCommandBuilderBase<string>
    {
        int? _characterLimit;

        public StringPropertyCommandBuilder(
            string categoryName, string displayName, Func<string> getter, Action<string> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            if (attribute is CommandCharacterLimitAttribute limit)
            {
                _characterLimit = limit._limit;
            }
        }

        protected override ICommand BuildCommand()
        {
            return new StringPropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey,
                _characterLimit);
        }
    }
}
