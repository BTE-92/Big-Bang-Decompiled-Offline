using UnityEngine;

public class PsUIScrollableBase : PsUIHeaderedCanvas
{
	protected UIScrollableCanvas m_scrollArea;

	public PsUIScrollableBase(UIComponent _parent)
		: base(_parent, "ScrollableContent")
	{
		SetWidth(0.8f, RelativeTo.ScreenWidth);
		SetHeight(0.75f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_scrollArea = new UIScrollableCanvas(this, string.Empty);
		m_scrollArea.SetHeight(1f, RelativeTo.ParentHeight);
		m_scrollArea.m_maxScrollInertialY = 0f;
		m_scrollArea.m_maxScrollInertialX = 50f / (1024f / (float)Screen.width);
		m_scrollArea.RemoveDrawHandler();
		m_scrollArea.m_passTouchesToScrollableParents = true;
		CreateContent(m_scrollArea);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		CreateHeaderContent(m_header);
	}

	public virtual void CreateHeaderContent(UIComponent _parent)
	{
	}

	public virtual void CreateContent(UIComponent _parent)
	{
	}
}
