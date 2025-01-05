using System;
using System.Collections;
using UnityEngine;

namespace NoaDebugger
{
    static partial class FileDownloader
    {
        public enum Status
        {
            None,
            Downloading,
            Completed,
            Canceled,
            Error
        }

        public struct DownloadInfo
        {
            public Status _status;
            public string _outputPath;
        }

        static Status _status = Status.None;
        static bool _isOpened;
        static string _outputPath;

        public static void DownloadFile(string fileName, string textData, Action<DownloadInfo> callback)
        {
            if (_isOpened)
            {
                throw new UnityException("The dialog is already displayed. Multiple dialogs cannot be started at the same time.");
            }
            _isOpened = true;
            GlobalCoroutine.Run(DownloadFileAsync(fileName, textData, callback));
        }

        static IEnumerator DownloadFileAsync(string fileName, string textData, Action<DownloadInfo> callback = null)
        {
            _status = Status.Downloading;

            DownloadFile(fileName, textData);

            while (_status == Status.Downloading)
            {
                yield return null;
            }

            _isOpened = false;

#if !UNITY_WEBGL || UNITY_EDITOR
            var info = new DownloadInfo()
            {
                _status = _status,
                _outputPath = _outputPath
            };
            callback?.Invoke(info);
#endif
        }

        static partial void DownloadFile(string fileName, string textData);

        static string GetMimeType(string extension)
        {
            string mimeType;
            switch (extension)
            {
                case "txt":
                    mimeType = "text/plain;charset=utf-8";
                    break;
                case "json":
                    mimeType = "application/json";
                    break;
                default:
                    _status = Status.Error;
                    mimeType = "";
                    break;
            }
            return mimeType;
        }
    }
}
