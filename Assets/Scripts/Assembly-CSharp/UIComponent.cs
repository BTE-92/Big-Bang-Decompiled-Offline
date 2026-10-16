using System;
using System.Collections.Generic;
using UnityEngine;

public class UIComponent
{
	public static int m_instanceCount;

	public bool m_uniqueCamera;

	public Camera m_camera;

	public bool m_listenCameraEvents;

	public bool m_hidden;

	public bool m_onCamera;

	public TransformC m_TC;

	public TouchAreaC m_TAC;

	public UIDrawDelegate d_Draw;

	public UIModel m_model;

	public string m_fieldName;

	public string m_identifier;

	public UIComponent m_parent;

	public List<UIComponent> m_childs;

	public bool m_disabled;

	public bool m_highlight;

	public bool m_highlightSecondary;

	public bool m_hit;

	public bool m_preventHit;

	public bool m_began;

	public bool m_end;

	public bool m_isDown;

	public bool m_dragged;

	public bool m_rogue;

	public TLTouch m_hittingTouch;

	public float m_width;

	public float m_height;

	public float m_actualWidth;

	public float m_actualHeight;

	public RelativeTo m_widthRelativeTo;

	public RelativeTo m_heightRelativeTo;

	public cpBB m_margins;

	public cpBB m_actualMargins;

	public RelativeTo m_marginsRelativeTo;

	protected float m_horizontalAlign;

	protected float m_verticalAlign;

	protected float m_depthOffset;

	protected RelativeTo m_tempWidthRelativeTo;

	protected RelativeTo m_tempHeightRelativeTo;

	protected RelativeTo m_tempMarginsRelativeTo;

	protected float m_tempReferenceWidth;

	protected float m_tempReferenceHeight;

	protected cpBB m_tempParentMargins;

	protected float m_touchAreaSizeMultipler;

	public UIComponent(UIComponent _parent, bool _touchable, string _tag = "", Camera _camera = null, UIModel _model = null, string _fieldName = "")
	{
		m_instanceCount++;
		m_childs = new List<UIComponent>();
		m_camera = _camera;
		if (m_camera == null)
		{
			if (_parent != null)
			{
				m_camera = _parent.m_camera;
			}
			else
			{
				m_camera = CameraS.m_uiCamera;
			}
		}
		m_model = _model;
		m_fieldName = _fieldName;
		m_disabled = false;
		m_highlight = false;
		m_highlightSecondary = false;
		m_hit = false;
		m_preventHit = false;
		m_began = false;
		m_end = false;
		m_hittingTouch = null;
		m_width = 1f;
		m_height = 1f;
		m_horizontalAlign = 0.5f;
		m_verticalAlign = 0.5f;
		m_depthOffset = -1f;
		m_margins = new cpBB(0f);
		m_touchAreaSizeMultipler = 1f;
		if (_parent == null)
		{
			m_widthRelativeTo = RelativeTo.ScreenWidth;
			m_heightRelativeTo = RelativeTo.ScreenHeight;
		}
		else
		{
			m_widthRelativeTo = RelativeTo.ParentWidth;
			m_heightRelativeTo = RelativeTo.ParentHeight;
		}
		m_marginsRelativeTo = RelativeTo.ScreenShortest;
		Entity entity = EntityManager.AddEntity(new string[2] { "UIComponent", _tag });
		entity.m_persistent = true;
		m_TC = TransformS.AddComponent(entity, "UI Component: " + _tag);
		if (_touchable)
		{
			m_TAC = TouchAreaS.AddRectArea(m_TC, _tag, m_width, m_height, m_camera);
			TouchAreaS.AddTouchEventListener(m_TAC, TouchHandler);
		}
		d_Draw = DrawHandler;
		Parent(_parent);
	}

	~UIComponent()
	{
		m_instanceCount--;
	}

	public virtual object GetValue()
	{
		if (m_model != null)
		{
			return m_model.GetValue(this);
		}
		return null;
	}

	public virtual void SetValue(object _value)
	{
		if (m_model != null)
		{
			m_model.SetValue(_value, this);
		}
		else
		{
			Debug.LogError("No model associated with this component");
		}
	}

