public class UIEditorTestMenu : UIVerticalList
{
	public UIRectSpriteButton m_restartButton;

	public UIRectSpriteButton m_editButton;

	public UIEditorTestMenu(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		SetAlign(1f, 1f);
		SetMargins(0.02f, RelativeTo.ScreenShortest);
		SetSpacing(0.03f, RelativeTo.ScreenShortest);
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.SetHorizontalAlign(1f);
		uIHorizontalList.RemoveDrawHandler();
		m_restartButton = new UIRectSpriteButton(uIHorizontalList, "Restart", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_retry"), false);
		m_restartButton.SetHeight(0.12f, RelativeTo.ScreenShortest);
		m_restartButton.SetVerticalAlign(1f);
		m_restartButton.SetTouchAreaSizeMultipler(1.5f);
		m_editButton = new UIRectSpriteButton(uIHorizontalList, "Edit", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_button_edit"), false);
		m_editButton.SetHeight(0.12f, RelativeTo.ScreenShortest);
		m_editButton.SetVerticalAlign(1f);
		m_editButton.SetTouchAreaSizeMultipler(1.2f);
		Update();
	}

	public override void Step()
	{
		if (m_editButton.m_hit)
		{
			PsState.m_activeGameLoop.ExitMinigame();
		}
		else if (m_restartButton.m_hit)
		{
			PsState.m_activeGameLoop.RestartMinigame();
		}
		base.Step();
	}
}
