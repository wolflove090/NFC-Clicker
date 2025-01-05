using System;

namespace NoaDebugger.DebugCommand
{
    sealed class BoolPropertyCommand : MutablePropertyCommandBase<bool>
    {
        protected override string TypeName => "Bool Property";

        public BoolPropertyCommand(
            string displayName, Func<bool> getter, Action<bool> setter, string groupName = null, int? groupOrder = null,
            string tagName = null, string description = null, int? order = null, string saveKey = null)
            : base(displayName, getter, setter, groupName, groupOrder, tagName, description, order, saveKey)
        {
            if (SavesOnUpdate && NoaDebuggerPrefs.HasKey(SaveKey))
            {
                SetValue(NoaDebuggerPrefs.GetBoolean(SaveKey, GetValue()));
            }
        }

        protected override void _Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public bool GetValue()
        {
            return InvokeGetter();
        }

        public void SetValue(bool value)
        {
            InvokeSetter(value);

            if (SavesOnUpdate)
            {
                NoaDebuggerPrefs.SetBoolean(SaveKey, value);
            }
        }
    }
}
