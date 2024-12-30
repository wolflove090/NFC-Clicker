#if UNITY_ANDROID && !UNITY_EDITOR

using System.Collections;
using System.IO;
using UnityEngine;

namespace NoaDebugger
{
    static partial class FileDownloader
    {
        static partial void DownloadFile(string fileName, string textData)
        {
            using (var fileDownloader = new AndroidJavaClass("NoaDebugger.FileDownloader"))
            {
                string extension = Path.GetExtension(fileName).Replace(".", "");
                string mimeType = FileDownloader.GetMimeType(extension);
                if (string.IsNullOrEmpty(mimeType))
                {
                    return;
                }
                fileDownloader.CallStatic("downloadFile", fileName, textData, mimeType);
            }
            GlobalCoroutine.Run(DownloadFileCompleted());
        }

        static IEnumerator DownloadFileCompleted()
        {
            using (var fileDownloader = new AndroidJavaClass("NoaDebugger.FileDownloader"))
            {
                var status = 0;
                while (status == 0)
                {
                    status = fileDownloader.CallStatic<int>("downloadStatus");
                    yield return null;
                }

                if (status == 1)
                {
                    _status = Status.Completed;
                }
                else
                {
                    _status = Status.Canceled;
                }
            }
        }
    }
}

#endif
