using System;

namespace NoaDebugger.DebugCommand
{
    sealed class ShortPropertyCommandBuilder : MutableNumericPropertyCommandBuilderBase<short>
    {
        public ShortPropertyCommandBuilder(
            string categoryName, string displayName, Func<short> getter, Action<short> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            base.PeekAttribute(attribute);

            if (_increment <= 0)
            {
                SendIncrementWarning();
                _increment = ShortPropertyCommand.DEFAULT_INCREMENT;
            }
        }

        protected override ICommand BuildCommand()
        {
            return new ShortPropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey,
                _inputRangeMin, _inputRangeMax, _increment);
        }

        protected override short? TryParse(object value)
        {
            if (short.TryParse(value.ToString(), out short result))
            {
                return result;
            }

            return null;
        }
    }
}