	public virtual void OnValueChange(object _newValue, object _oldValue)
	{
	}

	public virtual void Parent(UIComponent _parent)
	{
		DetachFromParent(false);
		if (_parent != null)
		{
			m_parent = _parent;
			m_parent.m_childs.Add(this);
			TransformS.ParentComponent(m_TC, m_parent.m_TC);
			UIManager.m_uiComponents.Remove(this);
			if (m_camera != m_parent.m_camera)
			{
				SetCamera(m_parent.m_camera, true, false);
			}
			InheritParentClip();
		}
		else if (!UIManager.m_uiComponents.Contains(this))
		{
			UIManager.m_uiComponents.Add(this);
		}
	}

	public void MoveToIndexAtParentsChildList(int _index)
	{
		if (m_parent != null)
		{
			m_parent.m_childs.Remove(this);
			m_parent.m_childs.Insert(_index, this);
		}
	}

	public virtual void InheritParentClip()
	{
		if (m_TAC != null)
		{
			UIComponent parent = m_parent;
			while (parent != null && (parent.m_TAC == null || (parent.m_TAC != null && !parent.m_TAC.m_clip)))
			{
				parent = parent.m_parent;
			}
			if (parent != null && parent.m_TAC != null && parent.m_TAC.m_clip)
			{
				m_TAC.m_clip = true;
				m_TAC.m_clipBB = parent.m_TAC.m_clipBB;
			}
		}
	}

	public virtual void DetachFromParent()
	{
		DetachFromParent(true);
	}

	public virtual void DetachFromParent(bool _addToManager)
	{
		if (m_parent != null)
		{
			m_parent.m_childs.Remove(this);
			TransformS.UnparentComponent(m_parent.m_TC);
			m_parent = null;
			m_widthRelativeTo = RelativeTo.ScreenWidth;
			m_heightRelativeTo = RelativeTo.ScreenHeight;
			if (m_TAC != null)
			{
				m_TAC.m_clip = false;
			}
		}
		if (_addToManager && !UIManager.m_uiComponents.Contains(this))
		{
			UIManager.m_uiComponents.Add(this);
		}
	}

	public virtual void DestroyChildren()
	{
		while (m_childs.Count > 0)
		{
			int index = m_childs.Count - 1;
			m_childs[index].Destroy();
		}
	}

	public virtual void DestroyChildren(int _startIndex)
	{
		if (_startIndex >= 0 && _startIndex < m_childs.Count)
		{
			for (int num = m_childs.Count - 1; num > _startIndex - 1; num--)
			{
				m_childs[num].Destroy();
			}
		}
	}

	public virtual void DestroyChildren(int _startIndex, int _endIndex)
	{
		if (_startIndex >= 0 && _startIndex <= m_childs.Count && _endIndex >= _startIndex && _endIndex <= m_childs.Count)
		{
			for (int num = _endIndex; num > _startIndex - 1; num--)
			{
				m_childs[num].Destroy();
			}
		}
	}

	public virtual void Destroy()
	{
		Destroy(true);
	}

	public virtual void Destroy(bool _removeFromManager)
	{
		DestroyChildren();
		EntityManager.RemoveEntity(m_TC.p_entity, true, true);
		m_TAC = null;
		if (m_parent != null)
		{
			m_parent.m_childs.Remove(this);
			m_parent = null;
		}
		if (_removeFromManager)
		{
			UIManager.m_uiComponents.Remove(this);
		}
		if (m_model != null)
		{
			m_model.RemoveBinding(this);
		}
		if (m_uniqueCamera)
		{
			CameraS.RemoveCamera(m_camera);
		}
		m_camera = null;
		m_hit = false;
		m_hittingTouch = null;
	}

	public virtual UIComponent GetRoot()
	{
		UIComponent uIComponent = this;
		while (uIComponent.m_parent != null)
		{
			uIComponent = uIComponent.m_parent;
		}
		return uIComponent;
	}

