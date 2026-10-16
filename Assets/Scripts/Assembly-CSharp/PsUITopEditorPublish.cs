public class PsUITopEditorPublish : UICanvas
{
	private PsUIGenericButton m_exitButton;

	public PsUITopEditorPublish(UIComponent _parent)
		: base(_parent, false, "TopContent", null, string.Empty)
	{
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "UpperLeft");
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(0f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		m_exitButton = new PsUIGenericButton(uIHorizontalList);
		m_exitButton.SetIcon("hud_icon_back", 0.06f);
		m_exitButton.SetOrangeColors();
		PsUIInfoBar psUIInfoBar = new PsUIInfoBar(this, string.Empty);
		psUIInfoBar.SetVerticalAlign(0f);
		psUIInfoBar.SetText("Once you GO LIVE, you can not make changes to your Level.");
	}

	public override void Step()
	{
		if (m_exitButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Back");
		}
		base.Step();
	}
}
