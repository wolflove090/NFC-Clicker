using System;

namespace NoaDebugger.DebugCommand
{
    sealed class UIntPropertyCommandBuilder : MutableNumericPropertyCommandBuilderBase<uint>
    {
        public UIntPropertyCommandBuilder(
            string categoryName, string displayName, Func<uint> getter, Action<uint> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            base.PeekAttribute(attribute);

            if (_increment <= 0)
            {
                SendIncrementWarning();
                _increment = UIntPropertyCommand.DEFAULT_INCREMENT;
            }
        }

        protected override ICommand BuildCommand()
        {
            return new UIntPropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey,
                _inputRangeMin, _inputRangeMax, _increment);
        }

        protected override uint? TryParse(object value)
        {
            if (uint.TryParse(value.ToString(), out uint result))
            {
                return result;
            }

            return null;
        }
    }
}
