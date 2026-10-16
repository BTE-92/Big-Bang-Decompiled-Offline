using UnityEngine;

public class PsUICenterWinDiamondChallenge : PsUICenterWinStarCollect
{
	public PsUICenterWinDiamondChallenge(UIComponent _parent)
		: base(_parent)
	{
	}

	public override void Preset()
	{
		PsMetagameManager.ShowResources(m_camera, false, false, true, false, false, 0.15f);
		m_topFrameName = "hud_big_diamond_top";
		m_bottomFrameName = "hud_big_diamond_bottom";
		m_spacing = -0.07f;
		m_centerVAlign = 1f;
	}

	public override void CreateReward(UIComponent _parent, int _score)
	{
		if (PsState.m_activeGameLoop.m_rewardOld < _score)
		{
			UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
			uIHorizontalList.SetHeight(0.175f, RelativeTo.ParentHeight);
			uIHorizontalList.RemoveDrawHandler();
			if (PsState.m_activeGameLoop.m_currentRunScore < _score)
			{
				UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "+" + PlayerPrefsX.GetDailyGemMultiplier(), "HurmeSemiBold_Font", 0.8f, RelativeTo.ParentHeight, "#fb62f4");
				UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_diamond_icon"));
				uIFittedSprite.SetHeight(1f, RelativeTo.ParentHeight);
			}
			if (PsState.m_activeGameLoop.m_currentRunScore >= _score)
			{
				m_flyers.Add(uIHorizontalList);
			}
		}
	}

	public override void FlyerTimerEvent()
	{
		PsMetagameManager.m_menuResourceView.CreateFlyingResources(PlayerPrefsX.GetDailyGemMultiplier(), m_camera.WorldToScreenPoint(m_flyers[0].m_TC.transform.position) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f, ResourceType.Diamonds);
		m_flyers.RemoveAt(0);
	}
}
