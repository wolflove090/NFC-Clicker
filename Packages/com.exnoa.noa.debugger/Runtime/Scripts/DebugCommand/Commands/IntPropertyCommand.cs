using System;
using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class IntPropertyCommand : MutableNumericPropertyCommandBase<int>
    {
        static readonly int DEFAULT_INPUT_RANGE_MIN = int.MinValue;
        static readonly int DEFAULT_INPUT_RANGE_MAX = int.MaxValue;
        public static readonly int DEFAULT_INCREMENT = 1;

        protected override string TypeName => "Int Property";

        public IntPropertyCommand(
            string displayName, Func<int> getter, Action<int> setter, string groupName = null, int? groupOrder = null,
            string tagName = null, string description = null, int? order = null, string saveKey = null,
            int? inputRangeMin = null, int? inputRangeMax = null, int? increment = null)
            : base(
                displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey, inputRangeMin,
                inputRangeMax, increment)
        {
            _inputRangeMin = inputRangeMin ?? IntPropertyCommand.DEFAULT_INPUT_RANGE_MIN;
            _inputRangeMax = inputRangeMax ?? IntPropertyCommand.DEFAULT_INPUT_RANGE_MAX;
            Increment = increment ?? IntPropertyCommand.DEFAULT_INCREMENT;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetInt(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override int GetValue()
        {
            return _ValidateValue(InvokeGetter());
        }

        public override void SetValue(int value)
        {
            value = _ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetInt(SaveKey, value);
            }
        }

        public override int FromString(string textValue)
        {
            if (IsNotNumeric(textValue))
            {
                return 0;
            }

            if (int.TryParse(textValue, out int result))
            {
                return result;
            }

            return textValue.FirstOrDefault() == '-' ? int.MinValue : int.MaxValue;
        }

        public override bool IsEqualOrUnderMin(int value)
        {
            return value <= _inputRangeMin;
        }

        public override bool IsEqualOrOverMax(int value)
        {
            return value >= _inputRangeMax;
        }

        public override int ValidateValueForFluctuation(int value, int magnification)
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

        int _ValidateValue(int value)
        {
            return Math.Clamp(value, _inputRangeMin, _inputRangeMax);
        }
    }
}