	public virtual void DestroyRoot()
	{
		UIComponent uIComponent = this;
		while (uIComponent.m_parent != null)
		{
			uIComponent = uIComponent.m_parent;
		}
		if (uIComponent != null)
		{
			uIComponent.Destroy();
		}
	}

	public virtual Camera GetUniqueCameraFromParentHierarchy()
	{
		if (m_uniqueCamera)
		{
			return m_camera;
		}
		if (m_parent != null)
		{
			return m_parent.GetUniqueCameraFromParentHierarchy();
		}
		return null;
	}

	public virtual void SetCamera(Camera _camera, bool _affectChildren = false, bool _uniqueCamera = true)
	{
		if (m_parent == null && _affectChildren && m_uniqueCamera)
		{
			UnityEngine.Object.Destroy(m_camera);
		}
		m_uniqueCamera = _uniqueCamera;
		m_camera = _camera;
		m_TC.transform.gameObject.layer = _camera.gameObject.layer;
		if (m_TAC != null)
		{
			TouchAreaS.SetCamera(m_TAC, _camera);
		}
		if (_affectChildren)
		{
			for (int i = 0; i < m_childs.Count; i++)
			{
				m_childs[i].SetCamera(_camera, _affectChildren, false);
			}
		}
	}

	public virtual void SetSize(float _widthRatio, float _heightRatio, RelativeTo _relativeTo)
	{
		SetWidth(_widthRatio, _relativeTo);
		SetHeight(_heightRatio, _relativeTo);
	}

	public virtual void SetWidth(float _widthRatio, RelativeTo _relativeTo)
	{
		if (_relativeTo == RelativeTo.OwnWidth)
		{
			_relativeTo = RelativeTo.ParentWidth;
			Debug.LogWarning("Can't set width relative to own width. Changing to parent width.");
		}
		if (m_parent == null && _relativeTo == RelativeTo.ParentWidth)
		{
			_relativeTo = RelativeTo.ScreenWidth;
		}
		else if (m_parent == null && _relativeTo == RelativeTo.ParentHeight)
		{
			_relativeTo = RelativeTo.ScreenHeight;
		}
		m_widthRelativeTo = _relativeTo;
		if (_relativeTo == RelativeTo.OwnHeight && m_heightRelativeTo == RelativeTo.OwnWidth)
		{
			m_heightRelativeTo = RelativeTo.ParentHeight;
			Debug.LogWarning("Setting height relative to parent height.");
		}
		m_width = _widthRatio;
	}

	public virtual void SetHeight(float _heightRatio, RelativeTo _relativeTo)
	{
		if (_relativeTo == RelativeTo.OwnHeight)
		{
			_relativeTo = RelativeTo.ParentHeight;
			Debug.LogWarning("Can't set height relative to own height. Changing to parent height.");
		}
		if (m_parent == null && _relativeTo == RelativeTo.ParentWidth)
		{
			_relativeTo = RelativeTo.ScreenWidth;
		}
		else if (m_parent == null && _relativeTo == RelativeTo.ParentHeight)
		{
			_relativeTo = RelativeTo.ScreenHeight;
		}
		m_heightRelativeTo = _relativeTo;
		if (_relativeTo == RelativeTo.OwnWidth && m_widthRelativeTo == RelativeTo.OwnHeight)
		{
			m_widthRelativeTo = RelativeTo.ParentWidth;
			Debug.LogWarning("Setting width relative to parent width.");
		}
		m_height = _heightRatio;
	}

	public virtual void SetMargins(cpBB _marginsBB, RelativeTo _relativeTo = RelativeTo.ScreenHeight)
	{
		m_margins = _marginsBB;
		m_marginsRelativeTo = _relativeTo;
	}

	public virtual void SetMargins(float _left, float _right, float _top, float _bottom, RelativeTo _relativeTo = RelativeTo.ScreenHeight)
	{
		m_margins.l = _left;
		m_margins.r = _right;
		m_margins.t = _top;
		m_margins.b = _bottom;
		m_marginsRelativeTo = _relativeTo;
	}

