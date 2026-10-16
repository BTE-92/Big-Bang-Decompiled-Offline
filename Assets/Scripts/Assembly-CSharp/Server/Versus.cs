using System;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
	public static class Versus
	{
		public static HttpC Create(Action<VersusMetaData> _okCallback = null, Action<HttpC> _failEventHandler = null, string _gameId = null, Action _errorCallback = null)
		{
			List<string> _tags = new List<string>();
			ResponseHandler<VersusMetaData> responseHandler = new ResponseHandler<VersusMetaData>(_okCallback, ClientTools.ParseVersusMetaData);
			WOEURL wOEURL = new WOEURL("/v4/versus/create");
			if (!string.IsNullOrEmpty(_gameId))
			{
				wOEURL.AddParameter("gameId", _gameId);
			}
			Debug.Log(wOEURL.ConstructURL());
			string json = ClientTools.GeneratePlayerSetDataJson(new Hashtable(), ref _tags);
			return Request.Post(wOEURL, null, json, responseHandler.RequestOk, _failEventHandler, _errorCallback);
		}

		public static HttpC OverwriteScore(string _challengeId, int _time, int _starts, string _gameId, bool _winRound, bool _loseRound, int _upgradeSum, string _playerUnit, DataBlob _dataBlob, Action<HttpC> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v2/versus/score/overwrite");
			wOEURL.AddParameter("challengeId", _challengeId);
			wOEURL.AddParameter("gameId", _gameId);
			wOEURL.AddParameter("time", _time.ToString());
			wOEURL.AddParameter("starts", _starts);
			string text = _time.ToString();
			wOEURL.AddParameter("hash", ClientTools.GenerateHash(_challengeId + KeyChain.GetUserId() + text));
			wOEURL.AddParameter("winRound", _winRound.ToString().ToLower());
			wOEURL.AddParameter("loseRound", _loseRound.ToString().ToLower());
			wOEURL.AddParameter("upgradeSum", _upgradeSum.ToString());
			wOEURL.AddParameter("playerUnit", _playerUnit);
			Debug.Log(wOEURL.ConstructURL());
			HttpC httpC = Request.Post(wOEURL, null, (_dataBlob.data == null) ? new byte[1] : _dataBlob.data, _okCallback, _failEventHandler, _errorCallback, null, new string[1] { "Highscore" });
			httpC.objectData = _dataBlob;
			return httpC;
		}

		public static HttpC Quit(string _challengeId, string _gameId, int _starts, bool _winRound, bool _loseRound, Action<HttpC> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/versus/score/quit");
			wOEURL.AddParameter("challengeId", _challengeId);
			wOEURL.AddParameter("gameId", _gameId);
			wOEURL.AddParameter("starts", _starts);
			wOEURL.AddParameter("winRound", _winRound.ToString().ToLower());
			wOEURL.AddParameter("loseRound", _loseRound.ToString().ToLower());
			return Request.Post(wOEURL, null, new byte[1], _okCallback, _failEventHandler, _errorCallback);
		}

		public static HttpC Win(string _challengeId, string _gameId, int _starts, Action<HttpC> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null)
		{
			List<string> _tags = new List<string>();
			WOEURL wOEURL = new WOEURL("/v1/versus/win");
			wOEURL.AddParameter("challengeId", _challengeId);
			wOEURL.AddParameter("gameId", _gameId);
			wOEURL.AddParameter("starts", _starts);
			string json = ClientTools.GeneratePlayerSetDataJson(new Hashtable(), ref _tags);
			return Request.Post(wOEURL, null, json, _okCallback, _failEventHandler, _errorCallback);
		}

		public static HttpC Forfeit(string _challengeId, string _gameId, int _starts, Action<HttpC> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null)
		{
			List<string> _tags = new List<string>();
			WOEURL wOEURL = new WOEURL("/v1/versus/forfeit");
			wOEURL.AddParameter("challengeId", _challengeId);
			wOEURL.AddParameter("gameId", _gameId);
			wOEURL.AddParameter("starts", _starts);
			string json = ClientTools.GeneratePlayerSetDataJson(new Hashtable(), ref _tags);
			return Request.Post(wOEURL, null, json, _okCallback, _failEventHandler, _errorCallback);
		}

		public static HttpC SetTries(string _challengeId, int _tries, Action<HttpC> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/versus/tries/set");
			wOEURL.AddParameter("challengeId", _challengeId);
			wOEURL.AddParameter("tries", _tries);
			return Request.Post(wOEURL, null, new byte[1], _okCallback, _failEventHandler, _errorCallback, null, new string[1] { "setTries" });
		}

		public static HttpC Claim(string _challengeId, Action<HttpC> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null)
		{
			List<string> _tags = new List<string>();
			WOEURL wOEURL = new WOEURL("/v1/versus/claim");
			wOEURL.AddParameter("challengeId", _challengeId);
			string json = ClientTools.GeneratePlayerSetDataJson(new Hashtable(), ref _tags);
			return Request.Post(wOEURL, null, json, _okCallback, _failEventHandler, _errorCallback);
		}

		public static HttpC ClaimAndSave(string _challengeId, string _pathJson, Action<HttpC> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null)
		{
			List<string> _tags = new List<string>();
			WOEURL wOEURL = new WOEURL("/v1/versus/claimAndSave");
			wOEURL.AddParameter("challengeId", _challengeId);
			string text = ClientTools.GeneratePlayerSetDataJson(new Hashtable(), ref _tags);
			string json = "{\"update\":" + text + ",\"progression\":" + _pathJson + "}";
			return Request.Post(wOEURL, null, json, _okCallback, _failEventHandler, _errorCallback);
		}
	}
}
