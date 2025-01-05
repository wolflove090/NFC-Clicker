using System;
using NoaDebugger.DebugCommand;

namespace NoaDebugger
{
    sealed class FloatPropertyCommandDefinition : MutablePropertyCommandDefinition<float>
    {
        public FloatPropertyCommandDefinition(
            string categoryName, string displayName, Func<float> getter, Action<float> setter,
            Attribute[] attributes = null)
            : base(categoryName, displayName, getter, setter, attributes) { }

        internal override ICommand CreateCommand()
        {
            return new FloatPropertyCommandBuilder(CategoryName, DisplayName, Getter, Setter, Attributes).Build();
        }
    }
}
