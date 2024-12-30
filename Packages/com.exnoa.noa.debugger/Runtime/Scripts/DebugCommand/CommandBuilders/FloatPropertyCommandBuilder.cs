using System;

namespace NoaDebugger.DebugCommand
{
    sealed class FloatPropertyCommandBuilder : MutableNumericPropertyCommandBuilderBase<float>
    {
        int _needDigits = 0;

        public FloatPropertyCommandBuilder(
            string categoryName, string displayName, Func<float> getter, Action<float> setter,
            Attribute[] attributes = null, string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            base.PeekAttribute(attribute);

            if (_increment <= 0)
            {
                SendIncrementWarning();
                _increment = FloatPropertyCommand.DEFAULT_INCREMENT;
            }
        }

        protected override ICommand BuildCommand()
        {
            return new FloatPropertyCommand(
                DisplayName, Getter, Setter, GroupName, GroupOrder, TagName, Description, Order, SaveKey,
                _inputRangeMin, _inputRangeMax, _increment, _needDigits);
        }

        protected override float? TryParse(object value)
        {
            string valueStr = value.ToString();

            if (float.TryParse(valueStr, out float result))
            {
                if (valueStr.Contains(NoaDebuggerDefine.DecimalPoint))
                {
                    int needDigits = valueStr.Length - valueStr.IndexOf(NoaDebuggerDefine.DecimalPoint) - 1;

                    if (needDigits > _needDigits)
                    {
                        _needDigits = needDigits;
                    }
                }

                return result;
            }

            return null;
        }
    }
}
