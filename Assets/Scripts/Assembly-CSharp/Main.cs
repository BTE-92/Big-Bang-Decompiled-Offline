using System;
using System.Globalization;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

public class Main : MonoBehaviour
{
	public static double m_EPOCHSeconds = 0.0;

	public static double m_gameTimeSinceAppStarted = 0.0;

	public static float m_resettingGameTime = 0f;

	public static float m_gameDeltaTime = 0f;

	public static int m_gameTicks = 0;

	public static int m_targetFPS = 60;

	public static float m_dt = 1f / (float)m_targetFPS;

	public static double m_dtD = 1.0 / (double)(float)m_targetFPS;

	public static IGame m_currentGame;

	public static bool m_paused = false;

	public static float m_timeScale = 1f;

	public static double m_timeScaleD = 1.0;

	public static int m_currentVersion = 0;

	private void Start()
	{
        //Extras had to be added to fix some issues specifically with the standalone windows builds.
        #region Extras
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        UnityEngine.Debug.unityLogger.logEnabled = false;
#endif
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;//For languages that use a , as a decimal seperator.
        ZipConstants.DefaultCodePage = 65001; //weird error
        #endregion
        SetEPOCHTime();
		m_currentGame = new PsGame("WhatOnEarth", "0-0-1");
		m_currentGame.Initialize(new StartupScene("StartupScene"));
	}

	private void Update()
	{
		SetEPOCHTime();
		m_currentGame.Update();
	}

	private void ResetTemporaryTime()
	{
		m_resettingGameTime = 0f;
	}

	private void SetEPOCHTime()
	{
		m_EPOCHSeconds = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	public static void SetTimeScale(float _scale)
	{
		m_timeScale = _scale;
		m_timeScaleD = _scale;
		Time.timeScale = _scale;
	}

	private void OnApplicationFocus(bool _focusStatus)
	{
		SetEPOCHTime();
		if (m_currentGame != null)
		{
			m_currentGame.OnApplicationFocus(_focusStatus);
		}
	}

	private void OnApplicationPause(bool _pauseStatus)
	{
		SetEPOCHTime();
		if (m_currentGame != null)
		{
			m_currentGame.OnApplicationPause(_pauseStatus);
		}
	}

	private void LateUpdate()
	{
		if (m_currentGame != null)
		{
			m_currentGame.LateUpdate();
		}
	}

	public static string GetApplicationDataPath()
	{
		string dataPath = Application.dataPath;
		if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			string text = dataPath;
			dataPath = text + "/../../SaveData/" + m_currentGame.m_projectCode + "/" + m_currentGame.m_projectVersion;
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			string text = dataPath;
			dataPath = text + "/../SaveData/" + m_currentGame.m_projectCode + "/" + m_currentGame.m_projectVersion;
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			dataPath = Application.dataPath;
		}
		else if (Application.platform == RuntimePlatform.OSXEditor)
		{
			dataPath = Application.dataPath;
		}
		else
		{
			Debug.Log(Application.dataPath);
			Debug.Log(Application.persistentDataPath);
			dataPath = Application.persistentDataPath + "/Documents";
		}
		if (!Directory.Exists(dataPath))
		{
			Directory.CreateDirectory(dataPath);
		}
		return dataPath;
	}

	public static string GetPersistentDataPath()
	{
		string persistentDataPath = Application.persistentDataPath;
		if (!Directory.Exists(persistentDataPath))
		{
			Directory.CreateDirectory(persistentDataPath);
		}
		return persistentDataPath;
	}

	public static string GetProgressionPath()
	{
		string text = GetApplicationDataPath() + "/" + m_currentGame.m_projectCode + "/Resources/" + m_currentGame.m_projectCode + "/Progression";
		Debug.Log("ProgressionPath: " + text);
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	public static bool FileExists(string _path)
	{
		return File.Exists(_path);
	}

	public static int CLIENT_VERSION()
	{
		if (m_currentVersion == 0)
		{
			string version = TrackedBundleVersion.Current.version;
			string text = string.Empty;
			string[] array = version.Split('.');
			for (int i = 0; i < array.Length; i++)
			{
				text += array[i];
			}
			m_currentVersion = Convert.ToInt32(text);
		}
		return m_currentVersion;
	}
}
