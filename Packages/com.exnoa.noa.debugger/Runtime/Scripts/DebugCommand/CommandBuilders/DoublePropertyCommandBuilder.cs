using System;

namespace NoaDebugger.DebugCommand
{
    sealed class DoublePropertyCommandBuilder : MutableNumericPropertyCommandBuilderBase<double>
    {
        int _needDigits = 0;

        public DoublePropertyCommandBuilder(
            string categoryName, string displayName, Func<double> getter, Action<double> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            base.PeekAttribute(attribute);

            if (_increment <= 0)
            {
                SendIncrementWarning();
                _increment = DoublePropertyCommand.DEFAULT_INCREMENT;
            }
        }

        protected override ICommand BuildCommand()
        {
            return new DoublePropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey,
                _inputRangeMin, _inputRangeMax, _increment, _needDigits);
        }

        protected override double? TryParse(object value)
        {
            string valueStr = value.ToString();

            if (double.TryParse(valueStr, out double result))
            {
                if (valueStr.Contains(NoaDebuggerDefine.DecimalPoint))
                {
                    int needDigits = valueStr.Length - valueStr.IndexOf(NoaDebuggerDefine.DecimalPoint) - 1;

                    if (needDigits > _needDigits)
                    {
                        _needDigits = needDigits;
                    }
                }

                return result;
            }

            return null;
        }
    }
}
