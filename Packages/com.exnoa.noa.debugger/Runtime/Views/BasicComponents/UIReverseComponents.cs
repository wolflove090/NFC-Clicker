using System;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    [Serializable]
    sealed class UIReverseComponents
    {
        [SerializeField] HorizontalOrVerticalLayoutGroup[] _reverseTargetLayouts;
        [SerializeField] GameObject[] _normalArrangementObjects;
        [SerializeField] GameObject[] _reverseArrangementObjects;

        public void Alignment(bool isReverse)
        {
            foreach (HorizontalOrVerticalLayoutGroup layout in _reverseTargetLayouts)
            {
                layout.reverseArrangement = isReverse;
            }

            foreach (GameObject normalObject in _normalArrangementObjects)
            {
                normalObject.SetActive(!isReverse);
            }

            foreach (GameObject reverseObject in _reverseArrangementObjects)
            {
                reverseObject.SetActive(isReverse);
            }
        }
    }
}
