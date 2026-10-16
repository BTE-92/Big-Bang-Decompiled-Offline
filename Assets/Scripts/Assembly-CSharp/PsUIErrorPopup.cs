public class PsUIErrorPopup : PsUIHeaderedCanvas
{
	private UIVerticalList m_verticalArea;

	public PsUIGenericButton m_proceedButton;

	public PsUIErrorPopup(UIComponent _parent)
		: base(_parent, string.Empty, false)
	{
		(GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
	}

	public void CreateHeader(string _title)
	{
		if (!string.IsNullOrEmpty(_title))
		{
			UIHorizontalList uIHorizontalList = new UIHorizontalList(m_header, string.Empty);
			uIHorizontalList.RemoveDrawHandler();
			uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
			uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
			uIHorizontalList.SetHorizontalAlign(0.5f);
			UIText uIText = new UIText(uIHorizontalList, false, string.Empty, _title, "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		}
		else
		{
			m_header.Destroy();
			m_header = null;
		}
	}

	public void CreateContent(string _error, string _proceedButtonTitle)
	{
		if (!string.IsNullOrEmpty(_error))
		{
			new UITextbox(this, false, string.Empty, _error, "KGSecondChances_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		}
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.RemoveTouchAreas();
		uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetVerticalAlign(0f);
		uIHorizontalList.SetMargins(0f, 0f, 0.075f, -0.075f);
		m_proceedButton = new PsUIGenericButton(uIHorizontalList);
		m_proceedButton.SetText(_proceedButtonTitle, 0.04f);
		m_proceedButton.SetOrangeColors();
		m_proceedButton.Update();
	}

	public override void Step()
	{
		if (m_proceedButton != null && m_proceedButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Proceed");
		}
		else
		{
			base.Step();
		}
	}
}
