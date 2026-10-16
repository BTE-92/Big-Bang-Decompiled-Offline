using System;
using System.IO;
using FMOD;
using FMOD.Studio;
using UnityEngine;

public static class PsFMODManager
{
	public static string[] pluginPaths = new string[0];

	public static GameObject m_listener = null;

	public static bool m_forceListenerZToZero;

	public static bool m_allowInit = false;

	private static string pluginPath
	{
		get
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return Application.dataPath + "/Plugins/x86";
			}
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXDashboardPlayer || Application.platform == RuntimePlatform.LinuxPlayer)
			{
				return Application.dataPath + "/Plugins";
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				UnityUtil.LogError("DSP Plugins not currently supported on iOS, contact support@fmod.org for more information");
				return string.Empty;
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
				string name = directoryInfo.Parent.Name;
				return "/data/data/" + name + "/lib";
			}
			UnityUtil.LogError("Unknown platform!");
			return string.Empty;
		}
	}

	public static void loadBank(string fileName)
	{
		string streamingAsset = getStreamingAsset(fileName);
		if (!string.IsNullOrEmpty(streamingAsset))
		{
			Bank bank = null;
			RESULT rESULT = FMOD_StudioSystem.instance.System.loadBankFile(streamingAsset, LOAD_BANK_FLAGS.NORMAL, out bank);
			switch (rESULT)
			{
			case RESULT.ERR_VERSION:
				UnityUtil.LogError(string.Format("Bank {0} built with an incompatible version of FMOD Studio.", fileName));
				break;
			default:
				UnityUtil.LogError(string.Format("Bank {0} encountered a loading error {1}", fileName, rESULT.ToString()));
				break;
			case RESULT.OK:
			case RESULT.ERR_EVENT_ALREADY_LOADED:
				break;
			}
			UnityUtil.Log("bank load: " + ((!(bank != null)) ? "failed!!" : "suceeded"));
		}
		else
		{
			UnityEngine.Debug.LogWarning("FMOD bank with filename " + fileName + " could not be found");
		}
	}

	public static string getStreamingAsset(string fileName)
	{
		if (fileName.StartsWith("Master") || fileName.StartsWith("FMOD_bank_list"))
		{
			string empty = string.Empty;
			empty = ((Application.platform == RuntimePlatform.Android) ? "file:///android_asset" : ((Application.platform != RuntimePlatform.MetroPlayerARM && Application.platform != RuntimePlatform.MetroPlayerX86 && Application.platform != RuntimePlatform.MetroPlayerX64) ? Application.streamingAssetsPath : "ms-appx:///Data/StreamingAssets"));
			return empty + "/" + fileName;
		}
		if (File.Exists(Application.persistentDataPath + "/" + fileName))
		{
			return Application.persistentDataPath + "/" + fileName;
		}
		return null;
	}

	public static bool Initialize()
	{
		if (m_allowInit)
		{
			LoadPlugins();
			string text = Application.streamingAssetsPath + "/FMOD_bank_list.txt";
			UnityUtil.Log("Loading Banks");
			try
			{
				WWW wWW = new WWW(text);
				while (!wWW.isDone)
				{
				}
				string[] array = wWW.text.Split('\n');
				wWW.Dispose();
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (!string.IsNullOrEmpty(text2) && text2.Length > 5)
					{
						UnityUtil.Log("Load " + text2);
						loadBank(text2);
					}
				}
			}
			catch (Exception ex)
			{
				UnityUtil.LogError("Cannot read " + text + ": " + ex.Message + " : No banks loaded.");
			}
			Update3DAttributes();
			return true;
		}
		return false;
	}

	private static void LoadPlugins()
	{
		FMOD.System system = null;
		ERRCHECK(FMOD_StudioSystem.instance.System.getLowLevelSystem(out system));
		if (Application.platform == RuntimePlatform.IPhonePlayer && pluginPaths.Length != 0)
		{
			UnityUtil.LogError("DSP Plugins not currently supported on iOS, contact support@fmod.org for more information");
			return;
		}
		string[] array = pluginPaths;
		foreach (string rawName in array)
		{
			string text = pluginPath + "/" + GetPluginFileName(rawName);
			UnityUtil.Log("Loading plugin: " + text);
			if (!File.Exists(text))
			{
				UnityUtil.LogWarning("plugin not found: " + text);
			}
			uint handle;
			ERRCHECK(system.loadPlugin(text, out handle));
		}
	}

	public static void Update3DAttributes()
	{
		FMOD.Studio.System system = FMOD_StudioSystem.instance.System;
		if (!(system != null) || !system.isValid())
		{
			return;
		}
		Vector3 pos = Vector3.zero;
		if (m_listener != null)
		{
			pos = m_listener.transform.position;
			if (m_forceListenerZToZero)
			{
				pos.z = 0f;
			}
		}
		FMOD.Studio.ATTRIBUTES_3D attributes = pos.to3DAttributes();
		ERRCHECK(system.setListenerAttributes(0, attributes));
	}

	private static string GetPluginFileName(string rawName)
	{
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
		{
			return rawName + ".dll";
		}
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXDashboardPlayer)
		{
			return rawName + ".dylib";
		}
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.LinuxPlayer)
		{
			return "lib" + rawName + ".so";
		}
		UnityUtil.LogError("Unknown platform!");
		return string.Empty;
	}

	private static void ERRCHECK(RESULT result)
	{
		UnityUtil.ERRCHECK(result);
	}

	public static string GetMusic(string _music)
	{
		string empty = string.Empty;
		if (File.Exists(getStreamingAsset(_music + ".bank")))
		{
			return "/Music/" + _music;
		}
		return "/Music/RacingMusic2";
	}
}
