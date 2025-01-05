using System;
using System.Collections;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class CoroutineCommandDefinition : CommandDefinition
    {
        Func<IEnumerator> Coroutine { get; }

        public CoroutineCommandDefinition(
            string categoryName, string displayName, Func<IEnumerator> coroutine, Attribute[] attributes = null)
            : base(categoryName, displayName, attributes)
        {
            Coroutine = coroutine;
        }

        internal override ICommand CreateCommand()
        {
            return new CoroutineCommandBuilder(CategoryName, DisplayName, Coroutine, Attributes).Build();
        }
    }
}
