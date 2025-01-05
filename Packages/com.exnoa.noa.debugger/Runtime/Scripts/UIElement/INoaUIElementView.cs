using UnityEngine;

namespace NoaDebugger
{
    interface INoaUIElementView
    {
        INoaUIElement Element { get; }
        GameObject GameObject { get; }
        bool IsDisposed { get; }
    }
}
