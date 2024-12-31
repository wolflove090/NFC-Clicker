using UnityEngine;
using UnityEngine.UIElements;
using System;

public class InGameScreenController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private Label counterLabel;
    private Button saveButton;

    Action _onSave = null;

    public void Init(Action onSave)
    {
        this._onSave = onSave;

        // 初期表示用にカウンターを更新
        this.Refresh(0);
    }

    public void Refresh(int count)
    {
        counterLabel.text = $"Current Sea Cucumber: {count}";
    }

    public void Active()
    {
        this.uiDocument.gameObject.SetActive(true);
    }

    public void Inactive()
    {
        this.uiDocument.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // UIDocumentからルート要素を取得
        var root = uiDocument.rootVisualElement;

        // name で UI要素を取得
        counterLabel = root.Q<Label>("CounterLabel");
        saveButton   = root.Q<Button>("SaveButton");

        // ボタンクリック時の処理を登録
        saveButton.clicked += this.OnSave;
    }

    void OnDisable()
    {
        // ボタンクリック時の処理を登録
        saveButton.clicked -= this.OnSave;
    }

    void OnSave()
    {
        this._onSave?.Invoke();
    }
}