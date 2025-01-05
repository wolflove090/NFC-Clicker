using System;
using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class UIntPropertyCommand : MutableNumericPropertyCommandBase<uint>
    {
        static readonly uint DEFAULT_INPUT_RANGE_MIN = uint.MinValue;
        static readonly uint DEFAULT_INPUT_RANGE_MAX = uint.MaxValue;
        public static readonly uint DEFAULT_INCREMENT = 1;

        protected override string TypeName => "UInt Property";

        public UIntPropertyCommand(
            string displayName, Func<uint> getter, Action<uint> setter, string groupName = null, int? groupOrder = null,
            string tagName = null, string description = null, int? order = null, string saveKey = null,
            uint? inputRangeMin = null, uint? inputRangeMax = null, uint? increment = null)
            : base(
                displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey, inputRangeMin,
                inputRangeMax, increment)
        {
            _inputRangeMin = inputRangeMin ?? UIntPropertyCommand.DEFAULT_INPUT_RANGE_MIN;
            _inputRangeMax = inputRangeMax ?? UIntPropertyCommand.DEFAULT_INPUT_RANGE_MAX;
            Increment = increment ?? UIntPropertyCommand.DEFAULT_INCREMENT;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetUInt(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override uint GetValue()
        {
            return _ValidateValue(InvokeGetter());
        }

        public override void SetValue(uint value)
        {
            value = _ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetUInt(SaveKey, value);
            }
        }

        public override uint FromString(string textValue)
        {
            if (IsNotNumeric(textValue))
            {
                return 0;
            }

            if (uint.TryParse(textValue, out uint result))
            {
                return result;
            }

            return textValue.FirstOrDefault() == '-' ? uint.MinValue : uint.MaxValue;
        }

        public override bool IsEqualOrUnderMin(uint value)
        {
            return value <= _inputRangeMin;
        }

        public override bool IsEqualOrOverMax(uint value)
        {
            return value >= _inputRangeMax;
        }

        public override uint ValidateValueForFluctuation(uint value, int magnification)
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

        uint _ValidateValue(uint value)
        {
            return Math.Clamp(value, _inputRangeMin, _inputRangeMax);
        }
    }
}
