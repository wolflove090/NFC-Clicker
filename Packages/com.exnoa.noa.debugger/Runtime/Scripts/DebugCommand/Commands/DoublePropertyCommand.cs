using System;
using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class DoublePropertyCommand : MutableNumericPropertyCommandBase<double>
    {
        static readonly double DEFAULT_INPUT_RANGE_MIN = double.MinValue;
        static readonly double DEFAULT_INPUT_RANGE_MAX = double.MaxValue;
        public static readonly double DEFAULT_INCREMENT = 1;

        protected override string TypeName => "Double Property";

        readonly int _needDigits;

        public DoublePropertyCommand(
            string displayName, Func<double> getter, Action<double> setter, string groupName = null,
            int? groupOrder = null, string tagName = null, string description = null, int? order = null,
            string saveKey = null, double? inputRangeMin = null, double? inputRangeMax = null, double? increment = null,
            int needDigits = 0)
            : base(
                displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey, inputRangeMin,
                inputRangeMax, increment)
        {
            _inputRangeMin = inputRangeMin ?? DoublePropertyCommand.DEFAULT_INPUT_RANGE_MIN;
            _inputRangeMax = inputRangeMax ?? DoublePropertyCommand.DEFAULT_INPUT_RANGE_MAX;
            Increment = increment ?? DoublePropertyCommand.DEFAULT_INCREMENT;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetDouble(SaveKey, GetValue()));
            }

            _needDigits = needDigits;
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override double GetValue()
        {
            return _ValidateValue(InvokeGetter());
        }

        public override void SetValue(double value)
        {
            value = _ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetDouble(SaveKey, value);
            }
        }

        public override double FromString(string textValue)
        {
            if (IsNotNumeric(textValue))
            {
                return 0;
            }

            if (double.TryParse(textValue, out double result))
            {
                return result;
            }

            return textValue.FirstOrDefault() == '-' ? double.MinValue : double.MaxValue;
        }

        public override bool IsEqualOrUnderMin(double value)
        {
            return value <= _inputRangeMin;
        }

        public override bool IsEqualOrOverMax(double value)
        {
            return value >= _inputRangeMax;
        }

        public override double ValidateValueForFluctuation(double value, int magnification)
        {
            double tmpValue = value;

            if (magnification < 0)
            {
                for (int i = 0; i > magnification; i--)
                {
                    if ((tmpValue < 0 && _inputRangeMin - tmpValue > -Increment) ||
                        (tmpValue > 0 && _inputRangeMin > tmpValue - Increment))
                    {
                        return _inputRangeMin;
                    }

                    tmpValue -= Increment;
                }
            }

            else if (magnification > 0)
            {
                for (int i = 0; i < magnification; i++)
                {
                    if ((tmpValue < 0 && _inputRangeMax < tmpValue + Increment) ||
                        (tmpValue > 0 && _inputRangeMax - tmpValue < Increment))
                    {
                        return _inputRangeMax;
                    }

                    tmpValue += Increment;
                }
            }

            double clamped = _ValidateValue(value + Increment * magnification);

            return Math.Round(clamped, _needDigits, MidpointRounding.AwayFromZero);
        }

        double _ValidateValue(double value)
        {
            return Math.Clamp(value , _inputRangeMin, _inputRangeMax);
        }
    }
}
