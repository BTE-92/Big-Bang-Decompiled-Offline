using System;
using UnityEngine;

public class UIScrollableCanvas : UICanvas
{
	public TransformC m_scrollTC;

	public float m_scrollInertiaY;

	public float m_scrollInertiaX;

	public float m_maxScrollInertialX;

	public float m_maxScrollInertialY;

	public float m_scrollFallOff;

	public float m_currentScrollX;

	public float m_currentScrollY;

	public bool m_overrideScrollPosition;

	public float m_scrollInertiaMultiplerX;

	public float m_scrollInertiaMultiplerY;

	public bool m_allowScroll;

	public bool m_draggedToUpdate;

	public bool m_recoverFromUpdate;

	public bool m_allowUpdate;

	public bool m_allowDragToBottom;

	public float m_updateSize;

	public RelativeTo m_updateRelativeTo;

	public RelativeTo m_tempUpdateRelativeTo;

	public float m_actualUpdateSize;

	public float m_tempReferenceUpdate;

	public Action m_dragUpdateDelegate;

	public float m_dragToBottomSize;

	public RelativeTo m_dragToBottomRelativeTo;

	public RelativeTo m_tempDragToBottomRelativeTo;

	public float m_actualDragToBottomSize;

	public float m_tempReferenceDragToBottom;

	public bool m_draggedToBottom;

	public Action m_dragBottomDelegate;

	public bool m_passTouchesToScrollableParents;

	protected Vector2 m_dragStartPos;

	protected float m_dragX;

	protected float m_dragY;

	protected bool m_freezeParentScrollWhenDragged;

	protected UIComponent m_setScrollTarget;

	protected bool m_setScrollToTarget;

	private Rect m_prevClipRect;

	public float m_xMinLimit;

	public float m_xMaxLimit;

	public float m_xPos;

	public UIScrollableCanvas(UIComponent _parent, string _tag)
		: base(_parent, true, _tag, null, string.Empty)
	{
		m_TAC.m_allowSecondary = true;
		m_TAC.m_clip = true;
		SetScrollable();
		SetScrollPosition(0f, 0f);
		m_maxScrollInertialX = 0f;
		m_maxScrollInertialY = 50f / (1024f / (float)Screen.width);
		m_scrollFallOff = 0.9f;
		m_scrollInertiaMultiplerX = 1f;
		m_scrollInertiaMultiplerY = 1f;
		m_allowScroll = false;
		m_freezeParentScrollWhenDragged = false;
		m_updateSize = 0.2f;
		m_updateRelativeTo = RelativeTo.ScreenHeight;
		m_setScrollToTarget = false;
		m_setScrollTarget = null;
	}

	public virtual void SetDragBottomDelegate(Action _delegate, float _distanceFromBotton = 0f, RelativeTo _distanceFromBottonRelativeTo = RelativeTo.ScreenHeight)
	{
		m_allowDragToBottom = true;
		m_dragToBottomSize = _distanceFromBotton;
		m_dragToBottomRelativeTo = _distanceFromBottonRelativeTo;
		m_dragBottomDelegate = _delegate;
	}

	protected virtual void SetScrollable()
	{
		m_uniqueCamera = true;
		m_camera = CameraS.AddCamera("UI Container Camera", true);
		m_scrollTC = TransformS.AddComponent(m_TAC.p_entity, "Scroll Transform");
		TransformS.ParentComponent(m_scrollTC, m_TC, Vector3.zero);
		m_camera.transform.parent = m_scrollTC.transform;
		m_camera.transform.localPosition = Vector3.zero;
		GameObject gameObject = m_TC.transform.gameObject;
		GameObject gameObject2 = m_scrollTC.transform.gameObject;
		gameObject2.layer = gameObject.layer;
		if (m_TAC != null)
		{
			MeshCollider collider = m_TAC.m_collider;
			TouchAreaBootstrap component = gameObject.GetComponent<TouchAreaBootstrap>();
			MeshCollider meshCollider = gameObject2.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = collider.sharedMesh;
			m_TAC.m_collider = meshCollider;
			m_TAC.m_TC = m_scrollTC;
			TouchAreaBootstrap touchAreaBootstrap = gameObject2.AddComponent<TouchAreaBootstrap>();
			touchAreaBootstrap.m_TAC = m_TAC;
			UnityEngine.Object.DestroyImmediate(collider);
			UnityEngine.Object.DestroyImmediate(component);
		}
		AssignCameraToComponentAndChildren(m_camera);
	}

