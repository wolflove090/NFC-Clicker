using System;
using System.Reflection;

namespace NoaDebugger.DebugCommand
{
    sealed class MethodCommand : CommandBase, ICommand
    {
        public bool IsInteractable { get; set; } = true;
        public bool IsVisible { get; set; } = true;

        protected override string TypeName => "Method";

        readonly Action _method;

        public MethodCommand(
            string displayName, Action method, string groupName = null, int? groupOrder = null, string tagName = null,
            string description = null, int? order = null)
            : base(displayName, groupName, groupOrder, tagName, description, order)
        {
            _method = method;
        }

        public void Accept(ICommandVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Invoke()
        {
            try
            {
                _method();
            }
            catch
            {
                throw;
            }
        }
    }
}
