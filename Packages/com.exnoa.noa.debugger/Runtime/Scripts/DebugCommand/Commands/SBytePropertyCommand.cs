using System;
using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class SBytePropertyCommand : MutableNumericPropertyCommandBase<sbyte>
    {
        static readonly sbyte DEFAULT_INPUT_RANGE_MIN = sbyte.MinValue;
        static readonly sbyte DEFAULT_INPUT_RANGE_MAX = sbyte.MaxValue;
        public static readonly sbyte DEFAULT_INCREMENT = 1;

        protected override string TypeName => "SByte Property";

        public SBytePropertyCommand(
            string displayName, Func<sbyte> getter, Action<sbyte> setter, string groupName = null,
            int? groupOrder = null, string tagName = null, string description = null, int? order = null,
            string saveKey = null, sbyte? inputRangeMin = null, sbyte? inputRangeMax = null, sbyte? increment = null)
            : base(
                displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey, inputRangeMin,
                inputRangeMax, increment)
        {
            _inputRangeMin = inputRangeMin ?? SBytePropertyCommand.DEFAULT_INPUT_RANGE_MIN;
            _inputRangeMax = inputRangeMax ?? SBytePropertyCommand.DEFAULT_INPUT_RANGE_MAX;
            Increment = increment ?? SBytePropertyCommand.DEFAULT_INCREMENT;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetSByte(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override sbyte GetValue()
        {
            return _ValidateValue(InvokeGetter());
        }

        public override void SetValue(sbyte value)
        {
            value = _ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetSByte(SaveKey, value);
            }
        }

        public override sbyte FromString(string textValue)
        {
            if (IsNotNumeric(textValue))
            {
                return 0;
            }

            if (sbyte.TryParse(textValue, out sbyte result))
            {
                return result;
            }

            return textValue.FirstOrDefault() == '-' ? sbyte.MinValue : sbyte.MaxValue;
        }

        public override bool IsEqualOrUnderMin(sbyte value)
        {
            return value <= _inputRangeMin;
        }

        public override bool IsEqualOrOverMax(sbyte value)
        {
            return value >= _inputRangeMax;
        }

        public override sbyte ValidateValueForFluctuation(sbyte value, int magnification)
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

        sbyte _ValidateValue(sbyte value)
        {
            return Math.Clamp(value, _inputRangeMin, _inputRangeMax);
        }
    }
}
