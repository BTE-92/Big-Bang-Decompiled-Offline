using UnityEngine;

public class UIPercentagePropertyButton : UIVerticalListButton
{
	public float m_minValue;

	public float m_maxValue;

	protected bool m_changeValue;

	public UIText m_label;

	public UIText m_value;

	public UIPercentagePropertyButton(UIComponent _parent, UIScrollableCanvas _scrollableCanvas, UIPagedCanvas _pagedCanvas, string _tag, string _label, UIModel _model, string _FLOATfieldName, float _minValue, float _maxValue)
		: base(_parent, _scrollableCanvas, _pagedCanvas, _tag, _model, _FLOATfieldName)
	{
		m_minValue = _minValue;
		m_maxValue = _maxValue;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		float num = (float)GetValue();
		string text = ((num - m_minValue) / (m_maxValue - m_minValue)).ToString("0%");
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
		}
	}

	protected override void OnTouchMove(TLTouch _touch, bool _inside)
	{
		base.OnTouchMove(_touch, _inside);
		if (m_changeValue)
		{
			float x = _touch.m_deltaPosition.x;
			float num = (float)GetValue();
			float num2 = Mathf.Max(m_minValue, Mathf.Min(m_maxValue, num + x / (Mathf.Max(m_actualWidth, m_maxValue - m_minValue) / (m_maxValue - m_minValue))));
			SetValue(num2);
			float num3 = (num2 - m_minValue) / (m_maxValue - m_minValue);
			m_value.SetText(num3.ToString("0%"));
		}
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		UnfreezeVerticalScroll(false);
		m_changeValue = false;
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		float num = float.Parse(_newValue.ToString());
		float num2 = (num - m_minValue) / (m_maxValue - m_minValue);
		m_value.SetText(num2.ToString("0%"));
	}
}
