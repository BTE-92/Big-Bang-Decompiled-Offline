using System;
using Server;
using UnityEngine;

public class PsGameModeRace : PsGameModeBase
{
	public float m_threeMedalTime;

	public float m_twoMedalTime;

	public float m_oneMedalTime;

	public bool m_giveThreeStars;

	public bool m_saveGhost;

	public PsGameModeRace(PsGameLoop _info)
		: base(PsGameMode.Race, _info)
	{
	}

	public void CalculateMedalTimes()
	{
		Debug.LogWarning(m_gameLoop.m_minigameMetaData.bestTime);
		Debug.LogWarning(m_gameLoop.m_minigameMetaData.medianTime);
		float num = HighScores.TimeScoreToTime(m_gameLoop.m_minigameMetaData.bestTime);
		float num2 = HighScores.TimeScoreToTime(m_gameLoop.m_minigameMetaData.medianTime);
		m_giveThreeStars = false;
		if (num <= 0f || num2 < num)
		{
			m_giveThreeStars = true;
			Debug.LogWarning("Something went wrong with medal times! Best: " + num + "/ Median: " + num2);
		}
		if (num2 == 0f)
		{
			num2 = num * 1.25f;
		}
		else if (num2 == num)
		{
			num2 *= 1.25f;
		}
		float result = num2 - (num2 - num) * 0.5f;
		float result2 = num2 + (num2 - num) * 0.3f;
		float result3 = num2 + (num2 - num) * 5f;
		if (m_gameLoop.m_minigameMetaData.totalWinners < 5)
		{
			result = num + num * 0.25f;
			result2 = num + num * 0.5f;
			result3 = num * 3f;
		}
		if (m_gameLoop.m_forcedMedalTimes != null && m_gameLoop.m_forcedMedalTimes.Length == 3 && !string.IsNullOrEmpty(m_gameLoop.m_forcedMedalTimes[0]) && !string.IsNullOrEmpty(m_gameLoop.m_forcedMedalTimes[1]) && !string.IsNullOrEmpty(m_gameLoop.m_forcedMedalTimes[2]))
		{
			Debug.Log("USE PRESET MEDAL TIMES");
			m_giveThreeStars = false;
			float.TryParse(m_gameLoop.m_forcedMedalTimes[2], out result3);
			if (result3 == 0f)
			{
				result3 = 60f;
			}
			float.TryParse(m_gameLoop.m_forcedMedalTimes[1], out result2);
			if (result2 == 0f)
			{
				result2 = result3 - 1f;
			}
			float.TryParse(m_gameLoop.m_forcedMedalTimes[0], out result);
			if (result == 0f)
			{
				result = result2 - 1f;
			}
		}
		if (!m_giveThreeStars)
		{
			m_threeMedalTime = result;
			m_twoMedalTime = result2;
			m_oneMedalTime = result3;
		}
		else
		{
			m_threeMedalTime = 2.1474836E+09f;
			m_twoMedalTime = 2.1474836E+09f;
			m_oneMedalTime = 2.1474836E+09f;
		}
		Debug.LogWarning("SCORE LIMITS: " + m_oneMedalTime + ", " + m_twoMedalTime + ", " + m_threeMedalTime);
	}

	public override void CreateMusic()
	{
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		if (UnityEngine.Random.Range(0, 2) < 1)
		{
			minigame.m_music = SoundS.AddComponent(minigame.m_environmentTC, PsFMODManager.GetMusic("RacingMusic1"), 1f, true);
		}
		else
		{
			minigame.m_music = SoundS.AddComponent(minigame.m_environmentTC, "/Music/RacingMusic2", 1f, true);
		}
		SoundS.PlaySound(minigame.m_music, true);
		SoundS.SetSoundParameter(minigame.m_music, "MusicState", 0f);
	}

	public override void ApplySettings(bool _createAreaLimits = true, bool _updateDomeGfx = false)
	{
		base.ApplySettings(_createAreaLimits, _updateDomeGfx);
	}

	public override void LoadGhosts(Action<GhostData[]> _customSUCCESSCallback = null)
	{
		GhostGetData ghostGetData = new GhostGetData();
		ghostGetData.minigameInfo = PsState.m_activeGameLoop;
		ghostGetData.times = GetGhostTimes();
		ServerGetGhosts(ghostGetData, _customSUCCESSCallback);
	}

