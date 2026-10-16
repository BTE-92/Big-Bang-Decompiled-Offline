using System;
using System.Collections;
using UnityEngine;

public class UIHashtableFloatPropertyButton : UIVerticalListButton
{
	protected float m_tempVal;

	public float m_minValue;

	public float m_maxValue;

	protected bool m_changeValue;

	protected string m_floatFieldName;

	private Hashtable m_upgrades;

	public UIText m_label;

	public UIText m_value;

	private UIModel m_keyboardModel;

	public UIHashtableFloatPropertyButton(UIComponent _parent, string _tag, string _label, UIModel _model, string _fieldName, string _FLOATfieldName, float _minValue = 0f, float _maxValue = 99999f)
		: base(_parent, null, null, _tag, _model, _fieldName)
	{
		m_minValue = _minValue;
		m_maxValue = _maxValue;
		m_floatFieldName = _FLOATfieldName;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		m_upgrades = (Hashtable)GetValue();
		string text = Convert.ToSingle(m_upgrades[m_floatFieldName]).ToString("0.000");
		m_label = new UIText(this, false, _tag, _label, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
		m_label.SetAlign(0f, 1f);
		m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
		m_value.SetAlign(1f, 0f);
	}

	protected override void OnTouchDragStart(TLTouch _touch)
	{
		base.OnTouchDragStart(_touch);
		Vector2 vector = _touch.m_currentPosition - _touch.m_startPosition;
		if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
		{
			FreezeVerticalScroll(false);
			m_changeValue = true;
			m_tempVal = (float)((Hashtable)GetValue())[m_floatFieldName];
		}
	}

	protected override void OnTouchMove(TLTouch _touch, bool _inside)
	{
		base.OnTouchMove(_touch, _inside);
		if (m_changeValue)
		{
			float x = _touch.m_deltaPosition.x;
			float tempVal = m_tempVal;
			m_tempVal = Mathf.Max(m_minValue, Mathf.Min(m_maxValue, tempVal + x / (Mathf.Max(m_actualWidth, (m_maxValue - m_minValue) * 10f) / (m_maxValue - m_minValue))));
			m_value.SetText(m_tempVal.ToString("0.000"));
		}
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (!_touch.m_dragged)
		{
			UITextInput uITextInput = new UITextInput(m_label.m_text, DoneCallback, CancelCallback, KeyPressedCallback, TouchScreenKeyboardType.NumbersAndPunctuation, false, 64);
			uITextInput.SetText(Convert.ToSingle(m_upgrades[m_floatFieldName]).ToString("0.000"));
			uITextInput.Update();
		}
		else
		{
			m_upgrades = (Hashtable)GetValue();
			m_upgrades[m_floatFieldName] = float.Parse(m_tempVal.ToString("0.000"));
			SetValue(m_upgrades);
		}
		UnfreezeVerticalScroll(false);
		m_changeValue = false;
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		m_upgrades = (Hashtable)_newValue;
		float num = Convert.ToSingle(m_upgrades[m_floatFieldName]);
		m_value.SetText(num.ToString("0.000"));
	}

	private void DoneCallback(string _value)
	{
		Debug.Log("DONE: " + _value);
		m_upgrades = (Hashtable)GetValue();
		float num = float.Parse(_value);
		m_upgrades[m_floatFieldName] = num;
		SetValue(m_upgrades);
		m_value.SetText(num.ToString("0.000"));
	}

	private void KeyPressedCallback(string _value)
	{
		Debug.Log("PRESSED: " + _value);
	}

	private void CancelCallback()
	{
		Debug.Log("CANCEL");
	}
}
