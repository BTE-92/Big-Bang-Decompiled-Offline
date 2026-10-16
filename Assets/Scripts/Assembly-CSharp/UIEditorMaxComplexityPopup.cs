public class UIEditorMaxComplexityPopup : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_okButton;

	public UITextbox m_box;

	public UIEditorMaxComplexityPopup(UIComponent _parent)
		: base(_parent, string.Empty)
	{
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
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "Complexity max'd out", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		m_box = new UITextbox(this, false, string.Empty, PsStrings.Get(StringID.EDITOR_PROMPT_MAX_COMPLEXITY_REACHED), "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
		UIHorizontalList uIHorizontalList2 = new UIHorizontalList(this, "nameChangeButtons");
		uIHorizontalList2.SetHeight(0.1f, RelativeTo.ScreenHeight);
		uIHorizontalList2.RemoveDrawHandler();
		uIHorizontalList2.SetVerticalAlign(0f);
		uIHorizontalList2.SetMargins(0f, 0f, 0.075f, -0.075f);
		m_okButton = new PsUIGenericButton(uIHorizontalList2);
		m_okButton.SetText("Ok");
		m_okButton.SetGreenColors();
	}

	public override void Step()
	{
		if (m_okButton.m_hit || m_exitButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).Destroy();
		}
		base.Step();
	}
}
