var NFCPlugin = {
    $ndef: null,
    $actlr: null,

    // NFCからテキストデータを読み込み
    LoadFromNFCText: function(callbackPtr) {
        if ('NDEFReader' in window) {
            ndef = new NDEFReader();
            ndef.scan().then(() => {
                console.log('NFCスキャンを開始しました...');

                ndef.onreading = event => {
                    const id = event.serialNumber;
                    const record = event.message.records[0];

                    const data = record.data;
                    const encoding = record.encoding;
                    const recordType = record.recordType;

                    if (recordType === 'text') {
                        console.log("read text");

                        // データをテキストに変換
                        const textDecoder = new TextDecoder(encoding);
                        const text = textDecoder.decode(data);
                        console.log(text);

                        // c#へ渡すため、テキストをポインタに変換
                        var encoder = new TextEncoder();
                        var strBuffer = encoder.encode(text + String.fromCharCode(0));
                        var strPtr = _malloc(strBuffer.length);
                        HEAP8.set(strBuffer, strPtr);

                        // C#側のコールバックを実行し、テキストを通知
                        {{{ makeDynCall('vi', 'callbackPtr') }}}(strPtr);
                        _free(strPtr);
                    }

                    console.log("end");

                    ndef.onreading = null; // イベントリスナーを解除
                };
            }).catch(error => {
                console.error('NFCスキャンに失敗しました。', error);
                ndef.onreading = null; // イベントリスナーを解除

            });
        } else {
            console.error('NFCがサポートされていないブラウザです。');
        }
    },

    // NFCにテキストデータを書き込み
    WriteToNFCText: function(dataPtr, callbackPtr) {
        if ('NDEFReader' in window) {
            console.log("start write");

            ndef = new NDEFReader();
            const decoder = new TextDecoder();
            actlr = new AbortController();

            // ポインタを文字列に変換
            const data = UTF8ToString(dataPtr);
            console.log(data);

            ndef.write(data, { signal: actlr.signal })
                .then(() => {
                    console.log("NFCカードにデータを書き込みました: " + data);
                    
                    // C#側のコールバックを実行し、テキストを通知
                    {{{ makeDynCall('v', 'callbackPtr') }}}();
                })
                .catch(error => {
                    console.error("NFCカードへの書き込みに失敗しました: ", error);

                    // C#側のコールバックを実行し、テキストを通知
                    {{{ makeDynCall('v', 'callbackPtr') }}}();
                });
        } else {
            console.error('NFCがサポートされていないブラウザです。');
        }
    },

    Cancel: function() {
        if(ndef == null)
            return;

        console.log("On Cancel");

        ndef.onreading = null; // イベントリスナーを解除
        if(actlr != null)
        {
            actlr.abort();
        }
    }
}

autoAddDeps(NFCPlugin, '$ndef');
autoAddDeps(NFCPlugin, '$actlr');
mergeInto(LibraryManager.library, NFCPlugin);