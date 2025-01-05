using UnityEngine;

namespace NoaDebugger.DebugCommand
{
    sealed class DragSliderHelper
    {
        const int SENSITIVITY = 2;

        public int Delimit(float distance)
        {
            return Mathf.FloorToInt(distance / SENSITIVITY);
        }
    }
}