	protected virtual void AssignCameraToComponentAndChildren(Camera _camera)
	{
		if (m_TAC != null)
		{
			m_TAC.m_camera = _camera;
			m_TAC.m_TC.transform.gameObject.layer = _camera.gameObject.layer;
		}
		m_TC.transform.gameObject.layer = _camera.gameObject.layer;
		for (int i = 0; i < m_TC.p_entity.m_components.Count; i++)
		{
			IComponent component = m_TC.p_entity.m_components[i];
			if (component.m_componentType == ComponentType.TouchArea)
			{
				TouchAreaS.SetCamera(component as TouchAreaC, _camera);
			}
			else if (component.m_componentType == ComponentType.Prefab)
			{
				PrefabS.SetCamera(component as PrefabC, _camera);
			}
		}
	}

	public virtual void SetScrollPosition(float _horizontal, float _vertical)
	{
		m_currentScrollX = _horizontal;
		m_currentScrollY = 0f - _vertical;
		m_overrideScrollPosition = true;
	}

	public virtual void SetScrollPositionToChild(UIComponent _child)
	{
		m_setScrollTarget = _child;
		m_setScrollToTarget = true;
		m_overrideScrollPosition = false;
	}

	public override void FreezeHorizontalScroll(bool _affectWholeHierarchy)
	{
		m_scrollInertiaMultiplerX = 0f;
		if (_affectWholeHierarchy)
		{
			base.FreezeHorizontalScroll(_affectWholeHierarchy);
		}
	}

	public override void UnfreezeHorizontalScroll(bool _affectWholeHierarchy)
	{
		m_scrollInertiaMultiplerX = 1f;
		if (_affectWholeHierarchy)
		{
			base.UnfreezeHorizontalScroll(_affectWholeHierarchy);
		}
	}

	public override void FreezeVerticalScroll(bool _affectWholeHierarchy)
	{
		m_scrollInertiaMultiplerY = 0f;
		if (_affectWholeHierarchy)
		{
			base.FreezeVerticalScroll(_affectWholeHierarchy);
		}
	}

	public override void UnfreezeVerticalScroll(bool _affectWholeHierarchy)
	{
		m_scrollInertiaMultiplerY = 1f;
		if (_affectWholeHierarchy)
		{
			base.UnfreezeVerticalScroll(_affectWholeHierarchy);
		}
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateAlign();
		UpdateMargins();
		if (d_Draw != null)
		{
			d_Draw(this);
		}
		UpdateChildren();
		ArrangeContents();
		UpdateUniqueCamera();
		if (m_parent == null)
		{
			UpdateSpecial();
		}
	}

	public override void UpdateSpecial()
	{
		UpdateUniqueCamera();
		base.UpdateSpecial();
	}

	public virtual void SetDragUpdate(float _size, RelativeTo _relativeTo, Action _delegate)
	{
		m_updateSize = _size;
		m_updateRelativeTo = _relativeTo;
		m_allowUpdate = true;
		m_dragUpdateDelegate = _delegate;
	}

	public override void UpdateSize()
	{
		base.UpdateSize();
		if (m_updateRelativeTo == RelativeTo.OwnHeight)
		{
			m_actualUpdateSize = m_actualHeight * m_updateSize;
		}
		else if (m_updateRelativeTo == RelativeTo.OwnWidth)
		{
			m_actualUpdateSize = m_actualWidth * m_updateSize;
		}
		else
		{
			m_actualUpdateSize = m_updateSize * m_tempReferenceUpdate;
		}
		if (m_dragToBottomRelativeTo == RelativeTo.OwnHeight)
		{
			m_actualDragToBottomSize = m_actualHeight * m_dragToBottomSize;
		}
		else if (m_dragToBottomRelativeTo == RelativeTo.OwnWidth)
		{
			m_actualDragToBottomSize = m_actualWidth * m_dragToBottomSize;
		}
		else
		{
			m_actualDragToBottomSize = m_dragToBottomSize * m_tempReferenceDragToBottom;
		}
	}

