using System;
using Server;

public class PsGameModeRaceFriendly : PsGameModeRaceVersus
{
	public PsGameModeRaceFriendly(PsGameLoop _info)
		: base(_info)
	{
	}

	protected override void CreateWinUI()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinRaceFriendly), typeof(PsUITopWin), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Continue", (m_gameLoop as PsGameLoopVersus).NextRound);
		PsIngameMenu.m_popupMenu.SetAction("Forfeit", (m_gameLoop as PsGameLoopVersus).ForfeitChallenge);
	}

	protected override void SendHighscoreAndGhost()
	{
		m_waitForHighscoreAndGhost = true;
		m_waitForNextGhost = true;
		DataBlob dataBlob = default(DataBlob);
		if (m_saveGhost && !m_totalWin)
		{
			if (m_recordingGhost != null && !m_recordingGhost.m_recording)
			{
				dataBlob = ClientTools.CreateGhostDataBlob(m_recordingGhost);
			}
			m_bestGhost = m_recordingGhost.DeepCopy();
			m_bestGhost.m_name = PlayerPrefsX.GetUserName();
			m_bestGhostBlob = dataBlob;
			PsMetricsData.m_lastGhostSize = dataBlob.data.Length;
		}
		if (m_saveGhost && !m_totalWin)
		{
			Friendly.OverwriteScore((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_timeScoreCurrent, PsState.m_activeMinigame.m_gameStartCount, m_gameLoop.m_minigameId, (m_gameLoop as PsGameLoopVersus).m_versusWon, (m_gameLoop as PsGameLoopVersus).m_tryCount >= PsState.m_versusChallengeTryAmount && !(m_gameLoop as PsGameLoopVersus).m_versusWon, GetUpgradeSum(), PsState.m_activeMinigame.m_playerUnitName, dataBlob, FriendlyScoreServerRequestOk, FriendlyScoreServerRequestFAILED, string.Empty);
		}
		else if (m_totalWin)
		{
			(m_gameLoop as PsTimedEventLoop).m_eventOver = true;
			PsMetagameManager.VersusChallengeRewards(true);
			Friendly.Win((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_minigameId, PsState.m_activeMinigame.m_gameStartCount, FriendlyScoreServerRequestOk, FriendlyWinFailed);
		}
		else
		{
			Friendly.Quit((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_minigameId, PsState.m_activeMinigame.m_gameStartCount, (m_gameLoop as PsGameLoopVersus).m_versusWon, (m_gameLoop as PsGameLoopVersus).m_tryCount >= PsState.m_versusChallengeTryAmount && !(m_gameLoop as PsGameLoopVersus).m_versusWon, FriendlyScoreServerRequestOk, FriendlyQuitFailed, string.Empty);
		}
		if ((m_gameLoop as PsGameLoopVersus).m_versusWon || (m_gameLoop as PsGameLoopVersus).m_tryCount >= PsState.m_versusChallengeTryAmount)
		{
			(m_gameLoop as PsGameLoopVersus).m_roundInfoSent = true;
		}
	}

	public override HttpC ServerGetGhosts(GhostGetData _data, Action<GhostData[]> _customSUCCESSCallback)
	{
		Action<GhostData[]> okCallback = GhostsLoadSUCCEED;
		if (_customSUCCESSCallback != null)
		{
			okCallback = _customSUCCESSCallback;
		}
		HttpC friendly = Server.Ghost.GetFriendly(_data.versusParticipant.score.playerId, (_data.minigameInfo as PsGameLoopVersus).m_timedEventId, okCallback, GhostsLoadFAILED, base.GhostLoadErrorHandler);
		friendly.objectData = _data;
		CreateGhostLoaderEntity();
		EntityManager.AddComponentToEntity(m_ghostLoaderEntity, friendly);
		return friendly;
	}

	private void FriendlyScoreServerRequestOk(HttpC _c)
	{
		if (m_saveGhost)
		{
			PsMetricsData.m_isSendingGhost = false;
			PsMetrics.GhostSaved(m_gameLoop.m_context.ToString(), PsMetricsData.m_lastGhostSize.ToString(), m_gameLoop.m_minigameMetaData.id, PsMetricsData.m_isNewGhost);
		}
		PsState.m_activeMinigame.m_gameStartCount = 0;
		m_waitForHighscoreAndGhost = false;
		if (m_saveGhost && m_gameLoop.m_versusData.GetOpponentInfo() == null)
		{
			LoadGhosts(base.NewGhostLoaded);
		}
		else
		{
			NewGhostLoaded();
		}
	}

	private void FriendlyScoreServerRequestFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Friendly.OverwriteScore((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_timeScoreCurrent, PsState.m_activeMinigame.m_gameStartCount, m_gameLoop.m_minigameId, (m_gameLoop as PsGameLoopVersus).m_versusWon, (m_gameLoop as PsGameLoopVersus).m_tryCount >= PsState.m_versusChallengeTryAmount && !(m_gameLoop as PsGameLoopVersus).m_versusWon, GetUpgradeSum(), PsState.m_activeMinigame.m_playerUnitName, m_bestGhostBlob, FriendlyScoreServerRequestOk, FriendlyScoreServerRequestFAILED, string.Empty));
	}

	private void FriendlyQuitFailed(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Friendly.Quit((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_minigameId, PsState.m_activeMinigame.m_gameStartCount, (m_gameLoop as PsGameLoopVersus).m_versusWon, (m_gameLoop as PsGameLoopVersus).m_tryCount >= PsState.m_versusChallengeTryAmount && !(m_gameLoop as PsGameLoopVersus).m_versusWon, FriendlyScoreServerRequestOk, FriendlyQuitFailed, string.Empty));
	}

	private void FriendlyWinFailed(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Friendly.Win((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_minigameId, PsState.m_activeMinigame.m_gameStartCount, FriendlyScoreServerRequestOk, FriendlyWinFailed));
	}
}
