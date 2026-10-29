using System;
using System.IO;
using UnityEngine;

// Writes the game's exceptions and errors to BBR_errors.txt in the app's data folder, so a bug that only shows on a phone can be read afterwards:
// iPhone / iPad: Files app > On My iPhone > Big Bang (the folder is shared by the Info.plist) ; Android: Android/data/<package>/files ; PC: the persistent data path.
// (iOS hides the game's own console messages in the device log, so the device log cannot show them.) The file is capped and only keeps errors.
public static class ErrorLogFile
{
    private const long MaxBytes = 300 * 1024;

    private static string s_path;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        try
        {
            s_path = Path.Combine(Application.persistentDataPath, "BBR_errors.txt");
            if (File.Exists(s_path) && new FileInfo(s_path).Length > MaxBytes)
            {
                File.Delete(s_path);
            }
            File.AppendAllText(s_path, "\n==== " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + Application.platform + " v" + Application.version + " ====\n");
            Application.logMessageReceived += OnLog;
        }
        catch (Exception)
        {
        }
    }

    private static void OnLog(string message, string stackTrace, LogType type)
    {
        if (type != LogType.Exception && type != LogType.Error && type != LogType.Assert)
        {
            return;
        }
        try
        {
            if (new FileInfo(s_path).Length > MaxBytes)
            {
                return;
            }
            File.AppendAllText(s_path, DateTime.Now.ToString("HH:mm:ss") + " [" + type + "] " + message + "\n" + stackTrace + "\n");
        }
        catch (Exception)
        {
        }
    }
}