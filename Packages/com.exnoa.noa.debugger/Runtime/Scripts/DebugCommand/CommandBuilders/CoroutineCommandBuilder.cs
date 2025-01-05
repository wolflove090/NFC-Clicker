using System;
using System.Collections;

namespace NoaDebugger.DebugCommand
{
    sealed class CoroutineCommandBuilder : CommandBuilderBase
    {
        readonly Func<IEnumerator> _coroutine;

        public CoroutineCommandBuilder(
            string categoryName, string displayName, Func<IEnumerator> coroutine, Attribute[] attributes = null)
            : base(categoryName, displayName, attributes)
        {
            _coroutine = coroutine;
        }

        protected override ICommand BuildCommand()
        {
            return new CoroutineCommand(DisplayName, _coroutine, GroupName, GroupOrder, TagName, Description, Order);
        }
    }
}
