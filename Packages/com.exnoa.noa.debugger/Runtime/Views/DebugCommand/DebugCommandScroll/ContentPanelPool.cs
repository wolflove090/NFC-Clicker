using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger.DebugCommand
{
    sealed class ContentPanelPool
    {
        readonly GameObject _prefab;
        readonly Transform _contentRoot;

        List<GameObject> _cacheList;

        public ContentPanelPool(GameObject prefab, Transform contentRoot)
        {
            _prefab = prefab;
            _contentRoot = contentRoot;
            _cacheList = new List<GameObject>();
        }

        public (GameObject, int) GetObjectAndIndex()
        {
            (GameObject, int) obj = _GetGameObjectAndIndex();
            obj.Item1.SetActive(true);

            LayoutElement layout = obj.Item1.GetComponent<LayoutElement>();
            if (layout != null)
            {
                layout.preferredWidth = -1; 
            }

            return obj;
        }

        (GameObject, int) _GetGameObjectAndIndex()
        {
            GameObject result = null;
            int index = 0;

            for (; index < _cacheList.Count; index++)
            {
                GameObject target = _cacheList[index];
                if (!target.activeSelf)
                {
                    result = target;
                    break;
                }
            }

            if (result == null)
            {
                result = Object.Instantiate(_prefab, _contentRoot);
                index = _cacheList.Count;
                _cacheList.Add(result);
            }

            return (result, index);
        }

        public void ReturnObject(int index)
        {
            _cacheList[index].SetActive(false);
        }
    }
}
