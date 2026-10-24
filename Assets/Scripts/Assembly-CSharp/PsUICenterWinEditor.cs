using System;
using UnityEngine;

public class PsUICenterWinEditor : UICanvas
{
	private PsUIGenericButton m_restartButton;

	private PsUIGenericButton m_editButton;

	private PsUIGenericButton m_saveButton;

	private PsUIGenericButton m_publishButton;

	protected float m_starAnimDuration = 0.4f;

	protected float m_starEffectDelay = 0.1f;

	private PsUIBasePopup m_subPopup;

	public PsUICenterWinEditor(UIComponent _parent)
		: base(_parent, false, "CenterContent", null, string.Empty)
	{
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0f);
		RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "RightTop");
		uIHorizontalList.SetAlign(1f, 1f);
		uIHorizontalList.SetMargins(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.RemoveDrawHandler();
		m_restartButton = new PsUIGenericButton(uIHorizontalList);
		m_restartButton.SetIcon("hud_icon_restart", 0.06f);
		m_restartButton.SetOrangeColors();
		m_restartButton.SetMargins(0.035f, 0.035f, 0.02f, 0.02f);
		if (PsState.m_activeGameLoop.m_gameMode is PsGameModeStarCollect)
		{
			CreateStarCollectMiddleArea(this);
		}
		else if (PsState.m_activeGameLoop.m_gameMode is PsGameModeRace)
		{
			CreateRaceMiddleArea(this);
		}
		UIVerticalList uIVerticalList = new UIVerticalList(this, "Center");
		uIVerticalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIVerticalList.RemoveDrawHandler();
		uIVerticalList.SetVerticalAlign(0f);
		uIVerticalList.SetMargins(0f, 0f, 0f, 0.05f);
		bool flag = PsState.m_activeGameLoop.m_scoreCurrent == 3;
		if (!flag)
		{
			UIText uIText = new UIText(uIVerticalList, false, string.Empty, "Collect all three stars to Go Live", "HurmeBold_Font", 0.04f, RelativeTo.ScreenHeight);
		}
		else
		{
			UIText uIText2 = new UIText(uIVerticalList, false, string.Empty, "Go live if your level is FUN to play!", "HurmeBold_Font", 0.04f, RelativeTo.ScreenHeight);
		}
		m_publishButton = new PsUIGenericButton(uIVerticalList);
		m_publishButton.SetText("GO LIVE!", 0.07f);
		m_publishButton.SetIcon("hud_icon_header_publish", 0.08f);
		m_publishButton.SetBlueColors();
		if (!flag)
		{
			m_publishButton.RemoveTouchAreas();
			m_publishButton.SetGrayColors();
		}
		UIVerticalList uIVerticalList2 = new UIVerticalList(this, "Center");
		uIVerticalList2.SetSpacing(0.03f, RelativeTo.ScreenShortest);
		uIVerticalList2.SetAlign(1f, 0f);
		uIVerticalList2.SetMargins(0f, 0.05f, 0f, 0.05f);
		uIVerticalList2.SetWidth(0.2f, RelativeTo.ScreenWidth);
		uIVerticalList2.RemoveDrawHandler();
		m_editButton = new PsUIGenericButton(uIVerticalList2);
		m_editButton.SetText("KEEP" + Environment.NewLine + "EDITING", 0.036f);
		m_editButton.SetMargins(0.04f, 0.04f, 0.007f, 0.007f);
		m_editButton.SetOrangeColors();
		m_editButton.SetHorizontalAlign(1f);
		m_saveButton = new PsUIGenericButton(uIVerticalList2);
		m_saveButton.SetText("SAVE &" + Environment.NewLine + "EXIT", 0.036f);
		m_saveButton.SetSpacing(0f, RelativeTo.ScreenHeight);
		m_saveButton.SetOrangeColors();
		m_saveButton.SetMargins(0.04f, 0.04f, 0.007f, 0.007f);
		m_saveButton.SetHorizontalAlign(1f);
	}

	protected virtual void CreateStarCollectMiddleArea(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.SetHeight(0.42f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetSpacing(-0.07f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		string frameName = "hud_big_star_top";
		string frameName2 = "hud_big_star_bottom";
		UIFittedSprite m_leftSlot = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName2));
		m_leftSlot.SetHeight(0.35f, RelativeTo.ScreenHeight);
		m_leftSlot.SetVerticalAlign(0f);
		m_leftSlot.SetDepthOffset(3f);
		if (PsState.m_activeGameLoop.m_scoreCurrent > 0)
		{
			UIFittedSprite uIFittedSprite = new UIFittedSprite(m_leftSlot, false, "LeftStar", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName));
			uIFittedSprite.SetHeight(1f, RelativeTo.ParentHeight);
			TweenS.AddTransformTween(uIFittedSprite.m_TC, TweenedProperty.Scale, TweenStyle.BackOut, Vector3.zero, Vector3.one, m_starAnimDuration, 0f, true);
			TimerS.AddComponent(m_leftSlot.m_TC.p_entity, "EffectTimer", 0f, m_starEffectDelay, false, delegate(TimerC _c)
			{
				CreateStarEffect(m_leftSlot.m_TC, 1);
				TimerS.RemoveComponent(_c);
			});
		}
		UIFittedSprite m_centerSlot = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName2));
		m_centerSlot.SetHeight(0.35f, RelativeTo.ScreenHeight);
		m_centerSlot.SetVerticalAlign(1f);
		if (PsState.m_activeGameLoop.m_scoreCurrent > 1)
		{
			UIFittedSprite uIFittedSprite2 = new UIFittedSprite(m_centerSlot, false, "CenterStar", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName));
			uIFittedSprite2.SetHeight(1f, RelativeTo.ParentHeight);
			TweenS.AddTransformTween(uIFittedSprite2.m_TC, TweenedProperty.Scale, TweenStyle.BackOut, Vector3.zero, Vector3.one, m_starAnimDuration, m_starAnimDuration, true);
			uIFittedSprite2.SetDepthOffset(-1f);
			TimerS.AddComponent(m_centerSlot.m_TC.p_entity, "EffectTimer", 0f, m_starAnimDuration + m_starEffectDelay, false, delegate(TimerC _c)
			{
				CreateStarEffect(m_centerSlot.m_TC, 2);
				TimerS.RemoveComponent(_c);
			});
		}
		UIFittedSprite m_rightSlot = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName2));
		m_rightSlot.SetHeight(0.35f, RelativeTo.ScreenHeight);
		m_rightSlot.SetVerticalAlign(0f);
		m_rightSlot.SetDepthOffset(3f);
		if (PsState.m_activeGameLoop.m_scoreCurrent > 2)
		{
			UIFittedSprite uIFittedSprite3 = new UIFittedSprite(m_rightSlot, false, "RightStar", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName));
			uIFittedSprite3.SetHeight(1f, RelativeTo.ParentHeight);
			TweenS.AddTransformTween(uIFittedSprite3.m_TC, TweenedProperty.Scale, TweenStyle.BackOut, Vector3.zero, Vector3.one, m_starAnimDuration, m_starAnimDuration * 2f, true);
			TimerS.AddComponent(m_rightSlot.m_TC.p_entity, "EffectTimer", 0f, m_starAnimDuration * 2f + m_starEffectDelay, false, delegate(TimerC _c)
			{
				CreateStarEffect(m_rightSlot.m_TC, 3);
				TimerS.RemoveComponent(_c);
			});
		}
	}

	protected virtual void CreateRaceMiddleArea(UIComponent _parent)
	{
		UIVerticalList uIVerticalList = new UIVerticalList(_parent, string.Empty);
		uIVerticalList.SetSpacing(-0.02f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList, false, string.Empty, "Your time", "HurmeBold_Font", 0.04f, RelativeTo.ScreenHeight, "#ffffff", "#121313");
		uIText.SetShadowShift(new Vector2(1f, -1f).normalized, 0.035f);
		UIText uIText2 = new UIText(uIVerticalList, false, string.Empty, HighScores.TicksToTimeString(Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks)), "KGSecondChances_Font", 0.11f, RelativeTo.ScreenHeight, "#2ce768", "#121313");
		uIText2.SetShadowShift(new Vector2(1f, -1f).normalized, 0.035f);
	}

	protected virtual void CreateStarEffect(TransformC _tc, int _starEffectNumber)
	{
		int resourceIdentifier = (int)Enum.Parse(typeof(RESOURCE), "StarReward" + _starEffectNumber + "_GameObject");
		PrefabC prefabC = PrefabS.AddComponent(_tc, Vector3.zero, ResourceManager.GetGameObject((RESOURCE)resourceIdentifier));
		prefabC.p_gameObject.transform.position -= new Vector3(0f, 0f, 250f);
		prefabC.p_gameObject.transform.Rotate(90f, 0f, 0f);
		PrefabS.SetCamera(prefabC, m_camera);
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
		SoundS.PlaySingleShotWithParameter("/Ingame/Events/GameEnd_StarGain", Vector3.zero, "Result", _starEffectNumber);
	}

	public override void Step()
	{
		if (m_restartButton.m_hit)
		{
			PsIngameMenu.CloseAll();
			PsState.m_activeGameLoop.RestartMinigame();
		}
		else if (m_editButton.m_hit)
		{
			PsIngameMenu.CloseAll();
			PsState.m_activeGameLoop.ExitMinigame();
		}
		else if (m_saveButton.m_hit)
		{
			PsState.m_activeGameLoop.ExitEditor(false);
		}
		else if (m_publishButton.m_hit)
		{
			m_subPopup = new PsUIBasePopup(typeof(PsUICenterEditorPublish), null, null, null, true, false);
			m_subPopup.SetAction("Exit", Back);
		}
		base.Step();
	}

	private void Back()
	{
		if (m_subPopup != null)
		{
			m_subPopup.Destroy();
			m_subPopup = null;
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		if (m_subPopup != null)
		{
			m_subPopup.Destroy();
			m_subPopup = null;
		}
	}
}
