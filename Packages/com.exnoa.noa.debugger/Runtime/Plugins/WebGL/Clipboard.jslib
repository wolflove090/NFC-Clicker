var NoaDebuggerClipboardCopy =
{
    NoaDebuggerCopyExecCommand: function(str)
    {
        try {
            var str = UTF8ToString(str);

            var listener = function(e){
                e.clipboardData.setData("text/plain" , str);
                e.preventDefault();
                document.removeEventListener("copy", listener);
            }

            document.addEventListener("copy" , listener);
            document.execCommand("copy");
        } catch (e) {
            console.log(e.message);
        }
    },

    NoaDebuggerCopyClipboard: function(str)
    {
        try {
            var str = UTF8ToString(str);

            navigator.permissions.query({ name: "clipboard-write" }).then((result) => {
                if (result.state === "granted" || result.state === "prompt") {
                    navigator.clipboard.writeText(str);
                }
            });
        } catch (e) {
            console.log(e.message);
        }
    },
};

mergeInto(LibraryManager.library, NoaDebuggerClipboardCopy);

