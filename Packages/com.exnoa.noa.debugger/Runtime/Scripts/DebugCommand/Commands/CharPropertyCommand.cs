using System;
using System.Collections.Generic;

namespace NoaDebugger.DebugCommand
{
    sealed class CharPropertyCommand : MutablePropertyCommandBase<char>
    {
        protected override string TypeName => "Char Property";

        public CharPropertyCommand(
            string displayName, Func<char> getter, Action<char> setter, string groupName = null, int? groupOrder = null,
            string tagName = null, string description = null, int? order = null, string saveKey = null)
            : base(displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey)
        {
            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetChar(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public char GetValue()
        {
            return InvokeGetter();
        }

        public void SetValue(char value)
        {
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetChar(SaveKey, value);
            }
        }
    }
}
