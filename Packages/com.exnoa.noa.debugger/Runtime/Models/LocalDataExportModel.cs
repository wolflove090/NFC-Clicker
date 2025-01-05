using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class LocalDataExportModel : ModelBase
    {
        public static void ExportText(string fileName, string text, UnityAction<FileDownloader.DownloadInfo> onCompleted)
        {
            FileDownloader.DownloadFile(fileName, text, info => onCompleted?.Invoke(info));
        }
    }
}
