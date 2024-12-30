using System;
using System.Collections.Generic;

namespace NoaDebugger
{
    interface IExportData
    {
        public DownloadInfo GetDownloadInfo();
    }
    
    sealed class DownloadInfo
    {
        const string EXPORT_FILE_DATE_TIME_FORMAT = "yyyyMMdd-HHmmss";
        const string EXPORT_FILE_EXTENSION = "json";
        readonly string _exportFilePrefix;
        
        public DownloadInfo(string prefix)
        {
            _exportFilePrefix = prefix;
        }

        public string GetExportFileName(string label, DateTime date)
        {
            var dateTimeLabel = date.ToString(DownloadInfo.EXPORT_FILE_DATE_TIME_FORMAT);

            List<string> strings = new List<string>(){_exportFilePrefix, dateTimeLabel, label};
            strings.RemoveAll(s => string.IsNullOrEmpty(s));
            var fileName = string.Join("-", strings);
            return $"{fileName}.{DownloadInfo.EXPORT_FILE_EXTENSION}";
        }
    }
}
