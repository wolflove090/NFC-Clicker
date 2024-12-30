var NFCPlugin = {

    // NFCからテキストデータを読み込み
    LoadFromNFCText: function(callbackPtr) {
        if ('NDEFReader' in window) {
            const ndef = new NDEFReader();
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
    WriteToNFCText: function(dataPtr) {
        if ('NDEFReader' in window) {
            console.log("start write");

            const ndef = new NDEFReader();
            const decoder = new TextDecoder();

            // ポインタを文字列に変換
            const data = UTF8ToString(dataPtr);
            console.log(data);

            ndef.write(data)
                .then(() => {
                    console.log("NFCカードにデータを書き込みました: " + data);
                })
                .catch(error => {
                    console.error("NFCカードへの書き込みに失敗しました: ", error);
                });
        } else {
            console.error('NFCがサポートされていないブラウザです。');
        }
    }
}

mergeInto(LibraryManager.library, NFCPlugin);