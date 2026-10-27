using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Server;

// Export / import de niveaux publies en fichier .zip (comme dans BBRo).
//  - un bouton "Export level" a cote de "Play" sur chaque niveau publie (Level Builder > Finished Levels)
//  - un bouton "Import level" dans le Level Builder : liste les .zip trouves dans les dossiers de telechargement et importe le choisi
// Le serveur fabrique / lit le zip (/v1/minigame/export, /v1/minigame/import) ; le telephone ne fait qu'ecrire / lire le fichier.
public static class LevelTransfer
{
    private const string Perm = "android.permission.WRITE_EXTERNAL_STORAGE";

    // A level with its screenshot and ghost is well under a megabyte: anything bigger is not a level (and would only waste memory, or
    // crash the game, to read, encode and post it). The server applies its own limits as well.
    public const int MaxImportBytes = 8 * 1024 * 1024;
    private const string TooBigMsg = "That file is too big to be a level (the limit is 8 MB).";
    private const string NotZipMsg = "That is not a level file. Pick the .zip you exported.";

#if UNITY_WEBGL && !UNITY_EDITOR
    // browser file picker / download (Plugins/WebGL/BBRWebGL.jslib). BBR_PickFile returns 1 when the browser will only open the picker on
    // the player's next tap (a file dialog needs a fresh user gesture), 0 when it was opened right away.
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int BBR_PickFile(string gameObjectName, string okMethod, string failMethod, string accept, int maxBytes);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void BBR_DownloadFile(string fileName, string base64);
#endif

#if UNITY_IOS && !UNITY_EDITOR
    // Apple's Files browser on top of the game (Plugins/iOS/BBRFilePicker.mm). It writes picked.zip / picked.name / picked.done in the given
    // folder, like the Android picker does.
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern int BBR_IOS_PickFile(string dir);
#endif

    // result of the browser picker (filled by WebPickReceiver)
    private static byte[] s_webData;
    private static string s_webStatus;

    public static void WebPicked(byte[] data)
    {
        s_webData = data;
        s_webStatus = "ok";
    }

