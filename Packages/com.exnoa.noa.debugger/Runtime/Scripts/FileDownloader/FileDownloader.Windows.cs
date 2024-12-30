#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

using System;
using System.IO;

namespace NoaDebugger
{
    static partial class FileDownloader
    {
        static partial void DownloadFile(string fileName, string textData)
        {
            string extension = Path.GetExtension(fileName);
            using (var windowsFileDialog = new WindowsFileDialog())
            {
                windowsFileDialog.SetFile(fileName);

                var filterDescription = $"{extension[1..].ToUpper()} Files (*{extension})";
                windowsFileDialog.SetFilter(filterDescription, extension);

                var status = false;
                try
                {
                    status = windowsFileDialog.ShowDialog();
                }
                catch (Exception)
                {
                    _status = Status.Error;
                    return;
                }

                if (status)
                {
                    string path = windowsFileDialog.FilePath();
                    try
                    {
                        File.WriteAllText(path, textData);
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
}

#endif
