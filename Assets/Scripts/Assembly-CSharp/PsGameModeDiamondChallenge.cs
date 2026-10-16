using System;
using System.Collections;
using UnityEngine;

public class PsGameModeDiamondChallenge : PsGameModeStarCollect
{
	public PsGameModeDiamondChallenge(PsGameLoop _info)
		: base(PsGameMode.Any, _info)
	{
	}

	public override void CreatePlayMenu(Action _restartAction, Action _pauseAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_playMenu = new PsUITopPlayDiamondChallenge(_restartAction, _pauseAction);
		PsIngameMenu.OpenController();
	}

	public override void WinMinigame()
	{
		m_saveGhost = false;
		m_gameLoop.m_timeScoreCurrent = HighScores.TicksToTimeScore(Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks), true);
		float num = HighScores.TimeScoreToTime(m_gameLoop.m_timeScoreCurrent);
		Debug.LogWarning("YOUR TIME: " + num);
		int collectedStars = PsState.m_activeMinigame.m_collectedStars;
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
		Debug.LogWarning("SEND SCORE TO SERVER! time: " + HighScores.TimeScoreToTime(m_gameLoop.m_timeScoreBest) + ", score: " + m_gameLoop.m_scoreBest);
		Debug.LogWarning("OLD TIME: " + HighScores.TimeScoreToTime(m_gameLoop.m_timeScoreOld));
		Debug.LogWarning("OLD SCORE: " + m_gameLoop.m_scoreOld);
		SendHighscoreAndGhost();
		PsAchievementManager.SendToServer();
		CreateWinUI();
		if (m_gameLoop.m_scoreCurrent > m_gameLoop.m_rewardOld)
		{
			int diamonds = (m_gameLoop.m_scoreCurrent - m_gameLoop.m_rewardOld) * PlayerPrefsX.GetDailyGemMultiplier();
			PsMetagameManager.GiveGoalReward(0, diamonds, m_gameLoop.GetDifficulty());
			m_gameLoop.m_rewardOld = m_gameLoop.m_scoreCurrent;
			PsMetagameManager.m_playerStats.updated = false;
		}
		PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
	}

	public override void EnterMinigame()
	{
		PsIngameMenu.CloseAll();
		PsMetagameManager.ShowResources(null, true, true, true, true, false);
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUIVehicleSelect), typeof(PsUITopStartChallenge), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.OpenController(true);
	}

	protected override void CreateWinUI()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinDiamondChallenge), typeof(PsUITopWin), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
	}

	public override void PauseMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterPauseRace), typeof(PsUITopPauseChallenge), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Resume", m_gameLoop.ResumeMinigame);
	}
}
