using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class NoaUIElementManager : MonoBehaviour
    {
        [SerializeField]
        Transform _uIElementRootPosition;
        [SerializeField]
        Transform _upperLeftPosition;
        [SerializeField]
        Transform _upperCenterPosition;
        [SerializeField]
        Transform _upperRightPosition;
        [SerializeField]
        Transform _middleCenterPosition;
        [SerializeField]
        Transform _lowerLeftPosition;
        [SerializeField]
        Transform _lowerCenterPosition;
        [SerializeField]
        Transform _lowerRightPosition;

        [SerializeField]
        GameObject _textElementPrefab;
        [SerializeField]
        GameObject _buttonElementPrefab;

        Dictionary<string, INoaUIElementView> _registeredElements = new Dictionary<string, INoaUIElementView>();

        void Update()
        {
            _CleanUpRegisteredElements();
        }

        void _CleanUpRegisteredElements()
        {
            var keysToRemove = new List<string>();

            foreach (var kvp in _registeredElements)
            {
                if (kvp.Value == null || kvp.Value.IsDisposed)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _registeredElements.Remove(key);
            }
        }

        public void RegisterUIElement(INoaUIElement element)
        {
            if (element == null)
            {
                LogModel.LogWarning("Warning: Tried to register a null UI element.");
                return;
            }

            if (string.IsNullOrEmpty(element.Key))
            {
                LogModel.LogWarning("Warning: Tried to register a UI element with a null or empty key.");
                return;
            }

            if (_registeredElements.ContainsKey(element.Key))
            {
                var elementView = _registeredElements[element.Key];
                if (elementView.Element.GetType() == element.GetType())
                {
                    _InitializeUIElement(element, _registeredElements[element.Key]);
                }
                else
                {
                    UnregisterUIElement(element.Key);
                    _CreateUIElement(element);
                }
            }
            else
            {
                _CreateUIElement(element);
            }
        }

        public void UnregisterUIElement(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                LogModel.LogWarning("Warning: Tried to unregister a UI element with a null or empty key.");
                return;
            }

            if (!_registeredElements.ContainsKey(key))
            {
                LogModel.LogWarning($"Warning: Tried to unregister non-existing UI element with key '{key}'.");
                return;
            }

            var elementView = _registeredElements[key];

            if (elementView is { IsDisposed: false })
                Destroy(elementView.GameObject);

            _registeredElements.Remove(key);
        }

        public void UnregisterAllUIElements()
        {
            foreach (var elementView in _registeredElements.Values)
            {
                Destroy(elementView.GameObject);
            }
            _registeredElements.Clear();
        }

        public bool IsUIElementRegistered(string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                return _registeredElements.Count > 0;
            }
            return _registeredElements.ContainsKey(key);
        }

        public void SetUIElementVisibility(string key, bool visible)
        {
            if (string.IsNullOrEmpty(key))
            {
                LogModel.LogWarning("Warning: Tried to set visibility for a UI element with a null or empty key.");
                return;
            }

            if (!_registeredElements.ContainsKey(key))
            {
                LogModel.LogWarning($"Warning: Tried to set visibility for non-existing UI element with key '{key}'.");
                return;
            }

            _registeredElements[key].GameObject.SetActive(visible);
        }

        public void SetAllUIElementsVisibility(bool visible)
        {
            foreach (var elementView in _registeredElements.Values)
            {
                elementView.GameObject.SetActive(visible);
            }
        }

        public bool IsUIElementVisible(string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                return _registeredElements.Values.Any(elementView => elementView.GameObject.activeSelf);
            }
            if (!_registeredElements.ContainsKey(key))
                return false;
            return _registeredElements[key].GameObject.activeSelf;
        }

        void _CreateUIElement(INoaUIElement element)
        {
            var elementView = _CreatePrefabForElementView(element);
            _InitializeUIElement(element, elementView);

            _registeredElements.Add(element.Key, elementView);
        }

        INoaUIElementView _CreatePrefabForElementView(INoaUIElement element)
        {
            if (element is NoaUIObjectElement)
            {
                var obj = new GameObject(element.Key).AddComponent<NoaUIObjectElementView>();
                var layoutGroup = obj.gameObject.AddComponent<VerticalLayoutGroup>();
                layoutGroup.childControlWidth = true;
                layoutGroup.childControlHeight = true;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;

                return obj;
            }

            if (element is NoaUITextElement)
                return Instantiate(_textElementPrefab).GetComponent<NoaUITextElementView>();

            if (element is NoaUIButtonElement)
                return Instantiate(_buttonElementPrefab).GetComponent<NoaUIButtonElementView>();

            return null;
        }

        void _InitializeUIElement(INoaUIElement element, INoaUIElementView elementView)
        {
            if (elementView.GameObject.TryGetComponent(out NoaUIObjectElementView objectElementView))
                objectElementView.Initialize((NoaUIObjectElement)element);

            if (elementView.GameObject.TryGetComponent(out NoaUITextElementView textElementView))
                textElementView.Initialize((NoaUITextElement)element);

            if (elementView.GameObject.TryGetComponent(out NoaUIButtonElementView buttonElementView))
                buttonElementView.Initialize((NoaUIButtonElement)element);

            var anchorTransform = _GetAnchorTransform(element.AnchorType);

            if (element.Parent != null)
            {
                _SetParentLayoutComponents(element);
                elementView.GameObject.transform.SetParent(element.Parent, false);
            }
            else if (anchorTransform != null)
            {
                elementView.GameObject.transform.SetParent(anchorTransform, false);
            }
            else
            {
                elementView.GameObject.transform.SetParent(_uIElementRootPosition, false);
            }
        }

        void _SetParentLayoutComponents(INoaUIElement element)
        {
            if (!element.Parent.TryGetComponent(out VerticalLayoutGroup verticalLayoutGroup))
            {
                var layoutGroup = element.Parent.gameObject.AddComponent<VerticalLayoutGroup>();
                layoutGroup.childControlWidth = true;
                layoutGroup.childControlHeight = true;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;
            }
            if (!element.Parent.TryGetComponent(out ContentSizeFitter contentSizeFitter))
            {
                var sizeFitter = element.Parent.gameObject.AddComponent<ContentSizeFitter>();
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

        }

        Transform _GetAnchorTransform(AnchorType anchorType)
        {
            switch (anchorType)
            {
                case AnchorType.UpperLeft:
                    return _upperLeftPosition;
                case AnchorType.UpperCenter:
                    return _upperCenterPosition;
                case AnchorType.UpperRight:
                    return _upperRightPosition;
                case AnchorType.MiddleCenter:
                    return _middleCenterPosition;
                case AnchorType.LowerLeft:
                    return _lowerLeftPosition;
                case AnchorType.LowerCenter:
                    return _lowerCenterPosition;
                case AnchorType.LowerRight:
                    return _lowerRightPosition;
                default:
                    return null;
            }
        }

        void OnDestroy()
        {
            foreach (var elementView in _registeredElements.Values)
            {
                if (elementView == null || elementView.IsDisposed)
                    continue;

                Destroy(elementView.GameObject);
            }
            _registeredElements.Clear();
            _registeredElements = null;
        }
    }
}
