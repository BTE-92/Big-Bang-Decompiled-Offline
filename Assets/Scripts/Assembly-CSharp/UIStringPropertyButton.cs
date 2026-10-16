public class UIStringPropertyButton : UIVerticalListButton
{
	public UIText m_label;

	public UIText m_value;

	public UIStringPropertyButton(UIComponent _parent, string _tag, string _label, UIModel _model, string _STRINGfieldName)
		: base(_parent, null, null, _tag, _model, _STRINGfieldName)
	{
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		string text = "-";
		if ((string)GetValue() != null)
		{
			text = (string)GetValue();
		}
		m_label = new UIText(this, false, _tag, _label, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
		m_label.SetAlign(0f, 1f);
		m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
		m_value.SetAlign(1f, 0f);
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (_inside)
		{
			UITextInput uITextInput = new UITextInput(m_label.m_text, m_model, m_fieldName);
			uITextInput.SetText((string)GetValue());
			uITextInput.Update();
		}
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		m_value.SetText((string)_newValue);
	}
}
