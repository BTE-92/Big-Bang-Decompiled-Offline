using System;
using System.Collections.Generic;
using Server;
using UnityEngine;

public static class PsMetrics
{
	public static void Initialize()
	{
		Metrics.Initialize("a132fbb1", "WQWM93PDWY44322VD3K8", "2XG43WSQHR5C77VCXMJQ", "m4ck5yjbw7pc");
		Login(PlayerPrefsX.GetFirstLogin());
	}

	public static void Login(bool _first = false)
	{
		if (_first)
		{
			Metrics.FirstTimeLogin();
			PlayerPrefsX.SetFirstLogin(false);
		}
		else
		{
			Metrics.Login();
		}
	}

	public static void ReturnedFromBackground()
	{
		Metrics.Login();
	}

	public static void PreloadStart()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("action_type", "started");
		Metrics.SendEvent("Preloader", dictionary);
	}

	public static void PreloadPaused()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("action_type", "paused");
		Metrics.SendEvent("Preloader", dictionary);
	}

	public static void PreloadFinished(bool _didDownload = true)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("action_type", "finished");
		dictionary.Add("didDownload", _didDownload.ToString());
		Metrics.SendEvent("Preloader", dictionary);
	}

	public static void LevelLoadStart()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("action_type", "started");
		Metrics.SendEvent("LevelLoad", dictionary);
	}

	public static void LevelLoadFinished()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("action_type", "finished");
		Metrics.SendEvent("LevelLoad", dictionary);
	}

	public static void ReportPerformance(float _perfValue)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("performanceValue", _perfValue.ToString());
		dictionary.Add("cpuSpeedInMHz", SystemInfo.processorFrequency.ToString());
		dictionary.Add("cpuName", SystemInfo.processorType);
		dictionary.Add("gpuName", SystemInfo.graphicsDeviceName);
		dictionary.Add("gpuVendor", SystemInfo.graphicsDeviceVendor);
		dictionary.Add("gpuMemory", SystemInfo.graphicsMemorySize.ToString());
		Metrics.SendEvent("DevicePerformance", dictionary);
	}

	public static void Tutorial(bool _completed = true)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Completed", _completed.ToString());
		Metrics.SendEvent("Tutorial", dictionary);
		FacebookManager.SendTutorialComplete(_completed);
	}

	public static void Progression(int _currentNode)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("ProgressionNode", _currentNode.ToString());
		Metrics.SendEvent("Progression", dictionary);
	}

	public static void OutOfKeys()
	{
		Keys("ranout");
	}

	private static void Keys(string _eventType)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("action_type", _eventType);
		Metrics.SendEvent("Keys", dictionary);
	}

	public static void GetKeys(string _amount, string _context)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("keyAmount", _amount);
		dictionary.Add("context", _context);
		Metrics.SendEvent("GetKeys", dictionary);
	}

	public static void UseKeys(string _amount, string _context)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("keyAmount", _amount);
		dictionary.Add("context", _context);
		Metrics.SendEvent("UseKeys", dictionary);
	}

	public static void UseDiamonds(string _amount, string _context)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("diamondAmount", _amount);
		dictionary.Add("context", _context);
		Metrics.SendEvent("UseDiamonds", dictionary);
	}

	public static void PurchaseDiamonds(string _amount, string _context)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("diamondAmount", _amount);
		dictionary.Add("context", _context);
		Metrics.SendEvent("PurchaseDiamonds", dictionary);
	}

	public static void ConvertDiamondsToCoins(string _diamondAmount, string _amountCoinsMissing, string _context, string _upgradeName, string _upgradeValue)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("diamondAmount", _diamondAmount);
		dictionary.Add("amountCoinsMissing", _amountCoinsMissing);
		dictionary.Add("context", _context.ToLower());
		dictionary.Add("upgradeName", _upgradeName);
		dictionary.Add("upgradeValue", _upgradeValue);
		Metrics.SendEvent("ConvertDiamonds", dictionary);
	}

	public static void VehicleUpgrade(string _vehicle, string _upgradeName, string _upgradeValue)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("vehicle", _vehicle);
		dictionary.Add("upgradeName", _upgradeName);
		dictionary.Add("upgradeValue", _upgradeValue);
		Metrics.SendEvent("VehicleUpgrade", dictionary);
	}

	public static void ButtonPressed(string _label, string _context = null)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("label", _label.ToLower());
		if (_context != null)
		{
			dictionary.Add("context", _context.ToLower());
		}
		Metrics.SendEvent("woe_ButtonPressed", dictionary);
	}

	public static void AdWatched(string _context)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("context", _context.ToLower());
		Metrics.SendEvent("woe_AdWatched", dictionary);
	}

	public static void MinigameRated(PsMetagameManager.SaveRatingData _data, string _quality, int _currentNode)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Context", _data.m_minigameContext);
		dictionary.Add("minigameId", _data.m_minigameId);
		dictionary.Add("minigameName", _data.m_minigameName);
		dictionary.Add("Mode", _data.m_minigameGameMode);
		dictionary.Add("rating", _data.m_rating.ToString());
		dictionary.Add("playerLevel", _data.m_playerLevel.ToString());
		dictionary.Add("levelQuality", _quality);
		dictionary.Add("currentNode", _currentNode.ToString());
		Metrics.SendEvent("MinigameRated", dictionary);
	}

	public static void MinigameStarted(PsGameLoop _minigameInfo)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Context", _minigameInfo.m_context.ToString());
		dictionary.Add("minigameId", _minigameInfo.GetGameId());
		dictionary.Add("minigameName", _minigameInfo.GetName());
		dictionary.Add("Mode", _minigameInfo.GetGameMode().ToString());
		Metrics.SendEvent("MinigameStarted", dictionary);
	}

	public static void MinigameCompleted(string _context, PsMinigameMetaData _meta, int _restartsUsed, long _timeSpent, bool _win)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Win", _win.ToString());
		dictionary.Add("Context", _context);
		dictionary.Add("Mode", _meta.gameMode.ToString());
		dictionary.Add("minigameId", _meta.id);
		dictionary.Add("minigameName", _meta.name);
		dictionary.Add("restarts", _restartsUsed.ToString());
		dictionary.Add("timeSpent", _timeSpent.ToString());
		Metrics.SendEvent("MinigameCompleted", dictionary);
	}

	public static void MinigameExited(string _context, PsMinigameMetaData _meta, long _timeSpent)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Context", _context);
		dictionary.Add("Mode", _meta.gameMode.ToString());
		dictionary.Add("minigameId", _meta.id);
		dictionary.Add("minigameName", _meta.name);
		dictionary.Add("timeSpent", _timeSpent.ToString());
		Metrics.SendEvent("MinigameExited", dictionary);
	}

	public static void MinigameSkipped(string _context, PsMinigameMetaData _meta, long _timeSpent)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Context", _context);
		dictionary.Add("Mode", _meta.gameMode.ToString());
		dictionary.Add("minigameId", _meta.id);
		dictionary.Add("minigameName", _meta.name);
		dictionary.Add("timeSpent", _timeSpent.ToString());
		Metrics.SendEvent("MinigameSkipped", dictionary);
	}

	public static void PlayerEnteredEditor()
	{
		if (!PlayerPrefsX.GetEditorUseFlag())
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("FirstTimeInEditor", true.ToString());
			PlayerPrefsX.SetEditorUseFlag();
			Metrics.SendEvent("woe_PlayerEnteredEditor", dictionary);
		}
	}

	public static void PlayerPublishedLevel()
	{
		if (!PlayerPrefsX.GetPublishFlag())
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("FirstLevelPublished", true.ToString());
			PlayerPrefsX.SetPublishFlag();
			Metrics.SendEvent("woe_PlayerEnteredEditor", dictionary);
		}
	}

	public static void LevelCreationStarted()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("New", "True");
		Metrics.SendEvent("woe_LevelCreationStarted", dictionary);
	}

	public static void LevelSaved()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("minigameId", PsState.m_activeGameLoop.m_minigameMetaData.id);
		string[] itemsUsed = PsState.m_activeGameLoop.m_minigameMetaData.itemsUsed;
		foreach (string text in itemsUsed)
		{
			int num = (int)PsState.m_activeMinigame.m_itemCount[text];
			Debug.Log(text + ":" + num);
			dictionary.Add(text, num.ToString());
		}
		Metrics.SendEvent("woe_LevelSaved", dictionary);
	}

	public static void LevelPublished()
	{
		PlayerPublishedLevel();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("minigameId", PsState.m_activeGameLoop.m_minigameMetaData.id);
		string[] itemsUsed = PsState.m_activeGameLoop.m_minigameMetaData.itemsUsed;
		foreach (string text in itemsUsed)
		{
			int num = (int)PsState.m_activeMinigame.m_itemCount[text];
			Debug.Log(text + ":" + num);
			dictionary.Add(text, num.ToString());
		}
		Metrics.SendEvent("woe_LevelPublished", dictionary);
	}

	public static void LevelDeleted(string _miniGameId)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("minigameId", _miniGameId);
		Metrics.SendEvent("woe_LevelDeleted", dictionary);
	}

	public static void LevelEditorOpened()
	{
		PsMetricsData.m_timeInEditor = 0;
	}

	public static void LevelEditorClosed(int _timeSpentInEditor)
	{
		Debug.LogError("!!!!! LevelEditorClosed - Time spent in editor:" + _timeSpentInEditor);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Context", "Editor");
		dictionary.Add("timeSpent", _timeSpentInEditor.ToString());
		Metrics.SendEvent("woe_EditorClosed", dictionary);
		PsMetricsData.m_timeInEditor = 0;
	}

	public static void GhostLoaded(string _context, string _size, string _minigame)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Context", _context);
		dictionary.Add("Minigame", _minigame);
		dictionary.Add("Size", _size);
		Metrics.SendEvent("woeServer_GhostLoaded", dictionary);
	}

	public static void GhostSaved(string _context, string _size, string _minigame, bool _newGhost)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Context", _context);
		dictionary.Add("Type", (!_newGhost) ? "Improved" : "New");
		dictionary.Add("Minigame", _minigame);
		dictionary.Add("Size", _size);
		Metrics.SendEvent("woeServer_GhostSaved", dictionary);
	}

	public static void ValidatedPurchase(string _identifier, string _total, string _currency, string _transactionId)
	{
		Metrics.SendRevenueEvent(_total, _currency, _transactionId);
		Analytics.TrackPurchase(_identifier, (float)Convert.ToDouble(_total), _currency);
		int num = (int)Convert.ToDouble(_total) * 100;
	}

	private static void LogDict(Dictionary<string, string> _dict)
	{
		foreach (KeyValuePair<string, string> item in _dict)
		{
			Debug.Log("Analytics: " + item.Key + " - " + item.Value);
		}
	}
}