	public virtual void SetMargins(float _margin, RelativeTo _relativeTo = RelativeTo.ScreenHeight)
	{
		m_margins.l = _margin;
		m_margins.r = _margin;
		m_margins.t = _margin;
		m_margins.b = _margin;
		m_marginsRelativeTo = _relativeTo;
	}

	public virtual void SetAlign(float _horizontal, float _vertical)
	{
		SetHorizontalAlign(_horizontal);
		SetVerticalAlign(_vertical);
	}

	public virtual void SetTouchAreaSizeMultipler(float _sizeMultipler)
	{
		m_touchAreaSizeMultipler = _sizeMultipler;
	}

	public virtual void SetHorizontalAlign(float _horizontal)
	{
		m_horizontalAlign = _horizontal;
	}

	public virtual void SetVerticalAlign(float _vertical)
	{
		m_verticalAlign = _vertical;
	}

	public virtual void SetDepthOffset(float _offset)
	{
		m_depthOffset = _offset;
	}

	public virtual void EnableTouchAreas(bool _affectChildren = true)
	{
		if (m_TAC != null && m_TAC.m_wasActive)
		{
			m_TAC.m_active = true;
		}
		if (_affectChildren)
		{
			for (int i = 0; i < m_childs.Count; i++)
			{
				m_childs[i].EnableTouchAreas(_affectChildren);
			}
		}
	}

	public virtual void DisableTouchAreas(bool _affectChildren = true)
	{
		if (m_TAC != null)
		{
			m_TAC.m_active = false;
		}
		if (_affectChildren)
		{
			for (int i = 0; i < m_childs.Count; i++)
			{
				m_childs[i].DisableTouchAreas(_affectChildren);
			}
		}
	}

	public virtual void CreateTouchAreas()
	{
		if (m_TAC == null)
		{
			m_TAC = TouchAreaS.AddRectArea(m_TC, m_TC.transform.name, m_width, m_height, m_camera);
			TouchAreaS.AddTouchEventListener(m_TAC, TouchHandler);
		}
	}

	public virtual void RemoveTouchAreas()
	{
		if (m_TAC != null)
		{
			TouchAreaS.RemoveArea(m_TAC);
			m_TAC = null;
		}
	}

	public virtual void PreventChildrenHit(bool _value)
	{
		for (int i = 0; i < m_childs.Count; i++)
		{
			m_childs[i].PreventHit(_value, true);
		}
	}

	public virtual void PreventHit(bool _value, bool _affectChildren)
	{
		m_preventHit = _value;
		if (_affectChildren)
		{
			PreventChildrenHit(_value);
		}
	}

	public virtual void Focus()
	{
	}

	public virtual void SetListenCameraEvents(bool _listen)
	{
		m_listenCameraEvents = _listen;
	}

	public virtual void EnterCamera()
	{
		m_onCamera = true;
		Show();
	}

	public virtual void ExitCamera()
	{
		m_onCamera = false;
		Hide();
	}

	public virtual void Hide()
	{
		m_hidden = true;
	}

	public virtual void Show()
	{
		m_hidden = false;
	}

	public virtual void FreezeHorizontalScroll(bool _affectWholeHierarchy)
	{
		if (m_parent != null)
		{
			m_parent.FreezeHorizontalScroll(_affectWholeHierarchy);
		}
	}

	public virtual void UnfreezeHorizontalScroll(bool _affectWholeHierarchy)
	{
		if (m_parent != null)
		{
			m_parent.UnfreezeHorizontalScroll(_affectWholeHierarchy);
		}
	}

	public virtual void FreezeVerticalScroll(bool _affectWholeHierarchy)
	{
		if (m_parent != null)
		{
			m_parent.FreezeVerticalScroll(_affectWholeHierarchy);
		}
	}

	public virtual void UnfreezeVerticalScroll(bool _affectWholeHierarchy)
	{
		if (m_parent != null)
		{
			m_parent.UnfreezeVerticalScroll(_affectWholeHierarchy);
		}
	}

	public virtual void SetActive(bool _value)
	{
		Activate(_value);
	}

	protected virtual void Activate(bool _value)
	{
		m_disabled = !_value;
	}

