public class PsGameModeStarCollectOwnPublished : PsGameModeStarCollect
{
	public PsGameModeStarCollectOwnPublished(PsGameLoop _info)
		: base(PsGameMode.StarCollect, _info)
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
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinStarCollect), typeof(PsUITopWin), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
	}

	public override void PauseMinigame()
	{
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterPauseRace), typeof(PsUITopPauseStarCollect), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", m_gameLoop.ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", m_gameLoop.RestartMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Resume", m_gameLoop.ResumeMinigame);
	}

	public override void GiveRewards()
	{
	}
}
