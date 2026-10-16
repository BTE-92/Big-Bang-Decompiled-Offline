using UnityEngine;

public class PsUICreatePublishedBanner : PsUICreateLevelBanner
{
	public bool m_claimed;

	public int m_rewardAmount;

	public PsUICreatePublishedBanner(UIComponent _parent, PsGameLoop _gameLoop)
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
		if (m_loop.m_minigameMetaData.rewardCoins > 0)
		{
			m_rewardAmount = m_loop.m_minigameMetaData.rewardCoins;
			m_button.SetText("CLAIM: " + m_loop.m_minigameMetaData.rewardCoins, 0.04f);
			m_button.SetIcon("menu_resources_coin_icon");
			m_button.SetTealColors();
			m_claimed = false;
		}
		else
		{
			m_button.SetText("Play", 0.04f);
			m_button.SetGreenColors();
			m_claimed = true;
		}
		m_button.SetHorizontalAlign(1f);
		m_button.SetDepthOffset(-20f);
	}

	public void SwitchToPlayButton()
	{
		m_button.RemoveIcon();
		m_button.m_UItext.m_text = "Play";
		m_button.SetGreenColors();
		m_button.SetHorizontalAlign(1f);
		m_button.SetDepthOffset(-20f);
		m_button.Update();
		m_button.m_parent.ArrangeContents();
	}

	public override void CreateScreenshot(UIComponent _parent)
	{
		base.CreateScreenshot(_parent);
		m_screenshot.LoadPicture();
	}

	public void ButtonClicked()
	{
		if (!m_claimed)
		{
			PsMetagameManager.ClaimMinigameReward(m_loop.m_minigameMetaData, (Vector2)m_camera.WorldToScreenPoint(m_button.m_TC.transform.position) - new Vector2(Screen.width, Screen.height) * 0.5f);
			m_claimed = true;
			SwitchToPlayButton();
		}
		else
		{
			PsMetagameManager.HideResources();
			m_loop.StartLoop();
		}
	}
}
