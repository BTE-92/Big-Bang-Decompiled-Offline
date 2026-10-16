using UnityEngine;

public class UIScaleToContentCanvas : UICanvas
{
	private bool m_shrinkHorizontally;

	private bool m_shrinkVertically;

	public UIScaleToContentCanvas(UIComponent _parent, string _tag, bool _shrinkHorizontally = true, bool _shrinkVertically = true)
		: base(_parent, false, _tag, null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(1f, RelativeTo.ParentHeight);
		m_shrinkHorizontally = _shrinkHorizontally;
		m_shrinkVertically = _shrinkVertically;
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		UpdateChildren();
		ArrangeContents();
		float num = m_contentWidth + m_actualMargins.l + m_actualMargins.r;
		float widthRatio = num / (float)Screen.width;
		if (m_shrinkHorizontally || num > m_actualWidth)
		{
			SetWidth(widthRatio, RelativeTo.ScreenWidth);
		}
		float num2 = m_contentHeight + m_actualMargins.t + m_actualMargins.b;
		float heightRatio = num2 / (float)Screen.height;
		if (m_shrinkVertically || num2 > m_actualHeight)
		{
			SetHeight(heightRatio, RelativeTo.ScreenHeight);
		}
		UpdateSize();
		UpdateMargins();
		UpdateAlign();
		UpdateChildrenAlign();
		if (!m_hidden && d_Draw != null)
		{
			d_Draw(this);
		}
	}
}
