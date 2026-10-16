using Server.Utility;
using UnityEngine;

public class PlayerPrefsX
{
	public const string METRICS_GUID = "MetricsGUID";

	public const string GAME_CENTER_ID = "GameCenterId";

	public const string GAME_CENTER_NAME = "GameCenterName";

	public const string FACEBOOK_ID = "FacebookId";

	public const string FACEBOOK_NAME = "FacebookName";

	public const string USER_NAME = "UserName";

	public const string USER_ID = "UserID";

	public const string ACCEPT_NOTIFICATIONS = "AcceptNotifications";

	public const string DEVICE_TOKEN = "deviceToken";

	public const string SOUND_MUTE_SOUNDFX = "soundMute";

	public const string SOUND_MUTE_MUSIC = "soundMuteMusic";

	public const string ITEM_DB_VERSION = "itemDb";

	public const string LEFTY_FLIP = "Lefty";

	public const string EVERYPLAY_ENABLED = "EveryplayEnabled";

	public const string LAST_LOGIN = "LastLogin";

	public const string PATH_VERSION = "pathVersion";

	public const string CAR_RENT_REFRESH = "carRentRefresh";

	public const string FRESHFREE_INTERVAL = "freshFreeInterval";

	public const string SESSION_ID = "sessionId";

	public const string START_BOLTS = "startBolts";

	public const string START_COINS = "startCoins";

	public const string START_DIAMONDS = "startDiamonds";

	public const string START_KEYS = "startKeys";

	public const string SKIP_TIMER = "skipTimer";

	public const string FB_CONNECT_REWARD = "fbConnectRewardAmount";

	public const string VIDEO_AD_TIME_OUT = "videoAdTimeOut";

	public const string VIDEO_AD_START_TIME_OUT = "videoAdStartTimeOut";

	public const string VIDEO_AD_COUNT = "videoAdCount";

	public const string VIDEO_AD_START_COUNT = "videoAdStartCount";

	public const string FRESH_LEVEL_COOLDOWN = "freshFreeCoolDown";

	public const string FRESH_LEVEL_START_COOLDWON = "freshFreeStartCoolDown";

	public const string FRESH_LEVEL_COUNT = "freshFreeCount";

	public const string FRESH_LEVEL_START_COUNT = "freshFreeStartCount";

	public const string DAILY_GEM_MULTIPLIER = "dailyGemMultiplier";

	public const string FIRST_LOGIN = "firstLogin";

	public const string COINBOOST_TIMER = "coinBoostTimer";

	public const string MAJOR_PATH = "majorPath";

	public const string LAST_SYNC = "LastSync";

	public const string USED_EDITOR = "playerUsedEditor";

	public const string PUBLISHED_LEVEL = "playerPublishedLevel";

