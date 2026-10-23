using System;
using System.IO;
using UnityEngine;

public static class IpConfig
{
    private const string CONFIG_FILENAME = "ipconfig.txt";
    private const string ROOT_FOLDER_NAME = "BigBangRacing";
    private const string SUB_FOLDER_NAME = "BigBang249_ipConfig";

    private const string DEFAULT_SERVER_URL =
        "https://woeprod.traplightgames.com";

    private const int MAX_FILE_LENGTH = 2048;

    private static string s_serverUrl;
    private static bool s_loaded;
    private static string s_activeConfigDir;

    public static string ServerUrl
    {
        get
        {
            if (!s_loaded) Load();
            return s_serverUrl;
        }
    }

    public static string ActiveConfigDirectory
    {
        get
        {
            if (s_activeConfigDir == null) ResolveConfigDirectory();
            return s_activeConfigDir;
        }
    }

    private static string ConfigPath
    {
        get { return Path.Combine(ActiveConfigDirectory, CONFIG_FILENAME); }
    }


    private static string ResolveConfigDirectory()
    {
        if (s_activeConfigDir != null)
            return s_activeConfigDir;

        string standaloneDir = GetStandaloneExecutableDirectory();
        if (standaloneDir != null)
        {
            if (TryEnsureWritableDirectory(standaloneDir))
            {
                s_activeConfigDir = standaloneDir;
                Debug.Log("[IpConfig] Desktop Exe Folder: " + s_activeConfigDir);
                return s_activeConfigDir;
            }
        }

        string emulatedRoot = GetEmulatedStorageRoot();
        if (emulatedRoot != null)
        {
            string candidate = Path.Combine(emulatedRoot, Path.Combine(ROOT_FOLDER_NAME, SUB_FOLDER_NAME));
            if (TryEnsureWritableDirectory(candidate))
            {
                s_activeConfigDir = candidate;
                return s_activeConfigDir;
            }
        }

        string appExternal = GetAppExternalFilesDir();
        if (appExternal != null)
        {
            string candidate = Path.Combine(appExternal, Path.Combine(ROOT_FOLDER_NAME, SUB_FOLDER_NAME));
            if (TryEnsureWritableDirectory(candidate))
            {
                s_activeConfigDir = candidate;
                return s_activeConfigDir;
            }
        }

        string fallback = Path.Combine(Application.persistentDataPath,
                                       Path.Combine(ROOT_FOLDER_NAME, SUB_FOLDER_NAME));
        TryEnsureWritableDirectory(fallback);
        s_activeConfigDir = fallback;
        Debug.Log("[IpConfig] Fallback persistent: " + s_activeConfigDir);
        return s_activeConfigDir;
    }

    private static string GetStandaloneExecutableDirectory()
    {
#if UNITY_STANDALONE && !UNITY_EDITOR
        try
        {
            DirectoryInfo parent = Directory.GetParent(Application.dataPath);
            if (parent != null)
            {
#if UNITY_STANDALONE_OSX
                if (parent.Parent != null && parent.Parent.Parent != null)
                    return parent.Parent.Parent.FullName;
#endif
                return parent.FullName;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[IpConfig] GetStandaloneExecutableDirectory failed: " + e.Message);
        }
#endif
        return null;
    }

    private static string GetEmulatedStorageRoot()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass env = new AndroidJavaClass("android.os.Environment");
            AndroidJavaObject dir = env.CallStatic<AndroidJavaObject>("getExternalStorageDirectory");
            string path = dir.Call<string>("getAbsolutePath");
            dir.Dispose();
            env.Dispose();
            return path;
        }
        catch (Exception e)
        {
            Debug.LogWarning("[IpConfig] getExternalStorageDirectory failed: " + e.Message);
            return null;
        }
#else
        return null;
#endif
    }

    private static string GetAppExternalFilesDir()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject dir = activity.Call<AndroidJavaObject>("getExternalFilesDir", (string)null);
            string path = null;
            if (dir != null)
            {
                path = dir.Call<string>("getAbsolutePath");
                dir.Dispose();
            }
            activity.Dispose();
            player.Dispose();
            return path;
        }
        catch (Exception e)
        {
            Debug.LogWarning("[IpConfig] getExternalFilesDir failed: " + e.Message);
            return null;
        }
