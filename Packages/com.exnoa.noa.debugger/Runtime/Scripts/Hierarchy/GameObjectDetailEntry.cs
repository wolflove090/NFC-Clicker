using System.Collections.Generic;

namespace NoaDebugger
{
     sealed class GameObjectDetailEntry
    {
        public string _name;

        public string _value;

        public bool _isOpen;

        public GameObjectDetailEntryCallback _callback;

        public List<GameObjectDetailEntry> _subDetailList  = new List<GameObjectDetailEntry>();

        public List<GameObjectDetailPanelData> GetPanelDataListWithSubDetail(GameObjectDetailEntryCallback callback)
        {
            var list = new List<GameObjectDetailPanelData>();
            _AddPanelDataList(list, callback);

            return list;
        }

        void _AddPanelDataList(List<GameObjectDetailPanelData> list, GameObjectDetailEntryCallback callback, int depth = 0)
        {
            _callback = callback;

            GameObjectDetailPanelData panelData = new GameObjectDetailPanelData(
                this, depth, _OnToggleOpen);
            list.Add(panelData);

            if (_isOpen)
            {
                foreach(var child in _subDetailList)
                {
                    child._AddPanelDataList(list, callback, depth + 1);
                }
            }
        }

        void _OnToggleOpen()
        {
            _isOpen = !_isOpen;
            _callback._onUpdateView?.Invoke();
        }
    }

    public sealed class GameObjectDetailEntryCallback
    {
        public System.Action _onUpdateView;
    }
}
