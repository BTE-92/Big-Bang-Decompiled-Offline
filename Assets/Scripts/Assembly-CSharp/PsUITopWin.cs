public class PsUITopWin : UICanvas
{
	private PsUIGenericButton m_everyplayButton;

	private PsUIGenericButton m_restartButton;

	public PsUITopWin(UIComponent _parent)
		: base(_parent, false, "TopWin", null, string.Empty)
	{
		RemoveDrawHandler();
		PsUILevelHeader psUILevelHeader = new PsUILevelHeader(this);
		psUILevelHeader.SetVerticalAlign(0.98f);
	}
}
