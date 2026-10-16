using System;

namespace Server
{
	public static class Ghost
	{
		public static HttpC GetVersusNew(string _playerId, string _challengeId, Action<GhostData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/ghost/versus/get");
			wOEURL.AddParameter("challengeId", _challengeId);
			wOEURL.AddParameter("playerId", _playerId);
			ResponseHandler<GhostData[]> responseHandler = new ResponseHandler<GhostData[]>(_okCallback, ClientTools.ParseGhostData);
			return Request.Get(wOEURL, "GHOST_VERSUS", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetFriendly(string _playerId, string _challengeId, Action<GhostData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/ghost/friendly/get");
			wOEURL.AddParameter("challengeId", _challengeId);
			wOEURL.AddParameter("playerId", _playerId);
			ResponseHandler<GhostData[]> responseHandler = new ResponseHandler<GhostData[]>(_okCallback, ClientTools.ParseGhostData);
			return Request.Get(wOEURL, "FRIENDLY_VERSUS", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetNextNew(string _gameId, int _time, Action<GhostData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/ghost/next");
			wOEURL.AddParameter("gameId", _gameId);
			wOEURL.AddParameter("time", _time);
			ResponseHandler<GhostData[]> responseHandler = new ResponseHandler<GhostData[]>(_okCallback, ClientTools.ParseGhostData);
			return Request.Get(wOEURL, "GHOST_NEXT", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetNext(string _gameId, int[] _times, Action<GhostData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v2/ghost/next");
			wOEURL.AddParameter("gameId", _gameId);
			string text = string.Empty;
			for (int i = 0; i < _times.Length; i++)
			{
				text += _times[i];
				if (i != _times.Length - 1)
				{
					text += ",";
				}
			}
			wOEURL.AddParameter("times", text);
			ResponseHandler<GhostData[]> responseHandler = new ResponseHandler<GhostData[]>(_okCallback, ClientTools.ParseGhostDatas);
			return Request.Get(wOEURL, "GHOST_NEXT_V2", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetTrophyGhosts(string _gameId, Action<GhostData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("v1/ghosts");
			wOEURL.AddParameter("gameId", _gameId);
			ResponseHandler<GhostData[]> responseHandler = new ResponseHandler<GhostData[]>(_okCallback, ClientTools.ParseGhostDatas);
			return Request.Get(wOEURL, "GHOST_NEXT_V3", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetCreatorNew(PsMinigameMetaData _meta, Action<GhostData[]> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/ghost/creator/get");
			wOEURL.AddParameter("gameId", _meta.id);
			wOEURL.AddParameter("creatorId", _meta.creatorId);
			ResponseHandler<GhostData[]> responseHandler = new ResponseHandler<GhostData[]>(_okCallback, ClientTools.ParseGhostData);
			return Request.Get(wOEURL, "GHOST_CREATOR", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}
	}
}
