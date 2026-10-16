using System;
using Server;
using UnityEngine;

public class PsGameModeRaceVersus : PsGameModeRace
{
	public DataBlob m_bestGhostBlob = default(DataBlob);

	public Ghost m_bestGhost;

	public int m_previousBest;

	public int m_bestVersusTime;

	public bool m_totalWin;

	public PsGameModeRaceVersus(PsGameLoop _info)
		: base(_info)
	{
	}

	public override void GiveRewards()
	{
	}

	public override void CreateMusic()
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		minigame.m_music = SoundS.AddComponent(minigame.m_environmentTC, PsFMODManager.GetMusic("ChallengeMusic"), 1f, true);
		SoundS.PlaySound(minigame.m_music, true);
		SoundS.SetSoundParameter(minigame.m_music, "MusicState", 0f);
	}

	public override void LoadGhosts(Action<GhostData[]> _customSUCCESSCallback = null)
	{
		VersusParticipant ownInfo = m_gameLoop.m_versusData.GetOwnInfo();
		VersusParticipant opponentInfo = m_gameLoop.m_versusData.GetOpponentInfo();
		if (opponentInfo != null && opponentInfo.score.time > 0)
		{
			DestroyPlaybackGhosts();
			GhostGetData ghostGetData = new GhostGetData();
			ghostGetData.minigameInfo = PsState.m_activeGameLoop;
			ghostGetData.versusParticipant = opponentInfo;
			ServerGetGhosts(ghostGetData, _customSUCCESSCallback);
		}
		else if (opponentInfo == null)
		{
			if (ownInfo.score.time > 0 || m_bestVersusTime > 0)
			{
				GhostGetData ghostGetData2 = new GhostGetData();
				ghostGetData2.minigameInfo = PsState.m_activeGameLoop;
				ghostGetData2.versusParticipant = ownInfo;
				ServerGetGhosts(ghostGetData2, _customSUCCESSCallback);
			}
			else
			{
				m_gameLoop.GhostsLoaded();
			}
		}
		else
		{
			m_gameLoop.GhostsLoaded();
		}
	}

	public override HttpC ServerGetGhosts(GhostGetData _data, Action<GhostData[]> _customSUCCESSCallback)
	{
		Action<GhostData[]> okCallback = GhostsLoadSUCCEED;
		if (_customSUCCESSCallback != null)
		{
			okCallback = _customSUCCESSCallback;
		}
		HttpC versusNew = Server.Ghost.GetVersusNew(_data.versusParticipant.score.playerId, (_data.minigameInfo as PsGameLoopVersus).m_timedEventId, okCallback, GhostsLoadFAILED, base.GhostLoadErrorHandler);
		versusNew.objectData = _data;
		CreateGhostLoaderEntity();
		EntityManager.AddComponentToEntity(m_ghostLoaderEntity, versusNew);
		return versusNew;
	}

	public override void WinMinigame()
	{
		m_saveGhost = false;
		m_totalWin = false;
		m_gameLoop.m_timeScoreCurrent = HighScores.TicksToTimeScore(Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks), true);
		float num = HighScores.TimeScoreToTime(m_gameLoop.m_timeScoreCurrent);
		if (m_gameLoop.m_versusData.GetOpponentInfo() == null || m_gameLoop.m_versusData.GetOpponentInfo().score.time == 0 || m_gameLoop.m_timeScoreCurrent < m_gameLoop.m_versusData.GetOpponentInfo().score.time)
		{
			m_gameLoop.m_versusData.currentPlayer = string.Empty;
			if (m_gameLoop.m_versusData.GetOpponentInfo() != null)
			{
				m_saveGhost = true;
				m_bestVersusTime = m_gameLoop.m_timeScoreCurrent;
				(m_gameLoop as PsGameLoopVersus).m_versusWon = true;
				(m_gameLoop as PsGameLoopVersus).m_versusData.currentPlayer = m_gameLoop.m_versusData.GetOpponentInfo().score.playerId;
			}
			else if (m_gameLoop.m_timeScoreCurrent < m_bestVersusTime)
			{
				m_saveGhost = true;
				m_previousBest = m_bestVersusTime;
				m_bestVersusTime = m_gameLoop.m_timeScoreCurrent;
			}
			if (m_gameLoop.m_versusData.matchBall)
			{
				m_totalWin = true;
			}
		}
		else if (m_gameLoop.m_timeScoreCurrent < m_bestVersusTime)
		{
			m_saveGhost = true;
			m_previousBest = m_bestVersusTime;
			m_bestVersusTime = m_gameLoop.m_timeScoreCurrent;
		}
		SendHighscoreAndGhost();
		CreateWinUI();
		PsAchievementManager.SendToServer();
	}

	protected virtual void SendHighscoreAndGhost()
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
		bool flag = false;
		if (m_saveGhost && !m_totalWin)
		{
			Versus.OverwriteScore((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_timeScoreCurrent, PsState.m_activeMinigame.m_gameStartCount, m_gameLoop.m_minigameId, (m_gameLoop as PsGameLoopVersus).m_versusWon, (m_gameLoop as PsGameLoopVersus).m_tryCount >= PsState.m_versusChallengeTryAmount && !(m_gameLoop as PsGameLoopVersus).m_versusWon, GetUpgradeSum(), PsState.m_activeMinigame.m_playerUnitName, dataBlob, VersusScoreServerRequestOk, VersusScoreServerRequestFAILED);
		}
		else if (m_totalWin)
		{
			(m_gameLoop as PsTimedEventLoop).m_eventOver = true;
			PsMetagameManager.VersusChallengeRewards(true);
			Versus.Win((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_minigameId, PsState.m_activeMinigame.m_gameStartCount, VersusScoreServerRequestOk, VersusWinFailed);
		}
		else
		{
			Versus.Quit((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_minigameId, PsState.m_activeMinigame.m_gameStartCount, (m_gameLoop as PsGameLoopVersus).m_versusWon, (m_gameLoop as PsGameLoopVersus).m_tryCount >= PsState.m_versusChallengeTryAmount && !(m_gameLoop as PsGameLoopVersus).m_versusWon, VersusScoreServerRequestOk, VersusQuitFailed);
		}
		if ((m_gameLoop as PsGameLoopVersus).m_versusWon || (m_gameLoop as PsGameLoopVersus).m_tryCount >= PsState.m_versusChallengeTryAmount)
		{
			(m_gameLoop as PsGameLoopVersus).m_roundInfoSent = true;
		}
	}

	private void VersusScoreServerRequestOk(HttpC _c)
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
			LoadGhosts(NewGhostLoaded);
		}
		else
		{
			NewGhostLoaded();
		}
	}

	private void VersusScoreServerRequestFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Versus.OverwriteScore((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_timeScoreCurrent, PsState.m_activeMinigame.m_gameStartCount, m_gameLoop.m_minigameId, (m_gameLoop as PsGameLoopVersus).m_versusWon, (m_gameLoop as PsGameLoopVersus).m_tryCount >= PsState.m_versusChallengeTryAmount && !(m_gameLoop as PsGameLoopVersus).m_versusWon, GetUpgradeSum(), PsState.m_activeMinigame.m_playerUnitName, m_bestGhostBlob, VersusScoreServerRequestOk, VersusScoreServerRequestFAILED));
	}

	private void VersusQuitFailed(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Versus.Quit((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_minigameId, PsState.m_activeMinigame.m_gameStartCount, (m_gameLoop as PsGameLoopVersus).m_versusWon, (m_gameLoop as PsGameLoopVersus).m_tryCount >= PsState.m_versusChallengeTryAmount && !(m_gameLoop as PsGameLoopVersus).m_versusWon, VersusScoreServerRequestOk, VersusQuitFailed));
	}

	private void VersusWinFailed(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Versus.Win((m_gameLoop as PsTimedEventLoop).m_timedEventId, m_gameLoop.m_minigameId, PsState.m_activeMinigame.m_gameStartCount, VersusScoreServerRequestOk, VersusWinFailed));
	}

	protected void NewGhostLoaded(GhostData[] _data)
	{
		DestroyGhostLoaderEntity();
		m_waitForNextGhost = false;
		Debug.LogWarning("NEW GHOST LOADED");
		DestroyPlaybackGhosts();
		GhostsLoadSUCCEED(_data);
	}

	public override void ApplySettings(bool _createAreaLimits, bool _updateDomeGfx)
	{
		PsState.m_activeMinigame.ApplySettings(_createAreaLimits, _updateDomeGfx);
	}

	public override void EnterMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUIVehicleSelect), typeof(PsUITopStartVersus), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsMetagameManager.ShowResources(PsIngameMenu.m_popupMenu.m_overlayCamera, true, true, true, false, false, 0.03f, true);
		PsIngameMenu.OpenController(true);
	}

	public override void CreatePlayMenu(Action _restartAction, Action _pauseAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_playMenu = new PsUITopPlayRaceVersus(_restartAction, _pauseAction);
		PsIngameMenu.OpenController();
	}

	protected override void CreateWinUI()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinRaceVersus), typeof(PsUITopWin), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Continue", (m_gameLoop as PsGameLoopVersus).NextRound);
		PsIngameMenu.m_popupMenu.SetAction("Forfeit", (m_gameLoop as PsGameLoopVersus).ForfeitChallenge);
	}

	public override void PauseMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterPauseRace), typeof(PsUITopPauseVersus), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Resume", m_gameLoop.ResumeMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Forfeit", (m_gameLoop as PsGameLoopVersus).ForfeitChallenge);
	}

	public override void LoseMinigame()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterLoseRace), typeof(PsUITopLoseRaceVersus), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Forfeit", (m_gameLoop as PsGameLoopVersus).ForfeitChallenge);
	}
}
