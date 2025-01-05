using System;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    /// <summary>
    /// Static class to manage registration and unregistration of UI elements.
    /// </summary>
    public static class NoaUIElement
    {
        /// <summary>
        /// Registers a UI element.
        /// </summary>
        /// <param name="element">The UI element to register.</param>
        public static void RegisterUIElement(INoaUIElement element)
        {
            NoaDebuggerManager.RegisterUIElement(element);
        }

        /// <summary>
        /// Unregisters a UI element based on its key.
        /// </summary>
        /// <param name="key">The key of the UI element to unregister.</param>
        public static void UnregisterUIElement(string key)
        {
            NoaDebuggerManager.UnregisterUIElement(key);
        }

        /// <summary>
        /// Unregisters all UI elements.
        /// </summary>
        public static void UnregisterAllUIElements()
        {
            NoaDebuggerManager.UnregisterAllUIElements();
        }

        /// <summary>
        /// Checks if a UI element is registered.
        /// If no key is specified, it checks if there are any registered UI elements.
        /// </summary>
        /// <param name="key">The key of the UI element to check registration, or null to check if there are any registered elements.</param>
        /// <returns>True if the specified element or at least one element is registered, false otherwise.</returns>
        public static bool IsUIElementRegistered(string key = null)
        {
            return NoaDebuggerManager.IsUIElementRegistered(key);
        }

        /// <summary>
        /// Sets the visibility of a single UI element.
        /// </summary>
        /// <param name="key">The key of the UI element to show or hide.</param>
        /// <param name="visible">If true, the element will be shown. Otherwise, it will be hidden.</param>
        public static void SetUIElementVisibility(string key, bool visible)
        {
            NoaDebuggerManager.SetUIElementVisibility(key, visible);
        }

        /// <summary>
        /// Sets the visibility of all registered UI elements.
        /// </summary>
        /// <param name="visible">If true, all elements will be shown. Otherwise, they will be hidden.</param>
        public static void SetAllUIElementsVisibility(bool visible)
        {
            NoaDebuggerManager.SetAllUIElementsVisibility(visible);
        }

        /// <summary>
        /// Checks the visibility of a UI element.
        /// If no key is specified, it checks if all UI elements are visible.
        /// </summary>
        /// <param name="key">The key of the UI element to check visibility, or null to check all elements.</param>
        /// <returns>True if the specified element or all elements are visible, false otherwise.</returns>
        public static bool IsUIElementVisible(string key = null)
        {
            return NoaDebuggerManager.IsUIElementVisible(key);
        }
    }

    /// <summary>
    /// Enum representing different anchor positions for UI elements.
    /// </summary>
    public enum AnchorType
    {
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleCenter,
        LowerLeft,
        LowerCenter,
        LowerRight,
    }

    /// <summary>
    /// Interface for a basic UI element with a key and anchor type.
    /// </summary>
    public interface INoaUIElement
    {
        string Key { get; }
        AnchorType AnchorType { get; }
        RectTransform Parent { get; }
    }

    /// <summary>
    /// Interface for a UI element that can be updated regularly.
    /// </summary>
    public interface IUpdatableNoaUIElement : INoaUIElement
    {
        /// <summary>
        /// Interval in seconds between updates.
        /// </summary>
        float UpdateInterval { get; }
        /// <summary>
        /// Method to update the value of the UI element.
        /// </summary>
        /// <returns>The updated value as a string.</returns>
        string UpdateValue();
    }

    /// <summary>
    /// Class representing a UI object element.
    /// </summary>
    public sealed class NoaUIObjectElement : INoaUIElement
    {
        string _key;
        AnchorType _anchorType;
        RectTransform _parent;

        GameObject _prefab;
        Action<GameObject> _onObjectCreated;
        float _width;
        float _height;

        public string Key => _key;
        public AnchorType AnchorType => _anchorType;
        public RectTransform Parent => _parent;

        public GameObject Prefab => _prefab;
        public Action<GameObject> OnObjectCreated => _onObjectCreated;
        public float Width => _width;
        public float Height => _height;

        /// <summary>
        /// Factory method to create a new UI object element.
        /// </summary>
        /// <param name="key">The unique key for the UI element.</param>
        /// <param name="prefab">The prefab to be used for the UI element.</param>
        /// <param name="onObjectCreated">Callback to be invoked when the object is created.</param>
        /// <param name="anchorType">The anchor type for the UI element.</param>
        /// <param name="width">The width of the UI element.</param>
        /// <param name="height">The height of the UI element.</param>
        /// <returns>A new instance of NoaUIObjectElement.</returns>
        public static NoaUIObjectElement Create(string key, GameObject prefab, Action<GameObject> onObjectCreated, AnchorType anchorType, float width = 0, float height = 0)
        {
            return new NoaUIObjectElement()
            {
                _key = key,
                _anchorType = anchorType,
                _prefab = prefab,
                _onObjectCreated = onObjectCreated,
                _width = width,
                _height = height
            };
        }

        /// <summary>
        /// Factory method to create a new UI object element with a parent transform.
        /// </summary>
        /// <param name="key">The unique key for the UI element.</param>
        /// <param name="prefab">The prefab to be used for the UI element.</param>
        /// <param name="onObjectCreated">Callback to be invoked when the object is created.</param>
        /// <param name="parent">The parent transform for the UI element.</param>
        /// <param name="width">The width of the UI element.</param>
        /// <param name="height">The height of the UI element.</param>
        /// <returns>A new instance of NoaUIObjectElement.</returns>
        public static NoaUIObjectElement Create(string key, GameObject prefab, Action<GameObject> onObjectCreated, RectTransform parent, float width = 0, float height = 0)
        {
            return new NoaUIObjectElement()
            {
                _key = key,
                _parent = parent,
                _prefab = prefab,
                _onObjectCreated = onObjectCreated,
                _width = width,
                _height = height
            };
        }
    }

    /// <summary>
    /// Class representing a UI text element that can be updated.
    /// </summary>
    public sealed class NoaUITextElement : IUpdatableNoaUIElement
    {
        string _key;
        AnchorType _anchorType;
        RectTransform _parent;

        Func<string> _value;
        float _updateInterval;

        public string Key => _key;
        public AnchorType AnchorType => _anchorType;
        public RectTransform Parent => _parent;

        public float UpdateInterval => _updateInterval;

        /// <summary>
        /// Factory method to create a new UI text element with a dynamic value.
        /// </summary>
        /// <param name="key">The unique key for the UI element.</param>
        /// <param name="value">A function returning the dynamic value for the text.</param>
        /// <param name="anchorType">The anchor type for the UI element.</param>
        /// <param name="updateInterval">The interval between updates, in seconds.</param>
        /// <returns>A new instance of NoaUITextElement.</returns>
        public static NoaUITextElement Create(string key, Func<string> value, AnchorType anchorType, float updateInterval = 0.1f)
        {
            return new NoaUITextElement()
            {
                _key = key,
                _anchorType = anchorType,
                _value = value,
                _updateInterval = updateInterval,
            };
        }

        /// <summary>
        /// Factory method to create a new UI text element with a dynamic value and specified parent transform.
        /// </summary>
        /// <param name="key">The unique key for the UI element.</param>
        /// <param name="value">A function returning the dynamic value for the text.</param>
        /// <param name="parent">The parent transform for the UI element.</param>
        /// <param name="updateInterval">The interval between updates, in seconds.</param>
        /// <returns>A new instance of NoaUITextElement.</returns>
        public static NoaUITextElement Create(string key, Func<string> value, RectTransform parent, float updateInterval = 0.1f)
        {
            return new NoaUITextElement()
            {
                _key = key,
                _parent = parent,
                _value = value,
                _updateInterval = updateInterval,
            };
        }

        /// <summary>
        /// Factory method to create a new UI text element with a static value.
        /// </summary>
        /// <param name="key">The unique key for the UI element.</param>
        /// <param name="value">The static value for the text.</param>
        /// <param name="anchorType">The anchor type for the UI element.</param>
        /// <param name="updateInterval">The interval between updates, in seconds.</param>
        /// <returns>A new instance of NoaUITextElement.</returns>
        public static NoaUITextElement Create(string key, string value, AnchorType anchorType, float updateInterval = 0.1f)
        {
            return new NoaUITextElement()
            {
                _key = key,
                _anchorType = anchorType,
                _value = () => value,
                _updateInterval = updateInterval,
            };
        }

        /// <summary>
        /// Factory method to create a new UI text element with a static value and specified parent transform.
        /// </summary>
        /// <param name="key">The unique key for the UI element.</param>
        /// <param name="value">The static value for the text.</param>
        /// <param name="parent">The parent transform for the UI element.</param>
        /// <param name="updateInterval">The interval between updates, in seconds.</param>
        /// <returns>A new instance of NoaUITextElement.</returns>
        public static NoaUITextElement Create(string key, string value, RectTransform parent, float updateInterval = 0.1f)
        {
            return new NoaUITextElement()
            {
                _key = key,
                _parent = parent,
                _value = () => value,
                _updateInterval = updateInterval,
            };
        }

        /// <summary>
        /// Method to update the value of the UI text element.
        /// </summary>
        /// <returns>The updated value as a string.</returns>
        public string UpdateValue()
        {
            return _value.Invoke();
        }
    }

    /// <summary>
    /// Class representing a UI button element.
    /// </summary>
    public sealed class NoaUIButtonElement : INoaUIElement
    {
        string _key;
        AnchorType _anchorType;
        RectTransform _parent;

        string _label;
        UnityAction _onClick;

        public string Key => _key;
        public AnchorType AnchorType => _anchorType;
        public RectTransform Parent => _parent;

        public string Label => _label;
        public UnityAction OnClick => _onClick;

        /// <summary>
        /// Factory method to create a new UI button element.
        /// </summary>
        /// <param name="key">The unique key for the UI element.</param>
        /// <param name="label">The label of the button.</param>
        /// <param name="onClick">Callback to be invoked when the button is clicked.</param>
        /// <param name="anchorType">The anchor type for the UI element.</param>
        /// <returns>A new instance of NoaUIButtonElement.</returns>
        public static NoaUIButtonElement Create(string key, string label, UnityAction onClick, AnchorType anchorType)
        {
            return new NoaUIButtonElement()
            {
                _key = key,
                _anchorType = anchorType,
                _label = label,
                _onClick = onClick,
            };
        }

        /// <summary>
        /// Factory method to create a new UI button element with specified parent transform.
        /// </summary>
        /// <param name="key">The unique key for the UI element.</param>
        /// <param name="label">The label of the button.</param>
        /// <param name="onClick">Callback to be invoked when the button is clicked.</param>
        /// <param name="parent">The parent transform for the UI element.</param>
        /// <returns>A new instance of NoaUIButtonElement.</returns>
        public static NoaUIButtonElement Create(string key, string label, UnityAction onClick, RectTransform parent)
        {
            return new NoaUIButtonElement()
            {
                _key = key,
                _parent = parent,
                _label = label,
                _onClick = onClick,
            };
        }
    }
}
