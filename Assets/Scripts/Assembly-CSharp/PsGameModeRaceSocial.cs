public class PsGameModeRaceSocial : PsGameModeRace
{
	public PsGameModeRaceSocial(PsGameLoop _info)
		: base(_info)
	{
	}

	public override void GiveRewards()
	{
	}

	public override void EnterMinigame()
	{
		PsIngameMenu.CloseAll();
		PsMetagameManager.ShowResources(null, true, true, true, true, false);
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(null, typeof(PsUITopStartSocial), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.OpenController(true);
	}

	protected override void CreateWinUI()
	{
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinRaceSocial), typeof(PsUITopWin), null, null, false);
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
}
