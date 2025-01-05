using UnityEngine;
using UnityEngine.UIElements;
using System;

public class TitleScreenController : MonoBehaviour
{
    // UIDocument コンポーネント
    [SerializeField] private UIDocument _uiDocument;

    private Button startButton;
    private Button loadButton;

    Action _initGame = null;
    Action _loadGame = null;
    
    public void Init(Action initGame, Action loadGame)
    {
        this._initGame = initGame;
        this._loadGame = loadGame;
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
        // UIDocument から UXML のルート要素を取得
        var root = _uiDocument.rootVisualElement;

        // name="StartButton", name="LoadButton" で検索
        startButton = root.Q<Button>("StartButton");
        loadButton  = root.Q<Button>("LoadButton");

        // ボタンを押したときの処理を登録
        startButton.clicked += this.InitGame;
        loadButton.clicked  += this.LoadGame;
    }

    void OnDisable()
    {
        // ボタンを押したときの処理を登録
        startButton.clicked -= this.InitGame;
        loadButton.clicked  -= this.LoadGame;
    }

    void InitGame()
    {
        this._initGame?.Invoke();
    }

    void LoadGame()
    {
        this._loadGame?.Invoke();
    }
}
