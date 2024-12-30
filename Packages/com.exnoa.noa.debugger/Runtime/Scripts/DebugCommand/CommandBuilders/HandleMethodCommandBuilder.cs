using System;

namespace NoaDebugger.DebugCommand
{
    sealed class HandleMethodCommandBuilder : CommandBuilderBase
    {
        readonly Func<MethodHandler> _method;

        public HandleMethodCommandBuilder(
            string categoryName, string displayName, Func<MethodHandler> method, Attribute[] attributes = null)
            : base(categoryName, displayName, attributes)
        {
            _method = method;
        }

        protected override ICommand BuildCommand()
        {
            return new HandleMethodCommand(DisplayName, _method, GroupName, GroupOrder, TagName, Description, Order);
        }
    }
}
