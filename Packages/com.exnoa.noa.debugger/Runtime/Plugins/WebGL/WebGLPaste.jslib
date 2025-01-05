mergeInto(LibraryManager.library, { 
    NoaDebuggerPasteStr: function(cb) {
        const agent = window.navigator.userAgent.toLowerCase()
        if (agent.indexOf("chrome") != -1) {
            document.addEventListener('paste', function (e) {
                const str = e.clipboardData.getData('text');
                e.preventDefault();
                var bufferSize = lengthBytesUTF8(str) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(str, buffer, bufferSize);
                dynCall( "vi", cb, [buffer] );
            });
            document.execCommand("paste");
        }
    },
});
