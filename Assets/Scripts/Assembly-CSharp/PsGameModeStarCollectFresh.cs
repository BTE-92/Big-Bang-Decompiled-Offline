using System.Collections;

public class PsGameModeStarCollectFresh : PsGameModeStarCollect
{
	public int m_realReward;

	protected PsGameModeStarCollectFresh(PsGameMode _gameMode, PsGameLoop _info)
		: base(_gameMode, _info)
	{
	}

	public PsGameModeStarCollectFresh(PsGameLoop _info)
		: base(PsGameMode.StarCollect, _info)
	{
	}

	protected override void CreateWinUI()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinStarCollectFresh), typeof(PsUITopWin), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
	}

	public override void PauseMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterPauseRace), typeof(PsUITopPauseFresh), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Resume", m_gameLoop.ResumeMinigame);
	}

	public override void GiveRewards()
	{
		if (m_gameLoop.m_scoreCurrent > m_gameLoop.m_rewardOld)
		{
			int num = m_gameLoop.m_scoreCurrent - m_gameLoop.m_rewardOld;
			m_gameLoop.m_rewardOld = m_gameLoop.m_scoreCurrent;
			for (int num2 = num; num2 > 0; num2--)
			{
				if (PsMetagameManager.m_playerStats.keys + num2 <= PsMetagameManager.m_playerStats.maxKeys)
				{
					PsMetagameManager.m_playerStats.keys += num2;
					m_realReward = num2;
					break;
				}
			}
		}
		PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
	}
}
