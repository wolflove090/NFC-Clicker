using System.Collections.Generic;
using UnityEngine;

namespace NoaDebugger
{
    sealed class ToggleButtonGroup : MonoBehaviour
    {
        readonly List<ToggleButtonBase> _toggles = new();

        public void Add(ToggleButtonBase toggle)
        {
            _toggles.Add(toggle);
        }

        public void Remove(ToggleButtonBase toggle)
        {
            _toggles.Remove(toggle);
        }

        public void Select(ToggleButtonBase toggle)
        {
            foreach (ToggleButtonBase t in _toggles)
            {
                if (t.IsOn && (t != toggle))
                {
                    t.Clear();
                }
            }
        }
    }
}
