using System.Collections.Generic;

namespace NoaDebugger.DebugCommand
{
    abstract class CommandBase
    {
        const string DEFAULT_GROUP_NAME = "Others";
        const int DEFAULT_ORDER = int.MaxValue;

        public string DisplayName { get; }
        public string GroupName { get; }
        public int? GroupOrder { get; }
        public string TagName { get; }
        public string Description { get; }
        public int Order { get; }

        protected abstract string TypeName { get; }

        protected CommandBase(
            string displayName, string groupName = null, int? groupOrder = null, string tagName = null,
            string description = null, int? order = null)
        {
            DisplayName = displayName;
            GroupName = groupName ?? CommandBase.DEFAULT_GROUP_NAME;
            GroupOrder = groupOrder;
            TagName = tagName;
            Description = description;
            Order = order ?? CommandBase.DEFAULT_ORDER;
        }

        public virtual Dictionary<string, string> CreateDetailContext()
        {
            var context = new Dictionary<string, string>
            {
                {"Type", TypeName}
            };

            if (TagName != null)
            {
                context.Add("TagName", TagName);
            }

            if (Order != CommandBase.DEFAULT_ORDER)
            {
                context.Add("Order", $"{Order}");
            }

            return context;
        }
    }
}
