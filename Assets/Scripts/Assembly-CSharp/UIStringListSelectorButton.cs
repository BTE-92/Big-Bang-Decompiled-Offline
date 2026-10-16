using System.Collections.Generic;
using UnityEngine;

public class UIStringListSelectorButton : UIVerticalListButton
{
	protected string[] m_valueLabels;

	public float m_minValue;

	public float m_maxValue;

	protected bool m_tempVal;

	protected float m_currentFloatValue;

	protected bool m_changeValue;

	public UIText m_label;

	private string m_selectionValue;

	private bool m_isSelected;

	private bool m_registerAsBound;

	public UIStringListSelectorButton(UIComponent _parent, string _tag, UIModel _model, string _selectedSTRINGLISTfieldName, string _selectionValue, string _selectionLabel, bool _registerAsBound = false)
		: base(_parent, null, null, _tag, _model, _selectedSTRINGLISTfieldName)
	{
		m_minValue = 0f;
		m_maxValue = 1f;
		m_selectionValue = _selectionValue;
		m_registerAsBound = _registerAsBound;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		List<string> list = (List<string>)GetValue();
		if (list.Contains(m_selectionValue))
		{
			m_isSelected = true;
		}
		else
		{
			m_isSelected = false;
		}
		m_label = new UIText(this, false, _tag, _selectionLabel, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
		m_label.SetAlign(0f, 1f);
	}

	protected override void OnTouchBegan(TLTouch _touch)
	{
		base.OnTouchBegan(_touch);
		m_changeValue = true;
	}

	protected override void OnTouchDragStart(TLTouch _touch)
	{
		base.OnTouchDragStart(_touch);
		m_changeValue = false;
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (m_changeValue)
		{
			List<string> list = (List<string>)GetValue();
			if (list.Contains(m_selectionValue))
			{
				list.Remove(m_selectionValue);
				m_isSelected = false;
			}
			else
			{
				list.Add(m_selectionValue);
				m_isSelected = true;
			}
			SetValue(list);
			m_changeValue = false;
			d_Draw(this);
		}
		UnfreezeVerticalScroll(false);
	}

	public override object GetValue()
	{
		if (m_model != null)
		{
			return m_model.GetValue(this, m_registerAsBound);
		}
		return null;
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		List<string> list = (List<string>)_newValue;
		if (list.Contains(m_selectionValue))
		{
			m_isSelected = true;
		}
		else
		{
			m_isSelected = false;
		}
	}

	public override void DrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(m_actualWidth, m_actualHeight * 0.95f, Vector2.zero);
		Color color;
		if (m_isSelected)
		{
			color = new Color(0.3f, 0.6f, 0.3f);
			if (m_highlight)
			{
				color = new Color(0.2f, 0.5f, 0.2f);
			}
		}
		else
		{
			color = new Color(0.4f, 0.4f, 0.4f);
			if (m_highlight)
			{
				color = new Color(0.3f, 0.3f, 0.3f);
			}
		}
		Camera camera = CameraS.m_uiCamera;
		if (m_parent != null)
		{
			camera = m_parent.m_camera;
		}
		uint num = DebugDraw.ColorToUInt(color);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, "UIComponent: Prefab");
	}
}
