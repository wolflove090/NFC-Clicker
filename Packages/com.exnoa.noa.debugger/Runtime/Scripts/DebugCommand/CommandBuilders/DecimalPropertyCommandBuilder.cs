using System;

namespace NoaDebugger.DebugCommand
{
    sealed class DecimalPropertyCommandBuilder : MutableNumericPropertyCommandBuilderBase<decimal>
    {
        int _needDigits = 0;

        public DecimalPropertyCommandBuilder(
            string categoryName, string displayName, Func<decimal> getter, Action<decimal> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            base.PeekAttribute(attribute);

            if (_increment <= 0)
            {
                SendIncrementWarning();
                _increment = DecimalPropertyCommand.DEFAULT_INCREMENT;
            }
        }

        protected override ICommand BuildCommand()
        {
            return new DecimalPropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey,
                _inputRangeMin, _inputRangeMax, _increment, _needDigits);
        }

        protected override decimal? TryParse(object value)
        {
            string valueStr = value.ToString();

            if (decimal.TryParse(valueStr, out decimal result))
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
