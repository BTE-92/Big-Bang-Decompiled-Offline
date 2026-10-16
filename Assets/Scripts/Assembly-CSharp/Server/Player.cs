using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MiniJSON;

namespace Server
{
	public static class Player
	{
		public class ProgressionAndSetDataObject
		{
			public Hashtable setData;

			public Hashtable pathJson;

			public Hashtable customisations;

			public List<Dictionary<string, object>> achievements;

			public bool sendMetrics;
		}

		private static bool m_firstTimeLogin;

		private static PsUIBasePopup m_changeName;

		public static HttpC GetPlayerInfo(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/info");
			return Request.Get(url, "GET_PLAYER_INFO", _okCallback, _failCallback, _errorCallback);
		}

		public static HttpC GetStatistics(Action<PlayerStatistics> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/stats/find");
			ResponseHandler<PlayerStatistics> responseHandler = new ResponseHandler<PlayerStatistics>(_okCallback, ClientTools.ParsePlayerStatistics);
			return Request.Get(url, "GET_PLAYER_STATS", responseHandler.RequestOk, _failCallback, _errorCallback);
		}

		public static HttpC RemovePlayer(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/remove");
			return Request.Post(url, "REMOVE_PLAYER", _okCallback, _failCallback, _errorCallback);
		}

		public static HttpC ResetPlayer(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/resetLevel");
			return Request.Post(url, "RESET_LEVEL", _okCallback, _failCallback, _errorCallback);
		}

		public static HttpC RemovePlayerData(string[] _fieldsToRemove, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/player/data/remove");
			wOEURL.AddParameter("fields", string.Join(",", _fieldsToRemove));
			return Request.Post(wOEURL, "REMOVE_PLAYER_DATA", _okCallback, _failCallback, _errorCallback);
		}

		public static HttpC SkipLevel(string _playerUnit, Hashtable _setData, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/player/skip");
			wOEURL.AddParameter("playerUnit", _playerUnit);
			return SetData(wOEURL, _setData, _okCallback, _failCallback, _errorCallback);
		}

		public static HttpC ClaimTutorial(string _tutorialIdentifier, Hashtable _setData, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/tutorial/claim");
			wOEURL.AddParameter("tutorial", _tutorialIdentifier);
			return SetData(wOEURL, _setData, _okCallback, _failCallback, _errorCallback);
		}

		public static HttpC SetData(Hashtable _setData, Hashtable _pathJson, Hashtable _customisations, List<Dictionary<string, object>> _achievements, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v2/player/data/change");
			Hashtable hashtable = new Hashtable();
			List<string> _tags = new List<string>();
			Hashtable hashtable2 = null;
			if (_setData != null)
			{
				_tags.Add("SetData");
				hashtable2 = ClientTools.GeneratePlayerSetData(_setData, ref _tags);
				hashtable.Add("update", hashtable2);
			}
			if (_pathJson != null)
			{
				hashtable.Add("progression", _pathJson);
			}
			if (_customisations != null)
			{
				hashtable.Add("customisation", _customisations);
			}
			if (_achievements != null)
			{
				hashtable.Add("achievements", _achievements);
			}
			string json = Json.Serialize(hashtable);
			string[] queueTags = null;
			if (_tags.Count() > 0)
			{
				queueTags = _tags.ToArray();
			}
			ResponseHandler<HttpC> responseHandler = new ResponseHandler<HttpC>(_okCallback, ClientTools.ParsePathSync);
			return Request.Post(url, "SET_DATA_V2", json, responseHandler.RequestOk, _failCallback, _errorCallback, null, queueTags);
		}

		private static HttpC SetData(WOEURL url, Hashtable _setData, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback)
		{
			List<string> _tags = new List<string>();
			_tags.Add("SetData");
			string json = ClientTools.GeneratePlayerSetDataJson(_setData, ref _tags);
			return Request.Post(url, null, json, _okCallback, _failCallback, _errorCallback, null, _tags.ToArray());
		}

