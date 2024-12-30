using UnityEngine;

namespace NoaDebugger
{
    sealed class SafeAreaAdjuster : MonoBehaviour
    {
        RectTransform _rect;

        void Start()
        {
            Adjust();
        }

        public void Adjust()
        {
            SetRect();

            var area = Screen.safeArea;

            var anchorMin = area.position;
            var anchorMax = area.position + area.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;

            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _rect.anchorMin = anchorMin;
            _rect.anchorMax = anchorMax;
        }

        void SetRect()
        {
            if (_rect == null)
            {
                _rect = GetComponent<RectTransform>();
            }
        }
    }
}
