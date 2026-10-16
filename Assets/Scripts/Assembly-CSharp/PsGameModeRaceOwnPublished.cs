public class PsGameModeRaceOwnPublished : PsGameModeRace
{
	public PsGameModeRaceOwnPublished(PsGameLoop _info)
		: base(_info)
	{
	}

	public override void GiveRewards()
	{
	}

	public override void EnterMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUIVehicleSelect), typeof(PsUITopStartOwnPublished), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsMetagameManager.ShowResources(PsIngameMenu.m_popupMenu.m_overlayCamera, true, true, true, true, false, 0.03f, true);
		PsIngameMenu.OpenController(true);
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
}
