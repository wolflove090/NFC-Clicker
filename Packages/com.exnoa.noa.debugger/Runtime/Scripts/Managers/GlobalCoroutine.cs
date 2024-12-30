using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class GlobalCoroutine : MonoBehaviour
    {
        static GlobalCoroutine _instance;

        public static void Init(Transform parent)
        {
            var obj = new GameObject("GlobalCoroutine");
            obj.gameObject.transform.parent = parent;
            GlobalCoroutine._instance = obj.AddComponent<GlobalCoroutine>();
        }

        public static void Dispose()
        {
            GlobalCoroutine._instance = null;
        }

        public static Coroutine Run(IEnumerator routine, UnityAction onComplete = null)
        {
            if (GlobalCoroutine._instance == null)
            {
                LogModel.DebugLogWarning("Instance has not been created.");
                return null;
            }
            return GlobalCoroutine._instance.StartCoroutine(GlobalCoroutine._instance.Routine(routine, onComplete));
        }

        IEnumerator Routine(IEnumerator src, UnityAction onComplete)
        {
            yield return StartCoroutine(src);
            onComplete?.Invoke();
        }

        public static void Stop(IEnumerator routine)
        {
            if (GlobalCoroutine._instance == null)
            {
                LogModel.DebugLogWarning("Instance has not been created.");
                return;
            }
            GlobalCoroutine._instance.StopCoroutine(routine);
        }
    }
}
