using UnityEngine;

public class PsUICenterWinRaceFresh : PsUICenterWinRace
{
	private int m_realReward;

	public PsUICenterWinRaceFresh(UIComponent _parent)
		: base(_parent)
	{
	}

	public override void CreateLikeButtons(UIComponent _parent)
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
		UIVerticalList uIVerticalList = new UIVerticalList(_parent, "ASDASD");
		uIVerticalList.SetVerticalAlign(0.485f);
		uIVerticalList.RemoveDrawHandler();
		if (PsState.m_activeGameLoop.m_rewardOld < _score)
		{
			UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, "WEWE");
			uIHorizontalList.SetHeight(0.175f, RelativeTo.ParentHeight);
			uIHorizontalList.RemoveDrawHandler();
			if (PsState.m_activeGameLoop.m_currentRunScore < _score)
			{
				UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "+1", "HurmeSemiBold_Font", 0.8f, RelativeTo.ParentHeight, "#ffe025");
				UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_diamond_icon"));
				uIFittedSprite.SetHeight(1f, RelativeTo.ParentHeight);
			}
			if (PsState.m_activeGameLoop.m_currentRunScore >= _score)
			{
				m_flyers.Add(uIHorizontalList);
			}
		}
		if (PsState.m_activeGameLoop.m_currentRunScore < _score)
		{
			UIVerticalList uIVerticalList2 = new UIVerticalList(uIVerticalList, string.Empty);
			uIVerticalList2.RemoveDrawHandler();
			PsGameModeRace psGameModeRace = PsState.m_activeGameLoop.m_gameMode as PsGameModeRace;
			float num;
			switch (_score)
			{
			case 1:
				num = psGameModeRace.m_oneMedalTime;
				break;
			case 2:
				num = psGameModeRace.m_twoMedalTime;
				break;
			default:
				num = psGameModeRace.m_threeMedalTime;
				break;
			}
			float num2 = num;
			UIText uIText2 = new UIText(uIVerticalList2, false, string.Empty, (HighScores.TimeScoreToTime(PsState.m_activeGameLoop.m_timeScoreCurrent) - num2).ToString("F3"), "KGSecondChances_Font", 0.07875f, RelativeTo.ParentHeight, "#52a7ad");
			UIText uIText3 = new UIText(uIVerticalList2, false, string.Empty, "faster", "KGSecondChances_Font", 0.07875f, RelativeTo.ParentHeight, "#52a7ad");
		}
	}

	public override void CreateFriendRaceButton()
	{
	}

	public override void FlyerTimerEvent()
	{
		PsMetagameManager.m_menuResourceView.CreateFlyingResources(1, m_camera.WorldToScreenPoint(m_flyers[0].m_TC.transform.position) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f, ResourceType.Diamonds);
		m_flyers.RemoveAt(0);
	}
}
