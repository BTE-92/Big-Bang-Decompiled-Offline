public class UIPopupHeader : UIVerticalList
{
	public UIPopupHeader(UIComponent _parent, string _tag, string _header, string _caption)
		: base(_parent, _tag)
	{
		SetHeight(0.2f, RelativeTo.ScreenHeight);
		SetMargins(0.05f, 0.05f, 0f, 0.025f);
		RemoveDrawHandler();
		new UITextbox(this, false, "SavePanelHeaderHeader", "<color=#bfff3f>" + _header + "</color>", "KGLetHerGo_Font", 0.07f, RelativeTo.ScreenHeight);
		if (_caption != string.Empty)
		{
			new UITextbox(this, false, "SavePanelHeaderCaption", _caption, "HurmeRegular_Font", 0.03f, RelativeTo.ScreenHeight);
		}
	}
}