#else
        return null;
#endif
    }

    // ---- Storage permission: call these yourself (e.g. from Main.Start) ----
    // Plain Android calls through AndroidJavaObject, so this works on Unity 5.4 (no UnityEngine.Android).

    private static int GetSdkInt()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass v = new AndroidJavaClass("android.os.Build$VERSION");
            int sdk = v.GetStatic<int>("SDK_INT");
            v.Dispose();
            return sdk;
        }
        catch { return 0; }
#else
        return 0;
#endif
    }

    // True when the storage permission is granted (always true off-device / below Android 6).
    public static bool HasStoragePermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            if (GetSdkInt() < 23) return true;

            AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            int result = activity.Call<int>("checkSelfPermission", "android.permission.WRITE_EXTERNAL_STORAGE");
            activity.Dispose();
            player.Dispose();
            return result == 0;
        }
        catch (Exception e)
        {
            Debug.LogWarning("[IpConfig] checkSelfPermission failed: " + e.Message);
            return false;
        }
#else
        return true;
#endif
    }

    // Shows the system Allow/Deny popup if the permission isn't granted yet.
    // Same call Unity's own Permission.RequestUserPermission ends up making: one permission,
    // request code 96489. Only WRITE is requested: READ and WRITE are the same permission group.
    public static void RequestStoragePermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            if (GetSdkInt() < 23 || HasStoragePermission()) return;

            AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");

            // must run on the Android UI thread, not Unity's thread
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                try
                {
                    string[] permissions = new string[] { "android.permission.WRITE_EXTERNAL_STORAGE" };
                    object[] args = new object[] { permissions, 96489 };
                    activity.Call("requestPermissions", args);
                }
                catch (Exception inner)
                {
                    Debug.LogError("[IpConfig] requestPermissions failed: " + inner.Message);
                }
            }));
        }
        catch (Exception e)
        {
            Debug.LogError("[IpConfig] RequestStoragePermission failed: " + e.Message);
        }
#endif
    }

    // Call after the permission was granted so the config folder is resolved again.
    public static void ResetAndReload()
    {
        s_activeConfigDir = null;
        s_loaded = false;
        Load();
    }

    private static bool TryEnsureWritableDirectory(string path)
    {
        try
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string probe = Path.Combine(path, ".write_probe");
            File.WriteAllText(probe, "ok");
            File.Delete(probe);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void Load()
    {
        s_loaded = true;

        try
        {
            string path = ConfigPath;

            if (!File.Exists(path))
            {
                Debug.Log("[IpConfig] No config file, writing default.");
                WriteDefault();
                return;
            }

            if (new FileInfo(path).Length > MAX_FILE_LENGTH)
            {
                Debug.LogWarning("[IpConfig] Config too large, resetting.");
                WriteDefault();
                return;
            }

            string raw = File.ReadAllText(path);
            string url = null;
            foreach (string line in raw.Split('\n'))
            {
                string trimmed = line.Trim();
                if (trimmed.Length > 0) { url = trimmed; break; }
            }

            if (string.IsNullOrEmpty(url) || !IsWellFormedUrl(url))
            {
                Debug.LogWarning("[IpConfig] Invalid URL in config, resetting.");
                WriteDefault();
                return;
            }

            s_serverUrl = url;
            Debug.Log("[IpConfig] Loaded: " + s_serverUrl);
        }
        catch (Exception e)
        {
            Debug.LogError("[IpConfig] Read failed (" + e.Message + "), resetting.");
            WriteDefault();
        }
    }

    private static void WriteDefault()
    {
        s_serverUrl = DEFAULT_SERVER_URL;
        try { File.WriteAllText(ConfigPath, DEFAULT_SERVER_URL); }
        catch (Exception e)
        { Debug.LogError("[IpConfig] Write failed: " + e.Message); }
    }

    public static void ResetToDefault()
    {
        try { if (File.Exists(ConfigPath)) File.Delete(ConfigPath); }
        catch (Exception e) { Debug.LogError("[IpConfig] Delete failed: " + e.Message); }
        WriteDefault();
        s_loaded = true;
    }

    private static bool IsWellFormedUrl(string url)
    {
        Uri result;
        if (!Uri.TryCreate(url, UriKind.Absolute, out result)) return false;
        return result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps;
    }

}