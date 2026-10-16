using System;
using System.Collections.Generic;
using System.IO;
using Server;
using UnityEngine;

public static class PsBackgroundDownloader
{
    private static int m_checkCount = 0;

    public static bool m_checksDone = false;

    private static bool m_fireNext = true;

    private static List<FileMetaData> m_metas = new List<FileMetaData>();

    public static void Initialize()
    {
        string rawContent = null;

#if !UNITY_ANDROID || UNITY_EDITOR
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "FMOD_bank_list.txt");
        try
        {
            if (File.Exists(filePath))
            {
                rawContent = File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogError("BANK LIST NOT FOUND");
                return;
            }
        }
        catch
        {
            Debug.LogError("BANK LIST NOT FOUND");
            return;
        }
#else
		string url = Application.streamingAssetsPath + "/FMOD_bank_list.txt";
		try
		{
			using (WWW wWW = new WWW(url))
			{
				System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
				while (!wWW.isDone)
				{
					if (stopwatch.ElapsedMilliseconds > 5000)
					{
						Debug.LogError("BANK LIST NOT FOUND");
						return;
					}
					System.Threading.Thread.Sleep(1);
				}

				if (!string.IsNullOrEmpty(wWW.error))
				{
					Debug.LogError("BANK LIST NOT FOUND");
					return;
				}

				rawContent = wWW.text;
			}
		}
		catch
		{
			Debug.LogError("BANK LIST NOT FOUND");
			return;
		}
#endif

        if (string.IsNullOrEmpty(rawContent))
        {
            return;
        }

        string[] array = rawContent.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string rawText in array)
        {
            string text = rawText.Trim();
            if (string.IsNullOrEmpty(text) || text.StartsWith("Master"))
            {
                continue;
            }

            string text2 = text;
            string extension = ".bank";
            if (text2.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                text2 = text2.Substring(0, text2.Length - extension.Length);
            }

            if (string.IsNullOrEmpty(text2))
            {
                continue;
            }

            m_checkCount++;
            Debug.Log(text2 + "/" + m_checkCount);
            HttpC httpC = Preload.Check(text2, FileCheckSUCCEED, FileCheckFAILED);
            if (httpC != null)
            {
                httpC.tag = text2;
            }
        }
    }

    private static void FileCheckSUCCEED(FileMetaData _meta)
    {
        m_checkCount--;
        if (_meta.version > KeyChain.GetFileVersion(_meta.name) || !File.Exists(Main.GetPersistentDataPath() + "/" + _meta.name + ".bank"))
        {
            if (File.Exists(Main.GetPersistentDataPath() + "/" + _meta.name + ".bank"))
            {
                File.Delete(Main.GetPersistentDataPath() + "/" + _meta.name + ".bank");
            }
            if (m_fireNext)
            {
                DownloadFile(_meta);
            }
            else
            {
                m_metas.Add(_meta);
            }
        }
        if (m_checkCount == 0)
        {
            m_checksDone = true;
        }
    }

    private static void DownloadFile(FileMetaData _meta)
    {
        m_fireNext = false;
        HttpC httpC = Preload.Download(_meta, FileDownloadSUCCEED, FileDownloadFAILED);
        httpC.ignoreInQueue = true;
    }

    private static void FileCheckFAILED(HttpC _c)
    {
        ServerManager.ThrowServerErrorException("Connection Error", ServerErrors.GetNetworkError(_c.www.error), delegate
        {
            HttpC httpC = Preload.Check(_c.tag, FileCheckSUCCEED, FileCheckFAILED);
            httpC.tag = _c.tag;
            return httpC;
        });
    }

    private static void FileDownloadSUCCEED(HttpC _c)
    {
        m_fireNext = true;
        FileMetaData fileMetaData = _c.objectData as FileMetaData;
        Debug.Log("FILE DOWNLOADED: " + fileMetaData.name);
        KeyChain.SetFileVersion(fileMetaData.name, fileMetaData.version);
        ClientTools.SaveFile(fileMetaData.name, ".bank", _c.www.bytes);
        if (SoundS.m_initialized)
        {
            PsFMODManager.loadBank(fileMetaData.name + ".bank");
        }
    }

    private static void FileDownloadFAILED(HttpC _c)
    {
        Debug.Log("FILE DOWNLOAD FAILED");
        HttpC httpC = Preload.Download(_c.objectData as FileMetaData, FileDownloadSUCCEED, FileDownloadFAILED);
        httpC.ignoreInQueue = true;
    }

    public static void Update()
    {
        if (m_metas.Count > 0 && m_fireNext)
        {
            DownloadFile(m_metas[0]);
            m_metas.RemoveAt(0);
        }
    }
}