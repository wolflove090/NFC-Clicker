using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// NFCの読み書きを行います
/// </summary>
public class NfcReader
{
    /// <summary>
    /// NFC読み込み
    /// </summary>
    [DllImport("__Internal")]
    static extern void LoadFromNFCText(Action<string> onLoaded);

    /// <summary>
    /// NFC書き込み
    /// </summary>
    [DllImport("__Internal")]
    static extern void WriteToNFCText(string data, Action onWrote);

    /// <summary>
    /// NFC書き込み
    /// </summary>
    [DllImport("__Internal")]
    static extern void Cancel();

    /// <summary>
    /// NFC読み込み後のコールバックを定義
    /// </summary>
    [MonoPInvokeCallback(typeof(Action<string>))]
    static void ReadCallback(string str)
    {
        _onLoaded?.Invoke(str);
        _onLoaded = null;
    }

    /// <summary>
    /// NFC書き込み後のコールバックを定義
    /// </summary>
    [MonoPInvokeCallback(typeof(Action))]
    static void WriteCallback()
    {
        _onWrote?.Invoke();
        _onWrote = null;
    }

    /// <summary>
    /// 外からコールバックを登録
    /// </summary>
    static Action<string> _onLoaded = null;

    /// <summary>
    /// 外からコールバックを登録
    /// </summary>
    static Action _onWrote = null;

    /// <summary>
    /// データを読み込み
    /// </summary>
    public static void LoadData(Action<string> onLoaded)
    {
        _onLoaded = onLoaded;
        LoadFromNFCText(ReadCallback);
    }

    /// <summary>
    /// データを保存
    /// </summary>
    public static void SaveData(string data, Action onWrote)
    {
        _onWrote = onWrote;
        WriteToNFCText(data, WriteCallback);
    }

    public static void OnCancel()
    {
        Debug.Log("Cancel");
        Cancel();
    }
}
