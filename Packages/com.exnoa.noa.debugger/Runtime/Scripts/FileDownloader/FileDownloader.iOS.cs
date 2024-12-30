#if UNITY_IOS && !UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;

namespace NoaDebugger
{
    static partial class FileDownloader
    {
        static partial void DownloadFile(string fileName, string textData)
        {
            UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath);
            string downloadPath = Application.persistentDataPath;
            try
            {
                if (!File.Exists(downloadPath))
                {
                    Directory.CreateDirectory(downloadPath);
                }
                string outputPath = Path.Combine(downloadPath, fileName);
                File.WriteAllText(outputPath, textData);
            }
            catch (Exception)
            {
                _status = Status.Error;
                return;
            }

            _status = Status.Completed;
        }
    }
}

#endif
