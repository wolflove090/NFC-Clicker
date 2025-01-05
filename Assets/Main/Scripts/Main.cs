using UnityEngine;
// using TMPro;
using UnityEngine.UIElements;
using System;

public class Main : MonoBehaviour
{
    [SerializeField]
    ObjectSpawner _spawner;

    [SerializeField]
    LoadingScreenController _loadingScreen;

    [SerializeField]
    TitleScreenController _titleScreen;

    [SerializeField]
    InGameScreenController _inGameScreen;
    
    void Start()
    {
        this._loadingScreen.Init(this.OnLoadingCancel);
        this._loadingScreen.Inactive();

        this._titleScreen.Init(this.InitGame, this.LoadGame);
        this._titleScreen.Active();

        this._inGameScreen.Init(this.SaveNfc);
        this._inGameScreen.Inactive();
    }

    void OnLoadingCancel()
    {
        Debug.Log("Cancel");
        NfcReader.OnCancel();
    }

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

        this._loadingScreen.Active();

        NfcReader.LoadData((data) => {
            int count = int.Parse(data);
            this._titleScreen.Inactive();
            this._inGameScreen.Active();
            this._spawner.Init(count, this.Refresh);
            this.Refresh(this._spawner.Counter);
            this._loadingScreen.Inactive();
        });
    }

    void Refresh(int count)
    {
        this._inGameScreen.Refresh(count);
    }

    void SaveNfc()
    {
        Debug.Log("Save");

        this._loadingScreen.Active();
        
        NfcReader.SaveData(this._spawner.Counter.ToString(), () => {
            this._loadingScreen.Inactive();
        });
    }
}
