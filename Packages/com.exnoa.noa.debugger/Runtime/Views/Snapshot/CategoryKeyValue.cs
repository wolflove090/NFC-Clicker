using System;
using System.Collections;
using System.Collections.Generic;
using NoaDebugger;
using UnityEngine;

namespace NoaDebugger
{
    sealed class CategoryKeyValue : MonoBehaviour
    {
        [SerializeField]
        NoaDebuggerText _key;
        [SerializeField]
        NoaDebuggerText _value;

        public void Init(NoaSnapshotCategoryItem item)
        {
            gameObject.SetActive(true);
            _key.text = item.Key;
            _value.text = item.Value;
            _value.color = _LogFontColorToColor(item.Color);
        }

        Color _LogFontColorToColor(NoaSnapshot.FontColor color)
        {
            switch (color)
            {
                case NoaSnapshot.FontColor.White:
                    return Color.white;
                case NoaSnapshot.FontColor.Black:
                    return Color.black;
                case NoaSnapshot.FontColor.Gray:
                    return NoaDebuggerDefine.TextColors.LogGray;
                case NoaSnapshot.FontColor.LightBlue:
                    return NoaDebuggerDefine.TextColors.LogLightBlue;
                case NoaSnapshot.FontColor.Orange:
                    return NoaDebuggerDefine.TextColors.LogOrange;
                case NoaSnapshot.FontColor.Yellow:
                    return NoaDebuggerDefine.TextColors.LogYellow;
                case NoaSnapshot.FontColor.Blue:
                    return NoaDebuggerDefine.TextColors.LogBlue;
                case NoaSnapshot.FontColor.Purple:
                    return NoaDebuggerDefine.TextColors.LogPurple;
                case NoaSnapshot.FontColor.Green:
                    return NoaDebuggerDefine.TextColors.LogGreen;
                case NoaSnapshot.FontColor.Red:
                    return NoaDebuggerDefine.TextColors.LogRed;
            }
            return Color.white;
        }

        void OnDestroy()
        {
            _key = default;
            _value = default;
        }
    }
}