		public static void CheckSocialLogin(string _service, string _socialId, string[] _friendIds, bool autoJoin, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/player/check/social");
			string json = GenerateSocialJson(_service, _socialId, _friendIds);
			wOEURL.AddParameter("noJoin", (!autoJoin).ToString().ToLower());
			Request.Post(wOEURL, null, json, _okCallback, _failCallback, _errorCallback);
		}

		public static void ResolveSocialLogin(string _service, string _socialId, string _connectUserId, string _disconnectFromUserId, string[] _friendIds, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/solve/social");
			string json = GenerateSocialJson(_service, _socialId, _friendIds, _connectUserId, _disconnectFromUserId);
			Request.Post(url, null, json, _okCallback, _failCallback, _errorCallback);
		}

		public static void RemoveSocialLogin(string _service, string _socialId, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/remove/social");
			string json = GenerateSocialJson(_service, _socialId, null);
			Request.Post(url, null, json, _okCallback, _failCallback, _errorCallback);
		}

		public static void SendSocialFriends(string _service, string _socialId, string[] _friendIds, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/follow/social");
			string json = GenerateSocialJson(_service, _socialId, _friendIds);
			Request.Post(url, null, json, _okCallback, _failCallback, _errorCallback);
		}

		public static void CheckAndJoinGameCenter(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v2/player/check/gameCenter");
			string json = GenerateGameCenterJSON(null);
			Request.Post(url, null, json, _okCallback, _failCallback, _errorCallback);
		}

		public static void SendGameCenterFriends(string[] _friendIds, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/follow/gameCenter");
			string json = GenerateGameCenterJSON(_friendIds);
			Request.Post(url, null, json, _okCallback, _failCallback, _errorCallback);
		}

		public static void SwitchGameCenter(string id, string[] _friendIds, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v2/player/join/gameCenter");
			wOEURL.AddParameter("oldId", id);
			PlayerPrefsX.SetUserName(string.Empty);
			KeyChain.SetUserName(string.Empty);
			string json = GenerateGameCenterJSON(_friendIds);
			Request.Post(wOEURL, null, json, _okCallback, _failCallback, _errorCallback);
		}

		public static HttpC Follow(string _followeeId, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/player/follow/add");
			wOEURL.AddParameter("followeeId", _followeeId);
			return Request.Post(wOEURL, null, _okCallback, _failCallback, _errorCallback, null, new string[1] { "Follow" });
		}

		public static HttpC UnFollow(string _followeeId, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/player/follow/remove");
			wOEURL.AddParameter("followerId", PlayerPrefsX.GetUserId());
			wOEURL.AddParameter("followeeId", _followeeId);
			return Request.Post(wOEURL, null, _okCallback, _failCallback, _errorCallback, null, new string[1] { "Follow" });
		}

		public static HttpC Followees(Action<PlayerData[]> _okCallback, Action<HttpC> _failCallback, string _playerId, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/player/follow/followees");
			wOEURL.AddParameter("followerId", _playerId);
			ResponseHandler<PlayerData[]> responseHandler = new ResponseHandler<PlayerData[]>(_okCallback, ClientTools.ParseFollowPlayers);
			return Request.Get(wOEURL, null, responseHandler.RequestOk, _failCallback, _errorCallback);
		}

		public static HttpC Followers(Action<PlayerData[]> _okCallback, Action<HttpC> _failCallback, string _playerId, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/player/follow/followers");
			wOEURL.AddParameter("followeeId", _playerId);
			ResponseHandler<PlayerData[]> responseHandler = new ResponseHandler<PlayerData[]>(_okCallback, ClientTools.ParseFollowPlayers);
			return Request.Get(wOEURL, null, responseHandler.RequestOk, _failCallback, _errorCallback);
		}

		public static HttpC Friends(Action<Friends> _okCallback, Action<HttpC> _failCallback, string _playerId, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/friends");
			ResponseHandler<Friends> responseHandler = new ResponseHandler<Friends>(_okCallback, ClientTools.ParseFriends);
			return Request.Get(url, null, responseHandler.RequestOk, _failCallback, _errorCallback);
		}

