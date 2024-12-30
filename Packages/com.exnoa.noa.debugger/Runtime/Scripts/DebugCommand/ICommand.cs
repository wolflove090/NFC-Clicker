using System.Collections.Generic;

namespace NoaDebugger.DebugCommand
{
    interface ICommand
    {
        string DisplayName { get; }

        string GroupName { get; }

        int? GroupOrder { get; }

        string TagName { get; }

        string Description { get; }

        int Order { get; }

        bool IsInteractable { get; set; }

        bool IsVisible { get; set; }

        void Accept(ICommandVisitor visitor);

        Dictionary<string, string> CreateDetailContext();

        public string GetDetailSuffix()
        {
            if (Description == null)
            {
                return null;
            }

            return $"\nDescription\n{Description}";
        }
    }
}
