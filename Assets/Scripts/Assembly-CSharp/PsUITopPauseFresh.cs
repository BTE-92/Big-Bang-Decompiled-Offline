public class PsUITopPauseFresh : PsUITopPause
{
	public PsUITopPauseFresh(UIComponent _parent)
		: base(_parent)
	{
	}

	public override void CreateMiddleArea()
	{
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetSpacing(0.03f, RelativeTo.ScreenHeight);
		uIVerticalList.SetAlign(0.5f, 1f);
		uIVerticalList.RemoveDrawHandler();
		new PsUITopBanner(uIVerticalList);
		UIVerticalList uIVerticalList2 = new UIVerticalList(uIVerticalList, string.Empty);
		uIVerticalList2.SetWidth(0.55f, RelativeTo.ScreenHeight);
		uIVerticalList2.SetMargins(0.15f, -0.15f, 0f, 0f);
		uIVerticalList2.RemoveDrawHandler();
		UIVerticalList uIVerticalList3 = new UIVerticalList(uIVerticalList2, string.Empty);
		uIVerticalList3.SetWidth(0.55f, RelativeTo.ScreenHeight);
		uIVerticalList3.SetMargins(0.025f);
		uIVerticalList3.SetDrawHandler(base.DescriptionDrawHandler);
		UITextbox uITextbox = new UITextbox(uIVerticalList3, false, string.Empty, PsState.m_activeGameLoop.GetDescription(), "KGSecondChances_Font", 0.03f, RelativeTo.ScreenHeight, false, Align.Center, Align.Top, "343b2e");
		uITextbox.SetWidth(1f, RelativeTo.ParentWidth);
	}
}
