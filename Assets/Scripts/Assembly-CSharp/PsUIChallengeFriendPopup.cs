public class PsUIChallengeFriendPopup : PsUIHeaderedCanvas
{
	public PsUIChallengeFriendPopup(UIComponent _parent)
		: base(_parent, "ChallengeFriendPopup")
	{
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		SetWidth(0.5f, RelativeTo.ScreenWidth);
		SetHeight(0.75f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.015f, 0.015f);
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
		string text = "Challenge friend";
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, text, "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
	}

	public void CreateContent(UIComponent _parent)
	{
		PsUIChallengeFriendList psUIChallengeFriendList = new PsUIChallengeFriendList(_parent);
	}
}
