using System;

namespace NoaDebugger.DebugCommand
{
    sealed class EnumPropertyCommand : MutablePropertyCommandBase<Enum>
    {
        readonly Type _enumType;
        readonly string[] _enumNames;

        protected override string TypeName => "Enum Property";

        public EnumPropertyCommand(
            string displayName, Func<Enum> getter, Action<Enum> setter, Type type, string groupName = null,
            int? groupOrder = null, string tagName = null, string description = null, int? order = null,
            string saveKey = null)
            : base(displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey)
        {
            _enumType = type;
            _enumNames = Enum.GetNames(_enumType);

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetString(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public string[] GetNames()
        {
            return _enumNames;
        }

        public string GetValue()
        {
            return InvokeGetter().ToString();
        }

        public void SetValue(string valueName)
        {
            Enum.TryParse(_enumType, valueName, out object value);
            InvokeSetter((Enum)value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetString(SaveKey, valueName);
            }
        }
    }
}
