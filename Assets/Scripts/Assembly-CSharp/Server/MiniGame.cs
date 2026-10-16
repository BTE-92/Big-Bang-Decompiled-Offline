using System;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
	public static class MiniGame
	{
		private static int PUBLISH = 1;

		private static int SAVE = 2;

		private static int HIDE = 3;

		public static HttpC GetMinigamesInSubGenre(PsSubgenre _subGenre, Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			return GetMinigamesInSubGenre(_subGenre.ToString(), _okCallback, _failureCallback, _errorCallback);
		}

		public static HttpC GetMinigamesInSubGenre(string _subGenre, Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/minigame/subgenre/find");
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			wOEURL.AddParameter("subGenre", _subGenre);
			return Request.Get(wOEURL, "MINIGAME_SUBGENRE_FIND", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetHidden(Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null, int _limit = 10)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			WOEURL wOEURL = new WOEURL("/v1/minigame/hidden");
			wOEURL.AddParameter("limit", _limit);
			return Request.Get(wOEURL, "MINIGAME_HIDDEN_FIND", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC SearchMinigames(MinigameSearchParametres _parametres, Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null, int _limit = 10)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			WOEURL wOEURL = new WOEURL("/v2/minigame/meta/search");
			if (_parametres.m_items != null && _parametres.m_items.Length > 0)
			{
				wOEURL.AddParameter("items", string.Join(",", _parametres.m_items));
			}
			if (_parametres.m_features != null && _parametres.m_features.Length > 0)
			{
				wOEURL.AddParameter("features", string.Join(",", _parametres.m_features));
			}
			if (_parametres.m_playerUnitFilter != null && _parametres.m_playerUnitFilter != "Any")
			{
				wOEURL.AddParameter("playerUnit", _parametres.m_playerUnitFilter);
			}
			if (_parametres.m_gameMode != PsGameMode.Any)
			{
				wOEURL.AddParameter("gameMode", _parametres.m_gameMode.ToString());
			}
			if (_parametres.m_difficulty != PsGameDifficulty.Any)
			{
				wOEURL.AddParameter("difficulty", _parametres.m_difficulty.ToString());
			}
			wOEURL.AddParameter("limit", _limit);
			HttpC httpC = Request.Get(wOEURL, "MINIGAME_SEARCH", responseHandler.RequestOk, _failureCallback, _errorCallback);
			httpC.objectData = _parametres;
			return httpC;
		}

		public static HttpC GetFolloweeMinigames(Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, int limit = 50, Action _errorCallback = null)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			WOEURL wOEURL = new WOEURL("/v1/minigame/followee/find");
			wOEURL.AddParameter("limit", limit);
			return Request.Get(wOEURL, "MINIGAME_FOLLOWEE_LIST_DOWNLOAD", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC Get(string _id, Action<PsMinigameMetaData> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/minigame/meta/find");
			ResponseHandler<PsMinigameMetaData> responseHandler = new ResponseHandler<PsMinigameMetaData>(_okCallback, ClientTools.ParseMinigameMetaData);
			wOEURL.AddParameter("id", _id);
			HttpC httpC = Request.Get(wOEURL, "MINIGAME_GET", responseHandler.RequestOk, _failureCallback, _errorCallback);
			httpC.objectData = _id;
			return httpC;
		}

		public static HttpC GetWithItems(string _items, Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			WOEURL wOEURL = new WOEURL("/v1/minigame/meta/findItems");
			wOEURL.AddParameter("items", _items);
			return Request.Get(wOEURL, "MINIGAME_WITH_ITEMS", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetFresh(Action<PsMinigameMetaData> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/minigame/oneFresh");
			wOEURL.AddParameter("playerUnit", "Motorcycle");
			wOEURL.AddParameter("gameMode", "Race");
			ResponseHandler<PsMinigameMetaData> responseHandler = new ResponseHandler<PsMinigameMetaData>(_okCallback, ClientTools.ParseMinigameMetaData);
			return Request.Get(wOEURL, "MINIGAME_ONE_FRESH_DOWNLOAD", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetHistory(string _playerId, Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, int _limit = 20, Action _errorCallback = null)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			WOEURL wOEURL = new WOEURL("/v1/minigame/history");
			wOEURL.AddParameter("playerId", _playerId);
			wOEURL.AddParameter("limit", _limit);
			return Request.Get(wOEURL, "MINIGAME_HISTORY_LIST_DOWNLOAD", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetOwn(Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, int _limit = 50, Action _errorCallback = null)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			WOEURL wOEURL = new WOEURL("/v1/minigame/own");
			wOEURL.AddParameter("limit", _limit);
			return Request.Get(wOEURL, "MINIGAME_OWN_FIND", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetOwnPublished(Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, int _limit = 50, Action _errorCallback = null)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			WOEURL wOEURL = new WOEURL("/v1/minigame/own/published");
			wOEURL.AddParameter("limit", _limit);
			return Request.Get(wOEURL, "MINIGAME_OWN_PUBLISHED_LIST_DOWNLOAD", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetOwnSaved(Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, int _limit = 50, Action _errorCallback = null)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			WOEURL wOEURL = new WOEURL("/v1/minigame/own/saved");
			wOEURL.AddParameter("limit", _limit);
			return Request.Get(wOEURL, "MINIGAME_OWN_SAVED_LIST_DOWNLOAD", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetPublished(Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, int limit = 50, Action _errorCallback = null)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			WOEURL wOEURL = new WOEURL("/v1/minigame/meta/published");
			wOEURL.AddParameter("limit", limit);
			return Request.Get(wOEURL, "MINIGAME_PUBLISHED_LIST_DOWNLOAD", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetLevelData(string _gameId, Action<byte[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			ResponseHandler<byte[]> responseHandler = new ResponseHandler<byte[]>(_okCallback, ClientTools.ParseBytes);
			WOEURL wOEURL = new WOEURL("/v1/minigame/data/find");
			wOEURL.AddParameter("id", _gameId);
			return Request.Get(wOEURL, "MINIGAME_DOWNLOAD", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC OverrideData(string _gameId, byte[] _data, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/minigame/data/save");
			wOEURL.AddParameter("id", _gameId);
			return Request.Post(wOEURL, "OVERRIDE_MINIGAME_DATA", _data, _okCallback, _failureCallback, _errorCallback);
		}

		public static HttpC ClaimReward(string _gameId, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/minigame/claim");
			List<string> _tags = new List<string>();
			wOEURL.AddParameter("gameId", _gameId);
			string json = ClientTools.GeneratePlayerSetDataJson(new Hashtable(), ref _tags);
			return Request.Post(wOEURL, "CLAIM_MINIGAME_REWARD", json, _okCallback, _failureCallback, _errorCallback, null, _tags.ToArray());
		}

		public static HttpC ClaimAllRewards(Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL url = new WOEURL("/v1/minigame/claimAll");
			List<string> _tags = new List<string>();
			string json = ClientTools.GeneratePlayerSetDataJson(new Hashtable(), ref _tags);
			return Request.Post(url, "CLAIM_MINIGAME_REWARDS", json, _okCallback, _failureCallback, _errorCallback, null, _tags.ToArray());
		}

		public static HttpC GetUnclaimedRewards(Action<RewardData> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			ResponseHandler<RewardData> responseHandler = new ResponseHandler<RewardData>(_okCallback, ClientTools.ParseUnclaimedRewards);
			WOEURL url = new WOEURL("/v1/minigame/getRewards");
			return Request.Get(url, "GET_MINIGAME_REWARDS", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC SaveHidden(PsGameLoop _minigameInfo, byte[] _screenshotData, Action<PsMinigameMetaData> _okCallback, Action<HttpC> _failureCallback, string _creationTime, Action _errorCallback = null)
		{
			return Save(_minigameInfo, _screenshotData, _okCallback, _failureCallback, HIDE, _creationTime, _errorCallback);
		}

		public static HttpC Save(PsGameLoop _minigameInfo, byte[] _screenshotData, Action<PsMinigameMetaData> _okCallback, Action<HttpC> _failureCallback, bool _publish, string _creationTime, Action _errorCallback = null)
		{
			if (_publish)
			{
				return Save(_minigameInfo, _screenshotData, _okCallback, _failureCallback, PUBLISH, _creationTime, _errorCallback);
			}
			return Save(_minigameInfo, _screenshotData, _okCallback, _failureCallback, SAVE, _creationTime, _errorCallback);
		}

		private static HttpC Save(PsGameLoop _minigameInfo, byte[] _screenshotData, Action<PsMinigameMetaData> _okCallback, Action<HttpC> _failureCallback, int _saveType, string _creationTime, Action _errorCallback = null)
		{
			DataBlob dataBlob = ClientTools.CreateLevelDataBlob(_screenshotData);
			string command = ((_saveType == PUBLISH) ? "/v3/minigame/publish" : ((_saveType != SAVE) ? "/v1/minigame/hide" : "/v2/minigame/save"));
			WOEURL wOEURL = new WOEURL(command);
			ResponseHandler<PsMinigameMetaData> responseHandler = new ResponseHandler<PsMinigameMetaData>(_okCallback, ClientTools.ParseMinigameMetaData);
			if (!string.IsNullOrEmpty(_minigameInfo.GetGameId()))
			{
				wOEURL.AddParameter("id", _minigameInfo.GetGameId());
			}
			wOEURL.AddParameter("timestamp", _creationTime);
			return Request.Post(wOEURL, null, dataBlob.data, responseHandler.RequestOk, _failureCallback, _errorCallback, dataBlob.header);
		}

		public static HttpC Publish(PsGameLoop _minigameInfo, byte[] _screenshotData, string _pathData, DataBlob _ghostData, Action<PsMinigameMetaData> _okCallback, Action<HttpC> _failureCallback, bool _hidden, string _creationTime, Action _errorCallback = null)
		{
			DataBlob dataBlob = ClientTools.CreatePublishLevelDataBlob(_screenshotData, _pathData, _ghostData);
			string command = (_hidden ? "/v1/minigame/hide" : "/v3/minigame/publish");
			WOEURL wOEURL = new WOEURL(command);
			ResponseHandler<PsMinigameMetaData> responseHandler = new ResponseHandler<PsMinigameMetaData>(_okCallback, ClientTools.ParseMinigameMetaData);
			if (!string.IsNullOrEmpty(_minigameInfo.GetGameId()))
			{
				wOEURL.AddParameter("id", _minigameInfo.GetGameId());
			}
			wOEURL.AddParameter("timestamp", _creationTime);
			return Request.Post(wOEURL, null, dataBlob.data, responseHandler.RequestOk, _failureCallback, _errorCallback, dataBlob.header);
		}

		public static HttpC Delete(string _gameId, Action<HttpC> _okCallback = null, Action<HttpC> _failureCallback = null, Action _errorCallback = null)
		{
			Debug.Log("Delete minigame " + _gameId);
			int second = DateTime.Now.Second;
			WOEURL wOEURL = new WOEURL("/v1/minigame/delete");
			wOEURL.AddParameter("id", _gameId);
			wOEURL.AddParameter("time", second);
			wOEURL.AddParameter("hash", ClientTools.GenerateHash(_gameId + second));
			PsMetrics.LevelDeleted(_gameId);
			return Request.Post(wOEURL, null, new byte[1], _okCallback, _failureCallback, _errorCallback);
		}

		public static HttpC GetPublishedByPlayerId(string _playerId, Action<PsMinigameMetaData[]> _okCallback, Action<HttpC> _failureCallback, int _limit = 50, Action _errorCallback = null)
		{
			ResponseHandler<PsMinigameMetaData[]> responseHandler = new ResponseHandler<PsMinigameMetaData[]>(_okCallback, ClientTools.ParseMinigameList);
			WOEURL wOEURL = new WOEURL("/v1/minigame/followee/published");
			wOEURL.AddParameter("playerId", _playerId);
			wOEURL.AddParameter("limit", _limit);
			return Request.Get(wOEURL, "MINIGAME_FOLLOWEE_PUBLISHED_LIST_DOWNLOAD", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}
	}
}
