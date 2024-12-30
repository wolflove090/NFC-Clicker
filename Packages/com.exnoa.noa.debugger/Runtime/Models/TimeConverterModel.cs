using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoaDebugger
{
    sealed class TimeUnitConverterModel
    {
        public static int MinutesToSeconds(int minutes)
        {
            return minutes * 60;
        }
        
        public static float SecondsToMinutes(float seconds)
        {
            return seconds / 60;
        }
    }    
}
