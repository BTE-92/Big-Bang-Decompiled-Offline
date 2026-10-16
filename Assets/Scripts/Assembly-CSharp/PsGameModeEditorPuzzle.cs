using System;
using UnityEngine;

public class PsGameModeEditorPuzzle : PsGameModeStarCollect
{
	public PsGameModeEditorPuzzle(PsGameLoop _info)
		: base(_info)
	{
	}

	public override void WinMinigame()
	{
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
			}
			else if (m_gameLoop.m_scoreCurrent == m_gameLoop.m_scoreBest && m_gameLoop.m_timeScoreCurrent < m_gameLoop.m_timeScoreBest)
			{
				m_gameLoop.m_timeScoreBest = m_gameLoop.m_timeScoreCurrent;
				m_gameLoop.m_minigameMetaData.timeScore = m_gameLoop.m_timeScoreBest;
			}
		}
		CreateWinUI();
		GiveRewards();
		PsAchievementManager.SendToServer();
	}

	public override void CreatePlayMenu(Action _restartAction, Action _pauseAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_playMenu = new PsUIEditorTutorialPlay(_restartAction, _pauseAction);
		PsIngameMenu.OpenController();
	}

	public override void CreateEditMenu(Action _exitAction, Action _testAction)
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_playMenu = new PsUIEditorTutorialEdit(_exitAction, _testAction);
	}

	public override bool isMaterialAvailable(int _index)
	{
		PsUnlockable psUnlockable = PsMetagameData.m_gameMaterials[0].m_items[_index];
		return (psUnlockable.m_unlocked && (PsState.m_activeGameLoop as PsGameLoopEditorPuzzle).m_limitedItems.Contains(psUnlockable.m_identifier)) || PsState.m_adminMode;
	}

	protected override void CreateWinUI()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinEditorPuzzle), typeof(PsUITopWin), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", delegate
		{
			m_gameLoop.ExitEditor(false);
		});
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
	}

	public override void LoseMinigame()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterLoseEditor), typeof(PsUITopLoseRace), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
	}

	public override void PauseMinigame()
	{
	}
}
