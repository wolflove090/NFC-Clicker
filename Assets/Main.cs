using UnityEngine;
// using TMPro;
using UnityEngine.UIElements;
using System;

public class Main : MonoBehaviour
{
    [SerializeField]
    ObjectSpawner _spawner;

    [SerializeField]
    TitleScreenController _titleScreen;


    [SerializeField]
    InGameScreenController _inGameScreen;

    // int _clickCount;
    
    void Start()
    {
        this._titleScreen.Init(this.InitGame, this.LoadGame);
        this._titleScreen.Active();

        this._inGameScreen.Init(this.SaveNfc);
        this._inGameScreen.Inactive();
    }

    // void Update()
    // {
    //     // 画面タップでカウントを上昇
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         this._clickCount++;

    //         this.Refresh();
    //     }
    // }

    /// <summary>
    /// 初期データで始める
    /// </summary>
    void InitGame()
    {
        Debug.Log("InitGame");
        
        this._titleScreen.Inactive();
        this._inGameScreen.Active();
        this._spawner.Init(0, this.Refresh);
    }

    /// <summary>
    /// データを読み込んで始める
    /// </summary>
    void LoadGame()
    {
        Debug.Log("LoadGame");

        NfcReader.LoadData((data) => {
            int count = int.Parse(data);
            // this._clickCount = int.Parse(data);
            this._titleScreen.Inactive();
            this._inGameScreen.Active();
            this._spawner.Init(count, this.Refresh);
            this.Refresh(this._spawner.Counter);
        });
    }

    void Refresh(int count)
    {
        this._inGameScreen.Refresh(count);
    }

    void SaveNfc()
    {
        Debug.Log("Save");
        NfcReader.SaveData(this._spawner.Counter.ToString());
    }
}
