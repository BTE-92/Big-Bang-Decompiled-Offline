using System;
using Server;
using UnityEngine;

public class PsGameModeStarCollect : PsGameModeBase
{
	public bool m_saveGhost;

	protected PsGameModeStarCollect(PsGameMode _gameMode, PsGameLoop _info)
		: base(_gameMode, _info)
	{
	}

	public PsGameModeStarCollect(PsGameLoop _info)
		: base(PsGameMode.StarCollect, _info)
	{
	}

	public override void LoadGhosts(Action<GhostData[]> _customSUCCESSCallback = null)
	{
		GhostGetData ghostGetData = new GhostGetData();
		ghostGetData.minigameInfo = PsState.m_activeGameLoop;
		ghostGetData.times = new int[1] { 2147483647 };
		ghostGetData.playerId = m_gameLoop.GetCreatorId();
		ServerGetGhosts(ghostGetData, _customSUCCESSCallback);
	}

	public override void CreatePlaybackGhostVisuals()
	{
	}

	public override void CreateMusic()
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		if (UnityEngine.Random.Range(0, 2) < 1)
		{
			minigame.m_music = SoundS.AddComponent(minigame.m_environmentTC, PsFMODManager.GetMusic("PuzzleMusic1"), 1f, true);
		}
		else
		{
			minigame.m_music = SoundS.AddComponent(minigame.m_environmentTC, PsFMODManager.GetMusic("PuzzleMusic2"), 1f, true);
		}
		SoundS.PlaySound(minigame.m_music, true);
		SoundS.SetSoundParameter(minigame.m_music, "MusicState", 0f);
	}

	public override void CreatePlayMenu(Action _restartAction, Action _pauseAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_playMenu = new PsUITopPlayStarCollect(_restartAction, _pauseAction);
		PsIngameMenu.OpenController();
	}

	public override void WinMinigame()
	{
		m_saveGhost = false;
		m_gameLoop.m_timeScoreCurrent = HighScores.TicksToTimeScore(Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks), true);
		float num = HighScores.TimeScoreToTime(m_gameLoop.m_timeScoreCurrent);
		Debug.LogWarning("YOUR TIME: " + num);
		int collectedStars = PsState.m_activeMinigame.m_collectedStars;
		if (collectedStars == 3)
		{
			PsAchievementManager.IncrementProgress("gainFiftyGoldMedals", 1);
		}
		m_gameLoop.m_currentRunScore = collectedStars;
		if (collectedStars >= m_gameLoop.m_scoreCurrent)
		{
			m_gameLoop.m_scoreCurrent = collectedStars;
			if (m_gameLoop.m_scoreCurrent > m_gameLoop.m_scoreBest)
			{
				m_gameLoop.m_scoreBest = m_gameLoop.m_scoreCurrent;
				m_gameLoop.m_minigameMetaData.score = m_gameLoop.m_scoreBest;
				m_gameLoop.m_timeScoreBest = m_gameLoop.m_timeScoreCurrent;
				m_gameLoop.m_minigameMetaData.timeScore = m_gameLoop.m_timeScoreBest;
				PsMetricsData.m_isSendingGhost = true;
				PsMetricsData.m_isNewGhost = true;
				m_saveGhost = true;
			}
			else if (m_gameLoop.m_scoreCurrent == m_gameLoop.m_scoreBest && m_gameLoop.m_timeScoreCurrent < m_gameLoop.m_timeScoreBest)
			{
				m_gameLoop.m_timeScoreBest = m_gameLoop.m_timeScoreCurrent;
				m_gameLoop.m_minigameMetaData.timeScore = m_gameLoop.m_timeScoreBest;
				PsMetricsData.m_isSendingGhost = true;
				PsMetricsData.m_isNewGhost = true;
				m_saveGhost = true;
			}
		}
		CheckAchievementsOnWin();
		Debug.LogWarning("SEND SCORE TO SERVER! time: " + HighScores.TimeScoreToTime(m_gameLoop.m_timeScoreBest) + ", score: " + m_gameLoop.m_scoreBest);
		Debug.LogWarning("OLD TIME: " + HighScores.TimeScoreToTime(m_gameLoop.m_timeScoreOld));
		Debug.LogWarning("OLD SCORE: " + m_gameLoop.m_scoreOld);
		SendHighscoreAndGhost();
		CreateWinUI();
		base.WinMinigame();
	}

	protected override void CreateWinUI()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinStarCollect), typeof(PsUITopWin), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
	}

	public override void PauseMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterPauseRace), typeof(PsUITopPauseStarCollect), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Skip", m_gameLoop.SkipMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Resume", m_gameLoop.ResumeMinigame);
	}

	protected HttpC SendHighscoreAndGhost()
	{
		m_waitForHighscoreAndGhost = true;
		m_waitForNextGhost = false;
		DataBlob ghostData = default(DataBlob);
		if (m_saveGhost)
		{
			if (m_recordingGhost != null && !m_recordingGhost.m_recording)
			{
				ghostData = ClientTools.CreateGhostDataBlob(m_recordingGhost);
			}
			PsMetricsData.m_lastGhostSize = ghostData.data.Length;
		}
		else
		{
			PsMetricsData.m_lastGhostSize = 0;
		}
		Debug.LogWarning("LAST SENT GHOST SIZE: " + PsMetricsData.m_lastGhostSize);
		bool mainpath = m_gameLoop.m_path != null && m_gameLoop.m_path.m_startNodeId == 0;
		return SendHighscoreAndGhostSafe(new HighScore.HighScoreAndGhostData(PsState.m_activeGameLoop.m_context, PsState.m_activeGameLoop.ElapsedTime(), ghostData, PsMetricsData.m_lastGhostSize, PsState.m_activeGameLoop.m_minigameMetaData, m_gameLoop.m_timeScoreCurrent, m_gameLoop.m_currentRunScore, m_gameLoop.m_boosterUsed, PsState.m_activeMinigame.m_playerUnitName, mainpath, GetUpgradeSum(), PsState.m_activeMinigame.m_gameDeathCount, PsState.m_activeMinigame.m_gameStartCount));
	}

	protected HttpC SendHighscoreAndGhostSafe(HighScore.HighScoreAndGhostData _data)
	{
		PsMetricsData.m_lastGhostSize = _data.ghostSize;
		return HighScore.SendHighscoreAndGhostSafe(_data, HighscoreServerRequestOK, delegate(HttpC c)
		{
			HighscoreServerRequestFAILED(c, _data);
		});
	}

	protected void HighscoreServerRequestFAILED(HttpC _c, HighScore.HighScoreAndGhostData _data)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => SendHighscoreAndGhostSafe(_data));
	}

	protected void HighscoreServerRequestOK(ScoreTable _data)
	{
		Debug.LogWarning("HIGHSCORES AND GHOST SENT, LOADING NEW GHOST");
		PsState.m_activeMinigame.m_gameStartCount = 0;
		PsMetricsData.m_isSendingGhost = false;
		if (m_saveGhost)
		{
			PsMetrics.GhostSaved(m_gameLoop.m_context.ToString(), PsMetricsData.m_lastGhostSize.ToString(), m_gameLoop.m_minigameMetaData.id, PsMetricsData.m_isNewGhost);
		}
		m_waitForHighscoreAndGhost = false;
	}
}
