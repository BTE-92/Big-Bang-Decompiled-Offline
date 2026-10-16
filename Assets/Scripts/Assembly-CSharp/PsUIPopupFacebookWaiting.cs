public class PsUIPopupFacebookWaiting : PsUIHeaderedCanvas
{
	public PsUIPopupFacebookWaiting(UIComponent _parent)
		: base(_parent, string.Empty, false)
	{
		(GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.Destroy();
		new UITextbox(this, false, string.Empty, "Connecting to Facebook...", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
	}
}
