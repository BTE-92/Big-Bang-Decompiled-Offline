public class PsUIPopupConfirmNewAccount : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_ok;

	private PsUIGenericButton m_cancel;

	public string m_serviceName;

	public PsUIPopupConfirmNewAccount(UIComponent _parent, string _serviceName)
		: base(_parent, string.Empty, false)
	{
		m_serviceName = _serviceName;
		if (_serviceName != "Facebook")
		{
			PsState.m_openSocialPopup = GetRoot() as PsUIBasePopup;
		}
		(GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_header, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "Create new account?", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		new UITextbox(this, false, string.Empty, "There is no progress for your " + m_serviceName + " account, do you want to create new account?", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		UIHorizontalList uIHorizontalList2 = new UIHorizontalList(this, string.Empty);
		uIHorizontalList2.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIHorizontalList2.RemoveDrawHandler();
		uIHorizontalList2.SetVerticalAlign(0f);
		uIHorizontalList2.SetMargins(0f, 0f, 0.075f, -0.075f);
		m_ok = new PsUIGenericButton(uIHorizontalList2);
		m_ok.SetText("Yes!", 0.04f);
		m_ok.SetHeight(0.075f, RelativeTo.ScreenHeight);
		m_ok.SetGreenColors();
		m_cancel = new PsUIGenericButton(uIHorizontalList2);
		m_cancel.SetText("Cancel", 0.04f);
		m_cancel.SetHeight(0.075f, RelativeTo.ScreenHeight);
	}

	public override void Step()
	{
		if (m_ok.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Proceed");
			PsState.m_openSocialPopup = null;
			(GetRoot() as PsUIBasePopup).Destroy();
		}
		else if (m_cancel.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Cancel");
			PsState.m_openSocialPopup = null;
			(GetRoot() as PsUIBasePopup).Destroy();
		}
		base.Step();
	}
}
