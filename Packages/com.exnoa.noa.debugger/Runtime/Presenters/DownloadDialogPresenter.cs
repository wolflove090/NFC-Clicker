using System;
using UnityEngine;
using UnityEngine.Events;

namespace NoaDebugger
{
    sealed class DownloadDialogPresenter : IDisposable
    {
        readonly DownloadDialog _dialogPrefab;
        DownloadDialog _dialog;
        string _label;

        public event UnityAction<string, UnityAction<FileDownloader.DownloadInfo>> OnExecDownload;
        
        public DownloadDialogPresenter(DownloadDialog dialogPrefab)
        {
            _dialogPrefab = dialogPrefab;
        }

        public void ShowDialog()
        {
            if (_dialog == null)
            {
                _dialog = GameObject.Instantiate(_dialogPrefab, NoaDebugger.GetDialogRoot());
            }

            var linker = new DownloadDialog.DownloadDialogLinker()
            {
                _initialLabel = _label,
                _onDownload = _OnExecDownload,
                _onChangeLabel = _OnChangeExportLabel
            };

            _dialog.Show(linker);
        }
        
        void _OnChangeExportLabel(string label)
        {
            _label = label;
        }

        void _OnExecDownload()
        {
            OnExecDownload?.Invoke(_label, _DownloadCompleted);
        }

        void _DownloadCompleted(FileDownloader.DownloadInfo info)
        {
            string label = info._status switch
            {
                FileDownloader.Status.Completed => NoaDebuggerDefine.DownloadCompletedText,
                FileDownloader.Status.Canceled => NoaDebuggerDefine.DownloadCanceledText,
                _ => NoaDebuggerDefine.DownloadFailedText
            };
            var linker = new ToastViewLinker
            {
                _label = label
            };
            NoaDebugger.ShowToast(linker);

            if (info._status == FileDownloader.Status.Completed)
            {
#if UNITY_EDITOR
                var dirName = System.IO.Path.GetDirectoryName (info._outputPath);
                Application.OpenURL($"file://{dirName}");
#endif
                _dialog.Hide();
            }
        }

        public void Dispose()
        {
            if (_dialog != null)
            {
                GameObject.Destroy(_dialog.gameObject);
                _dialog = null;
            }
        }
    }
}
