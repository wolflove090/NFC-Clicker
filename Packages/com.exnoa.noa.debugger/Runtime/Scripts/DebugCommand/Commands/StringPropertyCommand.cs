using System;
using System.Collections.Generic;

namespace NoaDebugger.DebugCommand
{
    sealed class StringPropertyCommand : MutablePropertyCommandBase<string>
    {
        readonly int _unlimited = 0;
        bool IsUnlimited => CharacterLimit == _unlimited;

        public int CharacterLimit { get; }

        protected override string TypeName => "String Property";

        public StringPropertyCommand(
            string displayName, Func<string> getter, Action<string> setter, string groupName = null,
            int? groupOrder = null, string tagName = null, string description = null, int? order = null,
            string saveKey = null, int? characterLimit = null)
            : base(displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey)
        {
            CharacterLimit = characterLimit ?? _unlimited;

            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetString(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public string GetValue()
        {
            return InvokeGetter();
        }

        public void SetValue(string value)
        {
            value = ValidateValue(value);
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetString(SaveKey, value);
            }
        }

        string ValidateValue(string value)
        {
            if (IsUnlimited || value.Length <= CharacterLimit)
            {
                return value;
            }

            return value.Substring(0, CharacterLimit);
        }

        public override Dictionary<string, string> CreateDetailContext()
        {
            Dictionary<string, string> context = base.CreateDetailContext();
            context.Add("CharacterLimit", IsUnlimited ? "Unlimited" : $"{CharacterLimit}");

            return context;
        }
    }
}
