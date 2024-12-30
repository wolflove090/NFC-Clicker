using UnityEngine;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class NoaUIObjectElementView : NoaUIElementViewBase<NoaUIObjectElement>
    {
        GameObject _gameObject;

        protected override void OnInitialize(NoaUIObjectElement element)
        {
            if (_gameObject == null)
            {
                if (element.Prefab == null)
                {
                    LogModel.LogWarning($"Warning: The prefab for the object with key '{element.Key}' is not assigned.");
                    return;
                }
                _gameObject = Instantiate(_element.Prefab, gameObject.transform);
                _gameObject.name = _element.Key;
                if (_element.Width > 0 && _element.Height > 0)
                {
                    var layoutElement = _gameObject.AddComponent<LayoutElement>();
                    layoutElement.preferredWidth = _element.Width;
                    layoutElement.preferredHeight = _element.Height;
                }
            }

            if (_element.OnObjectCreated == null)
            {
                LogModel.LogWarning($"Warning: The OnObjectCreated event for the object with key '{element.Key}' is not assigned.");
                return;
            }

            _element.OnObjectCreated.Invoke(_gameObject);
        }

        protected override void OnUpdate()
        {
            if (_gameObject == null)
            {
                Destroy(gameObject);
            }
        }

        protected override void Dispose()
        {
            if (_gameObject != null)
            {
                Destroy(_gameObject);
            }
            _gameObject = null;
        }
    }
}
