using System;
using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class DecimalPropertyCommand : MutableNumericPropertyCommandBase<decimal>
    {
        static readonly decimal DEFAULT_INPUT_RANGE_MIN = decimal.MinValue;
        static readonly decimal DEFAULT_INPUT_RANGE_MAX = decimal.MaxValue;
        public static readonly decimal DEFAULT_INCREMENT = 1;

        protected override string TypeName => "Decimal Property";

        readonly int _needDigits;

        public DecimalPropertyCommand(
            string displayName, Func<decimal> getter, Action<decimal> setter, string groupName = null,
            int? groupOrder = null, string tagName = null, string description = null, int? order = null,
            string saveKey = null, decimal? inputRangeMin = null, decimal? inputRangeMax = null,
            decimal? increment = null, int needDigits = 0)
            : base(
                displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey, inputRangeMin,
                inputRangeMax, increment)
        {
            _inputRangeMin = inputRangeMin ?? DecimalPropertyCommand.DEFAULT_INPUT_RANGE_MIN;
            _inputRangeMax = inputRangeMax ?? DecimalPropertyCommand.DEFAULT_INPUT_RANGE_MAX;
            Increment = increment ?? DecimalPropertyCommand.DEFAULT_INCREMENT;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetDecimal(SaveKey, GetValue()));
            }

            _needDigits = needDigits;
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override decimal GetValue()
        {
            return _ValidateValue(InvokeGetter());
        }

        public override void SetValue(decimal value)
        {
            value = _ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetDecimal(SaveKey, value);
            }
        }

        public override decimal FromString(string textValue)
        {
            if (IsNotNumeric(textValue))
            {
                return 0;
            }

            if (decimal.TryParse(textValue, out decimal result))
            {
                return result;
            }

            return textValue.FirstOrDefault() == '-' ? decimal.MinValue : decimal.MaxValue;
        }

        public override bool IsEqualOrUnderMin(decimal value)
        {
            return value <= _inputRangeMin;
        }

        public override bool IsEqualOrOverMax(decimal value)
        {
            return value >= _inputRangeMax;
        }

        public override decimal ValidateValueForFluctuation(decimal value, int magnification)
        {
            if (magnification < 0)
            {
                for (int i = 0; i > magnification; i--)
                {
                    if ((value < 0 && _inputRangeMin - value > -Increment) ||
                        (value > 0 && _inputRangeMin > value - Increment))
                    {
                        return _inputRangeMin;
                    }

                    value -= Increment;
                }
            }

            if (magnification > 0)
            {
                for (int i = 0; i < magnification; i++)
                {
                    if ((value < 0 && _inputRangeMax < value + Increment) ||
                        (value > 0 && _inputRangeMax - value < Increment))
                    {
                        return _inputRangeMax;
                    }

                    value += Increment;
                }
            }

            decimal clamped = _ValidateValue(value);

            return Math.Round(clamped, _needDigits, MidpointRounding.AwayFromZero);
        }

        decimal _ValidateValue(decimal value)
        {
            return Math.Clamp(value , _inputRangeMin, _inputRangeMax);
        }
    }
}
