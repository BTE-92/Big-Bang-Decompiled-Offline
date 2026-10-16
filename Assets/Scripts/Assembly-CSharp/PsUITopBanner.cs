public class PsUITopBanner : UICanvas
{
	public PsUITopBanner(UIComponent _parent)
		: base(_parent, false, "TopLevelBanner", null, string.Empty)
	{
		PsUILevelHeader psUILevelHeader = new PsUILevelHeader(this);
		psUILevelHeader.SetVerticalAlign(0.98f);
		RemoveDrawHandler();
	}
}
