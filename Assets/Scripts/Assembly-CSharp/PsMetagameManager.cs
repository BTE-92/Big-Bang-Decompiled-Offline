using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InAppPurchases;
using Server;
using UnityEngine;

public static class PsMetagameManager
{
	public class SaveRatingData
	{
		public PsRating m_rating;

		public string m_minigameId;

		public string m_minigameContext;

		public string m_minigameName;

		public string m_minigameGameMode;

		public bool m_playerReachedGoal;

		public int m_playerLevel;

		public string m_playerUnitName;

		public Hashtable setData;
	}

	public struct CommentInfo
	{
		public string gameId;

		public string comment;

		public string tag;

		public string tournamentId;
	}

	public static Entity m_utilityEntity;

	public static int m_coinsToDiamondsConvertAmount;

	public static double m_lastKeyUsedTime;

	public static int m_keyReloadTimeSeconds;

	public static int m_keyReloadTimeLeft;

	public static int m_keyReloadTimeLeftPrevious;

	public static bool m_keyReloadTimeUpdated;

	public static PlayerStats m_playerStats = new PlayerStats();

	public static PsUIMainMenuResources m_menuResourceView;

	public static bool m_initialized;

	public static List<PsTournamentMetaData> m_dailyTournaments = new List<PsTournamentMetaData>();

	public static List<PsTournamentMetaData> m_activeTournaments = new List<PsTournamentMetaData>();

	public static List<PsChallengeMetaData> m_dailyChallenges;

	public static PsMinigameMetaData m_freshMetadata;

	public static bool m_levelUpPopup = false;

	public static List<ServerQueueItem> m_serverQueueItems = new List<ServerQueueItem>();

	public static int m_queueWaitTicks = 0;

	public static PsUIPurchaseWaitPopup m_purchaseWaitPopup;

	public static Friends m_friends;

	public static Action<Friends> m_loadFriendsCallback = null;

	public static bool m_loadingFriends = false;

	public static List<PlayerData> m_followees;

	public static List<PlayerData> m_followers;

	public static ITutorial m_tutorialArrow;

	public static string m_tutorialKey = null;

	private static bool m_firstLevelComplete = false;

	public static bool m_globalCoinValueBoost = false;

	public static PsUIBasePopup m_splashBG;

	public static readonly int m_rateAppNodeID = 26;

	public static readonly int m_rateRemindTimes = 1;

	private static bool m_rateAlertShownInThisSession = false;

	public static void Initialize()
	{
		m_utilityEntity = EntityManager.AddEntity("MetagameManagerUtility");
		m_utilityEntity.m_persistent = true;
		m_keyReloadTimeSeconds = GetSecondsFromTimeString(GetKeyReloadTime());
		m_dailyChallenges = new List<PsChallengeMetaData>();
		m_initialized = true;
	}

	public static void ShowResources(Camera _camera = null, bool _horizontal = false, bool _coins = true, bool _diamonds = true, bool _keys = true, bool _bolts = true, float _topMargin = 0.03f, bool _showShopButtons = false, bool _tickets = false)
	{
		if (m_menuResourceView != null)
		{
			HideResources();
		}
		m_menuResourceView = new PsUIMainMenuResources(_camera, _horizontal, _coins, _diamonds, _keys, _bolts, _topMargin, _showShopButtons, _tickets);
		m_menuResourceView.SetAlign(1f, 1f);
		m_menuResourceView.Update();
	}

	public static void CreateSplashBG()
	{
		if (m_splashBG == null)
		{
			m_splashBG = new PsUIBasePopup(typeof(PsSplashScreen), null, null, null, true, false);
			EntityManager.RemoveAllTagsFromEntity(m_splashBG.m_TC.p_entity);
			m_splashBG.m_TC.p_entity.m_persistent = true;
		}
	}

	public static void DestroySplashBG()
	{
		if (m_splashBG != null)
		{
			m_splashBG.Destroy();
			m_splashBG = null;
		}
	}

	public static void HideResources()
	{
		if (m_menuResourceView != null)
		{
			m_menuResourceView.Destroy();
			m_menuResourceView = null;
		}
	}

