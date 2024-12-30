using System;

namespace NoaDebugger.DebugCommand
{
    sealed class MethodCommandBuilder : CommandBuilderBase
    {
        readonly Action _method;

        public MethodCommandBuilder(
            string categoryName, string displayName, Action method, Attribute[] attributes = null)
            : base(categoryName, displayName, attributes)
        {
            _method = method;
        }

        protected override ICommand BuildCommand()
        {
            return new MethodCommand(DisplayName, _method, GroupName, GroupOrder, TagName, Description, Order);
        }
    }
}
