public class PsUICreateSignalBanner : PsUICreateLevelBanner
{
	public PsUICreateSignalBanner(UIComponent _parent, PsGameLoopEditorPuzzle _gameLoop, PsUnlockable _signal)
		: base(_parent, _gameLoop, _signal)
	{
	}

	public override void Initialize()
	{
	}

	public override void CreateButton(UIComponent _parent)
	{
		if (m_loop != null)
		{
			m_button = new PsUIGenericButton(_parent);
			m_button.SetText("Study", 0.04f);
			m_button.SetGreenColors();
			m_button.SetHorizontalAlign(1f);
			m_button.SetDepthOffset(-20f);
		}
	}

	public override void CreateBannerEndIcon(UIComponent _parent)
	{
	}

	public override void CreateScreenshot(UIComponent _parent)
	{
		base.CreateScreenshot(_parent);
		UIFittedSprite uIFittedSprite = new UIFittedSprite(m_screenshot, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("item_dome_empty_icon"));
		uIFittedSprite.SetHeight(0.8f, RelativeTo.ParentHeight);
		uIFittedSprite = new UIFittedSprite(m_screenshot, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_signal.m_iconImage));
		uIFittedSprite.SetHeight(0.5f, RelativeTo.ParentHeight);
		uIFittedSprite.SetDepthOffset(-10f);
		uIFittedSprite.SetVerticalAlign(0.618f);
	}

	public override void CreateGameMode(UIComponent _parent)
	{
	}
}
