public class PsUICenterPauseRace : UICanvas
{
	private PsUIGenericButton m_resumeButton;

	public PsUICenterPauseRace(UIComponent _parent)
		: base(_parent, false, "CenterContent", null, string.Empty)
	{
		SetWidth(0.7f, RelativeTo.ScreenWidth);
		SetHeight(0.85f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		SetMargins(0f, 0f, 0f, 0.05f);
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, "Center");
		uIVerticalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIVerticalList.RemoveDrawHandler();
		uIVerticalList.SetVerticalAlign(0f);
		m_resumeButton = new PsUIGenericButton(uIVerticalList, 0.25f, 0.25f, 0.0085f);
		m_resumeButton.SetText("Resume", 0.07f);
	}

	public override void Step()
	{
		if (m_resumeButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Resume");
		}
		base.Step();
	}
}
