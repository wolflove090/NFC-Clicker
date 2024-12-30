using System;

namespace NoaDebugger.DebugCommand
{
    abstract class MutableNumericPropertyCommandBuilderBase<T> : MutablePropertyCommandBuilderBase<T> where T : struct
    {
        protected T? _inputRangeMin;
        protected T? _inputRangeMax;
        protected T? _increment;

        public MutableNumericPropertyCommandBuilderBase(
            string categoryName, string displayName, Func<T> getter, Action<T> setter, Attribute[] attributes = null,
            string saveKey = null)
            : base(categoryName, displayName, getter, setter, attributes, saveKey) { }

        protected override void PeekAttribute(Attribute attribute)
        {
            if (attribute is CommandInputRangeAttribute range)
            {
                _inputRangeMin = TryParse(range._min);
                _inputRangeMax = TryParse(range._max);
            }
            else if (attribute is CommandIncrementAttribute increment)
            {
                _increment = TryParse(increment._increment);
            }
        }

        protected abstract T? TryParse(object value);

        protected void SendIncrementWarning()
        {
            LogModel.LogWarning($"Please specify an increase amount greater than 0.\nCategory: {CategoryName}, Group: {GroupName}, Command: {DisplayName}, Increment: {_increment}");
        }
    }
}
