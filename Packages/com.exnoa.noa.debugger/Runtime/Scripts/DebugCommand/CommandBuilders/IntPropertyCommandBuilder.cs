using System;

namespace NoaDebugger.DebugCommand
{
    sealed class IntPropertyCommandBuilder : MutableNumericPropertyCommandBuilderBase<int>
    {
        public IntPropertyCommandBuilder(
            string categoryName, string displayName, Func<int> getter, Action<int> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            base.PeekAttribute(attribute);

            if (_increment <= 0)
            {
                SendIncrementWarning();
                _increment = IntPropertyCommand.DEFAULT_INCREMENT;
            }
        }

        protected override ICommand BuildCommand()
        {
            return new IntPropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey,
                _inputRangeMin, _inputRangeMax, _increment);
        }

        protected override int? TryParse(object value)
        {
            if (int.TryParse(value.ToString(), out int result))
            {
                return result;
            }

            return null;
        }
    }
}
