using System;

namespace NoaDebugger.DebugCommand
{
    sealed class LongPropertyCommandBuilder : MutableNumericPropertyCommandBuilderBase<long>
    {
        public LongPropertyCommandBuilder(
            string categoryName, string displayName, Func<long> getter, Action<long> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            base.PeekAttribute(attribute);

            if (_increment <= 0)
            {
                SendIncrementWarning();
                _increment = LongPropertyCommand.DEFAULT_INCREMENT;
            }
        }

        protected override ICommand BuildCommand()
        {
            return new LongPropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey,
                _inputRangeMin, _inputRangeMax, _increment);
        }

        protected override long? TryParse(object value)
        {
            if (long.TryParse(value.ToString(), out long result))
            {
                return result;
            }

            return null;
        }
    }
}
