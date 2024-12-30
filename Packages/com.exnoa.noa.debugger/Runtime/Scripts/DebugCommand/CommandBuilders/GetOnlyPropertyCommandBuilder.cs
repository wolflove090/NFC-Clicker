using System;

namespace NoaDebugger.DebugCommand
{
    sealed class GetOnlyPropertyCommandBuilder : CommandBuilderBase
    {
        readonly Func<object> _getter;

        public GetOnlyPropertyCommandBuilder(
            string categoryName, string displayName, Func<object> getter, Attribute[] attributes = null)
            : base(categoryName, displayName, attributes)
        {
            _getter = getter;
        }

        protected override ICommand BuildCommand()
        {
            return new GetOnlyPropertyCommand(DisplayName, _getter, GroupName, GroupOrder, TagName, Description, Order);
        }
    }
}
