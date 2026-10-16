public class PsUISubMenuContentHeaders : UIVerticalList
{
	public UIText header;

	public PsUISubMenuContentHeaders(UIComponent _parent, string _header, string _caption)
		: base(_parent, "SubMenuContentHeaders")
	{
		RemoveDrawHandler();
		header = new UIText(this, false, string.Empty, _header.ToUpper(), "KGSecondChances_Font", 0.05f, RelativeTo.ScreenShortest, "#ffd800", "#231f34");
		new UIText(this, false, string.Empty, _caption, "HurmeSemiBold_Font", 0.03f, RelativeTo.ScreenShortest, "#fcffe8");
	}
}
