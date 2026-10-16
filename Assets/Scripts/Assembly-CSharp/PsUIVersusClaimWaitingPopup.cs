public class PsUIVersusClaimWaitingPopup : PsUIHeaderedCanvas
{
	public PsUIVersusClaimWaitingPopup(UIComponent _parent)
		: base(_parent, string.Empty, false)
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

	public virtual void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "Claim Your Prize!", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
	}

	public virtual void CreateContent(UIComponent _parent)
	{
		_parent.RemoveTouchAreas();
		UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, "Loading...", "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
	}
}
