using System;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
	public static class LoginFlow
	{
		private static bool m_debugReset;

		private static bool m_firstTimeLogin;

		public static bool m_userDataLoaded;

		private static bool m_isCompeleted;

		public static bool isCompleted
		{
			get
			{
				return m_isCompeleted;
			}
		}

		public static HttpC Start(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			if (PsState.m_openSocialPopup != null)
			{
				PsState.m_openSocialPopup.Destroy();
			}
			Debug.Log("LoginFlow Start");
			m_isCompeleted = false;
			string text = ((!string.IsNullOrEmpty(KeyChain.GetUserId())) ? KeyChain.GetUserId() : "NullOrEmpty");
			string text2 = ((!string.IsNullOrEmpty(PlayerPrefsX.GetUserId())) ? PlayerPrefsX.GetUserId() : "NullOrEmpty");
			string text3 = ((!string.IsNullOrEmpty(KeyChain.GetUserName())) ? KeyChain.GetUserName() : "NullOrEmpty");
			Debug.LogWarning("LoginFlowStart: KeyChain userid: " + text + ", PlayerPrefsX userid: " + text2 + ", KeyChain username: " + text3);
			if ((string.IsNullOrEmpty(KeyChain.GetUserId()) && string.IsNullOrEmpty(PlayerPrefsX.GetUserId())) || string.IsNullOrEmpty(KeyChain.GetUserName()))
			{
				MarkovNameGenerator markovNameGenerator = new MarkovNameGenerator(PsResources.NameGeneratorBaseStrings, 1, 3);
				string nextName = markovNameGenerator.NextName;
				PlayerPrefsX.SetUserName(nextName);
				KeyChain.SetUserName(nextName);
				m_firstTimeLogin = true;
			}
			return Player.Login(delegate(HttpC c)
			{
				LoginOk(c, _okCallback, _failCallback);
			}, _failCallback);
		}

		private static void AfterLogin()
		{
			HelpshiftManager.SetUserMetaData(PlayerPrefsX.GetUserId(), PlayerPrefsX.GetUserName());
			if (FacebookManager.m_friendLoadPending)
			{
				FacebookManager.LoadFriends();
			}
		}

		private static void GraphLoadOk(HttpC _c, Action _callback, int _pathVersion)
		{
			ClientTools.ParseInitialGraphData(_c, _pathVersion);
			_callback();
		}

		private static void GraphLoadFail(HttpC _c, Action _callback, int _pathVersion)
		{
			Debug.Log("Path retry");
			Path.GetInitial(delegate(HttpC c)
			{
				GraphLoadOk(c, _callback, _pathVersion);
			}, delegate(HttpC c)
			{
				GraphLoadFail(c, _callback, _pathVersion);
			});
		}

		private static void LoadInitialPathData(Dictionary<string, object> _serverResponse, Action _callback)
		{
			int pathVersion = 1;
			bool flag = true;
			if (_serverResponse.ContainsKey("pathVersion"))
			{
				pathVersion = Convert.ToInt32(_serverResponse["pathVersion"]);
			}
			if (false || pathVersion == PlayerPrefsX.GetPathVersion())
			{
				Debug.Log("Loading path from local...");
				Dictionary<string, object> dictionary = ClientTools.LoadLocalPathData();
				if (dictionary != null)
				{
					ClientTools.ParseInitialGraphData(dictionary, PlayerPrefsX.GetPathVersion());
					flag = false;
					LoadData(_serverResponse, _callback);
				}
				else
				{
					Debug.LogWarning("LOCAL PATH LOAD FAIL!");
				}
			}
			if (!flag)
			{
				return;
			}
			Debug.Log("Load path from server...");
			Path.GetInitial(delegate(HttpC c)
			{
				GraphLoadOk(c, delegate
				{
					LoadData(_serverResponse, _callback);
				}, pathVersion);
			}, delegate(HttpC c)
			{
				GraphLoadFail(c, delegate
				{
					LoadData(_serverResponse, _callback);
				}, pathVersion);
			});
		}

		private static void LoadData(Dictionary<string, object> serverResponse, Action _callback)
		{
			PsMetagameManager.m_keyReloadTimeSeconds = PsMetagameManager.GetSecondsFromTimeString(PsMetagameManager.GetKeyReloadTime());
			List<PsPlanetPath> serverPaths = ClientTools.ParseProgressionPathData(serverResponse["paths"] as List<object>, false);
			PsPlanetManager.GetCurrentPlanet().UpdateLocalPaths(serverPaths);
			PsPlanetManager.GetCurrentPlanet().OpenUnlocks();
			bool flag = false;
			if (serverResponse.ContainsKey("sessionExpiration"))
			{
				flag = (bool)serverResponse["sessionExpiration"];
			}
			if (flag)
			{
				ClearServerQueueItems();
				ServerManager.ClearRetryCallBacks();
			}
			else
			{
				ServerManager.ExecuteRetryCallBacks();
			}
			if (serverResponse.ContainsKey("lastLogin"))
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)serverResponse["lastLogin"];
				PlayerPrefsX.SetLastLoginTime(Convert.ToUInt64(dictionary["$date"]).ToString());
			}
			if (m_firstTimeLogin)
			{
				m_firstTimeLogin = false;
			}
			if (serverResponse.ContainsKey("adsConfig"))
			{
				List<object> config = serverResponse["adsConfig"] as List<object>;
				PsAdMediation.Initialize(config);
			}
			if (serverResponse.ContainsKey("iapConfig"))
			{
				Dictionary<string, object> config2 = serverResponse["iapConfig"] as Dictionary<string, object>;
				PsIAPManager.Initialize(config2);
			}
			if (serverResponse.ContainsKey("achievements"))
			{
				List<object> list = serverResponse["achievements"] as List<object>;
				PsAchievementManager.UpdateFromList(list);
			}
			if (serverResponse.ContainsKey("timedEvents"))
			{
				PsPlanetManager.m_timedEvents = ClientTools.ParseTimedEvents((Dictionary<string, object>)serverResponse["timedEvents"]);
				if (PsPlanetManager.GetCurrentPlanet() != null)
				{
					PsPlanetManager.GetCurrentPlanet().m_floaterManager.LoadTimedEvents();
				}
				TimedEvents.m_subscribers();
			}
			if (serverResponse.ContainsKey("lastPathSync"))
			{
				PlayerPrefsX.SetLastSync((string)serverResponse["lastPathSync"]);
			}
			AfterLogin();
			m_isCompeleted = true;
			_callback();
		}

		private static void LoginOk(HttpC _c, Action<HttpC> _okCallback, Action<HttpC> _failCallback)
		{
			Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_c.www.text);
			if (ClientTools.ServerResponseOk(dictionary))
			{
				if (dictionary.ContainsKey("sessionId"))
				{
					PlayerPrefsX.SetSession((string)dictionary["sessionId"]);
					PsState.m_sessionId = PlayerPrefsX.GetSession();
				}
				PlayerData playerData = ClientTools.ParsePlayerData(dictionary);
				if (playerData.hash == null || string.Compare(playerData.hash, ClientTools.GenerateHash(playerData), true) != 0)
				{
					ClearServerQueueItems();
					ServerManager.ClearRetryCallBacks();
					ServerManager.m_playerAuthenticated = false;
					_failCallback(_c);
					return;
				}
				PsMetagameManager.SetPlayer(playerData);
				Debug.Log("PLAYER LOGGED IN");
				long num = (long)dictionary["clientVersion"];
				int num2 = (int)num;
				string message = (string)dictionary["versionInfo"];
				if (dictionary.ContainsKey("skipTimerLeft"))
				{
					PlayerPrefsX.SetSkipTimeLeft((int)Math.Ceiling(Main.m_EPOCHSeconds + (double)(long)dictionary["skipTimerLeft"]));
				}
				if (Main.CLIENT_VERSION() < num2)
				{
					Debug.Log("ClientVersion: " + Main.CLIENT_VERSION() + ", ServerVersion: " + num2);
					Debug.Log("Client has old version!");
					ServerManager.ThrowOldVersionMessage(message);
					return;
				}
				if (m_debugReset)
				{
					if (!dictionary.ContainsKey("reset"))
					{
						dictionary.Add("reset", "yep");
					}
					m_debugReset = false;
					Debug.LogWarning("WARNING! YOU HAVE DEBUG RESET FLAG ON");
				}
				if (dictionary.ContainsKey("resetLocal"))
				{
					Debug.LogWarning("Resetting local progression...");
					ClearServerQueueItems();
					ServerManager.ClearRetryCallBacks();
					ServerManager.m_playerAuthenticated = false;
					PsPlanetManager.GetCurrentPlanet().ResetLocalProgression();
					PlayerPrefsX.SetLastSync("0");
					PsState.m_inLoginFlow = false;
					m_isCompeleted = true;
					Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"));
					return;
				}
				if (dictionary.ContainsKey("reset"))
				{
					Debug.LogWarning("Reset progression...");
					ClearServerQueueItems();
					ServerManager.ClearRetryCallBacks();
					ServerManager.m_playerAuthenticated = false;
					ResetFlow.Start();
					return;
				}
				if (!dictionary.ContainsKey("gameCenterId"))
				{
					PlayerPrefsX.DeleteKey("GameCenterId");
					PlayerPrefsX.DeleteKey("GameCenterName");
					KeyChain.SetGameCenterId(string.Empty);
				}
				else
				{
					PlayerPrefsX.SetGameCenterId((string)dictionary["gameCenterId"]);
				}
				if (!dictionary.ContainsKey("facebookId"))
				{
					PlayerPrefsX.DeleteKey("FacebookId");
					PlayerPrefsX.DeleteKey("FacebookName");
				}
				else
				{
					PlayerPrefsX.SetFacebookId((string)dictionary["facebookId"]);
				}
				PlayerPrefsX.SetPlayerData(playerData);
				KeyChain.SetUserName(playerData.name);
				if (dictionary.ContainsKey("majorPath"))
				{
					PlayerPrefsX.SetMajorPath(Convert.ToInt32(dictionary["majorPath"]));
				}
				else
				{
					PlayerPrefsX.SetMajorPath(2);
				}
				if (GameCenterManager.IsLoggedIn())
				{
					GameCenterManager.LoadFriends();
				}
				if (playerData.keyTimeLeft > 0)
				{
					PsMetagameManager.m_lastKeyUsedTime = Main.m_EPOCHSeconds - (double)(PsMetagameManager.m_keyReloadTimeSeconds - playerData.keyTimeLeft);
					Debug.Log("CONTINUING KEY TIMER: " + playerData.keyTimeLeft);
				}
				m_userDataLoaded = true;
				LoadInitialPathData(dictionary, delegate
				{
					_okCallback(_c);
				});
			}
			else
			{
				_failCallback(_c);
			}
		}

		private static void NameChangeSUCCEED(HttpC _c, Action _callback)
		{
			Debug.Log("NAME CHANGE SUCCEED");
			if (_callback != null)
			{
				_callback();
			}
		}

		private static void NameChangeFAILED(HttpC _c, Action _callback = null)
		{
			Debug.Log("NAME CHANGE FAILED");
			ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Player.ChangeName(delegate(HttpC c)
			{
				NameChangeSUCCEED(c, _callback);
			}, delegate(HttpC d)
			{
				NameChangeFAILED(d, _callback);
			}));
		}

		private static void ClearServerQueueItems()
		{
			if (PsMetagameManager.m_serverQueueItems != null)
			{
				for (int i = 0; i < PsMetagameManager.m_serverQueueItems.Count; i++)
				{
					PsMetagameManager.m_serverQueueItems[i] = null;
				}
				PsMetagameManager.m_serverQueueItems.Clear();
			}
		}

		public static void Restart()
		{
			HttpS.ClearAllRequests();
			ClearServerQueueItems();
			Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"), new FadeLoadingScene(Color.black));
		}
	}
}
