using UnityEngine;

namespace NoaDebugger
{
    sealed class ClipboardModel : ModelBase
    {
#if UNITY_WEBGL && !UNITY_EDITOR

#if UNITY_6000_0_OR_NEWER

        [System.Runtime.InteropServices.DllImport("__Internal", EntryPoint = "NoaDebuggerCopyClipboard")]
        static extern string _Copy(string input);
#else
        [System.Runtime.InteropServices.DllImport("__Internal", EntryPoint = "NoaDebuggerCopyExecCommand")]
        static extern string _Copy(string input);
#endif

#else
        static void _Copy(string input)
        {
            GUIUtility.systemCopyBuffer = input;
        }
#endif

        public static void Copy(string input)
        {
            _Copy(input);
        }
    }
}