    public static void WebPickFailed(string why)
    {
        s_webStatus = (why == "cancel" || why == "toolarge") ? why : "error";
    }
    // Android: shared storage; Windows: Documents; iOS: the app's Documents folder (visible in the Files app)
    public static string ExportDir
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android) return "/storage/emulated/0/BigBangRacing/LevelExports";
            try
            {
                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                    return System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BigBangRacing"), "LevelExports");
            }
            catch (Exception) { }
            return System.IO.Path.Combine(Application.persistentDataPath, "ExportedLevels");
        }
    }

    private class ExportEntry
    {
        public PsUIGenericButton Button;
        public string Id;
        public string Name;
    }

    private static List<ExportEntry> s_exports = new List<ExportEntry>();
    private static PsUIGenericButton s_importButton;
    private static PsUICenterCreate s_center;
    private static bool s_waitingPick;
    private static string s_expId;
    private static string s_expName;

    // ---------- boutons ----------

    // appele a la creation de l'ecran Level Builder (apres le bouton "BUILD NEW!")
    public static void AddImportButton(UIComponent parent)
    {
        try
        {
            s_exports.Clear();
            // espace transparent entre "BUILD NEW!" et le bouton (la liste a un espacement negatif)
            UICanvas gap = new UICanvas(parent, false, string.Empty, null, string.Empty);
            gap.SetHeight(0.05f, RelativeTo.ScreenHeight);
            gap.SetWidth(0.1f, RelativeTo.ScreenHeight);
            gap.RemoveDrawHandler();
            s_importButton = new PsUIGenericButton(parent);
            s_importButton.SetOrangeColors();
            s_importButton.SetText("Import level", 0.04f);
            s_importButton.SetMargins(0.06f, 0.06f, 0.02f, 0.02f);
        }
        catch (Exception e)
        {
            Debug.Log("LevelTransfer.AddImportButton failed: " + e.Message);
        }
    }

    // appele a la creation du bouton "Play" d'un niveau publie
    public static void AddExportButton(PsUICreatePublishedBanner banner, UIComponent parent)
    {
        try
        {
            PsUIGenericButton b = new PsUIGenericButton(parent);
            b.SetBlueColors();
            b.SetText("Export level", 0.03f);
            b.SetHorizontalAlign(0f);
            b.SetDepthOffset(-20f);
            ExportEntry e = new ExportEntry();
            e.Button = b;
            e.Id = banner.m_loop.m_minigameId;
            e.Name = banner.m_loop.GetName();
            s_exports.Add(e);
        }
        catch (Exception e)
        {
            Debug.Log("LevelTransfer.AddExportButton failed: " + e.Message);
        }
    }

    // appele a chaque image par PsUICenterCreate.Step
    public static void Tick(PsUICenterCreate center)
    {
        s_center = center;
        if (s_waitingPick) PollPicker();
        if (s_importButton != null && s_importButton.m_hit)
        {
            if (!LaunchPicker()) new PsUIBasePopup(typeof(ImportSelect), null, null, null, false);
            return;
        }
        for (int i = 0; i < s_exports.Count; i++)
        {
            if (s_exports[i].Button != null && s_exports[i].Button.m_hit)
            {
                StartExport(s_exports[i].Id, s_exports[i].Name);
                return;
            }
        }
    }

    // ---------- permission stockage (Android 6+) ----------

    private static AndroidJavaObject Activity()
    {
        AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        return player.GetStatic<AndroidJavaObject>("currentActivity");
    }

    private static bool HasStorage()
    {
        if (Application.platform != RuntimePlatform.Android) return true;
        try
        {
            return Activity().Call<int>("checkSelfPermission", new object[] { Perm }) == 0;
        }
        catch (Exception)
        {
            return true;
        }
    }

    private static void OpenAppSettings()
    {
        try
        {
            AndroidJavaObject act = Activity();
            string pkg = act.Call<string>("getPackageName", new object[0]);
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("parse", new object[] { "package:" + pkg });
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", new object[] { "android.settings.APPLICATION_DETAILS_SETTINGS", uri });
            act.Call("startActivity", new object[] { intent });
        }
        catch (Exception e)
        {
            Debug.Log("OpenAppSettings failed: " + e.Message);
        }
    }

    // true si l'acces aux fichiers est autorise. Sinon : 1re fois on demande la permission, ensuite (refus definitif d'Android) on ouvre les reglages de l'appli.
    // settingsOpened indique laquelle des deux actions a eu lieu.
    public static bool EnsureStorage(out bool settingsOpened)
    {
        settingsOpened = false;
        if (HasStorage()) return true;
        int asked = PlayerPrefs.GetInt("bbr_storage_asked", 0);
        if (asked == 0)
        {
            PlayerPrefs.SetInt("bbr_storage_asked", 1);
            PlayerPrefs.Save();
            RequestStorage();
        }
        else
        {
            OpenAppSettings();
            settingsOpened = true;
        }
        return false;
    }

    public const string SettingsMsg = "Allow Storage access for this game in the settings that just opened, then try again.";

    private static void RequestStorage()
    {
        try
        {
            Activity().Call("requestPermissions", new object[] { new string[] { Perm }, 7001 });
        }
        catch (Exception e)
        {
            Debug.Log("requestPermissions failed: " + e.Message);
        }
    }

    // ---------- export ----------

    private static string SafeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "level";
        char[] bad = System.IO.Path.GetInvalidFileNameChars();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (Array.IndexOf(bad, c) >= 0 || c == ' ') c = '_';
            sb.Append(c);
        }
        string s = sb.ToString();
        if (s.Length > 60) s = s.Substring(0, 60);
        return s.Length == 0 ? "level" : s;
    }

    public static void StartExport(string id, string name)
    {
        bool opened;
        if (!EnsureStorage(out opened))
        {
            LevelMsg.Show("Export Failed", opened ? SettingsMsg : "Allow access to your files, then tap Export level again.");
            return;
        }
        s_expId = id;
        s_expName = name;
        WOEURL url = new WOEURL("/v1/minigame/export");
        url.AddParameter("id", id);
        Request.Get(url, "LEVEL_EXPORT", ExportOk, ExportFailed);
    }

    private static void ExportFailed(HttpC c)
    {
        LevelMsg.Show("Export Failed", "Failed to export level. Did you allow file permissions?");
    }

    private static void ExportOk(HttpC c)
    {
        try
        {
            byte[] data = c.www.bytes;
            if (data == null || data.Length < 22)
            {
                LevelMsg.Show("Export Failed", "Failed to export level. Did you allow file permissions?");
                return;
            }
#if UNITY_WEBGL && !UNITY_EDITOR
            // in a browser the file goes to the Downloads folder like any other download
            string webName = SafeName(s_expName) + "_" + s_expId + ".zip";
            BBR_DownloadFile(webName, Convert.ToBase64String(data));
            LevelMsg.Show("Export Succeeded", "Your browser downloaded: " + webName);
            return;
#endif
            string dir = ExportDir;
            if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
            string path = System.IO.Path.Combine(dir, SafeName(s_expName) + "_" + s_expId + ".zip");
            System.IO.File.WriteAllBytes(path, data);
            if (System.IO.File.Exists(path))
            {
                LevelMsg.Show("Export Succeeded", "Exported level to: " + path);
                return;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Export failed: " + e.Message);
        }
        LevelMsg.Show("Export Failed", "Failed to export level. Did you allow file permissions?");
    }

    // ---------- import ----------

    // ouvre le selecteur de fichiers d'Android (PickerActivity, ajoutee a l'APK) ; le fichier choisi est copie dans le cache de l'appli
    public static bool LaunchPicker()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (WindowsPicker.Launch(Application.temporaryCachePath))
            {
                s_waitingPick = true;
                return true;
            }
            return false;
        }
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                WebPickReceiver.Ensure();
                s_webData = null;
                s_webStatus = null;
                int deferred = BBR_PickFile(WebPickReceiver.ObjectName, "OnPicked", "OnPickFailed", ".zip,application/zip", MaxImportBytes);
                s_waitingPick = true;
                if (deferred == 1) LevelMsg.Show("Import level", "Tap OK, then choose the level file (.zip) in the window that opens.");
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Browser picker failed: " + e.Message);
                return false;
            }
