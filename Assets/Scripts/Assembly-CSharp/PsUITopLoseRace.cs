public class PsUITopLoseRace : UICanvas
{
	private PsUIGenericButton m_exitButton;

	private PsUIGenericButton m_everyplayButton;

	protected UIHorizontalList m_leftArea;

	public PsUITopLoseRace(UIComponent _parent)
		: base(_parent, false, "TopContent", null, string.Empty)
	{
		PsMetagameManager.HideResources();
		RemoveDrawHandler();
		m_leftArea = new UIHorizontalList(this, "UpperLeft");
		m_leftArea.SetMargins(0.025f, RelativeTo.ScreenShortest);
		m_leftArea.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		m_leftArea.SetAlign(0f, 1f);
		m_leftArea.RemoveDrawHandler();
		m_exitButton = new PsUIGenericButton(m_leftArea);
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
