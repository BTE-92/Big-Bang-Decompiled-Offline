public class PsUITopStartOwnPublished : UICanvas
{
	private PsUIGenericButton m_exitButton;

	public PsUITopStartOwnPublished(UIComponent _parent)
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
