public class PsUIPopupServerQueue : PsUIHeaderedCanvas
{
	private string[] m_tags;

	public PsUIPopupServerQueue(UIComponent _parent)
		: base(_parent, string.Empty, false)
	{
		(GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		SetWidth(0.4f, RelativeTo.ScreenWidth);
		SetHeight(0.35f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.Destroy();
		m_header = null;
	}

	public void CreateContent(string[] _tags, string _textBox = null)
	{
		m_tags = _tags;
		new UITextbox(this, false, string.Empty, _textBox, "KGSecondChances_Font", 0.03f, RelativeTo.ScreenShortest, false, Align.Center, Align.Middle);
	}

	public override void Step()
	{
		if (!HttpS.QueueTagsExist(m_tags))
		{
			(GetRoot() as PsUIBasePopup).CallAction("Server");
			(GetRoot() as PsUIBasePopup).CallAction("Proceed");
			(GetRoot() as PsUIBasePopup).Destroy();
		}
		base.Step();
	}
}
