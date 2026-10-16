public class UIFittedTextButton : UIFittedText
{
	public UIFittedTextButton(UIComponent _parent, string _tag, string _text)
		: base(_parent, true, _tag, _text, "HurmeRegular_Font", true)
	{
		SetWidth(1f, RelativeTo.ParentWidth);
		SetHeight(0.1f, RelativeTo.ScreenHeight);
	}
}
