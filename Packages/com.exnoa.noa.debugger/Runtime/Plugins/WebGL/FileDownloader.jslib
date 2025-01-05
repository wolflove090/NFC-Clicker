mergeInto(LibraryManager.library,
{
    NoaDebuggerDownloadFile: function(fileNamePointer, textPointer, mimeTypePointer)
    {
        const fileName = UTF8ToString(fileNamePointer);
        const text = UTF8ToString(textPointer);
        const mimeType = UTF8ToString(mimeTypePointer);

        var element = document.createElement('a');
        element.setAttribute('href', 'data:' + mimeType + ',' + encodeURIComponent(text));
        element.setAttribute('download', fileName);
        element.style.display = 'none';
        document.body.appendChild(element);
        element.click();
        document.body.removeChild(element);
    },
});
