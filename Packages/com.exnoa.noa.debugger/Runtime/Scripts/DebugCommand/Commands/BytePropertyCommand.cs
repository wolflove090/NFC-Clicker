using System;
using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class BytePropertyCommand : MutableNumericPropertyCommandBase<byte>
    {
        static readonly byte DEFAULT_INPUT_RANGE_MIN = byte.MinValue;
        static readonly byte DEFAULT_INPUT_RANGE_MAX = byte.MaxValue;
        public static readonly byte DEFAULT_INCREMENT = 1;

        protected override string TypeName => "Byte Property";

        public BytePropertyCommand(
            string displayName, Func<byte> getter, Action<byte> setter, string groupName = null, int? groupOrder = null,
            string tagName = null, string description = null, int? order = null, string saveKey = null,
            byte? inputRangeMin = null, byte? inputRangeMax = null, byte? increment = null)
            : base(
                displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey, inputRangeMin,
                inputRangeMax, increment)
        {
            _inputRangeMin = inputRangeMin ?? BytePropertyCommand.DEFAULT_INPUT_RANGE_MIN;
            _inputRangeMax = inputRangeMax ?? BytePropertyCommand.DEFAULT_INPUT_RANGE_MAX;
            Increment = increment ?? BytePropertyCommand.DEFAULT_INCREMENT;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetByte(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override byte GetValue()
        {
            return _ValidateValue(InvokeGetter());
        }

        public override void SetValue(byte value)
        {
            value = _ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetByte(SaveKey, value);
            }
        }

        public override byte FromString(string textValue)
        {
            if (IsNotNumeric(textValue))
            {
                return 0;
            }

            if (byte.TryParse(textValue, out byte result))
            {
                return result;
            }

            return textValue.FirstOrDefault() == '-' ? byte.MinValue : byte.MaxValue;
        }

        public override bool IsEqualOrUnderMin(byte value)
        {
            return value <= _inputRangeMin;
        }

        public override bool IsEqualOrOverMax(byte value)
        {
            return value >= _inputRangeMax;
        }

        public override byte ValidateValueForFluctuation(byte value, int magnification)
        {
            if (magnification < 0)
            {
                for (int i = 0; i > magnification; i--)
                {
                    if(Increment > value - _inputRangeMin)
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
                    if(Increment > _inputRangeMax - value)
                    {
                        return _inputRangeMax;
                    }

                    value += Increment;
                }

                return value;
            }

            return _ValidateValue(value);
        }

        byte _ValidateValue(byte value)
        {
            return Math.Clamp(value, _inputRangeMin, _inputRangeMax);
        }
    }
}
