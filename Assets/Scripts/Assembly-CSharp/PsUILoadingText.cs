public class PsUILoadingText : UICanvas
{
	public PsUILoadingText(UIComponent _parent)
		: base(_parent, false, "PsUILoadingText", null, string.Empty)
	{
		SetHeight(1f, RelativeTo.ScreenHeight);
		SetWidth(1f, RelativeTo.ScreenWidth);
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.SetAlign(1f, 0f);
		uIHorizontalList.SetMargins(0f, 0.04f, 0f, 0f);
		uIHorizontalList.RemoveDrawHandler();
		new PsUILoadingAnimation(uIHorizontalList, false).SetSize(0.15f, 0.15f, RelativeTo.ScreenHeight);
		new UIText(uIHorizontalList, false, string.Empty, "Loading...", "HurmeSemiBold_Font", 0.06f, RelativeTo.ScreenHeight);
	}
}
