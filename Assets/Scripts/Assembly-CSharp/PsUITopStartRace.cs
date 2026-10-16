public class PsUITopStartRace : UICanvas
{
	private PsUIGenericButton m_exitButton;

	private PsUISkipLevelButton m_skipButton;

	public PsUITopStartRace(UIComponent _parent)
		: base(_parent, false, "TopContent", null, string.Empty)
	{
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "UpperLeft");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(0f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		m_exitButton = new PsUIGenericButton(uIHorizontalList);
		m_exitButton.SetIcon("hud_icon_menu_exit", 0.06f);
		m_exitButton.SetOrangeColors();
		if (PsState.m_activeGameLoop.m_path != null && PsState.m_activeGameLoop.m_path.m_name == "MainPath" && PsState.m_activeGameLoop.m_nodeId == PsState.m_activeGameLoop.m_path.m_currentNodeId && PsState.m_activeGameLoop.m_scoreBest == 0 && PsState.m_activeMinigame.m_gameStartCount > 0)
		{
			m_skipButton = new PsUISkipLevelButton(uIHorizontalList, Skip, CancelSkip);
		}
		if (PsMetagameData.GetUnlockableByIdentifier("Upgrade").m_unlocked && PsAdMediation.adsAvailable() && PsMetagameManager.GetCoinBoostTimeLeft() <= 0)
		{
			UIHorizontalList uIHorizontalList2 = new UIHorizontalList(this, "Bottom");
			uIHorizontalList2.SetMargins(0.06f, RelativeTo.ScreenShortest);
			uIHorizontalList2.SetSpacing(0.025f, RelativeTo.ScreenShortest);
			uIHorizontalList2.SetAlign(0.5f, 0f);
			uIHorizontalList2.SetHeight(0.22f, RelativeTo.ScreenHeight);
			uIHorizontalList2.RemoveDrawHandler();
			new PsUIWatchIngameAdButton(uIHorizontalList2);
		}
	}

	public void Skip()
	{
		(GetRoot() as PsUIBasePopup).CallAction("Skip");
	}

	public void CancelSkip()
	{
	}

	public override void Step()
	{
		if (m_exitButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		base.Step();
	}
}
