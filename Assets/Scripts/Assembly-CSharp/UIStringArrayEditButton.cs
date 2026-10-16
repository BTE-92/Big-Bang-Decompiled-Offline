public class UIStringArrayEditButton : UIVerticalListButton
{
	public UIText m_label;

	public UIText m_value;

	public int m_arrayTargetIndex;

	public UIStringArrayEditButton(UIComponent _parent, string _tag, UIModel _model, string _STRINGARRAYfieldName, int _arrayTargetIndex)
		: base(_parent, null, null, _tag, _model, _STRINGARRAYfieldName)
	{
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		m_arrayTargetIndex = _arrayTargetIndex;
		string text = "-";
		if (((object[])GetValue())[_arrayTargetIndex] != null)
		{
			text = ((object[])GetValue())[_arrayTargetIndex].ToString();
		}
		m_label = new UIText(this, false, _tag, "[" + _arrayTargetIndex + "]", "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
		m_label.SetAlign(0f, 1f);
		m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
		m_value.SetAlign(1f, 0f);
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (_inside)
		{
			UITextInput uITextInput = new UITextInput(m_label.m_text + "[" + m_arrayTargetIndex + "]", m_model, m_fieldName, m_arrayTargetIndex);
			string text = "-";
			if (((object[])GetValue())[m_arrayTargetIndex] != null)
			{
				text = ((object[])GetValue())[m_arrayTargetIndex].ToString();
			}
			uITextInput.SetText(text);
			uITextInput.Update();
		}
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		string text = "-";
		if (((object[])_newValue)[m_arrayTargetIndex] != null)
		{
			text = ((object[])_newValue)[m_arrayTargetIndex].ToString();
		}
		m_value.SetText(text);
	}
}
