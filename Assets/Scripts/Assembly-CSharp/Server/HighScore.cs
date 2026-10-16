using System;
using UnityEngine;

namespace Server
{
	public static class HighScore
	{
		public class HighScoreAndGhostData
		{
			public PsMinigameContext context;

			public long elapsedTime;

			public DataBlob ghostData;

			public int ghostSize;

			public PsMinigameMetaData metadata;

			public int lastSentTimeScore;

			public int lastSentScore;

			public bool boosterUsed;

			public bool mainpath;

			public int upgradeSum;

			public int deathCount;

			public int starts;

			public string playerUnit;

			public HighScoreAndGhostData(PsMinigameContext _context, long _elapsedTime, DataBlob _ghostData, int _ghostSize, PsMinigameMetaData _metadata, int _lastSentTimeScore, int _lastSentScore, bool _boosterUsed, string _playerUnit, bool _mainpath = false, int _upgradeSum = 0, int _deathCount = 0, int _starts = 1)
			{
				context = _context;
				elapsedTime = _elapsedTime;
				ghostData = _ghostData;
				ghostSize = _ghostSize;
				metadata = _metadata;
				lastSentTimeScore = _lastSentTimeScore;
				lastSentScore = _lastSentScore;
				boosterUsed = _boosterUsed;
				mainpath = _mainpath;
				upgradeSum = _upgradeSum;
				deathCount = _deathCount;
				starts = _starts;
				playerUnit = _playerUnit;
			}
		}

