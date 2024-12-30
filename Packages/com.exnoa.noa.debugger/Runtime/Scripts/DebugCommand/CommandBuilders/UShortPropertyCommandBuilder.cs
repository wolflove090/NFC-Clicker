using System;

namespace NoaDebugger.DebugCommand
{
    sealed class UShortPropertyCommandBuilder : MutableNumericPropertyCommandBuilderBase<ushort>
    {
        public UShortPropertyCommandBuilder(
            string categoryName, string displayName, Func<ushort> getter, Action<ushort> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            base.PeekAttribute(attribute);

            if (_increment <= 0)
            {
                SendIncrementWarning();
                _increment = UShortPropertyCommand.DEFAULT_INCREMENT;
            }
        }

        protected override ICommand BuildCommand()
        {
            return new UShortPropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey,
                _inputRangeMin, _inputRangeMax, _increment);
        }

        protected override ushort? TryParse(object value)
        {
            if (ushort.TryParse(value.ToString(), out ushort result))
            {
                return result;
            }

            return null;
        }
    }
}
