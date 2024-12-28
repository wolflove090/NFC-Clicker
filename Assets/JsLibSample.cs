using UnityEngine;
using System;
using System.Runtime.InteropServices;
using AOT;

public class JsLibSample : MonoBehaviour
{
    [DllImport("__Internal")]
    static extern void Fuga(Action<string> hoge);

    [MonoPInvokeCallback(typeof(Action<string>))]
    static void Hoge(string str)
    {
        Debug.Log($"Called Hoge:{str}");
    }

    void Start()
    {
        Debug.Log("Exec");
        Fuga(Hoge);
    }
}
