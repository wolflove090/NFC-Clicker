using UnityEngine;
// using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class Main : MonoBehaviour
{
    // ===== タイトル ===== //

    // [SerializeField]
    // GameObject _title;
    // [SerializeField]
    // Button _initButton;
    // [SerializeField]
    // Button _loadButton;

    // UIDocument コンポーネント
    [SerializeField] private UIDocument uiDocument;

    private Button startButton;
    private Button loadButton;


    // ===== メイン ===== //

    // [SerializeField]
    // UnityEngine.UI.Button _saveButton;

    [SerializeField]
    TextMeshProUGUI _countLabel;

    int _clickCount;
    
    void Start()
    {
        this.uiDocument.gameObject.SetActive(true);

        // this._initButton.onClick.AddListener(this.InitGame);
        // this._loadButton.onClick.AddListener(this.LoadGame);

        // UIDocument から UXML のルート要素を取得
        var root = uiDocument.rootVisualElement;

        // name="StartButton", name="LoadButton" で検索
        startButton = root.Q<Button>("StartButton");
        loadButton  = root.Q<Button>("LoadButton");

        // ボタンを押したときの処理を登録
        startButton.clicked += InitGame;
        loadButton.clicked  += LoadGame;

        // this._saveButton.onClick.AddListener(this.SaveNfc);
    }

    void Update()
    {
        // 画面タップでカウントを上昇
        if (Input.GetMouseButtonDown(0))
        {
            this._clickCount++;

            this.Refresh();
        }
    }

    /// <summary>
    /// 初期データで始める
    /// </summary>
    void InitGame()
    {
        Debug.Log("InitGame");
        this._clickCount = 0;
        this.uiDocument.gameObject.SetActive(false);
    }

    /// <summary>
    /// データを読み込んで始める
    /// </summary>
    void LoadGame()
    {
        Debug.Log("LoadGame");

        NfcReader.LoadData((data) => {
            this._clickCount = int.Parse(data);
            this.Refresh();
            this.uiDocument.gameObject.SetActive(false);
        });
    }

    void Refresh()
    {
        this._countLabel.text = $"Count:{this._clickCount}";
    }

    void SaveNfc()
    {
        Debug.Log("Save");
        NfcReader.SaveData(this._clickCount.ToString());
    }
}
