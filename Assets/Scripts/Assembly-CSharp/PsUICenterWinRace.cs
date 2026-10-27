using System;
using System.Collections.Generic;
using UnityEngine;

public class PsUICenterWinRace : UICanvas
{
	private PsUIGenericButton m_restartButton;

	private PsUIGenericButton m_continueButton;

	private PsUIGenericButton m_everyplayButton;

	private UIRectSpriteButton m_pvpFriendButton;

	private UIRectSpriteButton m_thumbsUpButton;

	private UIRectSpriteButton m_thumbsDownButton;

	private UIHorizontalList m_likeArea;

	private PsRating m_rating = PsRating.Neutral;

	private bool m_ratingDone;

	protected bool m_animationsDone;

	private int m_rewardScoreAmount;

	private int m_oldScore;

	protected List<UIComponent> m_flyers = new List<UIComponent>();

	protected UIFittedSprite m_leftStar;

	protected UIFittedSprite m_centerStar;

	protected UIFittedSprite m_rightStar;

	protected UIFittedSprite m_leftSlot;

	protected UIFittedSprite m_centerSlot;

	protected UIFittedSprite m_rightSlot;

	private UIFittedSprite m_trophy;

	protected string m_topFrameName;

	protected string m_bottomFrameName;

	protected float m_starAnimDuration = 0.4f;

	protected float m_starEffectDelay = 0.1f;

	protected float m_spacing;

	protected float m_centerVAlign;

	private PsUIGenericButton m_offensiveButton;

	private PsUIBasePopup m_offensive;

