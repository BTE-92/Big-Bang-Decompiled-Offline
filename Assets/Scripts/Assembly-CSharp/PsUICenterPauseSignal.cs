public class PsUICenterPauseSignal : UICanvas
{
	private PsUIGenericButton m_resumeButton;

	public PsUICenterPauseSignal(UIComponent _parent)
		: base(_parent, false, "CenterContent", null, string.Empty)
	{
		SetWidth(0.7f, RelativeTo.ScreenWidth);
		SetHeight(0.85f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, "Center");
		uIVerticalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIVerticalList.RemoveDrawHandler();
		m_resumeButton = new PsUIGenericButton(uIVerticalList);
		m_resumeButton.SetText("Resume");
		m_resumeButton.SetGreenColors();
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
