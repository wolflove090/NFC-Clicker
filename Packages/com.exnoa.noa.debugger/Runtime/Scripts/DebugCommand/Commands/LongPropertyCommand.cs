using System;
using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class LongPropertyCommand : MutableNumericPropertyCommandBase<long>
    {
        static readonly long DEFAULT_INPUT_RANGE_MIN = long.MinValue;
        static readonly long DEFAULT_INPUT_RANGE_MAX = long.MaxValue;
        public static readonly long DEFAULT_INCREMENT = 1;

        protected override string TypeName => "Long Property";

        public LongPropertyCommand(
            string displayName, Func<long> getter, Action<long> setter, string groupName = null, int? groupOrder = null,
            string tagName = null, string description = null, int? order = null, string saveKey = null,
            long? inputRangeMin = null, long? inputRangeMax = null, long? increment = null)
            : base(
                displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey, inputRangeMin,
                inputRangeMax, increment)
        {
            _inputRangeMin = inputRangeMin ?? LongPropertyCommand.DEFAULT_INPUT_RANGE_MIN;
            _inputRangeMax = inputRangeMax ?? LongPropertyCommand.DEFAULT_INPUT_RANGE_MAX;
            Increment = increment ?? LongPropertyCommand.DEFAULT_INCREMENT;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetLong(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override long GetValue()
        {
            return _ValidateValue(InvokeGetter());
        }

        public override void SetValue(long value)
        {
            value = _ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetLong(SaveKey, value);
            }
        }

        public override long FromString(string textValue)
        {
            if (IsNotNumeric(textValue))
            {
                return 0;
            }

            if (long.TryParse(textValue, out long result))
            {
                return result;
            }

            return textValue.FirstOrDefault() == '-' ? long.MinValue : long.MaxValue;
        }

        public override bool IsEqualOrUnderMin(long value)
        {
            return value <= _inputRangeMin;
        }

        public override bool IsEqualOrOverMax(long value)
        {
            return value >= _inputRangeMax;
        }

        public override long ValidateValueForFluctuation(long value, int magnification)
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

        long _ValidateValue(long value)
        {
            return Math.Clamp(value, _inputRangeMin, _inputRangeMax);
        }
    }
}