	public PsUICenterWinRace(UIComponent _parent)
		: base(_parent, false, "CenterContent", null, string.Empty)
	{
		Preset();
		m_rating = PsState.m_activeGameLoop.GetRating();
		if (m_rating == PsRating.Unrated)
		{
			m_rating = PsRating.Neutral;
		}
		m_oldScore = PsState.m_activeGameLoop.m_rewardOld;
		m_rewardScoreAmount = PsState.m_activeGameLoop.m_currentRunScore - PsState.m_activeGameLoop.m_rewardOld;
		SetVerticalAlign(0f);
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, "Center");
		uIVerticalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIVerticalList.SetWidth(1f, RelativeTo.ParentWidth);
		uIVerticalList.SetVerticalAlign(0.55f);
		uIVerticalList.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
		uIHorizontalList.SetHeight(0.42f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetSpacing(m_spacing, RelativeTo.ScreenHeight);
		uIHorizontalList.SetVerticalAlign(1f);
		uIHorizontalList.RemoveDrawHandler();
		m_leftSlot = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_bottomFrameName));
		m_leftSlot.SetHeight(0.35f, RelativeTo.ScreenHeight);
		m_leftSlot.SetVerticalAlign(0f);
		m_leftSlot.SetDepthOffset(3f);
		m_leftSlot.RemoveDrawHandler();
		CreateReward(m_leftSlot, 1);
		if (PsState.m_activeGameLoop.m_currentRunScore > 0)
		{
			m_leftStar = new UIFittedSprite(m_leftSlot, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_topFrameName));
			m_leftStar.SetHeight(1f, RelativeTo.ParentHeight);
			TweenS.AddTransformTween(m_leftStar.m_TC, TweenedProperty.Scale, TweenStyle.BackOut, Vector3.zero, Vector3.one, m_starAnimDuration, 0f, true);
			TimerS.AddComponent(m_leftSlot.m_TC.p_entity, "EffectTimer", 0f, m_starEffectDelay, false, delegate(TimerC _c)
			{
				CreateStarEffect(m_leftSlot.m_TC, 1);
				TimerS.RemoveComponent(_c);
				if (m_oldScore < 1)
				{
					FlyerTimerEvent();
				}
			});
		}
		m_centerSlot = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_bottomFrameName));
		m_centerSlot.SetHeight(0.35f, RelativeTo.ScreenHeight);
		m_centerSlot.SetVerticalAlign(m_centerVAlign);
		m_centerSlot.RemoveDrawHandler();
		CreateReward(m_centerSlot, 2);
		if (PsState.m_activeGameLoop.m_currentRunScore > 1)
		{
			m_centerStar = new UIFittedSprite(m_centerSlot, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_topFrameName));
			m_centerStar.SetHeight(1f, RelativeTo.ParentHeight);
			TweenS.AddTransformTween(m_centerStar.m_TC, TweenedProperty.Scale, TweenStyle.BackOut, Vector3.zero, Vector3.one, m_starAnimDuration, m_starAnimDuration, true);
			TimerS.AddComponent(m_centerSlot.m_TC.p_entity, "EffectTimer", 0f, m_starAnimDuration + m_starEffectDelay, false, delegate(TimerC _c)
			{
				CreateStarEffect(m_centerSlot.m_TC, 2);
				TimerS.RemoveComponent(_c);
				if (m_oldScore < 2)
				{
					FlyerTimerEvent();
				}
			});
		}
		m_rightSlot = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_bottomFrameName));
		m_rightSlot.SetHeight(0.35f, RelativeTo.ScreenHeight);
		m_rightSlot.SetVerticalAlign(0f);
		m_rightSlot.SetDepthOffset(3f);
		m_rightSlot.RemoveDrawHandler();
		CreateReward(m_rightSlot, 3);
		if (PsState.m_activeGameLoop.m_currentRunScore > 2)
		{
			m_rightStar = new UIFittedSprite(m_rightSlot, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_topFrameName));
			m_rightStar.SetHeight(1f, RelativeTo.ParentHeight);
			TweenS.AddTransformTween(m_rightStar.m_TC, TweenedProperty.Scale, TweenStyle.BackOut, Vector3.zero, Vector3.one, m_starAnimDuration, m_starAnimDuration * 2f, true);
			TimerS.AddComponent(m_rightSlot.m_TC.p_entity, "EffectTimer", 0f, m_starAnimDuration * 2f + m_starEffectDelay, false, delegate(TimerC _c)
			{
				CreateStarEffect(m_rightSlot.m_TC, 3);
				TimerS.RemoveComponent(_c);
				if (m_oldScore < 3)
				{
					FlyerTimerEvent();
				}
			});
		}
		UIVerticalList uIVerticalList2 = new UIVerticalList(uIVerticalList, string.Empty);
		uIVerticalList2.RemoveDrawHandler();
		if (PsState.m_activeGameLoop.m_timeScoreCurrent == PsState.m_activeGameLoop.m_timeScoreBest)
		{
			UIText uIText = new UIText(uIVerticalList2, false, string.Empty, "New Record!", "HurmeBold_Font", 0.03f, RelativeTo.ScreenHeight, "#FFFFFF", "#000000");
			uIText.SetShadowShift(new Vector2(0f, -1f), 0.035f);
			UIText uIText2 = new UIText(uIVerticalList2, false, string.Empty, HighScores.TimeScoreToTimeString(PsState.m_activeGameLoop.m_timeScoreCurrent), "KGSecondChances_Font", 0.07f, RelativeTo.ScreenHeight, "#2ce768", "#121313");
			uIText2.SetShadowShift(new Vector2(0f, -1f), 0.035f);
		}
		else
		{
			UIText uIText3 = new UIText(uIVerticalList2, false, string.Empty, HighScores.TimeScoreToTimeString(PsState.m_activeGameLoop.m_timeScoreCurrent), "KGSecondChances_Font", 0.07f, RelativeTo.ScreenHeight, "#2ce768", "#121313");
			uIText3.SetShadowShift(new Vector2(0f, -1f), 0.035f);
			UIText uIText4 = new UIText(uIVerticalList2, false, string.Empty, "Your best time is: " + HighScores.TimeScoreToTimeString(PsState.m_activeGameLoop.m_timeScoreBest), "HurmeBold_Font", 0.03f, RelativeTo.ScreenHeight, "#DFDFDF", "#121313");
			uIText4.SetShadowShift(new Vector2(0f, -1f), 0.035f);
		}
		CreateVideoReplayButton();
		CreateRetryButton();
		CreateFriendRaceButton();
		UIVerticalList uIVerticalList3 = new UIVerticalList(this, string.Empty);
		uIVerticalList3.SetMargins(0.035f, RelativeTo.ScreenShortest);
		uIVerticalList3.SetAlign(1f, 0f);
		uIVerticalList3.SetSpacing(0.0275f, RelativeTo.ScreenHeight);
		uIVerticalList3.RemoveDrawHandler();
		m_continueButton = new PsUIGenericButton(uIVerticalList3, 0.25f, 0.25f, 0.0085f);
		m_continueButton.SetText("Continue", 0.07f);
		UIVerticalList uIVerticalList4 = new UIVerticalList(this, string.Empty);
		uIVerticalList4.SetMargins(0.03f, RelativeTo.ScreenShortest);
		uIVerticalList4.SetAlign(0f, 0f);
		uIVerticalList4.SetSpacing(0.025f, RelativeTo.ScreenHeight);
		uIVerticalList4.RemoveDrawHandler();
		if (PsMetagameData.GetUnlockableByIdentifier("Editor").m_unlocked)
		{
			CreateLikeButtons(uIVerticalList4);
			if (!(this is PsUICenterWinRaceFresh))
			{
				m_offensiveButton = new PsUIGenericButton(uIVerticalList4);
				m_offensiveButton.SetText("Offensive", 0.026f);
				m_offensiveButton.SetRedColors();
				m_offensiveButton.SetHorizontalAlign(0.03f);
			}
		}
	}

	// "Challenge a Friend!" button (picture from the UI atlas). It opens the friend list; in the offline game the built-in player BTE is the friend.
	public virtual void CreateFriendRaceButton()
	{
		m_pvpFriendButton = new UIRectSpriteButton(this, "ChallengeFriend", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_challenge_friend_button"));
		m_pvpFriendButton.SetHeight(0.2f, RelativeTo.ScreenHeight);
		m_pvpFriendButton.SetAlign(0.965f, 0.3f);
	}

	public virtual void Preset()
	{
		PsMetagameManager.ShowResources(m_camera, false, true, false, false, false, 0.15f);
		m_topFrameName = "hud_big_star_top";
		m_bottomFrameName = "hud_big_star_bottom";
		m_spacing = -0.07f;
		m_centerVAlign = 1f;
	}

	public virtual void CreateStarEffect(TransformC _tc, int _starEffectNumber)
	{
		int resourceIdentifier = (int)Enum.Parse(typeof(RESOURCE), "StarReward" + _starEffectNumber + "_GameObject");
		PrefabC prefabC = PrefabS.AddComponent(_tc, Vector3.zero, ResourceManager.GetGameObject((RESOURCE)resourceIdentifier));
		prefabC.p_gameObject.transform.position -= new Vector3(0f, 0f, 250f);
		prefabC.p_gameObject.transform.Rotate(90f, 0f, 0f);
		PrefabS.SetCamera(prefabC, m_camera);
		Debug.Log("EFFECT NUMBER: " + _starEffectNumber);
		ParticleSystem[] components = prefabC.p_gameObject.GetComponents<ParticleSystem>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Play();
		}
		components = prefabC.p_gameObject.GetComponentsInChildren<ParticleSystem>();
		for (int j = 0; j < components.Length; j++)
		{
			components[j].Play();
		}
		if (m_topFrameName.Equals("hud_big_diamond_top"))
		{
			SoundS.PlaySingleShotWithParameter("/Ingame/Events/GameEnd_DiamondGain", Vector3.zero, "Result", _starEffectNumber);
		}
		else if (m_topFrameName.Equals("hud_big_key_top"))
		{
			SoundS.PlaySingleShotWithParameter("/Ingame/Events/GameEnd_KeyGain", Vector3.zero, "Result", _starEffectNumber);
		}
		else
		{
			SoundS.PlaySingleShotWithParameter("/Ingame/Events/GameEnd_StarGain", Vector3.zero, "Result", _starEffectNumber);
		}
	}

	public virtual void CreateReward(UIComponent _parent, int _score)
	{
		UIVerticalList uIVerticalList = new UIVerticalList(_parent, string.Empty);
		uIVerticalList.SetVerticalAlign(0.385f);
		uIVerticalList.RemoveDrawHandler();
		if (PsState.m_activeGameLoop.m_rewardOld < _score)
		{
			UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
			uIHorizontalList.SetHeight(0.175f, RelativeTo.ParentHeight);
			uIHorizontalList.RemoveDrawHandler();
			if (PsState.m_activeGameLoop.m_currentRunScore < _score)
			{
				UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "+" + PsMetagameManager.GetGoalCoinReward(1, PsState.m_activeGameLoop.GetDifficulty()), "HurmeSemiBold_Font", 0.8f, RelativeTo.ParentHeight, "#ffe025");
				UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_coin_icon"));
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

	public virtual void CreateLikeButtons(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.SetSpacing(0.03f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		PsUICharacter psUICharacter = new PsUICharacter(uIHorizontalList, PsDialogueCharacter.Architect, PsDialogueCharacterPosition.Left);
		psUICharacter.SetHeight(0.25f, RelativeTo.ScreenHeight);
		psUICharacter.LookAtCamera();
		UICanvas uICanvas = new UICanvas(uIHorizontalList, false, "bubblecanvas", null, string.Empty);
		uICanvas.SetHeight(0.13f, RelativeTo.ScreenHeight);
		uICanvas.SetWidth(2.15f, RelativeTo.OwnHeight);
		uICanvas.SetMargins(0.02f, 0.02f, 0.0125f, -0.06f);
		uICanvas.SetDrawHandler(PsUIDrawHandlers.RatingSpeechBubble);
		uICanvas.SetVerticalAlign(0f);
		UITextbox uITextbox = new UITextbox(uICanvas, false, string.Empty, "Liked this level?", "HurmeSemiBold_Font", 0.03f, RelativeTo.ScreenHeight);
		uITextbox.m_tmc.m_textMesh.color = DebugDraw.HexToColor("#00b6ea");
		uITextbox.SetAlign(0.5f, 1f);
		m_likeArea = new UIHorizontalList(uICanvas, string.Empty);
		m_likeArea.SetSpacing(0f, RelativeTo.ScreenHeight);
		m_likeArea.SetHeight(0.15f, RelativeTo.ScreenHeight);
		m_likeArea.SetMargins(0.02f, 0.02f, 0f, 0f);
		m_likeArea.SetAlign(0.5f, 0f);
		m_likeArea.RemoveDrawHandler();
		string frameName = "menu_thumbs_up_off";
		string frameName2 = "menu_thumbs_down_off";
		if (m_rating == PsRating.Positive || m_rating == PsRating.Elated)
		{
			frameName = "menu_thumbs_up_on";
		}
		else if (m_rating == PsRating.Negative || m_rating == PsRating.Rejecting)
		{
			frameName2 = "menu_thumbs_down_on";
		}
		m_thumbsDownButton = new UIRectSpriteButton(m_likeArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName2));
		m_thumbsDownButton.SetHeight(1f, RelativeTo.ParentHeight);
		m_thumbsUpButton = new UIRectSpriteButton(m_likeArea, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName));
		m_thumbsUpButton.SetHeight(1f, RelativeTo.ParentHeight);
	}

	public virtual void CreateRetryButton()
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "Retry");
		uIHorizontalList.SetAlign(1f, 1f);
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.RemoveDrawHandler();
		m_restartButton = new PsUIGenericButton(uIHorizontalList);
		m_restartButton.SetIcon("hud_icon_restart", 0.06f);
		m_restartButton.SetOrangeColors();
		m_restartButton.SetMargins(0.035f, 0.035f, 0.02f, 0.02f);
	}

	public virtual void CreateVideoReplayButton()
	{
		if (EveryplayManager.IsEnabled())
		{
			m_everyplayButton = new PsUIGenericButton(this);
			m_everyplayButton.SetIcon("hud_button_replay", 0.06f);
			m_everyplayButton.SetText("Watch\nReplay!");
			m_everyplayButton.SetBlueColors();
			m_everyplayButton.SetMargins(0.035f, 0.035f, 0.02f, 0.02f);
			m_everyplayButton.SetAlign(0.5f, 0.025f);
		}
	}

	public virtual void FlyerTimerEvent()
	{
		PsMetagameManager.m_menuResourceView.CreateFlyingResources(PsMetagameManager.GetGoalCoinReward(1, PsState.m_activeGameLoop.GetDifficulty()), m_camera.WorldToScreenPoint(m_flyers[0].m_TC.transform.position) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f, ResourceType.Coins);
		m_flyers.RemoveAt(0);
	}

	public override void Step()
	{
		if (m_restartButton.m_hit)
		{
			RateLevel();
			(GetRoot() as PsUIBasePopup).CallAction("Restart");
		}
		else if (m_continueButton.m_hit)
		{
			RateLevel();
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		else if (m_everyplayButton != null && m_everyplayButton.m_hit)
		{
			Debug.Log("Opening everyplay...");
			EveryplayManager.StopRecording(0f);
			EveryplayManager.PlayLastRecording();
		}
		else if (m_thumbsUpButton != null && m_thumbsUpButton.m_hit)
		{
			if (m_rating == PsRating.Positive)
			{
				m_ratingDone = true;
				m_rating = PsRating.Neutral;
				m_thumbsUpButton.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame("menu_thumbs_up_off"));
				m_thumbsDownButton.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame("menu_thumbs_down_off"));
				m_likeArea.Update();
			}
			else
			{
				m_ratingDone = true;
				m_rating = PsRating.Positive;
				m_thumbsUpButton.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame("menu_thumbs_up_on"));
				m_thumbsDownButton.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame("menu_thumbs_down_off"));
				m_likeArea.Update();
				SoundS.PlaySingleShotWithParameter("/UI/Thumbs", Vector2.zero, "Rating", 2f);
			}
		}
		else if (m_thumbsDownButton != null && m_thumbsDownButton.m_hit)
		{
			if (m_rating == PsRating.Negative)
			{
				m_ratingDone = true;
				m_rating = PsRating.Neutral;
				m_thumbsUpButton.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame("menu_thumbs_up_off"));
				m_thumbsDownButton.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame("menu_thumbs_down_off"));
				m_likeArea.Update();
			}
			else
			{
				m_ratingDone = true;
				m_rating = PsRating.Negative;
				m_thumbsUpButton.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame("menu_thumbs_up_off"));
				m_thumbsDownButton.SetFrame(PsState.m_uiSheet.m_atlas.GetFrame("menu_thumbs_down_on"));
				m_likeArea.Update();
				SoundS.PlaySingleShotWithParameter("/UI/Thumbs", Vector2.zero, "Rating", -2f);
			}
		}
		else if (m_pvpFriendButton != null && m_pvpFriendButton.m_hit)
		{
			PsUIBasePopup friendlist = new PsUIBasePopup(typeof(PsUIChallengeFriendPopup));
			friendlist.SetAction("Exit", delegate
			{
				friendlist.Destroy();
			});
		}
		else if (m_offensiveButton != null && m_offensiveButton.m_hit)
		{
			m_offensive = new PsUIBasePopup(typeof(PsUICenterOffensive), typeof(PsUITopBanner), null, null, true, false);
			m_offensive.SetAction("Ok", OkOffensive);
			m_offensive.SetAction("Cancel", CancelOffensive);
		}
		base.Step();
	}

	private void RateLevel()
	{
		if (m_ratingDone)
		{
			PsMetagameManager.SendRating(m_rating, PsState.m_activeGameLoop, PsState.m_activeMinigame);
		}
	}

	public void OkOffensive()
	{
		m_offensive.Destroy();
		m_offensive = null;
		(GetRoot() as PsUIBasePopup).CallAction("Exit");
	}

	public void CancelOffensive()
	{
		m_offensive.Destroy();
		m_offensive = null;
	}
}