	public override void CalculateReferenceSizes()
	{
		base.CalculateReferenceSizes();
		if (m_allowUpdate)
		{
			m_tempUpdateRelativeTo = m_updateRelativeTo;
			if (m_updateRelativeTo == RelativeTo.ParentShortest)
			{
				m_tempUpdateRelativeTo = ((m_parent.m_actualWidth > m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
			}
			else if (m_updateRelativeTo == RelativeTo.ParentLongest)
			{
				m_tempUpdateRelativeTo = ((m_parent.m_actualWidth < m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
			}
			else if (m_updateRelativeTo == RelativeTo.ScreenShortest)
			{
				m_tempUpdateRelativeTo = ((Screen.width <= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
			}
			else if (m_updateRelativeTo == RelativeTo.ScreenLongest)
			{
				m_tempUpdateRelativeTo = ((Screen.width >= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
			}
			m_tempReferenceUpdate = Screen.height;
			if (m_tempUpdateRelativeTo == RelativeTo.ScreenWidth)
			{
				m_tempReferenceUpdate = Screen.width;
			}
			else if (m_tempUpdateRelativeTo == RelativeTo.ScreenHeight)
			{
				m_tempReferenceUpdate = Screen.height;
			}
			else if (m_tempUpdateRelativeTo == RelativeTo.ParentWidth)
			{
				m_tempReferenceUpdate = m_parent.m_actualWidth - m_tempParentMargins.l - m_tempParentMargins.r;
			}
			else if (m_tempUpdateRelativeTo == RelativeTo.ParentHeight)
			{
				m_tempReferenceUpdate = m_parent.m_actualHeight - m_tempParentMargins.b - m_tempParentMargins.t;
			}
		}
		if (m_allowDragToBottom)
		{
			m_tempDragToBottomRelativeTo = m_dragToBottomRelativeTo;
			if (m_dragToBottomRelativeTo == RelativeTo.ParentShortest)
			{
				m_tempDragToBottomRelativeTo = ((m_parent.m_actualWidth > m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
			}
			else if (m_dragToBottomRelativeTo == RelativeTo.ParentLongest)
			{
				m_tempDragToBottomRelativeTo = ((m_parent.m_actualWidth < m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
			}
			else if (m_dragToBottomRelativeTo == RelativeTo.ScreenShortest)
			{
				m_tempDragToBottomRelativeTo = ((Screen.width <= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
			}
			else if (m_dragToBottomRelativeTo == RelativeTo.ScreenLongest)
			{
				m_tempDragToBottomRelativeTo = ((Screen.width >= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
			}
			m_tempReferenceDragToBottom = Screen.height;
			if (m_tempDragToBottomRelativeTo == RelativeTo.ScreenWidth)
			{
				m_tempReferenceDragToBottom = Screen.width;
			}
			else if (m_tempDragToBottomRelativeTo == RelativeTo.ScreenHeight)
			{
				m_tempReferenceDragToBottom = Screen.height;
			}
			else if (m_tempDragToBottomRelativeTo == RelativeTo.ParentWidth)
			{
				m_tempReferenceDragToBottom = m_parent.m_actualWidth - m_tempParentMargins.l - m_tempParentMargins.r;
			}
			else if (m_tempDragToBottomRelativeTo == RelativeTo.ParentHeight)
			{
				m_tempReferenceDragToBottom = m_parent.m_actualHeight - m_tempParentMargins.b - m_tempParentMargins.t;
			}
		}
	}

	public override void UpdateUniqueCamera()
	{
		m_camera.orthographicSize = m_actualHeight * 0.5f;
		Vector3 position = m_TC.transform.position;
		m_camera.transform.localPosition = Vector3.forward * -500f;
		AdjustCamera();
	}

	public virtual void AdjustCamera()
	{
		if (!(m_camera != null) || !(m_camera.transform != null))
		{
			return;
		}
		UIComponent parent = m_parent;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		float num = 1f * (float)Screen.width;
		float num2 = 0f * (float)Screen.width;
		while (parent != null)
		{
			if (parent.GetType().IsSubclassOf(typeof(UIScrollableCanvas)))
			{
				UIScrollableCanvas uIScrollableCanvas = parent as UIScrollableCanvas;
				zero += uIScrollableCanvas.m_scrollTC.transform.localPosition;
				num = uIScrollableCanvas.m_camera.rect.xMax * (float)Screen.width;
				num2 = uIScrollableCanvas.m_camera.rect.xMin * (float)Screen.width;
				break;
			}
			zero2 += parent.m_TC.transform.localPosition;
			parent = parent.m_parent;
		}
		Vector3 position = m_TC.transform.position;
		float num3 = position.x - zero.x - m_actualWidth * 0.5f + (float)Screen.width * 0.5f;
		float actualWidth = m_actualWidth;
		float num4 = 0f;
		float num5 = 0f;
		if (num3 < num2)
		{
			num4 = num2 - num3;
			num3 = num2;
		}
		if (num3 + actualWidth > num)
		{
			num5 = num3 + actualWidth - num;
		}
		float num6 = position.y + (0f - zero.y) - m_actualHeight * 0.5f;
		float actualHeight = m_actualHeight;
		float y = m_camera.transform.localPosition.y;
		m_camera.transform.localPosition = new Vector3((0f - num5 + num4) * 0.5f, y, -500f);
		Rect rect = new Rect(num3 / (float)Screen.width, num6 / (float)Screen.height + 0.5f, (actualWidth - num4 - num5) / (float)Screen.width, actualHeight / (float)Screen.height);
		if (rect.width < 0f)
		{
			rect.width = 0f;
		}
		if (rect.height < 0f)
		{
			rect.height = 0f;
		}
		m_camera.rect = rect;
		if (m_TAC != null)
		{
			m_TAC.m_clip = true;
			m_TAC.m_clipBB.l = rect.xMin * (float)Screen.width;
			m_TAC.m_clipBB.r = rect.xMax * (float)Screen.width;
			m_TAC.m_clipBB.b = rect.yMin * (float)Screen.height;
			m_TAC.m_clipBB.t = rect.yMax * (float)Screen.height;
			if (m_prevClipRect.width != rect.width || m_prevClipRect.width != rect.height || m_prevClipRect.x != rect.x || m_prevClipRect.y != rect.y)
			{
				ChildrenInheritParentClip();
			}
			m_prevClipRect = rect;
		}
	}

	public override void ArrangeContents()
	{
		base.ArrangeContents();
		m_xMinLimit = 0f;
		m_xMaxLimit = Mathf.Max(m_contentWidth + m_actualMargins.l + m_actualMargins.r - m_actualWidth, 0f);
		m_allowScroll = true;
	}

	public override void Step()
	{
		float num = 0f;
		float num2 = 0f;
		if (m_allowScroll)
		{
			if (m_contentWidth > m_actualWidth - m_actualMargins.l - m_actualMargins.r)
			{
				num = m_contentCenterX - m_contentWidth * 0.5f + m_actualWidth * 0.5f - m_actualMargins.l;
			}
			if (m_contentHeight > m_actualHeight - m_actualMargins.t - m_actualMargins.b)
			{
				num2 = m_contentCenterY + m_contentHeight * 0.5f - m_actualHeight * 0.5f + m_actualMargins.t;
			}
			m_scrollInertiaX = Mathf.Min(m_maxScrollInertialX, Mathf.Max(0f - m_maxScrollInertialX, m_scrollInertiaX));
			m_scrollInertiaY = Mathf.Min(m_maxScrollInertialY, Mathf.Max(0f - m_maxScrollInertialY, m_scrollInertiaY));
			m_xPos = m_scrollTC.transform.localPosition.x - num;
			float num3 = m_scrollTC.transform.localPosition.y - num2;
			float num4 = Mathf.Max(m_contentHeight + m_actualMargins.t + m_actualMargins.b - m_actualHeight, 0f);
			if (m_overrideScrollPosition)
			{
				m_xPos = m_xMaxLimit * m_currentScrollX + num;
				num3 = num4 * m_currentScrollY + num2;
				TransformS.SetPosition(m_scrollTC, new Vector3(m_xPos, num3, 0f));
				m_overrideScrollPosition = false;
				base.Step();
				return;
			}
			float scrollInertiaX = m_scrollInertiaX;
			if ((m_xPos < m_xMinLimit || m_xPos > m_xMaxLimit) && m_TAC != null && m_TAC.m_touchCount > 0)
			{
				scrollInertiaX *= 0.382f;
			}
			else if (m_xPos < m_xMinLimit)
			{
				m_scrollInertiaX += m_xPos * (0f - (1f - m_scrollFallOff * 0.618f));
				m_scrollInertiaX = Mathf.Max(m_scrollInertiaX, m_xPos * (0f - (1f - m_scrollFallOff * 0.618f)));
				scrollInertiaX = (m_scrollInertiaX *= 0.382f);
			}
			else if (m_xPos > m_xMaxLimit)
			{
				m_scrollInertiaX += (m_xPos - m_xMaxLimit) * (0f - (1f - m_scrollFallOff * 0.618f));
				m_scrollInertiaX = Mathf.Min(m_scrollInertiaX, (m_xPos - m_xMaxLimit) * (0f - (1f - m_scrollFallOff * 0.618f)));
				scrollInertiaX = (m_scrollInertiaX *= 0.382f);
			}
			else
			{
				if (Mathf.Abs(m_scrollInertiaX) > 0.1f)
				{
					m_scrollInertiaX *= m_scrollFallOff;
				}
				else
				{
					m_scrollInertiaX = 0f;
				}
				scrollInertiaX = m_scrollInertiaX;
			}
			if (!m_recoverFromUpdate && !m_draggedToUpdate && m_allowUpdate && num3 > m_actualUpdateSize)
			{
				m_draggedToUpdate = true;
				m_dragUpdateDelegate();
			}
			if (num3 <= 0.1f && !m_draggedToUpdate && m_recoverFromUpdate)
			{
				m_recoverFromUpdate = false;
			}
			float num5 = 0f;
			if (m_draggedToUpdate)
			{
				num5 = m_actualUpdateSize;
			}
			if (!m_draggedToBottom && m_contentHeight > m_actualHeight && num3 <= 0f - num4 + m_actualDragToBottomSize)
			{
				m_draggedToBottom = true;
				if (m_dragBottomDelegate != null)
				{
					m_dragBottomDelegate();
				}
			}
			float scrollInertiaY = m_scrollInertiaY;
			if ((num3 > num5 || num3 < 0f - num4) && m_TAC != null && m_TAC.m_touchCount > 0)
			{
				scrollInertiaY *= 0.382f;
			}
			else if (num3 > num5)
			{
				m_scrollInertiaY += (num3 - num5) * (0f - (1f - m_scrollFallOff * 0.618f));
				m_scrollInertiaY = Mathf.Max(m_scrollInertiaY, (num3 - num5) * (0f - (1f - m_scrollFallOff * 0.618f)));
				scrollInertiaY = m_scrollInertiaY * 0.382f;
			}
			else if (num3 < 0f - num4)
			{
				m_scrollInertiaY += (num3 - (0f - num4)) * (0f - (1f - m_scrollFallOff * 0.618f));
				m_scrollInertiaY = Mathf.Min(m_scrollInertiaY, (num3 - (0f - num4)) * (0f - (1f - m_scrollFallOff * 0.618f)));
				scrollInertiaY = m_scrollInertiaY * 0.382f;
			}
			else
			{
				if (Mathf.Abs(m_scrollInertiaY) > 0.1f)
				{
					m_scrollInertiaY *= m_scrollFallOff;
				}
				else
				{
					m_scrollInertiaY = 0f;
				}
				scrollInertiaY = m_scrollInertiaY;
			}
			if (m_xMaxLimit > 0f)
			{
				m_currentScrollX = m_xPos / m_xMaxLimit;
			}
			else
			{
				m_currentScrollX = 0f;
			}
			if (num4 > 0f)
			{
				m_currentScrollY = num3 / (0f - num4);
			}
			else
			{
				m_currentScrollY = 0f;
			}
			scrollInertiaX *= m_scrollInertiaMultiplerX;
			scrollInertiaY *= m_scrollInertiaMultiplerY;
			if (scrollInertiaX != 0f || scrollInertiaY != 0f)
			{
				TransformS.Move(m_scrollTC, new Vector3(scrollInertiaX, scrollInertiaY, 0f));
			}
		}
		AdjustCamera();
		base.Step();
		if (m_setScrollToTarget)
		{
			m_currentScrollX = (m_setScrollTarget.m_TC.transform.position.x + m_xMaxLimit * 0.5f * num) / m_xMaxLimit;
			m_xPos = m_xMaxLimit * m_currentScrollX + num;
			TransformS.SetPosition(m_scrollTC, new Vector3(m_xPos, m_scrollTC.transform.localPosition.y, 0f));
			m_setScrollToTarget = false;
		}
		else
		{
			for (int i = 0; i < m_childs.Count; i++)
			{
				AreChildrenVisible(m_childs[i]);
			}
		}
	}

	public virtual void DragToUpdateHandled()
	{
		m_draggedToUpdate = false;
		m_recoverFromUpdate = true;
	}

	public virtual void DragToBottomHandled()
	{
		m_draggedToBottom = false;
	}

	public virtual void AreChildrenVisible(UIComponent _child)
	{
		if (_child.m_listenCameraEvents && _child.m_camera == m_camera)
		{
			float num = m_scrollTC.transform.position.y + m_actualHeight * 0.5f;
			float num2 = m_scrollTC.transform.position.y + m_actualHeight * -0.5f;
			float num3 = m_scrollTC.transform.position.x + m_actualWidth * -0.5f;
			float num4 = m_scrollTC.transform.position.x + m_actualWidth * 0.5f;
			Vector2 vector = _child.m_TC.transform.position;
			bool flag = false;
			if (m_camera.rect.width > 0f && m_camera.rect.height > 0f && vector.y + _child.m_actualHeight * 0.5f > num2 && vector.y + _child.m_actualHeight * -0.5f < num && vector.x + _child.m_actualWidth * 0.5f > num3 && vector.x + _child.m_actualWidth * -0.5f < num4)
			{
				flag = true;
			}
			if (flag && !_child.m_onCamera)
			{
				_child.EnterCamera();
			}
			if (!flag && _child.m_onCamera)
			{
				_child.ExitCamera();
			}
		}
		for (int i = 0; i < _child.m_childs.Count; i++)
		{
			AreChildrenVisible(_child.m_childs[i]);
		}
	}

	public virtual void FreezeParentScroll()
	{
		for (UIComponent parent = m_parent; parent != null; parent = parent.m_parent)
		{
			if (parent.GetType().IsSubclassOf(typeof(UIScrollableCanvas)))
			{
				parent.FreezeHorizontalScroll(false);
				parent.FreezeVerticalScroll(false);
				Debug.Log("FREEZE PARENT");
			}
		}
	}

	public override void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		base.TouchHandler(_touchArea, _touchPhase, _touchIsSecondary, _touchCount, _touches);
		if (_touchCount == 1)
		{
			TLTouch tLTouch = _touches[0];
			bool flag = _touchIsSecondary;
			if (_touchPhase == TouchAreaPhase.Began)
			{
				m_allowScroll = false;
				m_dragStartPos.x = _touches[0].m_currentPosition.x;
				m_dragStartPos.y = _touches[0].m_currentPosition.y;
				m_dragX = _touches[0].m_deltaPosition.x;
				m_dragY = _touches[0].m_deltaPosition.y;
			}
			if (flag && tLTouch.m_lockedSecondary == null && _touchPhase == TouchAreaPhase.RollIn)
			{
				TouchAreaS.LockSecondaryTouchArea(tLTouch);
				m_allowScroll = false;
			}
			switch (_touchPhase)
			{
			case TouchAreaPhase.DragStart:
				m_allowScroll = true;
				m_scrollInertiaX = 0f - tLTouch.m_deltaPosition.x;
				m_scrollInertiaY = 0f - tLTouch.m_deltaPosition.y;
				if (m_maxScrollInertialX != 0f || m_maxScrollInertialY != 0f)
				{
					PreventChildrenHit(true);
				}
				break;
			case TouchAreaPhase.MoveIn:
			case TouchAreaPhase.MoveOut:
				m_scrollInertiaX = (m_scrollInertiaX + (0f - tLTouch.m_deltaPosition.x)) * 0.5f;
				m_scrollInertiaY = (m_scrollInertiaY + (0f - tLTouch.m_deltaPosition.y)) * 0.5f;
				m_dragX = _touches[0].m_currentPosition.x - m_dragStartPos.x;
				m_dragY = _touches[0].m_currentPosition.y - m_dragStartPos.y;
				break;
			case TouchAreaPhase.StationaryIn:
			case TouchAreaPhase.StationaryOut:
				m_scrollInertiaX *= 0.5f;
				m_scrollInertiaY *= 0.5f;
				break;
			case TouchAreaPhase.DragEnd:
				PreventChildrenHit(false);
				UnfreezeVerticalScroll(false);
				UnfreezeHorizontalScroll(false);
				break;
			}
		}
		if (!m_passTouchesToScrollableParents)
		{
			return;
		}
		for (UIComponent parent = m_parent; parent != null; parent = parent.m_parent)
		{
			if (parent.GetType().IsSubclassOf(typeof(UIScrollableCanvas)))
			{
				parent.TouchHandler(parent.m_TAC, _touchPhase, _touchIsSecondary, _touchCount, _touches);
			}
		}
	}
}