	public const string NEW_ITEMS = "newItems";

	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
		PlayerPrefs.Save();
	}

	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}

	public static void SetEditorUseFlag(bool _state = true)
	{
		SetBool("playerUsedEditor", _state);
	}

	public static bool GetEditorUseFlag()
	{
		return GetBool("playerUsedEditor", false);
	}

	public static void SetPublishFlag(bool _state = true)
	{
		SetBool("playerPublishedLevel", _state);
	}

	public static bool GetPublishFlag()
	{
		return GetBool("playerPublishedLevel", false);
	}

	public static void SetMajorPath(int _version)
	{
		PlayerPrefs.SetInt("majorPath", _version);
	}

	public static int GetMajorPath()
	{
		return PlayerPrefs.GetInt("majorPath", 1);
	}

	public static void SetFirstLogin(bool _state)
	{
		SetBool("firstLogin", _state);
	}

	public static bool GetFirstLogin()
	{
		return GetBool("firstLogin", true);
	}

	public static void SetSession(string _sessionId)
	{
		PlayerPrefs.SetString("sessionId", _sessionId);
	}

	public static string GetSession()
	{
		string text = PlayerPrefs.GetString("sessionId");
		if (text != null && text != string.Empty)
		{
			return text;
		}
		return "new";
	}

	public static void SetClientConfig(ClientConfig _conf)
	{
		if (_conf == null)
		{
			_conf = new ClientConfig();
		}
		PlayerPrefs.SetInt("carRentRefresh", _conf.carRefreshMinutes);
		PlayerPrefs.SetInt("freshFreeInterval", _conf.freshFreeInterval);
		PlayerPrefs.SetInt("startBolts", _conf.boltsAtStart);
		PlayerPrefs.SetInt("startCoins", _conf.coinsAtStart);
		PlayerPrefs.SetInt("startDiamonds", _conf.diamondsAtStart);
		PlayerPrefs.SetInt("startKeys", _conf.keysAtStart);
		PlayerPrefs.SetInt("fbConnectRewardAmount", _conf.fbConnectReward);
		PlayerPrefs.SetInt("dailyGemMultiplier", _conf.dailyGemAmount);
		PlayerPrefs.SetInt("freshFreeStartCoolDown", _conf.freshFreeCoolDown);
		PlayerPrefs.SetInt("freshFreeStartCount", _conf.freshFreeCount);
		PlayerPrefs.SetInt("videoAdStartTimeOut", _conf.videoAdCoolDown);
		PlayerPrefs.SetInt("videoAdStartCount", _conf.videoAdCount);
		PlayerPrefs.Save();
	}

	public static ClientConfig GetClientConfig()
	{
		ClientConfig clientConfig = new ClientConfig();
		clientConfig.carRefreshMinutes = PlayerPrefs.GetInt("carRentRefresh");
		clientConfig.freshFreeInterval = PlayerPrefs.GetInt("freshFreeInterval");
		clientConfig.boltsAtStart = PlayerPrefs.GetInt("startBolts");
		clientConfig.coinsAtStart = PlayerPrefs.GetInt("startCoins");
		clientConfig.diamondsAtStart = PlayerPrefs.GetInt("startDiamonds");
		clientConfig.keysAtStart = PlayerPrefs.GetInt("startKeys");
		clientConfig.fbConnectReward = PlayerPrefs.GetInt("fbConnectRewardAmount");
		return clientConfig;
	}

	public static void SetPlayerData(PlayerData _data)
	{
		PlayerPrefs.SetString("UserID", _data.playerId);
		PlayerPrefs.SetString("UserName", _data.name);
		PlayerPrefs.SetInt("itemDb", _data.itemDbVersion);
		PlayerPrefs.SetInt("AcceptNotifications", _data.acceptNotifications ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static void SetLastLoginTime(string _value)
	{
		SaveString("LastLogin", _value);
	}

	public static void SetGameCenterName(string _value)
	{
		SaveString("GameCenterName", _value);
	}

	public static void SetFacebookName(string _value)
	{
		SaveString("FacebookName", _value);
	}

	public static string GetFacebookName()
	{
		return GetString("FacebookName");
	}

	public static void SetAcceptNotifications(bool _value)
	{
		SetBool("AcceptNotifications", _value);
	}

	public static bool GetAcceptNotifications()
	{
		return GetBool("AcceptNotifications");
	}

	public static void SetFacebookId(string _facebookId)
	{
		SaveString("FacebookId", _facebookId);
	}

	public static void SetGameCenterId(string _gameCenterId)
	{
		SaveString("GameCenterId", _gameCenterId);
	}

	public static void SetUserName(string _userName)
	{
		SaveString("UserName", _userName);
	}

	public static void SetUserId(string _userId)
	{
		SaveString("UserID", _userId);
	}

	public static void SetMuteSoundFX(bool _mute)
	{
		SetBool("soundMute", _mute);
	}

	public static void SetMuteMusic(bool _mute)
	{
		SetBool("soundMuteMusic", _mute);
	}

	public static void SetLefty(bool _lefty)
	{
		SetBool("Lefty", _lefty);
	}

	public static void SetEveryplayEnabled(bool _enabled)
	{
		SetBool("EveryplayEnabled", _enabled);
	}

	public static void SetItemDbVersion(int _version)
	{
		SaveInt("itemDb", _version);
	}

	public static void SetPathVersion(int _version)
	{
		SaveInt("pathVersion", _version);
	}

	public static int GetDailyGemMultiplier()
	{
		return GetInt("dailyGemMultiplier");
	}

	public static void SetSkipTimeLeft(int _time)
	{
		SaveInt("skipTimer", _time);
	}

	public static int GetSkipTimeLeft()
	{
		return GetInt("skipTimer");
	}

	public static void SetCoinBoostTimeLeft(int _time)
	{
		SaveInt("coinBoostTimer", _time);
	}

	public static int GetCoinBoostTimeLeft()
	{
		return GetInt("coinBoostTimer");
	}

	public static int GetVideoAdCount()
	{
		if (PlayerPrefs.HasKey("videoAdCount"))
		{
			return PlayerPrefs.GetInt("videoAdCount");
		}
		return -1;
	}

	public static int GetVideoAdStartCount()
	{
		return GetInt("videoAdStartCount");
	}

	public static void SetVideoAdCount(int _count)
	{
		SaveInt("videoAdCount", _count);
	}

	public static string GetVideoAdTimeOut()
	{
		return GetString("videoAdTimeOut");
	}

	public static int GetVideoAdStartTimeOut()
	{
		return GetInt("videoAdStartTimeOut");
	}

	public static void SetVideoAdTimeOut(string _date)
	{
		SaveString("videoAdTimeOut", _date);
	}

	public static int GetFreshCount()
	{
		if (PlayerPrefs.HasKey("freshFreeCount"))
		{
			return PlayerPrefs.GetInt("freshFreeCount");
		}
		return -1;
	}

	public static int GetFreshStartCount()
	{
		return GetInt("freshFreeStartCount");
	}

	public static void SetFreshCount(int _count)
	{
		SaveInt("freshFreeCount", _count);
	}

	public static string GetFreshTimeOut()
	{
		return GetString("freshFreeCoolDown");
	}

	public static int GetFreshStartTimeOut()
	{
		return GetInt("freshFreeStartCoolDown");
	}

	public static void SetFreshTimeOut(string _date)
	{
		SaveString("freshFreeCoolDown", _date);
	}

	public static int GetPathVersion()
	{
		return GetInt("pathVersion");
	}

	public static string GetLastLoginTime()
	{
		return GetString("LastLogin");
	}

	public static bool GetMuteSoundFX()
	{
		return GetBool("soundMute", false);
	}

	public static bool GetMuteMusic()
	{
		return GetBool("soundMuteMusic", false);
	}

	public static bool GetLefty()
	{
		return GetBool("Lefty", false);
	}

	public static bool GetEveryplayEnabled()
	{
		return GetBool("EveryplayEnabled", true);
	}

	public static string GetFacebookId()
	{
		return GetString("FacebookId");
	}

	public static string GetUserName()
	{
		string text = GetString("UserName");
		string userName = KeyChain.GetUserName();
		if (userName != string.Empty && text != userName)
		{
			SaveString("UserName", userName);
			return userName;
		}
		return text;
	}

	public static string GetUserId()
	{
		string text = GetString("UserID");
		string userId = KeyChain.GetUserId();
		if (userId != string.Empty && text != userId)
		{
			SaveString("UserID", userId);
			return userId;
		}
		return text;
	}

	public static int GetItemDbVersion()
	{
		return PlayerPrefs.GetInt("itemDb");
	}

	public static string GetGameCenterId()
	{
		return GetString("GameCenterId");
	}

	public static string GetDeviceToken()
	{
		return GetString("deviceToken");
	}

	public static void SetDeviceToken(string token)
	{
		SaveString("deviceToken", token);
	}

	public static string GetMetricsGUID()
	{
		return GetString("MetricsGUID");
	}

	public static void SetMetricsGUID(string guid)
	{
		SaveString("MetricsGUID", guid);
	}

	public static void SetLastSync(string _lastLogin)
	{
		SaveString("LastSync", _lastLogin);
	}

	public static string GetLastSync()
	{
		return GetString("LastSync");
	}

	public static string[] GetNewItems()
	{
		string text = GetString("newItems");
		string[] result = new string[0];
		if (!string.IsNullOrEmpty(text))
		{
			result = text.Split(',');
		}
		return result;
	}

	public static void SetNewItems(string[] _items)
	{
		string text = string.Empty;
		for (int i = 0; i < _items.Length; i++)
		{
			text += _items[i];
			if (i < _items.Length - 1)
			{
				text += ",";
			}
		}
		SaveString("newItems", text);
	}

	private static void SaveString(string name, string value)
	{
		PlayerPrefs.SetString(name, value);
		PlayerPrefs.Save();
	}

	private static void SaveInt(string name, int value)
	{
		PlayerPrefs.SetInt(name, value);
		PlayerPrefs.Save();
	}

	private static int GetInt(string name)
	{
		if (PlayerPrefs.HasKey(name))
		{
			return PlayerPrefs.GetInt(name);
		}
		return 0;
	}

	private static string GetString(string key)
	{
		string text = PlayerPrefs.GetString(key);
		if (text.Equals(string.Empty))
		{
			return null;
		}
		return text;
	}

	private static void SetBool(string _name, bool _booleanValue)
	{
		PlayerPrefs.SetInt(_name, _booleanValue ? 1 : 0);
		PlayerPrefs.Save();
	}

	private static bool GetBool(string _name)
	{
		return PlayerPrefs.GetInt(_name) == 1;
	}

	private static bool GetBool(string name, bool defaultValue)
	{
		if (PlayerPrefs.HasKey(name))
		{
			return GetBool(name);
		}
		return defaultValue;
	}
}
