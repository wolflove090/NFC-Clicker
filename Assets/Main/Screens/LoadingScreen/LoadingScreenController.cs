using UnityEngine;
using UnityEngine.UIElements;
using System;

public class LoadingScreenController : MonoBehaviour
{
    // UIDocument component
    [SerializeField] private UIDocument _uiDocument;

    private Button cancelButton;

    Action _onCancel = null;

    public void Init(Action onCancel)
    {
        this._onCancel = onCancel;
    }

    public void Active()
    {
        this._uiDocument.gameObject.SetActive(true);
    }

    public void Inactive()
    {
        this._uiDocument.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // Get the root element from the UIDocument
        var root = _uiDocument.rootVisualElement;

        // Find the button with name="cancel-button"
        cancelButton = root.Q<Button>("cancel-button");

        // Register the click event for the button
        cancelButton.clicked += this.Cancel;
    }

    void OnDisable()
    {
        // Unregister the click event for the button
        cancelButton.clicked -= this.Cancel;
    }

    void Cancel()
    {
        Debug.Log("Cancel");
        this._onCancel?.Invoke();
        this.Inactive();
    }
}
