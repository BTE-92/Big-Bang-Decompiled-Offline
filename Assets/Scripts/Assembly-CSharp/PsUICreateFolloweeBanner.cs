public class PsUICreateFolloweeBanner : PsUICreateLevelBanner
{
	public PsUICreateFolloweeBanner(UIComponent _parent, PsGameLoop _gameLoop)
		: base(_parent, _gameLoop)
	{
	}

	public override void CreateBannerEndIcon(UIComponent _parent)
	{
		base.CreateBannerEndIcon(_parent);
		m_bannerEndIcon.RemoveTouchAreas();
	}

	public override void CreateButton(UIComponent _parent)
	{
		m_button = new PsUIGenericButton(_parent);
		m_button.SetText("Play", 0.04f);
		m_button.SetGreenColors();
		m_button.SetHorizontalAlign(1f);
		m_button.SetDepthOffset(-20f);
	}

	public override void CreateScreenshot(UIComponent _parent)
	{
		base.CreateScreenshot(_parent);
		m_screenshot.LoadPicture();
	}
}
