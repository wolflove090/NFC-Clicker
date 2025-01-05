namespace NoaDebugger
{
    public class HierarchyPanelDataBase
    {
        public int _depth;
        public bool _isActive;
        public bool _isOpen;
        public bool _isSelected;
        public bool _hasChildren;

        public System.Action _toggleOpen;
        public System.Action _onUpdateView;
    }
}
