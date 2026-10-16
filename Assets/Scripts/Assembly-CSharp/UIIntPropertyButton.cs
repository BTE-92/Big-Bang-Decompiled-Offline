using UnityEngine;

public class UIIntPropertyButton : UIVerticalListButton
{
	protected int m_tempVal;

	public float m_minValue;

	public float m_maxValue;

	protected float m_currentFloatValue;

	protected bool m_changeValue;

	public UIText m_label;

	public UIText m_value;

	public UIIntPropertyButton(UIComponent _parent, string _tag, string _label, UIModel _model, string _INTfieldName, int _minValue = 0, int _maxValue = 99999)
		: base(_parent, null, null, _tag, _model, _INTfieldName)
	{
		m_minValue = _minValue;
		m_maxValue = _maxValue;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		m_currentFloatValue = float.Parse(GetValue().ToString());
		string text = Mathf.RoundToInt(m_currentFloatValue).ToString();
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
			m_tempVal = (int)GetValue();
			m_currentFloatValue = float.Parse(m_tempVal.ToString());
		}
	}

	protected override void OnTouchMove(TLTouch _touch, bool _inside)
	{
		base.OnTouchMove(_touch, _inside);
		if (m_changeValue)
		{
			float x = _touch.m_deltaPosition.x;
			float currentFloatValue = m_currentFloatValue;
			m_tempVal = Mathf.RoundToInt(m_currentFloatValue = Mathf.Max(m_minValue, Mathf.Min(m_maxValue, currentFloatValue + x / (Mathf.Max(m_actualWidth, m_maxValue - m_minValue) / (m_maxValue - m_minValue)))));
			m_value.SetText(m_tempVal.ToString());
		}
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (!_touch.m_dragged)
		{
			UITextInput uITextInput = new UITextInput(m_label.m_text, m_model, m_fieldName, TouchScreenKeyboardType.NumberPad);
			uITextInput.SetText(GetValue().ToString());
			uITextInput.Update();
		}
		else
		{
			SetValue(m_tempVal);
		}
		UnfreezeVerticalScroll(false);
		m_changeValue = false;
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		float f = float.Parse(_newValue.ToString());
		int num = Mathf.RoundToInt(f);
		m_value.SetText(num.ToString());
	}
}
