using System;
using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class ULongPropertyCommand : MutableNumericPropertyCommandBase<ulong>
    {
        static readonly ulong DEFAULT_INPUT_RANGE_MIN = ulong.MinValue;
        static readonly ulong DEFAULT_INPUT_RANGE_MAX = ulong.MaxValue;
        public static readonly ulong DEFAULT_INCREMENT = 1;

        protected override string TypeName => "ULong Property";

        public ULongPropertyCommand(
            string displayName, Func<ulong> getter, Action<ulong> setter, string groupName = null,
            int? groupOrder = null, string tagName = null, string description = null, int? order = null,
            string saveKey = null, ulong? inputRangeMin = null, ulong? inputRangeMax = null, ulong? increment = null)
            : base(
                displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey, inputRangeMin,
                inputRangeMax, increment)
        {
            _inputRangeMin = inputRangeMin ?? ULongPropertyCommand.DEFAULT_INPUT_RANGE_MIN;
            _inputRangeMax = inputRangeMax ?? ULongPropertyCommand.DEFAULT_INPUT_RANGE_MAX;
            Increment = increment ?? ULongPropertyCommand.DEFAULT_INCREMENT;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetULong(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override ulong GetValue()
        {
            return _ValidateValue(InvokeGetter());
        }

        public override void SetValue(ulong value)
        {
            value = _ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetULong(SaveKey, value);
            }
        }

        public override ulong FromString(string textValue)
        {
            if (IsNotNumeric(textValue))
            {
                return 0;
            }

            if (ulong.TryParse(textValue, out ulong result))
            {
                return result;
            }

            return textValue.FirstOrDefault() == '-' ? ulong.MinValue : ulong.MaxValue;
        }

        public override bool IsEqualOrUnderMin(ulong value)
        {
            return value <= _inputRangeMin;
        }

        public override bool IsEqualOrOverMax(ulong value)
        {
            return value >= _inputRangeMax;
        }

        public override ulong ValidateValueForFluctuation(ulong value, int magnification)
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

        ulong _ValidateValue(ulong value)
        {
            return Math.Clamp(value, _inputRangeMin, _inputRangeMax);
        }
    }
}
