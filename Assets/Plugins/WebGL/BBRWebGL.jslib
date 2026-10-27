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
    }
});
