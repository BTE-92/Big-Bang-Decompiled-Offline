public class PsUIHeaderedCanvas : UIHeaderedCanvas
{
	protected PsUIGenericButton m_exitButton;

	public PsUIHeaderedCanvas(UIComponent _parent, string _tag = "", bool _hasCloseButton = true, float _headerHeight = 0.125f, RelativeTo _headerHeightRelativeTo = RelativeTo.ScreenHeight)
		: base(_parent, _tag, _headerHeight, _headerHeightRelativeTo)
	{
		if (_hasCloseButton)
		{
			CreateCloseButton();
		}
	}

	public virtual void CreateCloseButton()
	{
		UICanvas uICanvas = new UICanvas(m_parent, false, string.Empty, null, string.Empty);
		uICanvas.SetSize(0.125f, 0.125f, RelativeTo.ScreenHeight);
		uICanvas.SetAlign(1f, 1f);
		uICanvas.SetMargins(0.25f, -0.25f, -0.25f, 0.25f, RelativeTo.OwnHeight);
		uICanvas.RemoveDrawHandler();
		uICanvas.SetDepthOffset(-20f);
		m_exitButton = new PsUIGenericButton(uICanvas);
		m_exitButton.SetOrangeColors();
		m_exitButton.SetIcon("menu_icon_close");
	}

	public override void Step()
	{
		if (m_exitButton != null && m_exitButton.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		base.Step();
	}
}
