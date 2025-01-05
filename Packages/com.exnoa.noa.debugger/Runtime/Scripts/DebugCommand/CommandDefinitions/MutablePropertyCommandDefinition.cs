using System;

namespace NoaDebugger
{
    abstract class MutablePropertyCommandDefinition<T> : CommandDefinition
    {
        protected Func<T> Getter { get; }

        protected Action<T> Setter { get; }

        protected MutablePropertyCommandDefinition(
            string categoryName, string displayName, Func<T> getter, Action<T> setter, Attribute[] attributes = null)
            : base(categoryName, displayName, attributes)
        {
            Getter = getter;
            Setter = setter;
        }
    }
}
