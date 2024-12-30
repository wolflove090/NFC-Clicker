using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Main : MonoBehaviour
{
    // ===== タイトル ===== //

    [SerializeField]
    GameObject _title;
    [SerializeField]
    Button _initButton;
    [SerializeField]
    Button _loadButton;

    // ===== メイン ===== //

    [SerializeField]
    Button _saveButton;

    [SerializeField]
    TextMeshProUGUI _countLabel;

    int _clickCount;
    
    void Start()
    {
        this._title.SetActive(true);

        this._initButton.onClick.AddListener(this.InitGame);
        this._loadButton.onClick.AddListener(this.LoadGame);

        this._saveButton.onClick.AddListener(this.SaveNfc);
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
        this._clickCount = 0;
        this._title.SetActive(false);
    }

    /// <summary>
    /// データを読み込んで始める
    /// </summary>
    void LoadGame()
    {
        NfcReader.LoadData((data) => {
            this._clickCount = int.Parse(data);
            this.Refresh();
            this._title.SetActive(false);
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
