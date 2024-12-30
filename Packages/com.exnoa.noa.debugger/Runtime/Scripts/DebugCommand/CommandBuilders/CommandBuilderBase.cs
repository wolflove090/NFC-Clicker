using System;

namespace NoaDebugger.DebugCommand
{
    abstract class CommandBuilderBase
    {
        protected string CategoryName { get; }

        protected string DisplayName { get; }

        protected string GroupName { get; private set; }

        protected int? GroupOrder { get; private set; }

        protected string TagName { get; private set; }

        protected string Description { get; private set; }

        protected int? Order { get; private set; }

        readonly Attribute[] _attributes = null;

        protected CommandBuilderBase(string categoryName, string displayName, Attribute[] attributes = null)
        {
            CategoryName = categoryName;
            DisplayName = displayName;
            _attributes = attributes;
        }

        public ICommand Build()
        {
            if (_attributes != null)
            {
                foreach (Attribute attribute in _attributes)
                {
                    if (attribute is CommandGroupAttribute group)
                    {
                        GroupName = group._name;
                        GroupOrder = group._order;
                    }
                    if (attribute is CommandTagAttribute tag)
                    {
                        TagName = tag._tag;
                    }
                    if (attribute is CommandDescriptionAttribute description)
                    {
                        Description = description._text;
                    }
                    if (attribute is CommandOrderAttribute order)
                    {
                        Order = order._order;
                    }
                    else
                    {
                        PeekAttribute(attribute);
                    }
                }
            }

            return BuildCommand();
        }

        protected virtual void PeekAttribute(Attribute attribute) { }

        protected abstract ICommand BuildCommand();
    }
}
