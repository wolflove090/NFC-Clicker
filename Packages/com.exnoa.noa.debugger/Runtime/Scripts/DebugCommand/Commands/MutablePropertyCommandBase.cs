using System;
using System.Collections.Generic;
using System.Reflection;

namespace NoaDebugger.DebugCommand
{
    abstract class MutablePropertyCommandBase<T> : CommandBase, ICommand
    {
        public bool IsInteractable { get; set; } = true;
        public bool IsVisible { get; set; } = true;

        readonly Func<T> _getter;
        readonly Action<T> _setter;

        protected string SaveKey { get; }

        protected bool SavesOnUpdate => !string.IsNullOrEmpty(SaveKey);

        protected MutablePropertyCommandBase(
            string displayName, Func<T> getter, Action<T> setter, string groupName = null, int? groupOrder = null,
            string tagName = null, string description = null, int? order = null, string saveKey = null)
            : base(displayName, groupName, groupOrder, tagName, description, order)
        {
            _getter = getter;
            _setter = setter;
            SaveKey = saveKey;
        }

        protected abstract void _Accept(ICommandVisitor visitor);
        public void Accept(ICommandVisitor visitor)
        {
            _Accept(visitor);
        }

        public override Dictionary<string, string> CreateDetailContext()
        {
            Dictionary<string, string> context = base.CreateDetailContext();
            context.Add("Value", $"{_getter()}");
            context.Add("SaveOnUpdate", $"{SavesOnUpdate}");

            return context;
        }

        protected T InvokeGetter()
        {
            try
            {
                return _getter();
            }
            catch
            {
                throw;
            }
        }

        protected void InvokeSetter(T value)
        {
            try
            {
                _setter(value);
            }
            catch
            {
                throw;
            }
        }
    }
}
