using System;

namespace NoaDebugger.DebugCommand
{
    sealed class SBytePropertyCommandBuilder : MutableNumericPropertyCommandBuilderBase<sbyte>
    {
        public SBytePropertyCommandBuilder(
            string categoryName, string displayName, Func<sbyte> getter, Action<sbyte> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            base.PeekAttribute(attribute);

            if (_increment <= 0)
            {
                SendIncrementWarning();
                _increment = SBytePropertyCommand.DEFAULT_INCREMENT;
            }
        }

        protected override ICommand BuildCommand()
        {
            return new SBytePropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey,
                _inputRangeMin, _inputRangeMax, _increment);
        }

        protected override sbyte? TryParse(object value)
        {
            if (sbyte.TryParse(value.ToString(), out sbyte result))
            {
                return result;
            }

            return null;
        }
    }
}
