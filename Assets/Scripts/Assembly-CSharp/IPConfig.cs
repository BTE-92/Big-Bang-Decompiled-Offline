using System;
using UnityEngine;

// The game talks to the embedded offline server (see OfflineServer.cs), so there is no server address to configure any more.
// What is left: the server url and the Android storage permission (used by the level Import/Export file picker).
public static class IpConfig
{
    public static string ServerUrl
    {
        get { return OfflineServer.BaseUrl; }
    }

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
}
