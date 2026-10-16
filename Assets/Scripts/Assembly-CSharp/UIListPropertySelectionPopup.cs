public class UIListPropertySelectionPopup : UIScrollableCanvas
{
	protected UIScrollableCanvas m_scrollableCanvas;

	protected UIVerticalList m_verticalArea;

	private UIHorizontalList m_upperLeftArea;

	private PsUIGenericButton m_closeButton;

	public UIListPropertySelectionPopup(UIModel _model, string _fieldName)
		: base(null, "UIListPropertySelectionPopup")
	{
		m_model = _model;
		m_fieldName = _fieldName;
		m_maxScrollInertialY = 0f;
		SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		UICanvas uICanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
		uICanvas.SetWidth(0.5f, RelativeTo.ScreenHeight);
		uICanvas.SetHeight(0.85f, RelativeTo.ScreenHeight);
		uICanvas.SetDrawHandler(PsUIPopup.PopupDrawHandler);
		m_scrollableCanvas = new UIScrollableCanvas(uICanvas, string.Empty);
		m_scrollableCanvas.RemoveDrawHandler();
		m_verticalArea = new UIVerticalList(m_scrollableCanvas, string.Empty);
		m_verticalArea.SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
		m_upperLeftArea = new UIHorizontalList(this, "UpperLeft");
		m_upperLeftArea.SetMargins(0.025f, RelativeTo.ScreenShortest);
		m_upperLeftArea.SetAlign(0f, 1f);
		m_upperLeftArea.RemoveDrawHandler();
		m_upperLeftArea.RemoveTouchAreas();
		m_upperLeftArea.SetDepthOffset(-50f);
		m_closeButton = new PsUIGenericButton(m_upperLeftArea);
		m_closeButton.SetText("Cancel", 0.04f);
		m_closeButton.SetHeight(0.06f, RelativeTo.ScreenHeight);
		m_closeButton.Update();
		Update();
	}

	public virtual void Close()
	{
		Destroy();
	}

	public override void Step()
	{
		if (m_closeButton.m_hit)
		{
			Destroy();
		}
		else
		{
			base.Step();
		}
	}
}