	public static void Update()
	{
		if (ServerManager.m_playerAuthenticated && m_initialized && !HttpS.m_gatesFreeze)
		{
			m_queueWaitTicks--;
			if (ServerManager.m_httpcErrorList.Count > 0)
			{
				for (int num = ServerManager.m_httpcErrorList.Count - 1; num >= 0; num--)
				{
					if (ServerManager.m_httpcErrorList[num].executed)
					{
						ServerManager.m_httpcErrorList.Remove(ServerManager.m_httpcErrorList[num]);
					}
				}
				if (Main.m_gameTicks % 60 == 0 || ServerManager.m_httpcErrorList.Count == 0)
				{
					Debug.LogWarning("httpc error count: " + ServerManager.m_httpcErrorList.Count);
				}
				if (ServerManager.m_httpcErrorList.Count == 0)
				{
					ServerManager.RemoveConnectingPopup();
				}
			}
			if (m_queueWaitTicks <= 0)
			{
				for (int i = 0; i < m_serverQueueItems.Count; i++)
				{
					if (!m_serverQueueItems[i].done)
					{
						m_serverQueueItems[i].Update();
					}
					if (m_serverQueueItems[i].done)
					{
						m_serverQueueItems[i] = null;
						m_serverQueueItems.Remove(m_serverQueueItems[i]);
						i--;
					}
				}
				m_queueWaitTicks = 0;
			}
		}
		if (m_tutorialArrow != null)
		{
			m_tutorialArrow.Step();
		}
		if (!m_initialized || !ServerManager.m_playerAuthenticated || PsState.m_inLoginFlow || Main.m_currentGame.m_currentScene.m_name == "StartupScene" || Main.m_currentGame.m_currentScene.m_name == "PreloadingScene")
		{
			return;
		}
		if (m_playerStats.keys < m_playerStats.maxKeys)
		{
			m_keyReloadTimeUpdated = false;
			m_keyReloadTimeLeft = (int)Math.Ceiling(m_lastKeyUsedTime + (double)m_keyReloadTimeSeconds - Main.m_EPOCHSeconds);
			if (m_keyReloadTimeLeft != m_keyReloadTimeLeftPrevious)
			{
				if (m_keyReloadTimeLeft <= 0)
				{
					AnnounceKeyReload();
				}
				m_keyReloadTimeUpdated = true;
				m_keyReloadTimeLeftPrevious = m_keyReloadTimeLeft;
			}
		}
		if (!Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.P))
		{
		}
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.K))
		{
			UseKeyForLevelUnlock(m_playerStats.keys);
		}
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.R))
		{
			Hashtable data = new Hashtable();
			UpdateResourceStats(10000000, 1000, 100, 10);
			SetPlayerData(data, false, PlayerDataSetSUCCEED, PlayerDataSetFAILED);
		}
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.Y))
		{
			new PsUITutorialArrowNodeSide();
		}
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.T))
		{
			Debug.LogError("creating FreshnFree");
			PsPlanetManager.GetCurrentPlanet().m_floaterManager.CreateFreshAndFreePlanet();
		}
		if (Input.GetKeyDown(KeyCode.RightShift) && PsState.m_activeMinigame != null && PsState.m_activeMinigame.m_gameStarted && !PsState.m_activeMinigame.m_gameEnded)
		{
			PsState.m_activeGameLoop.RestartMinigame();
		}
	}

	public static void AnnounceKeyReload()
	{
		int amount = 1;
		Hashtable hashtable = new Hashtable();
		m_playerStats.CumulateKeys(amount);
		if (m_playerStats.keys < m_playerStats.maxKeys)
		{
			m_keyReloadTimeSeconds = GetSecondsFromTimeString(GetKeyReloadTime());
			m_keyReloadTimeLeftPrevious = -1;
			m_keyReloadTimeLeft = m_keyReloadTimeSeconds;
			m_lastKeyUsedTime = Main.m_EPOCHSeconds;
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add("keyRefresh", m_keyReloadTimeLeft);
			hashtable.Add("Resources", hashtable2);
			PsMetrics.GetKeys(amount.ToString(), "clientKeyReload");
			SetPlayerData(hashtable, false, PlayerDataSetSUCCEED, PlayerDataSetFAILED);
		}
		else
		{
			SetPlayerData(hashtable, false, PurchaseKeysSUCCEED, PlayerDataSetFAILED);
		}
		Debug.Log("KEY RELOADED: " + m_playerStats.keys);
	}

	public static bool IsFreshLevelAvailable()
	{
		return GetFreshLevelCount() > 0 || GetFreshTimeOutSeconds() < 0;
	}

	public static void FreshLevelIsCreated()
	{
		int freshLevelCount = GetFreshLevelCount();
		if (GetFreshTimeOutSeconds() < 0)
		{
			SetFreshLevelCount(PlayerPrefsX.GetFreshStartCount() - 1);
		}
		else if (freshLevelCount > 0)
		{
			SetFreshLevelCount(freshLevelCount - 1);
		}
		SetFreshLevelTimeOut();
	}

	public static int GetFreshLevelCount()
	{
		int num = PlayerPrefsX.GetFreshCount();
		if (num == -1)
		{
			num = PlayerPrefsX.GetFreshStartCount();
			SetFreshLevelCount(num);
		}
		return num;
	}

	public static void SetFreshLevelCount(int _count)
	{
		PlayerPrefsX.SetFreshCount(_count);
	}

	public static int GetFreshTimeOutSeconds()
	{
		string freshTimeOut = PlayerPrefsX.GetFreshTimeOut();
		int result;
		if (freshTimeOut != null)
		{
			result = Convert.ToInt32(DateTime.ParseExact(freshTimeOut, "O", CultureInfo.InvariantCulture).Subtract(DateTime.Now).TotalSeconds);
		}
		else
		{
			result = PlayerPrefsX.GetFreshStartTimeOut();
			SetFreshLevelTimeOut();
		}
		return result;
	}

	public static void SetFreshLevelTimeOut()
	{
		int freshStartTimeOut = PlayerPrefsX.GetFreshStartTimeOut();
		PlayerPrefsX.SetFreshTimeOut(DateTime.Now.AddSeconds(freshStartTimeOut).ToString("O"));
	}

	public static void VersusChallengeRewards(bool _win)
	{
		if (_win)
		{
			if (m_playerStats.rank >= PsState.m_versusRankCap)
			{
				if (m_playerStats.cups < 3)
				{
					m_playerStats.cups++;
				}
				return;
			}
			m_playerStats.cups++;
			if (m_playerStats.cups > 3)
			{
				m_playerStats.rank++;
				m_playerStats.cups -= 3;
			}
		}
		else if (m_playerStats.rank > 5)
		{
			m_playerStats.cups--;
			if (m_playerStats.cups < 0)
			{
				m_playerStats.rank--;
				m_playerStats.cups = 2;
			}
		}
	}

	public static void AddNewEditorItems(List<PsUnlockable> _unlockables)
	{
		string[] second = new string[0];
		if (_unlockables != null)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < _unlockables.Count; i++)
			{
				if (_unlockables[i].m_container.m_type == PsUnlockableType.Unit || _unlockables[i].m_container.m_type == PsUnlockableType.GameMaterial)
				{
					list.Add(_unlockables[i].m_identifier);
				}
			}
			second = list.ToArray();
		}
		string[] newItems = PlayerPrefsX.GetNewItems();
		string[] array = newItems.Union(second).ToArray();
		PlayerPrefsX.SetNewItems(array);
		PsState.m_newEditorItems = array;
		PsState.m_newEditorItemCount = array.Length;
	}

	public static void RemoveNewEditorItem(string _identifier)
	{
		string[] newItems = PlayerPrefsX.GetNewItems();
		List<string> list = new List<string>();
		for (int i = 0; i < newItems.Length; i++)
		{
			if (newItems[i] != _identifier)
			{
				list.Add(newItems[i]);
			}
		}
		PlayerPrefsX.SetNewItems(list.ToArray());
		PsState.m_newEditorItems = list.ToArray();
		PsState.m_newEditorItemCount = list.Count;
	}

	public static void InitNewEditorItems()
	{
		PsState.m_newEditorItemCount = (PsState.m_newEditorItems = PlayerPrefsX.GetNewItems()).Length;
	}

	public static void GiveGoalReward(int _stars, int _diamonds, PsGameDifficulty _difficulty)
	{
		int goalCoinReward = GetGoalCoinReward(_stars, _difficulty);
		Debug.Log("REWARD FOR " + _stars + " STARS: " + goalCoinReward);
		UpdateResourceStats(goalCoinReward, _diamonds, _stars, 0);
	}

	public static int GetGoalCoinReward(int _score, PsGameDifficulty _difficulty)
	{
		int num = 1;
		return 5 * _score * num * Mathf.Min(m_playerStats.level, 40);
	}

	public static int GetSecondsFromTimeString(string _timeString)
	{
		int num = Mathf.Max(0, _timeString.IndexOf("h"));
		int num2 = Mathf.Max(0, _timeString.IndexOf("m"));
		int num3 = Mathf.Max(0, _timeString.IndexOf("s"));
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		if (num > 0)
		{
			num4 = int.Parse(_timeString.Substring(0, num));
			num7 = 2;
		}
		if (num2 > 0)
		{
			num5 = int.Parse(_timeString.Substring(num + num7, num2 - (num + num7)));
			num7 = 2;
		}
		if (num3 > 0)
		{
			num6 = int.Parse(_timeString.Substring(num2 + num7, num3 - (num2 + num7)));
		}
		return num4 * 60 * 60 + num5 * 60 + num6;
	}

	public static string GetTimeStringFromSeconds(int _seconds, bool _space = true)
	{
		int num = _seconds;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		string text = ((!_space) ? string.Empty : " ");
		num2 = Mathf.FloorToInt((float)_seconds / 60f);
		if (num2 > 0)
		{
			num = _seconds - num2 * 60;
			num3 = Mathf.FloorToInt((float)num2 / 60f);
			if (num3 > 0)
			{
				num2 -= num3 * 60;
				num4 = Mathf.FloorToInt((float)num3 / 24f);
				if (num4 > 0)
				{
					num3 -= num4 * 24;
					return num4 + "d" + text + num3 + "h";
				}
				return num3 + "h" + text + num2 + "m";
			}
			return num2 + "m" + text + num + "s";
		}
		return num + "s";
	}

	public static void UseKeyForVehicleRent(string _gameId, int _price = 1, bool _sendToServer = true)
	{
		if (m_playerStats.keys >= _price)
		{
			m_keyReloadTimeLeftPrevious = -1;
			m_keyReloadTimeSeconds = GetSecondsFromTimeString(GetKeyReloadTime());
			m_keyReloadTimeLeft = m_keyReloadTimeSeconds;
			m_lastKeyUsedTime = Main.m_EPOCHSeconds;
			m_playerStats.CumulateKeys(-_price);
			if (m_playerStats.keys <= 0)
			{
				PsMetrics.OutOfKeys();
			}
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add("keyRefresh", m_keyReloadTimeLeft);
			hashtable.Add("Resources", hashtable2);
			PsMetrics.UseKeys(_price.ToString(), "vehicleRent");
			if (_sendToServer)
			{
				SetPlayerData(new Hashtable(), false, PlayerDataSetSUCCEED, PlayerDataSetFAILED);
			}
			Debug.Log("KEY USED ######################## left: " + m_playerStats.keys);
		}
	}

	public static void UseGemsForVehicleRent(int _price, bool _sendToServer = true)
	{
		if (m_playerStats.diamonds >= _price)
		{
			m_playerStats.CumulateDiamonds(-_price);
			if (m_playerStats.keys <= 0)
			{
				PsMetrics.OutOfKeys();
			}
			PsMetrics.UseKeys(_price.ToString(), "vehicleRent");
			if (_sendToServer)
			{
				SetPlayerData(new Hashtable(), false, PlayerDataSetSUCCEED, PlayerDataSetFAILED);
			}
			Debug.Log("KEY USED ######################## left: " + m_playerStats.keys);
		}
	}

	public static void UseKeyForLevelUnlock(int _amount, bool _sendToServer = true)
	{
		if (m_playerStats.keys >= _amount)
		{
			m_keyReloadTimeLeftPrevious = -1;
			m_keyReloadTimeSeconds = GetSecondsFromTimeString(GetKeyReloadTime());
			m_keyReloadTimeLeft = m_keyReloadTimeSeconds;
			m_lastKeyUsedTime = Main.m_EPOCHSeconds;
			m_playerStats.CumulateKeys(-_amount);
			if (m_playerStats.keys <= 0)
			{
				PsMetrics.OutOfKeys();
			}
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add("keyRefresh", m_keyReloadTimeLeft);
			hashtable.Add("Resources", hashtable2);
			if (_sendToServer)
			{
				SetPlayerData(hashtable, false, PlayerDataSetSUCCEED, PlayerDataSetFAILED);
			}
			Debug.Log("KEY USED ######################## left: " + m_playerStats.keys);
		}
	}

	public static string GetKeyReloadTime()
	{
		int carRefreshMinutes = PlayerPrefsX.GetClientConfig().carRefreshMinutes;
		if (carRefreshMinutes > 0)
		{
			return carRefreshMinutes + "m";
		}
		return "6m";
	}

	public static int GetKeyReloadPrice()
	{
		int timeInSeconds = m_keyReloadTimeLeft + m_keyReloadTimeSeconds * (m_playerStats.maxKeys - 1);
		return SecondsToDiamonds(timeInSeconds);
	}

	public static int GetKeyIncreasePrice(int _newMax)
	{
		return (_newMax - PlayerPrefsX.GetClientConfig().keysAtStart) * 40 + 40;
	}

	public static bool RefreshKeysFromAd(int _getKeys, int _useKeys = 0, bool _sendToServer = true)
	{
		Hashtable hashtable = new Hashtable();
		if (_useKeys > 0)
		{
			m_keyReloadTimeLeftPrevious = -1;
			m_keyReloadTimeSeconds = GetSecondsFromTimeString(GetKeyReloadTime());
			m_keyReloadTimeLeft = m_keyReloadTimeSeconds;
			m_lastKeyUsedTime = Main.m_EPOCHSeconds;
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add("keyRefresh", m_keyReloadTimeLeft);
			hashtable.Add("Resources", hashtable2);
		}
		int amount = _getKeys - _useKeys;
		m_playerStats.CumulateKeys(amount);
		PsMetrics.GetKeys(amount.ToString(), "clientKeyAdRefresh");
		Debug.Log("PURCHASED " + _getKeys + ", used immediately: " + _useKeys);
		if (_sendToServer)
		{
			SetPlayerData(hashtable, false, FillKeysSUCCEED, PlayerDataSetFAILED);
		}
		return true;
	}

	public static bool PurchaseKeys(int _useKeys = 0, bool _sendToServer = true)
	{
		if (m_playerStats.diamonds >= GetKeyReloadPrice())
		{
			Hashtable hashtable = new Hashtable();
			if (_useKeys > 0)
			{
				m_keyReloadTimeLeftPrevious = -1;
				m_keyReloadTimeSeconds = GetSecondsFromTimeString(GetKeyReloadTime());
				m_keyReloadTimeLeft = m_keyReloadTimeSeconds;
				m_lastKeyUsedTime = Main.m_EPOCHSeconds;
				Hashtable hashtable2 = new Hashtable();
				hashtable2.Add("keyRefresh", m_keyReloadTimeLeft);
				hashtable.Add("Resources", hashtable2);
			}
			int amount = m_playerStats.maxKeys - m_playerStats.keys - _useKeys;
			m_playerStats.CumulateKeys(amount);
			m_playerStats.CumulateDiamonds(-GetKeyReloadPrice());
			Debug.Log("PURCHASED " + (m_playerStats.maxKeys - _useKeys) + ", used immediately: " + _useKeys);
			PsMetrics.GetKeys(amount.ToString(), "keysPurchased");
			PsMetrics.UseDiamonds(GetKeyReloadPrice().ToString(), "keyRefresh");
			if (_sendToServer)
			{
				SetPlayerData(hashtable, false, PurchaseKeysSUCCEED, PlayerDataSetFAILED);
			}
			return true;
		}
		return false;
	}

	public static bool IncreaseKeys(int _useKeys = 0, bool _sendToServer = true)
	{
		int keyIncreasePrice = GetKeyIncreasePrice(m_playerStats.maxKeys + 1);
		if (m_playerStats.diamonds >= keyIncreasePrice)
		{
			Hashtable hashtable = new Hashtable();
			if (_useKeys > 0)
			{
				m_keyReloadTimeLeftPrevious = -1;
				m_keyReloadTimeSeconds = GetSecondsFromTimeString(GetKeyReloadTime());
				m_keyReloadTimeLeft = m_keyReloadTimeSeconds;
				m_lastKeyUsedTime = Main.m_EPOCHSeconds;
				Hashtable hashtable2 = new Hashtable();
				hashtable2.Add("keyRefresh", m_keyReloadTimeLeft);
				hashtable.Add("Resources", hashtable2);
			}
			m_playerStats.maxKeys++;
			m_playerStats.CumulateKeys(m_playerStats.maxKeys - m_playerStats.keys - _useKeys);
			m_playerStats.CumulateDiamonds(-keyIncreasePrice);
			PsMetrics.UseDiamonds(GetKeyReloadPrice().ToString(), "keyIncrease");
			Debug.Log("PURCHASED " + (m_playerStats.maxKeys - _useKeys) + ", used immediately: " + _useKeys);
			if (_sendToServer)
			{
				SetPlayerData(hashtable, false, PurchaseKeysSUCCEED, PlayerDataSetFAILED);
			}
			return true;
		}
		return false;
	}

	public static HttpC GetUnlockerTimeLeftFromServer()
	{
		return Player.GetSkipTimerValue(delegate(HttpC _c)
		{
			Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_c.www.text);
			PlayerPrefsX.SetSkipTimeLeft((int)Math.Ceiling(Main.m_EPOCHSeconds + (double)Convert.ToInt32(dictionary["skipTimerLeft"])));
		}, delegate(HttpC _c)
		{
			Debug.Log("GETTING SKIP LEVEL TIME LEFT FROM SERVER FAILED");
			Hashtable hashtable = _c.objectData as Hashtable;
			ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), GetUnlockerTimeLeftFromServer);
		});
	}

	public static HttpC ResetUnlockerTime()
	{
		return Player.ResetSkipTimer(delegate
		{
			Debug.Log("Unlocker timer reset SUCCEED");
			GetUnlockerTimeLeftFromServer();
		}, delegate(HttpC _c)
		{
			Debug.Log("RESETTING SKIP LEVEL TIMER FAILED");
			Hashtable hashtable = _c.objectData as Hashtable;
			ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), ResetUnlockerTime);
		});
	}

	public static int GetUnlockerTimeLeft()
	{
		return (int)Math.Ceiling((double)PlayerPrefsX.GetSkipTimeLeft() - Main.m_EPOCHSeconds);
	}

	public static void ResetCoinBoostTime()
	{
		float num = PsState.m_coinValueBoosterSecs;
		Debug.Log("DOUBLED COIN VALUE FOR " + num + " secs.");
		PlayerPrefsX.SetCoinBoostTimeLeft((int)Math.Ceiling(Main.m_EPOCHSeconds + (double)num));
	}

	public static int GetCoinBoostTimeLeft()
	{
		return (int)Math.Ceiling((double)PlayerPrefsX.GetCoinBoostTimeLeft() - Main.m_EPOCHSeconds);
	}

	public static void UpdateCoinValueBooster()
	{
		bool flag = GetCoinBoostTimeLeft() > 0;
		if (flag == m_globalCoinValueBoost)
		{
			return;
		}
		m_globalCoinValueBoost = flag;
		if (!flag)
		{
			return;
		}
		List<Entity> entitiesByTag = EntityManager.GetEntitiesByTag("GTAG_COIN");
		if (entitiesByTag.Count <= 0)
		{
			return;
		}
		foreach (Entity item in entitiesByTag)
		{
			CustomObjectC customObjectC = EntityManager.GetComponentByEntity((ComponentType)31, item) as CustomObjectC;
			CollectibleCoin collectibleCoin = customObjectC.m_customObject as CollectibleCoin;
			collectibleCoin.SetCoinValueBoosterFX();
		}
	}

	public static bool PurchaseStars(int _stars, bool _sendToServer = true)
	{
		if (m_playerStats.diamonds >= StarsToDiamonds(_stars))
		{
			m_playerStats.CumulateDiamonds(-StarsToDiamonds(_stars));
			PsMetrics.UseDiamonds(StarsToDiamonds(_stars).ToString(), "unlockCheckpoint");
			if (_sendToServer)
			{
				SetPlayerData(new Hashtable(), false, PlayerDataSetSUCCEED, PlayerDataSetFAILED);
			}
			return true;
		}
		return false;
	}

	public static void ParticipateTournament(PsTournamentMetaData _tournamentMetaData)
	{
		_tournamentMetaData.participated = true;
		m_playerStats.CumulateCoins(-_tournamentMetaData.participationCoinsCost);
		m_playerStats.CumulateDiamonds(-_tournamentMetaData.participationDiamondsCost);
		PsServerRequest.ServerParticipateTournament(_tournamentMetaData, ParticipateTournamentSUCCEED, ParticipateTournamentFAILED);
	}

	public static void ClaimTournament(PsTournamentMetaData _tournamentMetaData, Vector2 _startPos)
	{
		Debug.Log("CLAIMING TOURNAMENT");
		_tournamentMetaData.claimed = true;
		if (_tournamentMetaData.playerRewardCoins > 0)
		{
			m_playerStats.CumulateCoinsWithFlyingResources(_tournamentMetaData.playerRewardCoins, _startPos);
		}
		else if (_tournamentMetaData.playerRewardDiamonds > 0)
		{
			m_playerStats.CumulateDiamondsWithFlyingResources(_tournamentMetaData.playerRewardDiamonds, _startPos);
		}
		PsServerRequest.ServerClaimTournament(_tournamentMetaData, ClaimTournamentSUCCEED, ClaimTournamentFAILED);
	}

	public static void ClaimMinigameReward(PsMinigameMetaData _metaData, Vector2 _buttonPos)
	{
		Debug.Log("CLAIMING MINIGAME REWARD");
		m_playerStats.CumulateCoinsWithFlyingResources(_metaData.rewardCoins, _buttonPos);
		PsServerRequest.ServerClaimMinigame(_metaData, ClaimMinigameSUCCEED, ClaimMinigameFAILED);
	}

	public static void ClaimAllMinigames(int _amount, Vector2 _buttonPos)
	{
		Debug.Log("CLAIMING ALL MINIGAMES");
		m_playerStats.CumulateCoinsWithFlyingResources(_amount, _buttonPos);
		PsServerRequest.ServerClaimAllMinigames(ClaimAllMinigamesSUCCEED, ClaimAllMinigamesFAILED);
	}

	public static int SecondsToDiamonds(int timeInSeconds)
	{
		int[] array = new int[4] { 60, 3600, 86400, 604800 };
		int[] array2 = new int[4] { 1, 35, 200, 600 };
		if (timeInSeconds <= 0)
		{
			return 0;
		}
		if (timeInSeconds <= array[0])
		{
			return array2[0];
		}
		for (int i = 1; i < array.Length - 1; i++)
		{
			if (timeInSeconds <= array[i])
			{
				return (int)((float)(timeInSeconds - array[i - 1]) / ((float)(array[i] - array[i - 1]) / (float)(array2[i] - array2[i - 1])) + (float)array2[i - 1]);
			}
		}
		return (int)((float)(timeInSeconds - array[array.Length - 2]) / ((float)(array[array.Length - 1] - array[array.Length - 2]) / (float)(array2[array2.Length - 1] - array2[array2.Length - 2])) + (float)array2[array2.Length - 2]);
	}

	public static int CoinsToDiamonds(int _coinAmount)
	{
		int[] array = new int[6] { 10, 1000, 10000, 100000, 1000000, 10000000 };
		int[] array2 = new int[6] { 1, 50, 250, 1250, 6250, 62500 };
		if (_coinAmount <= 0)
		{
			return 0;
		}
		if (_coinAmount <= array[0])
		{
			return array2[0];
		}
		for (int i = 1; i < array.Length - 1; i++)
		{
			if (_coinAmount <= array[i])
			{
				return (int)((float)(_coinAmount - array[i - 1]) / ((float)(array[i] - array[i - 1]) / (float)(array2[i] - array2[i - 1])) + (float)array2[i - 1]);
			}
		}
		return (int)((float)(_coinAmount - array[array.Length - 2]) / ((float)(array[array.Length - 1] - array[array.Length - 2]) / (float)(array2[array2.Length - 1] - array2[array2.Length - 2])) + (float)array2[array2.Length - 2]);
	}

	public static int StarsToDiamonds(int _starAmount)
	{
		int[] array = new int[7] { 1, 3, 6, 10, 20, 60, 200 };
		int[] array2 = new int[7] { 25, 60, 100, 140, 250, 600, 1000 };
		if (_starAmount <= 0)
		{
			return 0;
		}
		if (_starAmount <= array[0])
		{
			return array2[0];
		}
		for (int i = 1; i < array.Length - 1; i++)
		{
			if (_starAmount <= array[i])
			{
				return (int)((float)(_starAmount - array[i - 1]) / ((float)(array[i] - array[i - 1]) / (float)(array2[i] - array2[i - 1])) + (float)array2[i - 1]);
			}
		}
		return (int)((float)(_starAmount - array[array.Length - 2]) / ((float)(array[array.Length - 1] - array[array.Length - 2]) / (float)(array2[array2.Length - 1] - array2[array2.Length - 2])) + (float)array2[array2.Length - 2]);
	}

	public static void Purchase(string _identifier, Action<string> _okCallback, Action<string, string, IAP.TransactionResult> _errorCallback)
	{
		Debug.Log("IAP: Start purchasing " + _identifier);
		PsIAPManager.PurchaseConsumable(_identifier, delegate(string id, IAP.TransactionResult result, string error, string transactionId)
		{
			FinishPurchase(id, result, error, transactionId, _okCallback, _errorCallback);
		});
	}

	private static void FinishPurchase(string _identifier, IAP.TransactionResult _result, string _error, string _transactionId, Action<string> _okCallback, Action<string, string, IAP.TransactionResult> _errorCallback)
	{
		if (_result == IAP.TransactionResult.Success && !string.IsNullOrEmpty(_identifier))
		{
			Debug.Log("IAP: Finishing purchase of " + _identifier);
			ServerProduct serverProductById = PsIAPManager.GetServerProductById(_identifier);
			IAPProduct iAPProductById = PsIAPManager.GetIAPProductById(_identifier);
			if (serverProductById != null)
			{
				ShowFlyingDiamonds(serverProductById.amount);
				PsMetrics.PurchaseDiamonds(serverProductById.amount.ToString(), "shop");
				if (iAPProductById != null)
				{
					Debug.Log("IAP: Send revenue event");
					PsMetrics.ValidatedPurchase(serverProductById.amount.ToString(), iAPProductById.price, iAPProductById.currencyCode, _transactionId);
				}
				_okCallback("Purchase successful");
			}
			else
			{
				_errorCallback("Product not found", null, _result);
			}
		}
		else
		{
			_errorCallback(_error, _identifier, _result);
		}
	}

	private static void ShowFlyingDiamonds(int _amount)
	{
		if (m_menuResourceView != null && m_menuResourceView.m_showDiamonds)
		{
			m_menuResourceView.CreateAddedResources(ResourceType.Diamonds, _amount, 0f);
		}
	}

	public static void ShowRateAppIfPossible(PsGameLoop _gameLoop)
	{
		int rateStatus = KeyChain.GetRatingStatus();
		if (rateStatus > m_rateRemindTimes || rateStatus < 0 || m_rateAlertShownInThisSession)
		{
			return;
		}
		int levelNumber = PsPlanetManager.GetCurrentPlanet().m_mainPath.GetCurrentNodeInfo().m_levelNumber;
		if (_gameLoop != null && levelNumber >= m_rateAppNodeID && _gameLoop.m_scoreBest == 3 && _gameLoop.m_scoreOld < 3)
		{
			Action action = delegate
			{
				KeyChain.SetRatingStatus(rateStatus + 1);
			};
			HelpshiftManager.ShowAlertToRate("com.traplight.bigbangracing", delegate
			{
				KeyChain.SetRatingStatus(-1);
			}, action, action);
			m_rateAlertShownInThisSession = true;
		}
	}

	public static void UpdateResourceStats(int _coins, int _diamonds, int _stars, int _bolts)
	{
		m_playerStats.coins += _coins;
		m_playerStats.diamonds += _diamonds;
		m_playerStats.stars += _stars;
		m_playerStats.bolts += _bolts;
		m_playerStats.updated = true;
	}

	public static void SendRating(PsRating _newRating, PsGameLoop _minigameInfo, Minigame _minigame)
	{
		Hashtable hashtable = new Hashtable();
		if (_newRating != _minigameInfo.GetRating())
		{
			_minigameInfo.SetRating(_newRating);
			SaveRatingData saveRatingData = new SaveRatingData();
			saveRatingData.m_rating = _newRating;
			saveRatingData.m_minigameId = _minigameInfo.GetGameId();
			saveRatingData.m_minigameName = _minigameInfo.GetName();
			saveRatingData.m_minigameContext = _minigameInfo.m_context.ToString();
			saveRatingData.m_minigameGameMode = _minigameInfo.GetGameMode().ToString();
			saveRatingData.m_playerReachedGoal = _minigame.m_playerReachedGoal;
			saveRatingData.m_playerUnitName = _minigame.m_playerUnitName;
			saveRatingData.m_playerLevel = m_playerStats.level;
			if (hashtable.Count > 0)
			{
				saveRatingData.setData = hashtable;
			}
			PsMetrics.MinigameRated(saveRatingData, _minigameInfo.m_minigameMetaData.quality.ToString(), (_minigameInfo.m_path.m_name == "MainPath") ? _minigameInfo.m_path.m_currentNodeId : 0);
			PsServerRequest.ServerSendRating(saveRatingData, SendRatingSUCCEED, SendRatingFAILED);
			Debug.Log("SENDING RATING");
		}
	}

	public static void SetPlayerData(Hashtable _data, bool _sendMetrics, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		SetPlayerDataAndProgression(_data, null, _sendMetrics, _okCallback, _failCallback, _errorCallback);
	}

	public static void SetPlayerDataAndProgression(Hashtable _data, Hashtable _progressionJson, bool _sendMetrics)
	{
		SetPlayerDataAndProgression(_data, _progressionJson, _sendMetrics, PlayerDataAndProgressionSetSUCCEED, PlayerDataAndProgressionSetFAILED);
	}

	public static void SetPlayerDataAndProgression(Hashtable _data, Hashtable _progressionJson, bool _sendMetrics, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		if (m_playerStats.isDirty || _data.Count > 0 || _progressionJson != null)
		{
			new PsServerQueueFlow(null, delegate
			{
				PsServerRequest.ServerPlayerSetDataAndProgression(_data, _progressionJson, null, PsAchievementManager.GetUpdatedAchievements(), _sendMetrics, _okCallback, _failCallback, _errorCallback);
			}, new string[4] { "SetData", "Coins", "Diamonds", "Progression" });
			m_playerStats.SetDirty(false);
		}
	}

	public static void SetPlayer(PlayerData _player)
	{
		KeyChain.SetUserId(_player.playerId);
		PlayerPrefsX.SetUserId(_player.playerId);
		PlayerPrefsX.SetClientConfig(_player.clientConfig);
		m_playerStats.coins = _player.coins;
		m_playerStats.diamonds = _player.diamonds;
		m_playerStats.bolts = _player.bolts;
		m_playerStats.stars = _player.stars;
		m_playerStats.keys = _player.keys;
		m_playerStats.maxKeys = _player.maxKeys;
		m_playerStats.itemLevel = _player.itemLevel;
		m_playerStats.level = _player.level;
		m_playerStats.cups = _player.cups;
		m_playerStats.rank = _player.rank;
		if (_player.upgrades != null)
		{
			m_playerStats.upgrades = _player.upgrades;
		}
		if (_player.boosters != null)
		{
			m_playerStats.boosters = _player.boosters;
		}
		if (_player.data != null)
		{
			PsMetagamePlayerData.m_playerData = _player.data;
		}
		if (_player.claimedTutorials != null)
		{
			PsMetagameData.m_unlockedTutorials = _player.claimedTutorials;
		}
		m_playerStats.updated = true;
	}

	public static void SendQuit(PsGameLoop _minigameInfo)
	{
		SendQuitData sendQuitData = new SendQuitData();
		sendQuitData.startCount = PsState.m_activeMinigame.m_gameStartCount;
		if (sendQuitData.startCount >= 1)
		{
			sendQuitData.gameLoop = _minigameInfo;
			bool mainPath = _minigameInfo.m_context == PsMinigameContext.Level;
			PsServerRequest.ServerSendQuit(sendQuitData, SendQuitSUCCEED, SendQuitFAILED, null, mainPath);
		}
	}

	public static void GiveLevelTemplate(PsLevelTemplate _template, Action<PsMinigameMetaData> _succeedCallback = null)
	{
		UnityEngine.Random.seed = DateTime.Now.Millisecond;
		int index = Mathf.Clamp(UnityEngine.Random.Range(0, _template.m_templateMinigames.Count), 0, _template.m_templateMinigames.Count - 1);
		string text = _template.m_templateMinigames[index];
		Hashtable hashtable = new Hashtable();
		hashtable.Add("researchIdentifier", _template.m_identifier);
		PsGameMode psGameMode = _template.m_gameMode;
		if (_template.m_gameMode == PsGameMode.Any)
		{
			psGameMode = ((UnityEngine.Random.value >= 0.5f) ? PsGameMode.Race : PsGameMode.StarCollect);
		}
		hashtable.Add("name", Minigame.GetRandomName());
		hashtable.Add("description", Minigame.GetDefaultDescription(psGameMode));
		hashtable.Add("gameMode", psGameMode);
		if (_template.m_playerUnit != "Any")
		{
			hashtable.Add("playerUnit", _template.m_playerUnit);
		}
		if (_template.m_limitedItems != null && _template.m_limitedItems.Count > 0)
		{
			hashtable.Add("itemsUsed", _template.m_limitedItems.ToArray());
		}
		Debug.LogWarning("dublicating template level: " + text);
		if (_succeedCallback == null)
		{
			_succeedCallback = GiveLevelTemplateSUCCEED;
		}
		PsServerRequest.GiveLevelTemplate(text, hashtable, _succeedCallback, GiveLevelTemplateFAILED);
	}

	public static void GetFriends(Action<Friends> _callback, bool _forceRefresh = false)
	{
		if (!m_loadingFriends && m_friends != null && m_friends.HasContacts() && !_forceRefresh)
		{
			Debug.Log("local friends");
			_callback(m_friends);
			return;
		}
		if (m_loadFriendsCallback != null)
		{
			m_loadFriendsCallback = (Action<Friends>)Delegate.Combine(m_loadFriendsCallback, _callback);
		}
		else
		{
			m_loadFriendsCallback = _callback;
		}
		if (!m_loadingFriends)
		{
			m_loadingFriends = true;
			Debug.Log("Getting friends from server");
			PsServerRequest.ServerGetFriends(PlayerPrefsX.GetUserId(), LoadFriendsSucceed, LoadFriendsFailed);
		}
	}

	public static void LoadFriendsSucceed(Friends _data)
	{
		m_loadingFriends = false;
		if (m_loadFriendsCallback != null)
		{
			m_friends = _data;
			m_loadFriendsCallback(m_friends);
			m_loadFriendsCallback = null;
		}
	}

	public static void LoadFriendsFailed(HttpC _c)
	{
		PsServerRequest.ServerGetFriends(PlayerPrefsX.GetUserId(), LoadFriendsSucceed, LoadFriendsFailed);
	}

	public static void GetFollowees(string _playerId)
	{
		PsServerRequest.ServerGetFollowees(_playerId, FolloweesSUCCEED, FolloweesFAILED);
	}

	public static void GetFollowers(string _playerId)
	{
		PsServerRequest.ServerGetFollowers(_playerId, FollowersSUCCEED, FollowersFAILED);
	}

	public static List<PlayerData> GetFriends(List<PlayerData> _followees, List<PlayerData> _followers)
	{
		List<PlayerData> list = new List<PlayerData>();
		for (int num = _followers.Count - 1; num >= 0; num--)
		{
			for (int i = 0; i < _followees.Count; i++)
			{
				if (m_followers[num].playerId == m_followees[i].playerId)
				{
					list.Add(m_followers[num]);
				}
			}
		}
		return list;
	}

	public static void Follow(string _playerId)
	{
		PsServerRequest.ServerFollow(_playerId, FollowSUCCEED, FollowFAILED);
	}

	public static void UnFollow(string _playerId)
	{
		PsServerRequest.ServerUnFollow(_playerId, UnFollowSUCCEED, UnFollowFAILED);
	}

	public static void Comment(string _gameId, string _comment, string _tag, string _tournamentId)
	{
		PsServerRequest.ServerComment(new CommentInfo
		{
			gameId = _gameId,
			comment = _comment,
			tag = _tag,
			tournamentId = _tournamentId
		}, CommentSUCCEED, CommentFAILED);
	}

	public static void UpsertAchievement(string _json)
	{
		PsServerRequest.ServerAchievement(_json, AchievementUpsertSUCCEED, AchievementUpsertFAILED);
	}

	public static void SkipLevel(int _diamondPrice, string _playerUnit)
	{
		m_playerStats.CumulateDiamonds(-_diamondPrice);
		m_playerStats.CumulateStars(1);
		PsServerRequest.ServerPlayerSkipLevel(_playerUnit, new Hashtable(), PlayerSkipLevelSUCCEED, PlayerSkipLevelFAILED);
	}

	public static void SaveProgression(Hashtable _progressionJSON, bool _sendMetrics)
	{
		SendPathData data = new SendPathData();
		data.jsonData = _progressionJSON;
		data.sendMetrics = _sendMetrics;
		new PsServerQueueFlow(delegate
		{
			PsServerRequest.ServerSaveProgression(data, SaveProgressionSUCCEED, SaveProgressionFAILED);
		}, null, new string[1] { "Progression" });
	}

	public static void DeleteMinigame(string _minigameId)
	{
		PsServerRequest.ServerDeleteMinigame(_minigameId, DeleteMinigameSUCCEED, DeleteMinigameFAILED);
	}

	public static void GiveLevelTemplateSUCCEED(PsMinigameMetaData _metaData)
	{
		Debug.LogWarning("TEMPLATE DUPLICATE SUCCEED");
	}

	public static void GiveLevelTemplateFAILED(HttpC _c)
	{
		Debug.LogWarning("TEMPLATE DUPLICATE FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.GiveLevelTemplate((string)_c.objectData, new Hashtable(), GiveLevelTemplateSUCCEED, GiveLevelTemplateFAILED));
	}

	public static void ClaimAllMinigamesSUCCEED(HttpC _c)
	{
		Debug.Log("CLAIM ALL MINIGAMES SUCCEED");
	}

	public static void ClaimAllMinigamesFAILED(HttpC _c)
	{
		Debug.Log("CLAIM ALL MINIGAMES FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerClaimAllMinigames(ClaimAllMinigamesSUCCEED, ClaimAllMinigamesFAILED));
	}

	public static void DeleteMinigameSUCCEED(HttpC _c)
	{
		Debug.Log("DELETE MINIGAME SUCCEED");
	}

	public static void DeleteMinigameFAILED(HttpC _c)
	{
		Debug.Log("DELETE MINIGAME FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerDeleteMinigame((string)_c.objectData, DeleteMinigameSUCCEED, DeleteMinigameFAILED));
	}

	public static void ResetProgressionSUCCEED(HttpC _c)
	{
		Debug.Log("PROGRESSION RESET SUCCEED");
		PsPlanetSerializer.SaveProgressionLocally(PsPlanetManager.GetCurrentPlanet());
	}

	public static void ResetProgressionFAILED(HttpC _c)
	{
		Debug.Log("PROGRESSION RESET FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerResetProgression(ResetProgressionSUCCEED, ResetProgressionFAILED));
	}

	public static void SaveProgressionSUCCEED(HttpC _c)
	{
		Debug.Log("PROGRESSION SAVE SUCCEED");
		if (((SendPathData)_c.objectData).sendMetrics)
		{
			PsMetrics.Progression(PsPlanetManager.GetCurrentPlanet().GetMainPathCurrentNodeId());
			if (PsPlanetManager.GetCurrentPlanet().GetMainPathCurrentNodeId() == 2 && !m_firstLevelComplete)
			{
				m_firstLevelComplete = true;
				PsMetrics.Tutorial();
			}
		}
		PsPlanetSerializer.SaveProgressionLocally(PsPlanetManager.GetCurrentPlanet());
	}

	public static void SaveProgressionFAILED(HttpC _c)
	{
		Debug.Log("PROGRESSION SAVE FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerSaveProgression((SendPathData)_c.objectData, SaveProgressionSUCCEED, SaveProgressionFAILED));
	}

	public static void CommentSUCCEED(HttpC _c)
	{
		Debug.Log("COMMENT SUCCEED");
	}

	public static void CommentFAILED(HttpC _c)
	{
		Debug.Log("COMMENT FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerComment((CommentInfo)_c.objectData, CommentSUCCEED, CommentFAILED));
	}

	public static void ShopPurchaseSUCCEED(HttpC _c)
	{
		Debug.Log("SHOP PURCHASE SUCCEED");
		if (m_purchaseWaitPopup != null)
		{
			m_purchaseWaitPopup.Close();
			m_purchaseWaitPopup = null;
		}
	}

	public static void ShopPurchaseFAILED(HttpC _c)
	{
		Debug.Log("SHOP PURCHASE FAILED");
		Hashtable setData = _c.objectData as Hashtable;
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerPlayerSetData(setData, ShopPurchaseSUCCEED, ShopPurchaseFAILED));
	}

	public static void PlayerDataAndProgressionSetSUCCEED(HttpC _c)
	{
		PsPlanetSerializer.SaveProgressionLocally(PsPlanetManager.GetCurrentPlanet());
		if (((Player.ProgressionAndSetDataObject)_c.objectData).sendMetrics)
		{
			PsMetrics.Progression(PsPlanetManager.GetCurrentPlanet().GetMainPathCurrentNodeId());
		}
		Debug.Log("PLAYER DATA & PROGRESSION SET SUCCEED");
	}

	public static void PlayerDataAndProgressionSetFAILED(HttpC _c)
	{
		Debug.Log("PLAYER DATA & PROGRESSION SET FAILED");
		Player.ProgressionAndSetDataObject data = _c.objectData as Player.ProgressionAndSetDataObject;
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerPlayerSetDataAndProgression(data.setData, data.pathJson, data.customisations, data.achievements, data.sendMetrics, PlayerDataAndProgressionSetSUCCEED, PlayerDataAndProgressionSetFAILED));
	}

	public static void PlayerDataSetSUCCEED(HttpC _c)
	{
		Debug.Log("PLAYER DATA SET SUCCEED");
	}

	public static void PlayerDataSetFAILED(HttpC _c)
	{
		Debug.Log("PLAYER DATA SET FAILED");
		Hashtable setData = _c.objectData as Hashtable;
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerPlayerSetData(setData, PlayerDataSetSUCCEED, PlayerDataSetFAILED));
	}

	private static void ParticipateTournamentSUCCEED(HttpC _c)
	{
		Debug.Log("PARTICIPATE TOURNAMENT SUCCEED");
	}

	private static long GetTournamentIdHash(string _tournamentId)
	{
		long num = 0L;
		for (int i = 0; i < _tournamentId.Length; i++)
		{
			num += Convert.ToInt64(_tournamentId[i]) * 32 * (i + 1);
		}
		return num;
	}

	private static void ParticipateTournamentFAILED(HttpC _c)
	{
		Debug.Log("PARTICIPATE TOURNAMENT FAILED");
		PsTournamentMetaData data = (PsTournamentMetaData)_c.objectData;
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerParticipateTournament(data, ParticipateTournamentSUCCEED, ParticipateTournamentFAILED));
	}

	private static void ClaimTournamentSUCCEED(HttpC _c)
	{
		Debug.Log("CLAIM TOURNAMENT SUCCEED");
	}

	private static void ClaimTournamentFAILED(HttpC _c)
	{
		Debug.Log("CLAIM TOURNAMENT FAILED");
		PsTournamentMetaData data = (PsTournamentMetaData)_c.objectData;
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerClaimTournament(data, ClaimTournamentSUCCEED, ClaimTournamentFAILED));
	}

	private static void ClaimMinigameSUCCEED(HttpC _c)
	{
		Debug.Log("CLAIM MINIGAME SUCCEED");
	}

	private static void ClaimMinigameFAILED(HttpC _c)
	{
		Debug.Log("CLAIM MINIGAME FAILED");
		PsMinigameMetaData data = (PsMinigameMetaData)_c.objectData;
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerClaimMinigame(data, ClaimMinigameSUCCEED, ClaimMinigameFAILED));
	}

	private static void PurchaseKeysSUCCEED(HttpC _c)
	{
		Debug.Log("PURCHASE KEYS SUCCEED");
	}

	private static void FillKeysSUCCEED(HttpC _c)
	{
		Debug.Log("FILL KEYS SUCCEED");
	}

	private static void SendRatingSUCCEED(HttpC _c)
	{
		Debug.Log("SEND RATING SUCCEED");
	}

	private static void SendRatingFAILED(HttpC _c)
	{
		Debug.Log("Send rating FAILED");
		SaveRatingData data = (SaveRatingData)_c.objectData;
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerSendRating(data, SendRatingSUCCEED, SendRatingFAILED));
	}

	private static void UseKeySUCCEED(HttpC _c)
	{
		PsMetricsData.m_testersUsed++;
		Debug.Log("USE KEY SUCCEED");
	}

	public static void GetScreenshotERROR()
	{
		Debug.LogWarning("SCREENSHOT NOT FOUND");
	}

	public static void GetPreloadCheckERROR()
	{
		Debug.LogWarning("FILE NOT FOUND");
	}

	public static void SendQuitSUCCEED(HttpC _c)
	{
		Debug.Log("SEND QUIT SUCCEED");
	}

	public static void SendQuitFAILED(HttpC _c)
	{
		Debug.Log("SEND QUIT FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerSendQuit(_c.objectData as SendQuitData, SendQuitSUCCEED, SendQuitFAILED));
	}

	public static void FolloweesSUCCEED(PlayerData[] _data)
	{
		Debug.Log("FOLLOWEES SUCCEED");
		m_followees = new List<PlayerData>(_data);
	}

	public static void FolloweesFAILED(HttpC _c)
	{
		Debug.Log("FOLLOWEES FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerGetFollowees((string)_c.objectData, FolloweesSUCCEED, FolloweesFAILED));
	}

	public static void FollowersSUCCEED(PlayerData[] _data)
	{
		Debug.Log("LOAD FOLLOWERS SUCCEED");
		m_followers = new List<PlayerData>(_data);
	}

	public static void FollowersFAILED(HttpC _c)
	{
		Debug.Log("LOAD FOLLOWERS FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerGetFollowers((string)_c.objectData, FollowersSUCCEED, FollowersFAILED));
	}

	public static void FollowSUCCEED(HttpC _c)
	{
		Debug.Log("FOLLOW SUCCEED");
	}

	public static void FollowFAILED(HttpC _c)
	{
		Debug.Log("FOLLOW FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerFollow((string)_c.objectData, FollowSUCCEED, FollowFAILED));
	}

	public static void UnFollowSUCCEED(HttpC _c)
	{
		Debug.Log("UNFOLLOW SUCCEED");
	}

	public static void UnFollowFAILED(HttpC _c)
	{
		Debug.Log("UNFOLLOW FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerUnFollow((string)_c.objectData, UnFollowSUCCEED, UnFollowFAILED));
	}

	public static void PlayerSkipLevelSUCCEED(HttpC _c)
	{
		Debug.Log("SKIP LEVEL SUCCEED");
	}

	public static void PlayerSkipLevelFAILED(HttpC _c)
	{
		Debug.Log("SKIP LEVEL FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerPlayerSkipLevel((string)_c.objectData, new Hashtable(), PlayerSkipLevelSUCCEED, PlayerSkipLevelFAILED));
	}

	public static void AchievementUpsertSUCCEED(HttpC _c)
	{
		Debug.Log("ACHIEVEMENT UPSERT SUCCEED");
	}

	public static void AchievementUpsertFAILED(HttpC _c)
	{
		Debug.Log("ACHIEVEMENT UPSERT FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => PsServerRequest.ServerAchievement((string)_c.objectData, AchievementUpsertSUCCEED, AchievementUpsertFAILED));
	}
}
