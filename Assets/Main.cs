using UnityEngine;
// using TMPro;
using UnityEngine.UIElements;
using System;

public class Main : MonoBehaviour
{
    [SerializeField]
    TitleScreenController _titleScreen;


    [SerializeField]
    InGameScreenController _inGameScreen;

    int _clickCount;
    
    void Start()
    {
        this._titleScreen.Init(this.InitGame, this.LoadGame);
        this._titleScreen.Active();

        this._inGameScreen.Init(this.SaveNfc, this._clickCount);
        this._inGameScreen.Inactive();
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
        this._titleScreen.Inactive();
        this._inGameScreen.Active();
    }

    /// <summary>
    /// データを読み込んで始める
    /// </summary>
    void LoadGame()
    {
        Debug.Log("LoadGame");

        NfcReader.LoadData((data) => {
            this._clickCount = int.Parse(data);
            this._titleScreen.Inactive();
            this._inGameScreen.Active();
            this.Refresh();
        });
    }

    void Refresh()
    {
        this._inGameScreen.Refresh(this._clickCount);
    }

    void SaveNfc()
    {
        Debug.Log("Save");
        NfcReader.SaveData(this._clickCount.ToString());
    }
}
