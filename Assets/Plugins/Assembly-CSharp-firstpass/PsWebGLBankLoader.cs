// NEW FILE. Only compiled for WebGL players (see the #if below). Put it in the same folder as PsFMODManager.cs (Assembly-CSharp-firstpass),
// because FMOD_Listener lives in that assembly and cannot see Assembly-CSharp.
//
// WebGL has no real filesystem for StreamingAssets: Application.streamingAssetsPath is an
// HTTP URL, so System.IO / FMOD loadBankFile can't read it. This class downloads
// FMOD_bank_list.txt and the Master banks over HTTP up front, then hands the raw bytes to
// FMOD_Listener, which loads them with loadBankMemory.
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PsWebGLBankLoader : MonoBehaviour
{
    private const string ListFileName = "FMOD_bank_list.txt";

    private static readonly Dictionary<string, byte[]> s_banks = new Dictionary<string, byte[]>();
    private static string[] s_bankList = new string[0];
    private static bool s_started;
    private static bool s_done;

    // True once the downloads have finished (or failed).
    public static bool IsReady { get { return s_done; } }

    public static string[] BankList { get { return s_bankList; } }

    public static byte[] GetBankBytes(string fileName)
    {
        byte[] data;
        return s_banks.TryGetValue(fileName, out data) ? data : null;
    }

    // Call as early as possible (the preloader constructor is fine). Safe to call repeatedly.
    public static void Begin()
    {
        if (s_started)
        {
            return;
        }
        s_started = true;
        GameObject go = new GameObject("PsWebGLBankLoader");
        DontDestroyOnLoad(go);
        PsWebGLBankLoader loader = go.AddComponent<PsWebGLBankLoader>();
        loader.StartCoroutine(loader.Run());
    }

    private IEnumerator Run()
    {
        yield return StartCoroutine(LoadAll());
        s_done = true; // set even on failure so the game doesn't hang on the loading screen
    }

    private IEnumerator LoadAll()
    {
        string baseUrl = Application.streamingAssetsPath + "/";

        string listText = null;
        using (UnityWebRequest req = UnityWebRequest.Get(baseUrl + ListFileName))
        {
            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError("PsWebGLBankLoader: cannot fetch " + baseUrl + ListFileName + ": " + req.error);
                yield break;
            }
            listText = req.downloadHandler.text;
        }

        string[] lines = listText.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> names = new List<string>();
        for (int i = 0; i < lines.Length; i++)
        {
            string name = lines[i].Trim();
            if (name.Length > 0)
            {
                names.Add(name);
            }
        }
        s_bankList = names.ToArray();

        for (int i = 0; i < s_bankList.Length; i++)
        {
            string name = s_bankList[i];
            // Same rule as FMOD_Listener: only the local Master banks are loaded at boot.
            // The others are downloaded by PsBackgroundDownloader.
            if (!name.StartsWith("Master"))
            {
                continue;
            }
            string url = baseUrl + name.Replace(" ", "%20");
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();
                if (req.isNetworkError || req.isHttpError)
                {
                    Debug.LogError("PsWebGLBankLoader: cannot fetch " + url + ": " + req.error);
                    continue;
                }
                s_banks[name] = req.downloadHandler.data;
                Debug.Log("PsWebGLBankLoader: fetched " + name + " (" + req.downloadHandler.data.Length + " bytes)");
            }
        }
    }
}
#endif
