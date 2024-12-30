using System;
using System.Collections.Generic;

namespace NoaDebugger.DebugCommand
{
    abstract class MutableNumericPropertyCommandBase<T> : MutablePropertyCommandBase<T> where T : struct
    {
        protected T _inputRangeMin;
        protected T _inputRangeMax;

        readonly bool _isSetRange;

        public T Increment { get; protected set; }

        protected MutableNumericPropertyCommandBase(
            string displayName, Func<T> getter, Action<T> setter, string groupName = null, int? groupOrder = null,
            string tagName = null, string description = null, int? order = null, string saveKey = null,
            T? inputRangeMin = null, T? inputRangeMax = null, T? increment = null)
            : base(displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey)
        {
            _isSetRange = inputRangeMin != null && inputRangeMax != null;
        }

        public override Dictionary<string, string> CreateDetailContext()
        {
            Dictionary<string, string> context = base.CreateDetailContext();
            context.Add("InputRange", _isSetRange ? $"{_inputRangeMin} ~ {_inputRangeMax}" : "None");
            context.Add("Increment", $"{Increment}");

            return context;
        }


        public abstract T GetValue();

        public abstract void SetValue(T value);

        public abstract T FromString(string textValue);

        public abstract bool IsEqualOrUnderMin(T value);

        public abstract bool IsEqualOrOverMax(T value);

        public abstract T ValidateValueForFluctuation(T value, int magnification);

        protected bool IsNotNumeric(string text)
        {
            return string.IsNullOrEmpty(text) || text is "-" or ".";
        }
    }
}
