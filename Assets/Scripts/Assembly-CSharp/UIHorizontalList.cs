using UnityEngine;

public class UIHorizontalList : UICanvas
{
	public float m_spacing;

	public float m_actualSpacing;

	public RelativeTo m_spacingRelativeTo;

	private float m_forcedHeight;

	public UIHorizontalList(UIComponent _parent, string _tag)
		: base(_parent, false, _tag, null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(0f, RelativeTo.ParentHeight);
	}

	public virtual void SetSpacing(float _value, RelativeTo _relativeTo)
	{
		m_spacing = _value;
		m_spacingRelativeTo = _relativeTo;
	}

	protected virtual void UpdateSpacing()
	{
		RelativeTo relativeTo = m_spacingRelativeTo;
		if (m_spacingRelativeTo == RelativeTo.ParentShortest)
		{
			relativeTo = ((m_parent.m_actualWidth > m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
		}
		else if (m_spacingRelativeTo == RelativeTo.ParentLongest)
		{
			relativeTo = ((m_parent.m_actualWidth < m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
		}
		else if (m_spacingRelativeTo == RelativeTo.ScreenShortest)
		{
			relativeTo = ((Screen.width <= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
		}
		else if (m_spacingRelativeTo == RelativeTo.ScreenLongest)
		{
			relativeTo = ((Screen.width >= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
		}
		float num = 0f;
		switch (relativeTo)
		{
		case RelativeTo.ParentWidth:
			num = ((m_parent == null) ? ((float)Screen.width) : m_parent.m_actualWidth);
			break;
		case RelativeTo.ParentHeight:
			num = ((m_parent == null) ? ((float)Screen.height) : m_parent.m_actualHeight);
			break;
		case RelativeTo.ScreenHeight:
			num = Screen.height;
			break;
		case RelativeTo.ScreenWidth:
			num = Screen.width;
			break;
		case RelativeTo.OwnHeight:
			num = m_actualHeight;
			break;
		case RelativeTo.OwnWidth:
			num = m_actualWidth;
			break;
		}
		m_actualSpacing = m_spacing * num;
	}

	public override void SetHeight(float _heightRatio, RelativeTo _relativeTo)
	{
		m_forcedHeight = _heightRatio;
		base.SetHeight(_heightRatio, _relativeTo);
	}

	public override void ArrangeContents()
	{
		float num = 99999f;
		float num2 = -99999f;
		float num3 = 99999f;
		float num4 = -99999f;
		m_contentX = m_actualWidth * -0.5f + m_actualMargins.l;
		float num5 = 0f;
		int num6 = 0;
		for (int i = 0; i < m_childs.Count; i++)
		{
			UIComponent uIComponent = m_childs[i];
			if (uIComponent.m_rogue)
			{
				num6++;
				continue;
			}
			if (i - num6 > 0)
			{
				m_contentX += m_actualSpacing;
			}
			float num7 = m_contentX + uIComponent.m_actualWidth * 0.5f;
			uIComponent.UpdateVerticalAlign(num7);
			float num8 = m_actualWidth - m_actualMargins.l - m_actualMargins.r;
			float num9 = num8 - uIComponent.m_actualWidth;
			if (num9 > 0f)
			{
				float horizontalAlign = (num7 + num9 * 0.5f) / num9;
				uIComponent.SetHorizontalAlign(horizontalAlign);
			}
			m_contentX += uIComponent.m_actualWidth;
			if (m_forcedHeight == 0f)
			{
				if (uIComponent.m_actualHeight > num5)
				{
					num5 = uIComponent.m_actualHeight;
				}
			}
			else
			{
				float num10 = uIComponent.m_TC.transform.localPosition.y - uIComponent.m_actualHeight * 0.5f;
				float num11 = uIComponent.m_TC.transform.localPosition.y + uIComponent.m_actualHeight * 0.5f;
				if (num11 > num4)
				{
					num4 = num11;
				}
				if (num10 < num3)
				{
					num3 = num10;
				}
			}
			float num12 = uIComponent.m_TC.transform.localPosition.x - uIComponent.m_actualWidth * 0.5f;
			float num13 = uIComponent.m_TC.transform.localPosition.x + uIComponent.m_actualWidth * 0.5f;
			if (num13 > num2)
			{
				num2 = num13;
			}
			if (num12 < num)
			{
				num = num12;
			}
		}
		if (m_forcedHeight == 0f)
		{
			m_contentHeight = num5;
			m_contentCenterY = 0f;
		}
		else
		{
			m_contentHeight = num4 - num3;
			m_contentCenterY = num3 + m_contentHeight * 0.5f;
		}
		m_contentWidth = num2 - num;
		m_contentCenterX = num + m_contentWidth * 0.5f;
		m_contentY = m_actualHeight * 0.5f - m_actualMargins.t - m_contentHeight - m_actualMargins.b;
		m_contentX += m_actualMargins.l;
	}

	public override void Update()
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		if (m_forcedHeight == 0f)
		{
			base.SetHeight(1f, RelativeTo.ParentHeight);
		}
		CalculateReferenceSizes();
		UpdateSize();
		UpdateMargins();
		UpdateSpacing();
		UpdateChildren();
		ArrangeContents();
		float num = m_contentWidth + m_actualMargins.l + m_actualMargins.r;
		float widthRatio = num / m_tempReferenceWidth;
		if (m_tempReferenceWidth == 0f)
		{
			widthRatio = Screen.width;
		}
		SetWidth(widthRatio, RelativeTo.ParentWidth);
		if (m_forcedHeight == 0f)
		{
			float num2 = m_contentHeight + m_actualMargins.t + m_actualMargins.b;
			float heightRatio = num2 / m_tempReferenceHeight;
			if (m_tempReferenceHeight == 0f)
			{
				heightRatio = Screen.height;
			}
			base.SetHeight(heightRatio, RelativeTo.ParentHeight);
		}
		UpdateSize();
		UpdateAlign();
		UpdateChildrenAlign();
		ArrangeContents();
		if (!m_hidden && d_Draw != null)
		{
			d_Draw(this);
		}
		if (m_parent == null)
		{
			UpdateSpecial();
		}
	}

	public override void RemoveTouchAreas()
	{
		Debug.LogWarning("unnecessary touch area removal");
		base.RemoveTouchAreas();
	}
}
