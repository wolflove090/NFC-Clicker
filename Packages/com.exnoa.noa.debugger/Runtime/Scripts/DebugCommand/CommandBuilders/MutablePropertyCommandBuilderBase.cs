using System;

namespace NoaDebugger.DebugCommand
{
    abstract class MutablePropertyCommandBuilderBase<T> : CommandBuilderBase
    {
        protected Func<T> Getter { get; }
        protected Action<T> Setter { get; }
        protected string SaveKey { get; }

        protected MutablePropertyCommandBuilderBase(
            string categoryName, string displayName, Func<T> getter, Action<T> setter, Attribute[] attributes = null,
            string saveKey = null)
            : base(categoryName, displayName, attributes)
        {
            Getter = getter;
            Setter = setter;
            SaveKey = saveKey;
        }
    }
}