	protected virtual void Highlight(bool _value)
	{
		m_highlight = _value;
		m_isDown = _value;
	}

	protected virtual void HighlightSecondary(bool _value)
	{
		m_highlightSecondary = _value;
		m_isDown = _value;
	}

	protected virtual void Hit(TLTouch _hittingTouch)
	{
		m_hit = true;
		m_hittingTouch = _hittingTouch;
	}

	public void SetRogue()
	{
		m_rogue = true;
	}

	public virtual void UpdateSize()
	{
		if (m_widthRelativeTo == RelativeTo.OwnHeight)
		{
			m_actualHeight = m_height * m_tempReferenceHeight;
			m_actualWidth = m_actualHeight * m_width;
		}
		else if (m_heightRelativeTo == RelativeTo.OwnWidth)
		{
			m_actualWidth = m_width * m_tempReferenceWidth;
			m_actualHeight = m_actualWidth * m_height;
		}
		else if (m_widthRelativeTo == RelativeTo.World)
		{
			m_actualWidth = m_width;
			if (m_heightRelativeTo == RelativeTo.World)
			{
				m_actualHeight = m_height;
			}
			else
			{
				m_actualHeight = m_height * m_tempReferenceHeight;
			}
		}
		else if (m_heightRelativeTo == RelativeTo.World)
		{
			m_actualHeight = m_height;
			if (m_widthRelativeTo == RelativeTo.World)
			{
				m_actualWidth = m_width;
			}
			else
			{
				m_actualWidth = m_width * m_tempReferenceWidth;
			}
		}
		else
		{
			m_actualWidth = m_width * m_tempReferenceWidth;
			m_actualHeight = m_height * m_tempReferenceHeight;
		}
		UpdateCollider();
	}

	public virtual void UpdateCollider()
	{
		if (m_TAC != null)
		{
			if (m_TAC.m_colliderShape == ColliderShape.Rect)
			{
				TouchAreaS.ResizeRectCollider(m_TAC, m_actualWidth * m_touchAreaSizeMultipler, m_actualHeight * m_touchAreaSizeMultipler);
			}
			else if (m_TAC.m_colliderShape == ColliderShape.Circle)
			{
				TouchAreaS.ResizeCircleCollider(m_TAC, Mathf.Max(m_actualWidth, m_actualHeight) * m_touchAreaSizeMultipler * 0.5f);
			}
		}
	}

