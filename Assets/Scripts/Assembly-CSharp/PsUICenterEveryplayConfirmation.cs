public class PsUICenterEveryplayConfirmation : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_ok;

	private PsUIGenericButton m_cancel;

	public PsUICenterEveryplayConfirmation(UIComponent _parent)
		: base(_parent, string.Empty, false)
	{
		(GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.Destroy();
		new UITextbox(this, false, string.Empty, "Hey! We recorded a replay of your actions! Wanna see it and share it?", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetVerticalAlign(0f);
		uIHorizontalList.SetMargins(0f, 0f, 0.075f, -0.075f);
		m_ok = new PsUIGenericButton(uIHorizontalList);
		m_ok.SetText("Yes!", 0.04f);
		m_ok.SetHeight(0.075f, RelativeTo.ScreenHeight);
		m_ok.SetOrangeColors();
		m_cancel = new PsUIGenericButton(uIHorizontalList);
		m_cancel.SetText("No", 0.04f);
		m_cancel.SetHeight(0.075f, RelativeTo.ScreenHeight);
		m_cancel.SetOrangeColors();
	}

	public override void Step()
	{
		if (m_ok.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Proceed");
		}
		else if (m_cancel.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		base.Step();
	}
}
