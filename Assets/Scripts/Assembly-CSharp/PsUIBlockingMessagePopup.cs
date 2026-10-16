public class PsUIBlockingMessagePopup : PsUIHeaderedCanvas
{
	private UITextbox m_text;

	public PsUIBlockingMessagePopup(UIComponent _parent)
		: base(_parent, string.Empty, false)
	{
		(GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		SetWidth(0.5f, RelativeTo.ScreenWidth);
		SetHeight(0.2f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.5f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.Destroy();
		m_header = null;
		m_text = new UITextbox(this, false, string.Empty, "TEMP_POPUP", "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
	}

	public void SetText(string _msg)
	{
		m_text.SetText(_msg);
	}
}
