var FugaPlugin = {
    Fuga: function(hogePtr) {

        console.log("fuga");

        var str = "Unko";

        var encoder = new TextEncoder();
        var strBuffer = encoder.encode(str + String.fromCharCode(0));
        var strPtr = _malloc(strBuffer.length);
        HEAP8.set(strBuffer, strPtr);

        Module['dynCall_vi'](hogePtr, strPtr);
        // {{{ makeDynCall('vi', 'hogePtr') }}}(strPtr);
        _free(strPtr);

        console.log("fuga end");
    }
};

mergeInto(LibraryManager.library, FugaPlugin);