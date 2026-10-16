public class PsUICenterWaitForHighscoresAndGhost : UICanvas
{
	public PsUICenterWaitForHighscoresAndGhost(UIComponent _parent)
		: base(_parent, false, "CenterContent", null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(this, "Center");
		uIVerticalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIVerticalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList, false, string.Empty, "Resetting Track", "HurmeSemiBold_Font", 0.03f, RelativeTo.ScreenHeight);
		new PsUILoadingAnimation(uIVerticalList, false);
	}

	public override void Step()
	{
		base.Step();
		if (!PsState.m_activeGameLoop.m_gameMode.m_waitForHighscoreAndGhost && !PsState.m_activeGameLoop.m_gameMode.m_waitForNextGhost)
		{
			PsState.m_activeGameLoop.m_gameMode.DestroyGhostLoaderEntity();
			(GetRoot() as PsUIBasePopup).CallAction("Done");
			DestroyRoot();
		}
	}
}