#else
            return false;
#endif
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IOS && !UNITY_EDITOR
            try
            {
                string iosDir = Application.temporaryCachePath;
                if (!System.IO.Directory.Exists(iosDir)) System.IO.Directory.CreateDirectory(iosDir);
                string[] iosOld = new string[] { "picked.zip", "picked.tmp", "picked.name", "picked.done" };
                for (int i = 0; i < iosOld.Length; i++)
                {
                    string f = System.IO.Path.Combine(iosDir, iosOld[i]);
                    if (System.IO.File.Exists(f)) System.IO.File.Delete(f);
                }
                if (BBR_IOS_PickFile(iosDir) == 1)
                {
                    s_waitingPick = true;
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.Log("iOS picker failed: " + e.Message);
            }
#endif
            return false;
        }
        if (Application.platform != RuntimePlatform.Android) return false;
        try
        {
            string dir = Application.temporaryCachePath;
            string[] old = new string[] { "picked.zip", "picked.tmp", "picked.name", "picked.done" };
            for (int i = 0; i < old.Length; i++)
            {
                string f = System.IO.Path.Combine(dir, old[i]);
                if (System.IO.File.Exists(f)) System.IO.File.Delete(f);
            }
            AndroidJavaObject act = Activity();
            string pkg = act.Call<string>("getPackageName", new object[0]);
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", new object[0]);
            intent.Call<AndroidJavaObject>("setClassName", new object[] { pkg, "com.bbr.picker.PickerActivity" });
            intent.Call<AndroidJavaObject>("putExtra", new object[] { "dir", dir });
            act.Call("startActivity", new object[] { intent });
            s_waitingPick = true;
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("LaunchPicker failed: " + e.Message);
            return false;
        }
    }

    private static void PollPicker()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (s_webStatus == null) return;
            s_waitingPick = false;
            string st = s_webStatus;
            byte[] picked = s_webData;
            s_webStatus = null;
            s_webData = null;
            if (st == "ok") StartImportBytes(picked);
            else if (st == "toolarge") LevelMsg.Show("Import Failed", TooBigMsg);
            else if (st == "error") LevelMsg.Show("Import Failed", "Make sure you are importing a valid level!");
            return;
        }
        try
        {
            string dir = Application.temporaryCachePath;
            string done = System.IO.Path.Combine(dir, "picked.done");
            if (!System.IO.File.Exists(done)) return;
            s_waitingPick = false;
            string status = ReadText(done);
            if (status == "ok")
            {
                string pickedPath = System.IO.Path.Combine(dir, "picked.zip");
                if (FileTooBig(pickedPath))
                {
                    LevelMsg.Show("Import Failed", TooBigMsg);
                    return;
                }
                StartImportBytes(ReadBytes(pickedPath));
            }
            else if (status == "toolarge")
            {
                LevelMsg.Show("Import Failed", TooBigMsg);
            }
            else if (status == "error")
            {
                LevelMsg.Show("Import Failed", "Make sure you are importing a valid level!");
            }
        }
        catch (Exception e)
        {
            s_waitingPick = false;
            Debug.Log("PollPicker failed: " + e.Message);
        }
    }

    private static string ReadText(string path)
    {
        byte[] b = ReadBytes(path);
        return b == null ? string.Empty : Encoding.UTF8.GetString(b).Trim();
    }

    private static byte[] ReadBytes(string path)
    {
        try
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                byte[] data = new byte[(int)fs.Length];
                int off = 0;
                while (off < data.Length)
                {
                    int n = fs.Read(data, off, data.Length - off);
                    if (n <= 0) break;
                    off += n;
                }
                return data;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Read failed: " + e.Message);
            return null;
        }
    }

    public static List<string> FindZips()
    {
        s_times.Clear();
        List<string> found = new List<string>();
        string[] dirs = new string[]
        {
            ExportDir,
            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ExportDir), "LevelImports"),
            "/storage/emulated/0/Download",
            "/storage/emulated/0/Downloads",
            System.IO.Path.Combine(Application.persistentDataPath, "ExportedLevels"),
            Application.persistentDataPath,
            DownloadsFolder()
        };
        for (int d = 0; d < dirs.Length; d++)
        {
            try
            {
                if (string.IsNullOrEmpty(dirs[d]) || !System.IO.Directory.Exists(dirs[d])) continue;
                string[] files = System.IO.Directory.GetFiles(dirs[d], "*.zip");
                for (int i = 0; i < files.Length; i++)
                    if (!found.Contains(files[i])) found.Add(files[i]);
            }
            catch (Exception) { }
        }
        found.Sort(CompareNewest);
        return found;
    }

    private static string DownloadsFolder()
    {
        try
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return string.IsNullOrEmpty(home) ? string.Empty : System.IO.Path.Combine(home, "Downloads");
        }
        catch (Exception) { return string.Empty; }
    }

    private static Dictionary<string, long> s_times = new Dictionary<string, long>();

    private static long TimeOf(string path)
    {
        long t;
        if (s_times.TryGetValue(path, out t)) return t;
        t = 0;
        try
        {
            t = System.IO.File.GetLastWriteTimeUtc(path).Ticks;
        }
        catch (Exception) { }
        s_times[path] = t;
        return t;
    }

    private static int CompareNewest(string a, string b)
    {
        return TimeOf(b).CompareTo(TimeOf(a));
    }

    public static bool EnsureRead(out bool settingsOpened)
    {
        return EnsureStorage(out settingsOpened);
    }

    private static bool FileTooBig(string path)
    {
        try
        {
            return new System.IO.FileInfo(path).Length > MaxImportBytes;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static void StartImport(string path)
    {
        if (FileTooBig(path))
        {
            LevelMsg.Show("Import Failed", TooBigMsg);
            return;
        }
        StartImportBytes(ReadBytes(path));
    }

    public static void StartImportBytes(byte[] data)
    {
        if (data == null || data.Length < 22)
        {
            LevelMsg.Show("Import Failed", "Make sure you are importing a valid level!");
            return;
        }
        if (data.Length > MaxImportBytes)
        {
            LevelMsg.Show("Import Failed", TooBigMsg);
            return;
        }
        // a zip starts with "PK" (local file header, or the end record of an empty one)
        bool isZip = data[0] == (byte)'P' && data[1] == (byte)'K' && ((data[2] == 3 && data[3] == 4) || (data[2] == 5 && data[3] == 6));
        if (!isZip)
        {
            LevelMsg.Show("Import Failed", NotZipMsg);
            return;
        }
        WOEURL url = new WOEURL("/v1/minigame/import");
        Request.Post(url, "LEVEL_IMPORT", data, ImportOk, ImportFailed);
    }

    private static void ImportFailed(HttpC c)
    {
        LevelMsg.Show("Import Failed", "Make sure you are importing a valid level!");
    }

    private static void ImportOk(HttpC c)
    {
        string txt = c.www.text ?? string.Empty;
        if (txt.IndexOf("\"ok\":true") >= 0)
        {
            LevelMsg.Show("Level imported!", "The level was added to your Finished Levels.");
            RefreshLevelBuilder();
        }
        else if (txt.IndexOf("already") >= 0)
        {
            LevelMsg.Show("Import Failed", "You already have this level.");
        }
        else
        {
            LevelMsg.Show("Import Failed", "Make sure you are importing a valid level!");
        }
    }

    private static void RefreshLevelBuilder()
    {
        try
        {
            if (s_center == null) return;
            s_center.m_publishedBanners.Clear();
            s_center.m_savedBanners.Clear();
            s_center.m_hiddenBanners.Clear();
            s_exports.Clear();
            s_center.LoadContent();
        }
        catch (Exception e)
        {
            Debug.Log("Refresh failed: " + e.Message);
        }
    }
}

// petite fenetre de message (titre + texte + OK)
public class LevelMsg : PsUIHeaderedCanvas
{
    private PsUIGenericButton m_okButton;
    private UIText m_headerText;
    private UITextbox m_box;

    public LevelMsg(UIComponent _parent)
        : base(_parent, string.Empty)
    {
        (GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
        SetWidth(0.65f, RelativeTo.ScreenWidth);
        SetHeight(0.5f, RelativeTo.ScreenHeight);
        SetVerticalAlign(0.4f);
        SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
        SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
        m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
        m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
        UIHorizontalList row = new UIHorizontalList(m_header, string.Empty);
        row.RemoveDrawHandler();
        row.SetSpacing(0.02f, RelativeTo.ScreenHeight);
        row.SetMargins(0.025f, 0.025f, 0f, 0f);
        row.SetHorizontalAlign(0.5f);
        m_headerText = new UIText(row, false, string.Empty, " ", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
        m_box = new UITextbox(this, false, string.Empty, " ", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
        UIHorizontalList buttons = new UIHorizontalList(this, "msgButtons");
        buttons.SetHeight(0.1f, RelativeTo.ScreenHeight);
        buttons.RemoveDrawHandler();
        buttons.SetVerticalAlign(0f);
        buttons.SetMargins(0f, 0f, 0.075f, -0.075f);
        m_okButton = new PsUIGenericButton(buttons);
        m_okButton.SetText("Ok");
        m_okButton.SetGreenColors();
    }

    public static void Show(string header, string text)
    {
        PsUIBasePopup popup = new PsUIBasePopup(typeof(LevelMsg), null, null, null, false);
        LevelMsg m = popup.m_mainContent as LevelMsg;
        if (m != null)
        {
            m.m_headerText.SetText(header);
            m.m_box.SetText(text);
        }
        popup.Update();
    }

    public override void Step()
    {
        if (m_okButton.m_hit || m_exitButton.m_hit)
        {
            (GetRoot() as PsUIBasePopup).Destroy();
        }
        base.Step();
    }
}

// fenetre de choix du fichier .zip a importer
public class ImportSelect : PsUIHeaderedCanvas
{
    private List<PsUIGenericButton> m_buttons = new List<PsUIGenericButton>();
    private List<string> m_paths = new List<string>();

    public ImportSelect(UIComponent _parent)
        : base(_parent, string.Empty)
    {
        SetWidth(0.75f, RelativeTo.ScreenWidth);
        SetHeight(0.75f, RelativeTo.ScreenHeight);
        SetVerticalAlign(0.4f);
        SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
        (GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
        SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
        m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
        m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
        UIHorizontalList row = new UIHorizontalList(m_header, string.Empty);
        row.RemoveDrawHandler();
        row.SetHorizontalAlign(0.5f);
        new UIText(row, false, string.Empty, "Import level", "KGSecondChances_Font", 0.065f, RelativeTo.ScreenHeight, "#7AF2FF");
        CreateContent();
    }

    private static string Display(string path)
    {
        string n = System.IO.Path.GetFileName(path);
        int dot = n.LastIndexOf('.');
        if (dot > 0) n = n.Substring(0, dot);
        int us = n.LastIndexOf('_');
        if (us > 0 && n.Length - us - 1 >= 24) n = n.Substring(0, us);
        if (n.Length > 28) n = n.Substring(0, 28);
        Loc.Keep(n);
        return n;
    }

    private void CreateContent()
    {
        bool opened;
        bool allowed = LevelTransfer.EnsureRead(out opened);
        List<string> zips = allowed ? LevelTransfer.FindZips() : new List<string>();
        UIVerticalList list = new UIVerticalList(this, string.Empty);
        list.SetWidth(1f, RelativeTo.ParentWidth);
        list.SetSpacing(0.02f, RelativeTo.ScreenHeight);
        list.SetMargins(0f, 0f, 0.03f, 0f);
        list.RemoveDrawHandler();
        new UIText(list, false, string.Empty, "Select the level file to import", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenHeight);
        if (!allowed)
        {
            new UIText(list, false, string.Empty, opened ? LevelTransfer.SettingsMsg : "Allow access to your files, then tap Import level again.", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenHeight);
            return;
        }
        if (zips.Count == 0)
        {
            new UIText(list, false, string.Empty, "No level files found.\nPut the .zip file you received in your Download folder.", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenHeight);
            return;
        }
        int max = Math.Min(zips.Count, 5);
        for (int i = 0; i < max; i++)
        {
            PsUIGenericButton b = new PsUIGenericButton(list);
            b.SetBlueColors();
            b.SetText(Display(zips[i]), 0.03f, 0.55f);
            b.SetHeight(0.075f, RelativeTo.ScreenHeight);
            m_buttons.Add(b);
            m_paths.Add(zips[i]);
        }
    }

    public override void Step()
    {
        if (m_exitButton.m_hit)
        {
            (GetRoot() as PsUIBasePopup).Destroy();
            return;
        }
        for (int i = 0; i < m_buttons.Count; i++)
        {
            if (m_buttons[i].m_hit)
            {
                string path = m_paths[i];
                (GetRoot() as PsUIBasePopup).Destroy();
                LevelTransfer.StartImport(path);
                return;
            }
        }
        base.Step();
    }
}

// receives the result of the browser's file picker (called from BBRWebGL.jslib through SendMessage)
public class WebPickReceiver : MonoBehaviour
{
    public const string ObjectName = "BBRLevelPick";

    public static void Ensure()
    {
        if (GameObject.Find(ObjectName) != null) return;
        GameObject go = new GameObject(ObjectName);
        DontDestroyOnLoad(go);
        go.AddComponent<WebPickReceiver>();
    }

    // payload: "<file name>|<base64 of the file>"
    public void OnPicked(string payload)
    {
        try
        {
            int bar = payload.IndexOf('|');
            LevelTransfer.WebPicked(Convert.FromBase64String(bar >= 0 ? payload.Substring(bar + 1) : payload));
        }
        catch (Exception)
        {
            LevelTransfer.WebPickFailed("error");
        }
    }

    public void OnPickFailed(string why)
    {
        LevelTransfer.WebPickFailed(why);
    }
}
