#if NOA_DEBUGGER_DEBUG && UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace NoaDebugger
{
    sealed class NoaDebuggerTextCreateObjectMenu
    {
        [MenuItem("GameObject/UI/Text - NoaDebuggerText", false, 2001)]
        static void CreateTextMeshProGuiObjectPerform(MenuCommand menuCommand)
        {
            GameObject go = ObjectFactory.CreateGameObject("Text (ND TMP)");
            NoaDebuggerText tmpro = go.AddComponent<NoaDebuggerText>();
            tmpro.text = "New Text";
            tmpro.raycastTarget = false;
            
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            Selection.activeObject = go;
        }
    }
}
#endif
