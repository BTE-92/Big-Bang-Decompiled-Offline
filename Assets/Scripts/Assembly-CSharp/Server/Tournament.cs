using System;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
	public static class Tournament
	{
		public static HttpC Participate(string _tournament, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/tournament/unlock");
			List<string> _tags = new List<string>();
			wOEURL.AddParameter("tournamentId", _tournament);
			string json = ClientTools.GeneratePlayerSetDataJson(new Hashtable(), ref _tags);
			return Request.Post(wOEURL, "TOURNAMENT_PARTICIPATE", json, _okCallback, _failureCallback, _errorCallback, null, _tags.ToArray());
		}

		public static HttpC GetTournaments(Action<PsTournaments> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			ResponseHandler<PsTournaments> responseHandler = new ResponseHandler<PsTournaments>(_okCallback, ClientTools.ParseTournaments);
			WOEURL url = new WOEURL("/v1/tournament/find");
			return Request.Get(url, "TOURNAMENT_GET_ACTIVE", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC Get(string _tournamentId, Action<PsTournamentMetaData> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			ResponseHandler<PsTournamentMetaData> responseHandler = new ResponseHandler<PsTournamentMetaData>(_okCallback, ClientTools.ParseTournament);
			WOEURL wOEURL = new WOEURL("/v1/tournament/get");
			wOEURL.AddParameter("tournamentId", _tournamentId);
			Debug.Log(wOEURL.ConstructURL());
			return Request.Get(wOEURL, "TOURNAMENT_GET", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC TournamentClaim(PsTournamentMetaData _data, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/tournament/claim");
			List<string> _tags = new List<string>();
			wOEURL.AddParameter("tournamentId", _data.tournamentId);
			string json = ClientTools.GeneratePlayerSetDataJson(new Hashtable(), ref _tags);
			return Request.Post(wOEURL, "TOURNAMENT_CLAIM", json, _okCallback, _failureCallback, _errorCallback, null, _tags.ToArray());
		}

		public static HttpC TournamentCreate(string _gameId, string _name, int _costCoins, int _costDiamonds, int _duration, Action<HttpC> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/tournament/create");
			wOEURL.AddParameter("gameId", _gameId);
			wOEURL.AddParameter("name", _name);
			wOEURL.AddParameter("coins", _costCoins);
			wOEURL.AddParameter("diamonds", _costDiamonds);
			wOEURL.AddParameter("duration", _duration);
			return Request.Post(wOEURL, "TOURNAMENT_CREATE", _okCallback, _failureCallback, _errorCallback);
		}

		public static HttpC TournamentGetNext(PsTournamentMetaData _meta, int _time, Action<HighscoreDataEntry> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/tournament/score/next");
			wOEURL.AddParameter("tournamentId", _meta.tournamentId);
			wOEURL.AddParameter("time", _time);
			ResponseHandler<HighscoreDataEntry> responseHandler = new ResponseHandler<HighscoreDataEntry>(_okCallback, HighScores.ParseDataEntryJSON);
			return Request.Get(wOEURL, string.Empty, responseHandler.RequestOk, _failureCallback, _errorCallback);
		}
	}
}
