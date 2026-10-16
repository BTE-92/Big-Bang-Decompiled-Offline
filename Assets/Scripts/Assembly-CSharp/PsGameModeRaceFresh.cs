using System.Collections;

public class PsGameModeRaceFresh : PsGameModeRace
{
	public int m_realReward;

	protected PsGameModeRaceFresh(PsGameMode _gameMode, PsGameLoop _info)
		: base(_info)
	{
	}

	public PsGameModeRaceFresh(PsGameLoop _info)
		: base(_info)
	{
	}

	public override void GiveRewards()
	{
		if (m_gameLoop.m_scoreCurrent > m_gameLoop.m_rewardOld)
		{
			int diamonds = m_gameLoop.m_scoreCurrent - m_gameLoop.m_rewardOld;
			m_gameLoop.m_rewardOld = m_gameLoop.m_scoreCurrent;
			PsMetagameManager.UpdateResourceStats(0, diamonds, 0, 0);
			PsMetagameManager.m_playerStats.updated = false;
		}
		PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
	}

	protected override void CreateWinUI()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinRaceFresh), typeof(PsUITopWin), null, null, false);
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
}
