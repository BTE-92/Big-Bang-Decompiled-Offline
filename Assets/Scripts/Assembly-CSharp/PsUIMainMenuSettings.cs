public class PsUIMainMenuSettings : UICanvas
{
	public UIRectSpriteButton m_button;

	public PsUIMainMenuSettings(UIComponent _parent)
		: base(_parent, false, "MainMenuSettings", null, string.Empty)
	{
		SetSize(0.2f, 0.2f, RelativeTo.ScreenShortest);
		SetMargins(0.04f, 0f, 0f, 0.025f, RelativeTo.ScreenShortest);
		RemoveDrawHandler();
		m_button = new UIRectSpriteButton(this, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_icon_settings"));
		m_button.SetAlign(0f, 0f);
	}
}
