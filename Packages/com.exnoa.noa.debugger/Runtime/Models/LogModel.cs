using System;
using UnityEngine;

namespace NoaDebugger
{
    static class LogModel
    {
        const string DEBUG_SYMBOL = "NOA_DEBUGGER_DEBUG";

        [System.Diagnostics.Conditional(DEBUG_SYMBOL)]
        public static void DebugLog(string message)
        {
            Debug.Log($"[NoaDebugger] {message}");
        }

        [System.Diagnostics.Conditional(DEBUG_SYMBOL)]
        public static void DebugLog(object message)
        {
            Debug.Log($"[NoaDebugger] {message}");
        }

        [System.Diagnostics.Conditional(DEBUG_SYMBOL)]
        public static void DebugLogWarning(string message)
        {
            Debug.LogWarning($"[NoaDebugger] {message}");
        }

        [System.Diagnostics.Conditional(DEBUG_SYMBOL)]
        public static void DebugLogWarning(string message, UnityEngine.Object context)
        {
            Debug.LogWarning($"[NoaDebugger] {message}", context);
        }

        [System.Diagnostics.Conditional(DEBUG_SYMBOL)]
        public static void ThrowException(string message)
        {
            throw new Exception(message);
        }

        public static void Log(object message)
        {
            Debug.Log($"[NoaDebugger] {message}");
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning($"[NoaDebugger] {message}");
        }

        public static void Log(object message, UnityEngine.Object context)
        {
            Debug.Log($"[NoaDebugger] {message}", context);
        }

        public static void LogWarning(string message, UnityEngine.Object context)
        {
            Debug.LogWarning($"[NoaDebugger] {message}", context);
        }

        public static void CollectNoaDebuggerErrorLog(string message, string stackTrace)
        {
            if (!NoaDebugger.IsInternalError(stackTrace))
            {
                return;
            }

            if (NoaDebugger.ContainsCustomClassNameByText(stackTrace))
            {
                return;
            }

            Debug.Log($"[Error Log] {message} {Environment.NewLine}{stackTrace}");
        }
    }
}
