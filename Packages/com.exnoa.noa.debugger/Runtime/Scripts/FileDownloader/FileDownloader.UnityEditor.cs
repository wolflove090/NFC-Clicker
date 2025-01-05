#if UNITY_EDITOR

using System;
using System.IO;
using UnityEngine.Events;

namespace NoaDebugger
{
    static partial class FileDownloader
    {
        static partial void DownloadFile(string fileName, string textData)
        {
            _outputPath = "";
            string extension = Path.GetExtension(fileName).Replace(".", "");
            _outputPath = UnityEditor.EditorUtility.SaveFilePanel("Please select a folder", "", fileName, extension);

            if (!string.IsNullOrEmpty(_outputPath))
            {
                try
                {
                    File.WriteAllText(_outputPath, textData);
                }
                catch (Exception)
                {
                    _status = Status.Error;
                    return;
                }
                _status = Status.Completed;
            }
            else
            {
                _status = Status.Canceled;
            }
        }
    }
}

#endif
