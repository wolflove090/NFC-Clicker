using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NoaDebugger
{
    sealed class DownloadDialog : DialogBase<DownloadDialog.DownloadDialogLinker>
    {
        const string CAPTION = "Download";
        const int MAX_LABEL_CHAR_NUM = 20;
        static readonly char[] InvalidCharacters = {'¥', '/', '\\', ':', '*', '?', '\"', '<', '>', '|'};

        [SerializeField, Header("Label")]
        NoaDebuggerInputField _labelInput;

        [SerializeField, Header("Download button")]
        Button _downloadButton;

        protected override void _OnShow(DownloadDialogLinker linker)
        {
            if (String.IsNullOrEmpty(linker._caption))
            {
                linker._caption = DownloadDialog.CAPTION;
            }

            _labelInput.UseCustomValidation(_LabelValidate);
            _labelInput.characterLimit = DownloadDialog.MAX_LABEL_CHAR_NUM;

            _labelInput.text = linker._initialLabel;
            _downloadButton.onClick.AddListener(linker._onDownload);
            _labelInput.onValueChanged.AddListener(linker._onChangeLabel);

            base._OnShow(linker);
        }

        protected override void _OnHide()
        {
            base._OnHide();

            _downloadButton.onClick.RemoveAllListeners();
            _labelInput.onValueChanged.RemoveAllListeners();
        }

        char _LabelValidate(string text, int charIndex, char addedChar)
        {
            if (DownloadDialog.InvalidCharacters.Contains(addedChar))
            {
                return '\0';
            }

            return addedChar;
        }

        public sealed class DownloadDialogLinker : DialogBaseViewLinker
        {
            public string _initialLabel;

            public UnityAction _onDownload;

            public UnityAction<string> _onChangeLabel;
        }
    }
}