		public static void ChangeNamePrompt(Action _proceed)
		{
			TouchAreaS.Enable();
			if (m_changeName == null)
			{
				m_changeName = new PsUIBasePopup(typeof(PsUIPopupChangeName), null, null, null, false);
				m_changeName.SetAction("Proceed", _proceed);
				(m_changeName.m_mainContent as PsUIPopupChangeName).CreateUI(true);
				m_changeName.Update();
			}
		}

		public static HttpC Login(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			m_changeName = null;
			int num = PsPlanetManager.GetCurrentPlanet().GetMainPathCurrentNodeId() - 1;
			Debug.Log("Login to WoE server");
			WOEURL wOEURL = new WOEURL("/v4/player/login");
			PsPlanetPath psPlanetPath = null;
			try
			{
				psPlanetPath = PsPlanetSerializer.LoadProgression(PsPlanetManager.GetCurrentPlanet(), "MainPath");
			}
			catch
			{
				psPlanetPath = null;
			}
			string value = ((!string.IsNullOrEmpty(PlayerPrefsX.GetLastSync()) && psPlanetPath != null) ? PlayerPrefsX.GetLastSync() : "0");
			wOEURL.AddParameter("lastPathSync", value);
			return Request.Post(wOEURL, null, ClientTools.GenerateUserJSON(), _okCallback, _failCallback, _errorCallback, null, new string[1] { "login" });
		}

		public static HttpC GetSkipTimerValue(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/skipTimeLeft");
			return Request.Post(url, null, _okCallback, _failCallback, _errorCallback);
		}

		public static HttpC ResetSkipTimer(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/resetSkipTimer");
			return Request.Post(url, null, _okCallback, _failCallback, _errorCallback);
		}

		public static HttpC ChangeName(Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/player/changeName");
			string json = ClientTools.GenerateUserJSON();
			return Request.Post(url, null, json, _okCallback, _failureCallback, _errorCallback);
		}

		public static PlayerData[] ParsePlayers(PlayerData[] _players, bool _fb, bool _gc)
		{
			List<PlayerData> list = new List<PlayerData>();
			for (int i = 0; i < _players.Length; i++)
			{
				if (_fb && _players[i].facebookId != null)
				{
					list.Add(_players[i]);
				}
				else if (_gc && _players[i].gameCenterId != null)
				{
					list.Add(_players[i]);
				}
			}
			return list.ToArray();
		}

		private static string GenerateSocialJson(string _service, string _socialId, string[] _friendIds, string _connectId = null, string _disconnectId = null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("service", _service);
			hashtable.Add("socialId", _socialId);
			hashtable.Add("friendList", _friendIds);
			if (!string.IsNullOrEmpty(_connectId))
			{
				hashtable.Add("connectId", _connectId);
			}
			if (!string.IsNullOrEmpty(_disconnectId))
			{
				hashtable.Add("disconnectId", _disconnectId);
			}
			string text = Json.Serialize(hashtable);
			Debug.Log("SOCIAL JSON GENERATED: " + text);
			return text;
		}

		private static string GenerateGameCenterJSON(string[] _friendIds)
		{
			Hashtable hashtable = new Hashtable();
			string userName = PlayerPrefsX.GetUserName();
			if (userName != string.Empty && userName != null)
			{
				hashtable.Add("name", PlayerPrefsX.GetUserName());
			}
			if (PlayerPrefsX.GetGameCenterId() != null)
			{
				hashtable.Add("gameCenterId", PlayerPrefsX.GetGameCenterId());
			}
			if (_friendIds != null)
			{
				hashtable.Add("gcFriends", _friendIds);
			}
			string text = Json.Serialize(hashtable);
			Debug.Log("GC JOIN JSON GENERATED: " + text);
			return text;
		}

		private static void LoginComplete()
		{
			Debug.Log("Login complete");
		}
	}
}