		public static HttpC GetNext(PsMinigameMetaData _meta, int _time, Action<HighscoreDataEntry> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v1/highscore/next");
			wOEURL.AddParameter("gameId", _meta.id);
			wOEURL.AddParameter("gameMode", _meta.gameMode);
			wOEURL.AddParameter("time", _time);
			ResponseHandler<HighscoreDataEntry> responseHandler = new ResponseHandler<HighscoreDataEntry>(_okCallback, HighScores.ParseDataEntryJSON);
			return Request.Get(wOEURL, "GAME_FEED_LIST_DOWNLOAD", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC Get(PsMinigameMetaData _meta, Action<ScoreTable> _okCallback, Action<HttpC> _failureCallback, int limit = 100, Action _errorCallback = null)
		{
			WOEURL wOEURL = new WOEURL("/v2/highscore/find");
			wOEURL.AddParameter("gameId", _meta.id);
			wOEURL.AddParameter("gameMode", _meta.gameMode);
			wOEURL.AddParameter("limit", limit);
			ResponseHandler<ScoreTable> responseHandler = new ResponseHandler<ScoreTable>(_okCallback, HighScores.ParseScoreTableJSON);
			return Request.Get(wOEURL, "FIND_HIGHSCORES", responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC GetTournament(string _tournamentId, Action<ScoreTable> _okCallback, Action<HttpC> _failureCallback, Action _errorCallback = null)
		{
			ResponseHandler<ScoreTable> responseHandler = new ResponseHandler<ScoreTable>(_okCallback, HighScores.ParseScoreTableJSON);
			WOEURL wOEURL = new WOEURL("/v2/tournament/score/find");
			wOEURL.AddParameter("tournamentId", _tournamentId);
			return Request.Get(wOEURL, null, responseHandler.RequestOk, _failureCallback, _errorCallback);
		}

		public static HttpC SendTournament(int _time, int _starts, PsTournamentMetaData _meta, DataBlob _dataBlob, Action<ScoreTable> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null)
		{
			ResponseHandler<ScoreTable> responseHandler = new ResponseHandler<ScoreTable>(_okCallback, HighScores.ParseScoreTableJSON);
			WOEURL wOEURL = new WOEURL("/v2/tournament/score/send");
			wOEURL.AddParameter("tournamentId", _meta.tournamentId);
			wOEURL.AddParameter("gameId", _meta.gameId);
			wOEURL.AddParameter("starts", _starts);
			string text = string.Empty;
			if (_time != 0)
			{
				wOEURL.AddParameter("time", _time.ToString());
				text = _time.ToString();
			}
			wOEURL.AddParameter("hash", ClientTools.GenerateHash(_meta.tournamentId + PlayerPrefsX.GetUserId() + text));
			wOEURL.AddParameter("name", WWW.EscapeURL(PlayerPrefsX.GetUserName()));
			wOEURL.AddParameter("returnScores", "true");
			wOEURL.AddParameter("playerUnit", _meta.playerUnit);
			Debug.Log(wOEURL.ConstructURL());
			HttpC httpC = Request.Post(wOEURL, null, (_dataBlob.data == null) ? new byte[1] : _dataBlob.data, responseHandler.RequestOk, _failEventHandler, _errorCallback, null, new string[1] { "Highscore" });
			httpC.objectData = _dataBlob;
			return httpC;
		}

		public static HttpC SendHighscoreAndGhostSafe(HighScoreAndGhostData _data, Action<ScoreTable> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null)
		{
			PsMetricsData.m_lastGhostSize = _data.ghostSize;
			return Send(_data.context, _data.elapsedTime, _data.lastSentTimeScore, _data.lastSentScore, _data.starts, _data.metadata, _data.ghostData, _data.upgradeSum, _data.playerUnit, _okCallback, _failEventHandler, _errorCallback, _data.mainpath, _data.boosterUsed, _data.deathCount);
		}

		private static HttpC Send(PsMinigameContext _context, long _timeSpentInGame, int _time, int _stars, int _starts, PsMinigameMetaData _meta, DataBlob _ghostData, int _upgradeSum, string _playerUnit, Action<ScoreTable> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null, bool _mainPath = false, bool _boost = false, int _deathCount = 0)
		{
			ResponseHandler<ScoreTable> responseHandler = new ResponseHandler<ScoreTable>(_okCallback, HighScores.ParseScoreTableJSON);
			WOEURL wOEURL = new WOEURL("/v5/highscore/send");
			wOEURL.AddParameter("gameId", _meta.id);
			wOEURL.AddParameter("starts", _starts);
			string text = string.Empty;
			if (_time != 0)
			{
				wOEURL.AddParameter("time", _time.ToString());
				text = _time.ToString();
			}
			wOEURL.AddParameter("stars", _stars.ToString());
			wOEURL.AddParameter("hash", ClientTools.GenerateHash(_meta.id + PlayerPrefsX.GetUserId() + text + _stars));
			wOEURL.AddParameter("name", WWW.EscapeURL(PlayerPrefsX.GetUserName()));
			wOEURL.AddParameter("returnScores", "false");
			wOEURL.AddParameter("gameMode", _meta.gameMode);
			wOEURL.AddParameter("mainPath", _mainPath.ToString().ToLower());
			wOEURL.AddParameter("playerUnit", _playerUnit);
			wOEURL.AddParameter("boost", _boost.ToString().ToLower());
			wOEURL.AddParameter("upgradeSum", _upgradeSum.ToString());
			wOEURL.AddParameter("deathCount", _deathCount);
			if (_context == PsMinigameContext.Challenge)
			{
				wOEURL.AddParameter("dailyGem", true.ToString().ToLower());
			}
			Debug.LogWarning(wOEURL.ConstructURL());
			PsMetrics.MinigameCompleted(_context.ToString(), _meta, PsState.m_activeMinigame.m_gameStartCount, _timeSpentInGame, true);
			HttpC httpC = Request.Post(wOEURL, null, (_ghostData.data == null) ? new byte[1] : _ghostData.data, responseHandler.RequestOk, _failEventHandler, _errorCallback, null, new string[1] { "Highscore" });
			httpC.objectData = _ghostData;
			return httpC;
		}

		public static HttpC SendQuit(PsMinigameContext _context, long _timeSpentInGame, int _starts, PsMinigameMetaData _meta, Action<HttpC> _okCallback = null, Action<HttpC> _failEventHandler = null, Action _errorCallback = null, bool _mainPath = false, int _deathCount = 0)
		{
			WOEURL wOEURL = new WOEURL("/v2/highscore/quit");
			wOEURL.AddParameter("gameId", _meta.id);
			wOEURL.AddParameter("starts", _starts);
			wOEURL.AddParameter("hash", ClientTools.GenerateHash(_meta.id + PlayerPrefsX.GetUserId() + _starts));
			wOEURL.AddParameter("gameMode", _meta.gameMode);
			wOEURL.AddParameter("mainPath", _mainPath.ToString().ToLower());
			wOEURL.AddParameter("playerUnit", _meta.playerUnit);
			wOEURL.AddParameter("deathCount", _deathCount);
			PsMetrics.MinigameCompleted(_context.ToString(), _meta, PsState.m_activeMinigame.m_gameStartCount, _timeSpentInGame, _context == PsMinigameContext.Block);
			Debug.LogWarning(wOEURL.ConstructURL());
			return Request.Post(wOEURL, null, _okCallback, _failEventHandler, _errorCallback, null, new string[1] { "Quit" });
		}
	}
}
