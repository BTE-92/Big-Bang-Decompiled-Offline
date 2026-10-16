using System;

public class UIListPropertyButton : UIVerticalListButton
{
	public UIText m_label;

	public UIText m_value;

	public Type m_selectorClassType;

	public UIListPropertyButton(UIComponent _parent, string _tag, string _label, UIModel _model, string _fieldName, Type _selectorClassType)
		: base(_parent, null, null, _tag, _model, _fieldName)
	{
		m_selectorClassType = _selectorClassType;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		string text = "-";
		if (GetValue() != null)
		{
			text = GetValue().ToString();
		}
		m_label = new UIText(this, false, _tag, _label, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
		m_label.SetAlign(0f, 1f);
		m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
		m_value.SetAlign(1f, 0f);
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		string text = "-";
		if (_newValue != null)
		{
			text = _newValue.ToString();
		}
		m_value.SetText(text);
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (_inside && !_touch.m_dragged && m_selectorClassType != null)
		{
			object[] args = new object[2] { m_model, m_fieldName };
			Activator.CreateInstance(m_selectorClassType, args);
		}
	}
}
