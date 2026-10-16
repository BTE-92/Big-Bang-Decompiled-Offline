public class PsUIVersusNotEnoughTokensPopup : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_ok;

	private VersusMetaData m_versusData;

	private bool m_proceed;

	public PsUIVersusNotEnoughTokensPopup(UIComponent _parent)
		: base(_parent, string.Empty)
	{
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		CreateContent(this);
		CreateHeaderContent(m_header);
	}

	public void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "Out of tickets", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
	}

	public void CreateContent(UIComponent _parent)
	{
		_parent.RemoveTouchAreas();
		UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, "Get more <color=#aff6ff>race tickets</color> from level checkpoints", "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle, "#aff6f");
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetAlign(1f, 0f);
		uIHorizontalList.SetMargins(0f, 0.1f, 0.05f, -0.05f);
		m_ok = new PsUIGenericButton(uIHorizontalList);
		m_ok.SetAlign(1f, 1f);
		m_ok.SetText("OK", 0.05f);
		m_ok.SetGreenColors();
	}

	public override void Step()
	{
		if (m_ok.m_hit)
		{
			(GetRoot() as PsUIBasePopup).Destroy();
		}
		base.Step();
	}
}
