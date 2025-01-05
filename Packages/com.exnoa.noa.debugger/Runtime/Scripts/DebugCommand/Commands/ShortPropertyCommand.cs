using System;
using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class ShortPropertyCommand : MutableNumericPropertyCommandBase<short>
    {
        static readonly short DEFAULT_INPUT_RANGE_MIN = short.MinValue;
        static readonly short DEFAULT_INPUT_RANGE_MAX = short.MaxValue;
        public static readonly short DEFAULT_INCREMENT = 1;

        protected override string TypeName => "Short Property";

        public ShortPropertyCommand(
            string displayName, Func<short> getter, Action<short> setter, string groupName = null,
            int? groupOrder = null, string tagName = null, string description = null, int? order = null,
            string saveKey = null, short? inputRangeMin = null, short? inputRangeMax = null, short? increment = null)
            : base(
                displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey, inputRangeMin,
                inputRangeMax, increment)
        {
            _inputRangeMin = inputRangeMin ?? ShortPropertyCommand.DEFAULT_INPUT_RANGE_MIN;
            _inputRangeMax = inputRangeMax ?? ShortPropertyCommand.DEFAULT_INPUT_RANGE_MAX;
            Increment = increment ?? ShortPropertyCommand.DEFAULT_INCREMENT;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetShort(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override short GetValue()
        {
            return _ValidateValue(InvokeGetter());
        }

        public override void SetValue(short value)
        {
            value = _ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetShort(SaveKey, value);
            }
        }

        public override short FromString(string textValue)
        {
            if (IsNotNumeric(textValue))
            {
                return 0;
            }

            if (short.TryParse(textValue, out short result))
            {
                return result;
            }

            return textValue.FirstOrDefault() == '-' ? short.MinValue : short.MaxValue;
        }

        public override bool IsEqualOrUnderMin(short value)
        {
            return value <= _inputRangeMin;
        }

        public override bool IsEqualOrOverMax(short value)
        {
            return value >= _inputRangeMax;
        }

        public override short ValidateValueForFluctuation(short value, int magnification)
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

                return value;
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

                return value;
            }

            return _ValidateValue(value);
        }

        short _ValidateValue(short value)
        {
            return Math.Clamp(value, _inputRangeMin, _inputRangeMax);
        }
    }
}
