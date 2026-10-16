using System;
using System.Collections;
using System.Collections.Generic;
using Server;

public static class PsServerRequest
{
	public static HttpC GiveLevelTemplate(string _templateId, Hashtable _ht, Action<PsMinigameMetaData> _okCallback, Action<HttpC> _failCallback, Action _errorCallbacl = null)
	{
		HttpC httpC = Template.Duplicate(_templateId, _ht, _okCallback, _failCallback);
		httpC.objectData = _templateId;
		EntityManager.AddComponentToEntity(PsMetagameManager.m_utilityEntity, httpC);
		return httpC;
	}

	public static HttpC ServerParticipateTournament(PsTournamentMetaData _data, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Tournament.Participate(_data.tournamentId, _okCallback, _failCallback);
		httpC.objectData = _data;
		EntityManager.AddComponentToEntity(PsMetagameManager.m_utilityEntity, httpC);
		return httpC;
	}

	public static HttpC ServerClaimTournament(PsTournamentMetaData _data, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Tournament.TournamentClaim(_data, _okCallback, _failCallback);
		httpC.objectData = _data;
		EntityManager.AddComponentToEntity(PsMetagameManager.m_utilityEntity, httpC);
		return httpC;
	}

	public static HttpC ServerClaimMinigame(PsMinigameMetaData _data, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = MiniGame.ClaimReward(_data.id, _okCallback, _failCallback);
		httpC.objectData = _data;
		EntityManager.AddComponentToEntity(PsMetagameManager.m_utilityEntity, httpC);
		return httpC;
	}

	public static HttpC ServerSendRating(PsMetagameManager.SaveRatingData _data, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Rating.Save(_data, _okCallback, _failCallback);
		httpC.objectData = _data;
		EntityManager.AddComponentToEntity(PsMetagameManager.m_utilityEntity, httpC);
		return httpC;
	}

	public static HttpC ServerPlayerSetData(Hashtable _setData, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Player.SetData(_setData, null, null, null, _okCallback, _failCallback, _errorCallback);
		httpC.objectData = _setData;
		return httpC;
	}

	public static HttpC ServerPlayerSetDataAndProgression(Hashtable _setData, Hashtable _pathJson, Hashtable _customisations, List<Dictionary<string, object>> _achievements, bool _sendMetrics, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		Player.ProgressionAndSetDataObject progressionAndSetDataObject = new Player.ProgressionAndSetDataObject();
		progressionAndSetDataObject.pathJson = _pathJson;
		progressionAndSetDataObject.setData = _setData;
		progressionAndSetDataObject.customisations = _customisations;
		progressionAndSetDataObject.achievements = _achievements;
		progressionAndSetDataObject.sendMetrics = _sendMetrics;
		HttpC httpC = Player.SetData(progressionAndSetDataObject.setData, progressionAndSetDataObject.pathJson, progressionAndSetDataObject.customisations, progressionAndSetDataObject.achievements, _okCallback, _failCallback, _errorCallback);
		httpC.objectData = progressionAndSetDataObject;
		return httpC;
	}

	public static HttpC ServerChangeName(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		return Player.ChangeName(_okCallback, _failCallback, _errorCallback);
	}

	public static HttpC ServerSendQuit(SendQuitData _sendQuitData, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null, bool _mainPath = false)
	{
		HttpC httpC = HighScore.SendQuit(_sendQuitData.gameLoop.m_context, _sendQuitData.gameLoop.ElapsedTime(), _sendQuitData.startCount, _sendQuitData.gameLoop.m_minigameMetaData, _okCallback, _failCallback, _errorCallback, _mainPath);
		httpC.objectData = _sendQuitData;
		return httpC;
	}

	public static HttpC ServerGetFriends(string _playerId, Action<Friends> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Player.Friends(_okCallback, _failCallback, _playerId, _errorCallback);
		httpC.objectData = _playerId;
		return httpC;
	}

	public static HttpC ServerGetFollowees(string _playerId, Action<PlayerData[]> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Player.Followees(_okCallback, _failCallback, _playerId, _errorCallback);
		httpC.objectData = _playerId;
		return httpC;
	}

	public static HttpC ServerGetFollowers(string _playerId, Action<PlayerData[]> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Player.Followers(_okCallback, _failCallback, _playerId, _errorCallback);
		httpC.objectData = _playerId;
		return httpC;
	}

	public static HttpC ServerFollow(string _playerId, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Player.Follow(_playerId, _okCallback, _failCallback, _errorCallback);
		httpC.objectData = _playerId;
		return httpC;
	}

	public static HttpC ServerUnFollow(string _playerId, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Player.UnFollow(_playerId, _okCallback, _failCallback, _errorCallback);
		httpC.objectData = _playerId;
		return httpC;
	}

	public static HttpC ServerComment(PsMetagameManager.CommentInfo _info, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Comment.Save(_info.gameId, _info.comment, _info.tag, _info.tournamentId, _okCallback, _failCallback, _errorCallback);
		httpC.objectData = _info;
		return httpC;
	}

	public static HttpC ServerAchievement(string _json, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Server.Achievement.Upsert(_json, _okCallback, _failCallback, _errorCallback);
		httpC.objectData = _json;
		return httpC;
	}

	public static HttpC ServerSaveProgression(SendPathData _data, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Player.SetData(null, _data.jsonData, null, null, _okCallback, _failCallback, _errorCallback);
		httpC.objectData = _data;
		return httpC;
	}

	public static HttpC ServerResetProgression(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		return Path.ResetProgression(_okCallback, _failCallback, _errorCallback);
	}

	public static HttpC ServerPlayerSkipLevel(string _playerUnit, Hashtable _setData, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = Player.SkipLevel(_playerUnit, _setData, _okCallback, _failCallback, _errorCallback);
		httpC.objectData = _playerUnit;
		return httpC;
	}

	public static HttpC ServerDeleteMinigame(string _minigameId, Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		HttpC httpC = MiniGame.Delete(_minigameId, _okCallback, _failCallback, _errorCallback);
		httpC.objectData = _minigameId;
		return httpC;
	}

	public static HttpC ServerClaimAllMinigames(Action<HttpC> _okCallback, Action<HttpC> _failCallback, Action _errorCallback = null)
	{
		return MiniGame.ClaimAllRewards(_okCallback, _failCallback, _errorCallback);
	}
}