	public virtual void UpdateAlign()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		if (m_parent != null)
		{
			num = m_parent.m_actualWidth - m_tempParentMargins.l - m_tempParentMargins.r;
			num2 = m_parent.m_actualHeight - m_tempParentMargins.b - m_tempParentMargins.t;
		}
		float x = (num - m_actualWidth) * (-0.5f + m_horizontalAlign) + (m_tempParentMargins.l - m_tempParentMargins.r) * 0.5f;
		float y = (num2 - m_actualHeight) * (-0.5f + m_verticalAlign) + (m_tempParentMargins.b - m_tempParentMargins.t) * 0.5f;
		TransformS.SetPosition(_position: new Vector3(x, y, m_depthOffset), _c: m_TC);
	}

	public virtual void UpdateHorizontalAlign(float _yPos)
	{
		float num = Screen.width;
		if (m_parent != null)
		{
			num = m_parent.m_actualWidth - m_tempParentMargins.l - m_tempParentMargins.r;
		}
		float x = (num - m_actualWidth) * (-0.5f + m_horizontalAlign) + (m_tempParentMargins.l - m_tempParentMargins.r) * 0.5f;
		TransformS.SetPosition(_position: new Vector3(x, _yPos, m_depthOffset), _c: m_TC);
	}

	public virtual void UpdateVerticalAlign(float _xPos)
	{
		float num = Screen.height;
		if (m_parent != null)
		{
			num = m_parent.m_actualHeight - m_tempParentMargins.b - m_tempParentMargins.t;
		}
		float y = (num - m_actualHeight) * (-0.5f + m_verticalAlign) + (m_tempParentMargins.b - m_tempParentMargins.t) * 0.5f;
		TransformS.SetPosition(_position: new Vector3(_xPos, y, m_depthOffset), _c: m_TC);
	}

	public virtual void UpdateMargins()
	{
		float num = 0f;
		if (m_tempMarginsRelativeTo == RelativeTo.ParentWidth)
		{
			num = m_parent.m_actualWidth;
		}
		else if (m_tempMarginsRelativeTo == RelativeTo.ParentHeight)
		{
			num = m_parent.m_actualHeight;
		}
		else if (m_tempMarginsRelativeTo == RelativeTo.ScreenHeight)
		{
			num = Screen.height;
		}
		else if (m_tempMarginsRelativeTo == RelativeTo.ScreenWidth)
		{
			num = Screen.width;
		}
		else if (m_tempMarginsRelativeTo == RelativeTo.OwnHeight)
		{
			num = m_actualHeight;
		}
		else if (m_tempMarginsRelativeTo == RelativeTo.OwnWidth)
		{
			num = m_actualWidth;
		}
		else if (m_tempMarginsRelativeTo == RelativeTo.World)
		{
			num = 1f;
		}
		m_actualMargins.l = m_margins.l * num;
		m_actualMargins.r = m_margins.r * num;
		m_actualMargins.t = m_margins.t * num;
		m_actualMargins.b = m_margins.b * num;
	}

	public virtual void CalculateReferenceSizes()
	{
		m_tempParentMargins = default(cpBB);
		if (m_parent != null)
		{
			m_tempParentMargins = m_parent.m_actualMargins;
		}
		m_tempWidthRelativeTo = m_widthRelativeTo;
		if (m_widthRelativeTo == RelativeTo.ParentShortest)
		{
			m_tempWidthRelativeTo = ((m_parent.m_actualWidth > m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
		}
		else if (m_widthRelativeTo == RelativeTo.ParentLongest)
		{
			m_tempWidthRelativeTo = ((m_parent.m_actualWidth < m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
		}
		else if (m_widthRelativeTo == RelativeTo.ScreenShortest)
		{
			m_tempWidthRelativeTo = ((Screen.width <= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
		}
		else if (m_widthRelativeTo == RelativeTo.ScreenLongest)
		{
			m_tempWidthRelativeTo = ((Screen.width >= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
		}
		m_tempHeightRelativeTo = m_heightRelativeTo;
		if (m_heightRelativeTo == RelativeTo.ParentShortest)
		{
			m_tempHeightRelativeTo = ((m_parent.m_actualWidth > m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
		}
		else if (m_heightRelativeTo == RelativeTo.ParentLongest)
		{
			m_tempHeightRelativeTo = ((m_parent.m_actualWidth < m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
		}
		else if (m_heightRelativeTo == RelativeTo.ScreenShortest)
		{
			m_tempHeightRelativeTo = ((Screen.width <= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
		}
		else if (m_heightRelativeTo == RelativeTo.ScreenLongest)
		{
			m_tempHeightRelativeTo = ((Screen.width >= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
		}
		m_tempMarginsRelativeTo = m_marginsRelativeTo;
		if (m_marginsRelativeTo == RelativeTo.ParentShortest)
		{
			m_tempMarginsRelativeTo = ((m_parent.m_actualWidth > m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
		}
		else if (m_marginsRelativeTo == RelativeTo.ParentLongest)
		{
			m_tempMarginsRelativeTo = ((m_parent.m_actualWidth < m_parent.m_actualHeight) ? RelativeTo.ParentHeight : RelativeTo.ParentWidth);
		}
		else if (m_marginsRelativeTo == RelativeTo.ScreenShortest)
		{
			m_tempMarginsRelativeTo = ((Screen.width <= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
		}
		else if (m_marginsRelativeTo == RelativeTo.ScreenLongest)
		{
			m_tempMarginsRelativeTo = ((Screen.width >= Screen.height) ? RelativeTo.ScreenWidth : RelativeTo.ScreenHeight);
		}
		m_tempReferenceWidth = Screen.width;
		if (m_tempWidthRelativeTo == RelativeTo.ScreenWidth)
		{
			m_tempReferenceWidth = Screen.width;
		}
		else if (m_tempWidthRelativeTo == RelativeTo.ScreenHeight)
		{
			m_tempReferenceWidth = Screen.height;
		}
		else if (m_tempWidthRelativeTo == RelativeTo.World)
		{
			m_tempReferenceWidth = 1f;
		}
		else if (m_tempWidthRelativeTo == RelativeTo.ParentWidth)
		{
			m_tempReferenceWidth = m_parent.m_actualWidth - m_tempParentMargins.l - m_tempParentMargins.r;
		}
		else if (m_tempWidthRelativeTo == RelativeTo.ParentHeight)
		{
			m_tempReferenceWidth = m_parent.m_actualHeight - m_tempParentMargins.b - m_tempParentMargins.t;
		}
		m_tempReferenceHeight = Screen.height;
		if (m_tempHeightRelativeTo == RelativeTo.ScreenWidth)
		{
			m_tempReferenceHeight = Screen.width;
		}
		else if (m_tempHeightRelativeTo == RelativeTo.ScreenHeight)
		{
			m_tempReferenceHeight = Screen.height;
		}
		else if (m_tempHeightRelativeTo == RelativeTo.ParentWidth)
		{
			m_tempReferenceHeight = m_parent.m_actualWidth - m_tempParentMargins.l - m_tempParentMargins.r;
		}
		else if (m_tempHeightRelativeTo == RelativeTo.World)
		{
			m_tempReferenceHeight = 1f;
		}
		else if (m_tempHeightRelativeTo == RelativeTo.ParentHeight)
		{
			m_tempReferenceHeight = m_parent.m_actualHeight - m_tempParentMargins.b - m_tempParentMargins.t;
		}
	}

	public virtual void UpdateChildren()
	{
		InheritParentClip();
		for (int i = 0; i < m_childs.Count; i++)
		{
			m_childs[i].Update();
		}
	}

	public virtual void UpdateChildrenSizes()
	{
		for (int i = 0; i < m_childs.Count; i++)
		{
			m_childs[i].CalculateReferenceSizes();
			m_childs[i].UpdateSize();
			m_childs[i].ArrangeContents();
		}
	}

	public virtual void UpdateChildrenAlign()
	{
		for (int i = 0; i < m_childs.Count; i++)
		{
			m_childs[i].UpdateAlign();
		}
	}

	public virtual void ChildrenInheritParentClip()
	{
		for (int i = 0; i < m_childs.Count; i++)
		{
			m_childs[i].InheritParentClip();
			m_childs[i].ChildrenInheritParentClip();
		}
	}

	public virtual void ArrangeContents()
	{
	}

	public virtual void UpdateUniqueCamera()
	{
	}

	public virtual void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateAlign();
		UpdateMargins();
		if (!m_hidden && d_Draw != null)
		{
			d_Draw(this);
		}
		UpdateChildren();
		ArrangeContents();
		if (m_parent == null)
		{
			UpdateSpecial();
		}
		UpdateUniqueCamera();
	}

	public virtual void UpdateSpecial()
	{
		for (int i = 0; i < m_childs.Count; i++)
		{
			m_childs[i].UpdateSpecial();
		}
	}

	public virtual void Step()
	{
		for (int i = 0; i < m_childs.Count; i++)
		{
			m_childs[i].Step();
		}
		m_began = false;
		m_end = false;
		m_hit = false;
		m_hittingTouch = null;
	}

	protected virtual void OnTouchBegan(TLTouch _touch)
	{
		m_began = true;
		m_end = false;
		Highlight(true);
	}

	protected virtual void OnTouchMove(TLTouch _touch, bool _inside)
	{
	}

	protected virtual void OnTouchStationary(TLTouch _touch, bool _inside)
	{
	}

	protected virtual void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		if (m_end)
		{
			return;
		}
		if (_inside)
		{
			if (!m_preventHit || (m_preventHit && !_touch.m_dragged))
			{
				Hit(_touch);
			}
			Highlight(false);
			HighlightSecondary(false);
		}
		m_end = true;
	}

	protected virtual void OnTouchRollIn(TLTouch _touch, bool _secondary)
	{
		if (!m_end)
		{
			if (_secondary)
			{
				m_began = true;
				HighlightSecondary(true);
			}
			else
			{
				m_began = true;
				Highlight(true);
			}
		}
	}

	protected virtual void OnTouchRollOut(TLTouch _touch, bool _secondary)
	{
		if (!m_end)
		{
			if (_secondary)
			{
				m_end = true;
				HighlightSecondary(false);
			}
			else
			{
				m_end = true;
				Highlight(false);
			}
		}
	}

	protected virtual void OnTouchDragStart(TLTouch _touch)
	{
		m_dragged = true;
	}

	protected virtual void OnTouchDragEnd(TLTouch _touch)
	{
		m_dragged = false;
	}

	public virtual void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (m_disabled)
		{
			return;
		}
		TLTouch touch = _touches[0];
		if (_touchIsSecondary)
		{
			switch (_touchPhase)
			{
			case TouchAreaPhase.RollIn:
				OnTouchRollIn(touch, true);
				break;
			case TouchAreaPhase.RollOut:
				OnTouchRollOut(touch, true);
				break;
			}
			return;
		}
		switch (_touchPhase)
		{
		case TouchAreaPhase.MoveIn:
			OnTouchMove(touch, true);
			break;
		case TouchAreaPhase.MoveOut:
			OnTouchMove(touch, false);
			break;
		case TouchAreaPhase.StationaryIn:
			OnTouchStationary(touch, true);
			break;
		case TouchAreaPhase.StationaryOut:
			OnTouchStationary(touch, false);
			break;
		case TouchAreaPhase.Began:
			OnTouchBegan(touch);
			break;
		case TouchAreaPhase.ReleaseIn:
			OnTouchRelease(touch, true);
			break;
		case TouchAreaPhase.ReleaseOut:
			OnTouchRelease(touch, false);
			break;
		case TouchAreaPhase.RollIn:
			OnTouchRollIn(touch, false);
			break;
		case TouchAreaPhase.RollOut:
			OnTouchRollOut(touch, false);
			break;
		case TouchAreaPhase.DragStart:
			OnTouchDragStart(touch);
			break;
		case TouchAreaPhase.DragEnd:
			OnTouchDragEnd(touch);
			break;
		}
	}

	public virtual void SetDrawHandler(UIDrawDelegate _handler)
	{
		RemoveDrawHandler();
		if (_handler != null)
		{
			d_Draw = (UIDrawDelegate)Delegate.Combine(d_Draw, _handler);
		}
	}

	public virtual void SetTouchHandler(TouchEventDelegate _handler)
	{
		if (m_TAC != null)
		{
			TouchAreaS.RemoveAllTouchEventListeners(m_TAC);
			TouchAreaS.AddTouchEventListener(m_TAC, _handler);
		}
		else
		{
			Debug.LogWarning("UI element is not set touchable");
		}
	}

	public virtual void RemoveDrawHandler()
	{
		if (d_Draw != null)
		{
			Delegate[] invocationList = d_Draw.GetInvocationList();
			Delegate[] array = invocationList;
			foreach (Delegate obj in array)
			{
				d_Draw = (UIDrawDelegate)Delegate.Remove(d_Draw, (UIDrawDelegate)obj);
			}
		}
	}

	public virtual void DrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		DebugDraw.CreateBox(m_camera, m_TC, Vector2.zero, m_actualWidth, m_actualHeight);
		if (m_highlight)
		{
			SpriteS.SetColorByTransformComponent(m_TC, Color.green);
		}
		else if (m_highlightSecondary)
		{
			SpriteS.SetColorByTransformComponent(m_TC, Color.yellow);
		}
		else if (m_hit)
		{
			SpriteS.SetColorByTransformComponent(m_TC, Color.cyan);
		}
		Camera camera = m_camera;
		if (m_parent != null)
		{
			camera = m_parent.m_camera;
		}
		SpriteS.ConvertSpritesToPrefabComponent(m_TC, camera, true);
	}
}