	public override void GhostsLoaded(GhostData[] _data)
	{
		Debug.LogWarning("ghosts searched with times: " + m_threeMedalTime + " " + m_twoMedalTime + " " + m_oneMedalTime);
		base.GhostsLoaded(_data);
		if (m_ghostScores.Count == 3)
		{
			m_oneMedalTime = HighScores.TimeScoreToTime(m_ghostScores[0]);
			m_twoMedalTime = HighScores.TimeScoreToTime(m_ghostScores[1]);
			m_threeMedalTime = HighScores.TimeScoreToTime(m_ghostScores[2]);
		}
		else if (m_ghostScores.Count == 2)
		{
			m_twoMedalTime = HighScores.TimeScoreToTime(m_ghostScores[0]);
			m_threeMedalTime = HighScores.TimeScoreToTime(m_ghostScores[1]);
		}
		else if (m_ghostScores.Count == 1)
		{
			m_threeMedalTime = HighScores.TimeScoreToTime(m_ghostScores[0]);
		}
		Debug.LogWarning("ghost times: " + m_oneMedalTime + ", " + m_twoMedalTime + ", " + m_threeMedalTime);
	}

	public int[] GetGhostTimes()
	{
		int[] array = new int[3];
		if (m_giveThreeStars)
		{
			array[0] = int.MaxValue;
			array[1] = int.MaxValue;
			array[2] = int.MaxValue;
		}
		else
		{
			array[0] = HighScores.TimeToTimeScore(m_oneMedalTime);
			array[1] = HighScores.TimeToTimeScore(m_twoMedalTime);
			array[2] = HighScores.TimeToTimeScore(m_threeMedalTime);
		}
		return array;
	}

	public override void CreatePlayMenu(Action _restartAction, Action _pauseAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_playMenu = new PsUITopPlayRace(_restartAction, _pauseAction);
		PsIngameMenu.OpenController();
	}

	public override void WinMinigame()
	{
		m_saveGhost = false;
		m_gameLoop.m_timeScoreCurrent = HighScores.TicksToTimeScore(Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks), true);
		float num = HighScores.TimeScoreToTime(m_gameLoop.m_timeScoreCurrent);
		Debug.LogWarning("YOUR TIME: " + num + ", SCORE LIMITS: " + m_threeMedalTime + ", " + m_twoMedalTime + ", " + m_oneMedalTime);
		int num2 = 0;
		if (num <= m_threeMedalTime)
		{
			PsAchievementManager.IncrementProgress("gainFiftyGoldMedals", 1);
			num2 = 3;
		}
		else if (num <= m_twoMedalTime)
		{
			num2 = 2;
		}
		else if (num <= m_oneMedalTime)
		{
			num2 = 1;
		}
		if (m_gameLoop.m_timeScoreCurrent < m_gameLoop.m_timeScoreBest)
		{
			m_gameLoop.m_timeScoreBest = m_gameLoop.m_timeScoreCurrent;
			m_gameLoop.m_minigameMetaData.timeScore = m_gameLoop.m_timeScoreBest;
			PsMetricsData.m_isSendingGhost = true;
			PsMetricsData.m_isNewGhost = true;
			m_saveGhost = true;
		}
		m_gameLoop.m_currentRunScore = num2;
		if (num2 > m_gameLoop.m_scoreCurrent)
		{
			m_gameLoop.m_scoreCurrent = num2;
			if (m_gameLoop.m_scoreCurrent > m_gameLoop.m_scoreBest)
			{
				m_gameLoop.m_scoreBest = m_gameLoop.m_scoreCurrent;
				m_gameLoop.m_minigameMetaData.score = m_gameLoop.m_scoreBest;
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
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinRace), typeof(PsUITopWin), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
	}

	public override void PauseMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterPauseRace), typeof(PsUITopPauseRace), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Skip", m_gameLoop.SkipMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Resume", m_gameLoop.ResumeMinigame);
	}

	private HttpC SendHighscoreAndGhost()
	{
		m_waitForHighscoreAndGhost = true;
		m_waitForNextGhost = true;
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

	private void HighscoreServerRequestFAILED(HttpC _c, HighScore.HighScoreAndGhostData _data)
	{
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => SendHighscoreAndGhostSafe(_data));
	}

	private void HighscoreServerRequestOK(ScoreTable _data)
	{
		Debug.LogWarning("HIGHSCORES AND GHOST SENT, LOADING NEW GHOST");
		if (m_saveGhost)
		{
			PsMetricsData.m_isSendingGhost = false;
			PsMetrics.GhostSaved(m_gameLoop.m_context.ToString(), PsMetricsData.m_lastGhostSize.ToString(), m_gameLoop.m_minigameMetaData.id, PsMetricsData.m_isNewGhost);
		}
		PsState.m_activeMinigame.m_gameStartCount = 0;
		m_waitForHighscoreAndGhost = false;
		m_waitForNextGhost = false;
	}

	protected void NewGhostLoaded()
	{
		m_waitForNextGhost = false;
		Debug.LogWarning("NEW GHOST LOADED (not really...)");
	}
}
