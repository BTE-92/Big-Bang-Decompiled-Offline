using UnityEngine;

public class UIScrollableSnappingCanvas : UIScrollableCanvas
{
	public float m_prevSnapPos;

	public int m_changePage;

	public int m_itemCount;

	private bool m_draggingRight;

	private bool m_draggingLeft;

	public float m_currentSnapValueX;

	public int m_currentPageX;

	public int m_previousPageX;

	public UIScrollableSnappingCanvas(UIComponent _parent, string _tag, int _itemCount)
		: base(_parent, _tag)
	{
		m_itemCount = _itemCount;
		m_maxScrollInertialX = 200f / (1024f / (float)Screen.width);
		m_maxScrollInertialY = 0f;
		m_dragStartPos = Vector2.zero;
	}

	public override void Step()
	{
		float num = (m_contentWidth - m_actualWidth + m_actualMargins.l + m_actualMargins.r) / (float)(m_itemCount - 1);
		m_currentSnapValueX = m_xPos / num;
		m_previousPageX = m_currentPageX;
		m_currentPageX = Mathf.RoundToInt(m_currentSnapValueX);
		if (!m_draggingLeft && !m_draggingRight)
		{
			if (m_changePage == 0 && m_scrollInertiaMultiplerX > 0f && (Input.GetKeyDown(KeyCode.RightArrow) || m_dragX < (float)Screen.width * -0.2f))
			{
				if (m_currentPageX < m_itemCount - 1)
				{
					m_changePage = 1;
				}
				m_draggingLeft = false;
				m_dragX = 0f;
			}
			else if (m_changePage == 0 && m_scrollInertiaMultiplerX > 0f && (Input.GetKeyDown(KeyCode.LeftArrow) || m_dragX > (float)Screen.width * 0.2f))
			{
				if (m_currentPageX > 0)
				{
					m_changePage = -1;
				}
				m_draggingRight = false;
				m_dragX = 0f;
			}
			if (m_prevSnapPos != (float)m_currentPageX)
			{
				m_changePage = 0;
			}
			m_prevSnapPos = m_currentPageX;
			float num2 = num * (float)(m_currentPageX + m_changePage);
			float num3 = (num2 - m_xPos) * 0.175f;
			if (num3 != 0f)
			{
				m_scrollInertiaX = (m_scrollInertiaX + num3) / 2f;
			}
			if (m_changePage != 0)
			{
				m_allowScroll = true;
			}
		}
		base.Step();
	}

	public override void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		switch (_touchPhase)
		{
		case TouchAreaPhase.Began:
		case TouchAreaPhase.RollIn:
			m_dragStartPos.x = _touches[0].m_currentPosition.x;
			m_dragStartPos.y = _touches[0].m_currentPosition.y;
			m_dragX = _touches[0].m_deltaPosition.x;
			m_dragY = _touches[0].m_deltaPosition.y;
			break;
		case TouchAreaPhase.MoveIn:
		case TouchAreaPhase.MoveOut:
			m_dragX = _touches[0].m_currentPosition.x - m_dragStartPos.x;
			m_dragY = _touches[0].m_currentPosition.y - m_dragStartPos.y;
			if (m_dragX > 0f)
			{
				m_draggingRight = true;
				m_draggingLeft = false;
			}
			else if (m_dragX < 0f)
			{
				m_draggingLeft = true;
				m_draggingRight = false;
			}
			break;
		case TouchAreaPhase.RollOut:
		case TouchAreaPhase.ReleaseIn:
		case TouchAreaPhase.ReleaseOut:
			m_draggingLeft = false;
			m_draggingRight = false;
			break;
		}
		base.TouchHandler(_touchArea, _touchPhase, _touchIsSecondary, _touchCount, _touches);
	}

	public override void FreezeHorizontalScroll(bool _affectWholeHierarchy)
	{
		base.FreezeHorizontalScroll(_affectWholeHierarchy);
		m_draggingLeft = false;
		m_draggingRight = false;
	}
}
