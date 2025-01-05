using System;
using System.Linq;

namespace NoaDebugger.DebugCommand
{
    sealed class FloatPropertyCommand : MutableNumericPropertyCommandBase<float>
    {
        static readonly float DEFAULT_INPUT_RANGE_MIN = float.MinValue;
        static readonly float DEFAULT_INPUT_RANGE_MAX = float.MaxValue;
        public static readonly float DEFAULT_INCREMENT = 1;

        protected override string TypeName => "Float Property";

        readonly int _needDigits;

        public FloatPropertyCommand(
            string displayName, Func<float> getter, Action<float> setter, string groupName = null,
            int? groupOrder = null, string tagName = null, string description = null, int? order = null,
            string saveKey = null, float? inputRangeMin = null, float? inputRangeMax = null, float? increment = null,
            int needDigits = 0)
            : base(
                displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey, inputRangeMin,
                inputRangeMax, increment)
        {
            _inputRangeMin = inputRangeMin ?? FloatPropertyCommand.DEFAULT_INPUT_RANGE_MIN;
            _inputRangeMax = inputRangeMax ?? FloatPropertyCommand.DEFAULT_INPUT_RANGE_MAX;
            Increment = increment ?? FloatPropertyCommand.DEFAULT_INCREMENT;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetFloat(SaveKey, GetValue()));
            }

            _needDigits = needDigits;
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override float GetValue()
        {
            return _ValidateValue(InvokeGetter());
        }

        public override void SetValue(float value)
        {
            value = _ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetFloat(SaveKey, value);
            }
        }

        public override float FromString(string textValue)
        {
            if (IsNotNumeric(textValue))
            {
                return 0;
            }

            if (float.TryParse(textValue, out float result))
            {
                return result;
            }

            return textValue.FirstOrDefault() == '-' ? float.MinValue : float.MaxValue;
        }

        public override bool IsEqualOrUnderMin(float value)
        {
            return value <= _inputRangeMin;
        }

        public override bool IsEqualOrOverMax(float value)
        {
            return value >= _inputRangeMax;
        }

        public override float ValidateValueForFluctuation(float value, int magnification)
        {
            float tmpValue = value;

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

            float clamped = _ValidateValue(value + Increment * magnification);

            return MathF.Round(clamped, _needDigits, MidpointRounding.AwayFromZero);
        }

        float _ValidateValue(float value)
        {
            return Math.Clamp(value , _inputRangeMin, _inputRangeMax);
        }
    }
}
