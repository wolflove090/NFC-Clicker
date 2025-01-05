using UnityEngine;

namespace NoaDebugger
{
    interface IMenuInfo
    {
        string Name { get; }

        string MenuName { get; }

        int SortNo { get; }
    }

    enum ToolPinStatus
    {
        None = 0,

        Off = 1,

        On = 2
    }

    enum ToolNotificationStatus
    {
        None = 0,

        Error = 1
    }

    interface INoaDebuggerTool
    {
        void Init();

        ToolNotificationStatus NotifyStatus { get; }

        IMenuInfo MenuInfo();

        void ShowView(Transform parent);

        ToolPinStatus GetPinStatus();

        void TogglePin(Transform parent);

        void InitFloatingWindow(Transform parent);

        void AlignmentUI(bool isReverse);

        void OnHidden();

        void OnToolDispose();
    }
}
