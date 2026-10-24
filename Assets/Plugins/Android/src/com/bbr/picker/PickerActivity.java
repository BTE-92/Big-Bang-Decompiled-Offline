package com.bbr.picker;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;

import java.io.File;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.OutputStream;

// Ouvre le selecteur de fichiers d'Android (ACTION_OPEN_DOCUMENT), copie le fichier choisi dans le dossier de cache de l'appli
// (picked.zip + picked.name) puis ecrit picked.done ("ok", "cancel" ou "error"). Le jeu (Unity) lit ces fichiers.
public class PickerActivity extends Activity {
    private static final int REQ = 7001;
    private static final long MAX_BYTES = 32L * 1024L * 1024L;
    private String dir;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        dir = getIntent().getStringExtra("dir");
        if (dir == null) {
            File f = getExternalCacheDir();
            if (f == null) f = getCacheDir();
            dir = f.getAbsolutePath();
        }
        if (savedInstanceState != null) return; // l'activite a ete recreee : le resultat arrivera quand meme
        try {
            Intent i = new Intent(Intent.ACTION_OPEN_DOCUMENT);
            i.addCategory(Intent.CATEGORY_OPENABLE);
            i.setType("*/*");
            i.putExtra(Intent.EXTRA_MIME_TYPES, new String[] { "application/zip", "application/x-zip-compressed", "application/octet-stream" });
            startActivityForResult(i, REQ);
        } catch (Exception e) {
            done("error");
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode != REQ) return;
        if (resultCode != RESULT_OK || data == null || data.getData() == null) {
            done("cancel");
            return;
        }
        Uri uri = data.getData();
        try {
            InputStream in = getContentResolver().openInputStream(uri);
            if (in == null) { done("error"); return; }
            File tmp = new File(dir, "picked.tmp");
            OutputStream out = new FileOutputStream(tmp);
            byte[] buf = new byte[65536];
            long total = 0;
            int n;
            while ((n = in.read(buf)) > 0) {
                total += n;
                if (total > MAX_BYTES) { in.close(); out.close(); tmp.delete(); done("error"); return; }
                out.write(buf, 0, n);
            }
            in.close();
            out.close();
            File zip = new File(dir, "picked.zip");
            if (zip.exists()) zip.delete();
            tmp.renameTo(zip);
            String name = uri.getLastPathSegment();
            write("picked.name", name == null ? "" : name);
            done("ok");
        } catch (Exception e) {
            done("error");
        }
    }

    private void write(String file, String text) {
        try {
            FileOutputStream o = new FileOutputStream(new File(dir, file));
            o.write(text.getBytes("UTF-8"));
            o.close();
        } catch (Exception e) { }
    }

    private void done(String status) {
        write("picked.done", status);
        finish();
    }
}
