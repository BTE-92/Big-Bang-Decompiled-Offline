mergeInto(LibraryManager.library, {
    // Writes the in-memory file system (the offline server's database under persistentDataPath) to IndexedDB.
    BBR_SyncFileSystem: function () {
        try {
            if (typeof FS !== 'undefined' && FS.syncfs) {
                FS.syncfs(false, function (err) { if (err) console.log('BBR syncfs error: ' + err); });
            }
        } catch (e) {
            console.log('BBR syncfs failed: ' + e);
        }
    },

    // Opens the browser's file picker and hands the chosen file back to Unity as "<name>|<base64>" with SendMessage(go, okMethod, ...).
    // A file dialog needs a fresh user gesture: when Unity asks for it a few frames after the tap, browsers that expose
    // navigator.userActivation tell whether the tap is still fresh; if not, the picker opens on the player's next tap/key and 1 is returned.
    BBR_PickFile: function (goPtr, okPtr, failPtr, acceptPtr) {
        var str = (typeof UTF8ToString !== 'undefined') ? UTF8ToString : Pointer_stringify;
        var go = str(goPtr), ok = str(okPtr), fail = str(failPtr), accept = str(acceptPtr);
        var send = function (method, value) {
            try { SendMessage(go, method, value); } catch (e) { console.log('BBR pick: SendMessage failed: ' + e); }
        };
        var open = function () {
            var input = document.createElement('input');
            input.type = 'file';
            input.accept = accept;
            input.style.display = 'none';
            document.body.appendChild(input);
            var cleanup = function () { try { document.body.removeChild(input); } catch (e) { } };
            input.addEventListener('change', function () {
                var f = input.files && input.files[0];
                if (!f) { send(fail, 'cancel'); cleanup(); return; }
                var reader = new FileReader();
                reader.onload = function () {
                    var bytes = new Uint8Array(reader.result);
                    var bin = '';
                    for (var i = 0; i < bytes.length; i += 0x8000) {
                        bin += String.fromCharCode.apply(null, bytes.subarray(i, i + 0x8000));
                    }
                    send(ok, f.name + '|' + btoa(bin));
                    cleanup();
                };
                reader.onerror = function () { send(fail, 'error'); cleanup(); };
                reader.readAsArrayBuffer(f);
            });
            input.addEventListener('cancel', function () { send(fail, 'cancel'); cleanup(); });
            input.click();
        };
        if (navigator.userActivation && !navigator.userActivation.isActive) {
            var events = ['mousedown', 'touchend', 'keydown'];
            var armed = function () {
                for (var i = 0; i < events.length; i++) document.removeEventListener(events[i], armed, true);
                open();
            };
            for (var i = 0; i < events.length; i++) document.addEventListener(events[i], armed, true);
            return 1;
        }
        open();
        return 0;
    },

    // Gives the browser a file to download (base64 content).
    BBR_DownloadFile: function (namePtr, b64Ptr) {
        var str = (typeof UTF8ToString !== 'undefined') ? UTF8ToString : Pointer_stringify;
        var name = str(namePtr), b64 = str(b64Ptr);
        try {
            var bin = atob(b64);
            var bytes = new Uint8Array(bin.length);
            for (var i = 0; i < bin.length; i++) bytes[i] = bin.charCodeAt(i);
            var blob = new Blob([bytes], { type: 'application/zip' });
            var url = URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = url;
            a.download = name;
            a.style.display = 'none';
            document.body.appendChild(a);
            a.click();
            setTimeout(function () { document.body.removeChild(a); URL.revokeObjectURL(url); }, 1000);
        } catch (e) {
            console.log('BBR download failed: ' + e);
        }
    }
});
