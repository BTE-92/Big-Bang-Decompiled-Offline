public class PsUIPopupUpdateMinigameLevelReq : PsUIHeaderedCanvas
{
	public UITextbox m_textBox;

	public PsUIPopupUpdateMinigameLevelReq(UIComponent _parent)
		: base(_parent, string.Empty, false)
	{
		(GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		SetDepthOffset(-10f);
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.Destroy();
		m_header = null;
		m_textBox = new UITextbox(this, false, string.Empty, "Updating level requirements...", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
	}
}
