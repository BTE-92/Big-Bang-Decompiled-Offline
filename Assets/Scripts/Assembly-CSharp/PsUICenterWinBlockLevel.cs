public class PsUICenterWinBlockLevel : UICanvas
{
	private PsUIGenericButton m_restartButton;

	private PsUIGenericButton m_continueButton;

	private PsUIGenericButton m_everyplayButton;

	private UIVerticalList m_likeArea;

	private bool m_ratingDone;

	public PsUICenterWinBlockLevel(UIComponent _parent)
		: base(_parent, false, "CenterContent", null, string.Empty)
	{
		PsMetagameManager.ShowResources(m_camera, true, true, false, false, false);
		SetWidth(0.85f, RelativeTo.ScreenWidth);
		SetHeight(0.85f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, "Center");
		uIVerticalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIVerticalList.SetWidth(1f, RelativeTo.ParentWidth);
		uIVerticalList.SetVerticalAlign(1f);
		uIVerticalList.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
		uIHorizontalList.SetHeight(0.42f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetSpacing(-0.07f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetVerticalAlign(1f);
		uIHorizontalList.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList2 = new UIHorizontalList(uIVerticalList, string.Empty);
		uIHorizontalList2.SetHeight(0.35f, RelativeTo.ScreenHeight);
		uIHorizontalList2.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIHorizontalList2.RemoveDrawHandler();
		m_likeArea = new UIVerticalList(uIHorizontalList2, string.Empty);
		m_likeArea.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		m_likeArea.SetWidth(0.225f, RelativeTo.ScreenHeight);
		m_likeArea.RemoveDrawHandler();
		UIVerticalList uIVerticalList2 = new UIVerticalList(uIHorizontalList2, string.Empty);
		uIVerticalList2.SetVerticalAlign(0f);
		uIVerticalList2.RemoveDrawHandler();
		m_continueButton = new PsUIGenericButton(uIVerticalList2);
		m_continueButton.SetText("Continue", 0.07f);
		UIVerticalList uIVerticalList3 = new UIVerticalList(uIHorizontalList2, string.Empty);
		uIVerticalList3.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIVerticalList3.SetWidth(0.25f, RelativeTo.ScreenHeight);
		uIVerticalList3.SetAlign(1f, 1f);
		uIVerticalList3.RemoveDrawHandler();
		m_restartButton = new PsUIGenericButton(uIVerticalList3);
		m_restartButton.SetIcon("hud_icon_restart", 0.1f);
		m_restartButton.SetOrangeColors();
		m_restartButton.SetHorizontalAlign(1f);
		m_restartButton.SetMargins(0.035f, 0.035f, 0.02f, 0.02f);
	}

	public override void Step()
	{
		if (m_restartButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Restart");
		}
		else if (m_continueButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		base.Step();
	}
}
