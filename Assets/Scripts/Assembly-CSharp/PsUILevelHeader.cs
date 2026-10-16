using UnityEngine;

public class PsUILevelHeader : UICanvas
{
	private UITextbox m_description;

	private UIFittedText m_creatorName;

	public PsUILevelHeader(UIComponent _parent)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		PsGameLoop activeGameLoop = PsState.m_activeGameLoop;
		string text;
		string text2;
		string text3;
		string facebookId;
		if (activeGameLoop is PsGameLoopEditor)
		{
			text = PlayerPrefsX.GetUserName();
			text2 = ((activeGameLoop.m_minigameMetaData.timeSpentEditing <= 0) ? "Unsaved Level" : activeGameLoop.GetName());
			text3 = string.Empty;
			facebookId = PlayerPrefsX.GetFacebookId();
		}
		else
		{
			text = activeGameLoop.GetCreatorName();
			text2 = activeGameLoop.GetName();
			text3 = activeGameLoop.GetDescription();
			facebookId = activeGameLoop.GetFacebookId();
		}
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_main_top");
		Frame frame2 = PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_main_bottom");
		Frame frame3 = null;
		Frame frame4 = null;
		Frame frame5 = null;
		if (activeGameLoop is PsGameLoopVersus)
		{
			frame3 = PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_gamemode_challenge");
		}
		else if (activeGameLoop is PsGameLoopEditorPuzzle)
		{
			frame3 = PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_gamemode_builder");
		}
		else if (activeGameLoop is PsGameLoopEditorTemplate)
		{
			frame3 = PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_gamemode_empty");
		}
		else if (activeGameLoop is PsGameLoopFresh)
		{
			frame3 = PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_gamemode_diamonds");
		}
		else if (activeGameLoop is PsGameLoopChallenge)
		{
			frame3 = PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_gamemode_diamonds");
		}
		else
		{
			switch (activeGameLoop.GetGameMode())
			{
			case PsGameMode.Race:
				frame3 = PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_gamemode_race");
				break;
			case PsGameMode.Any:
			case PsGameMode.StarCollect:
				frame3 = PsState.m_uiSheet.m_atlas.GetFrame("menu_gamebanner_gamemode_catch");
				break;
			}
		}
		int num = Mathf.Max(activeGameLoop.m_scoreCurrent, activeGameLoop.m_scoreBest);
		frame4 = ((activeGameLoop is PsGameLoopFresh) ? PsState.m_uiSheet.m_atlas.GetFrame("menu_mode_diamond_" + num) : ((!(activeGameLoop is PsGameLoopChallenge)) ? PsState.m_uiSheet.m_atlas.GetFrame("menu_mode_star_" + num) : PsState.m_uiSheet.m_atlas.GetFrame("menu_mode_diamond_" + num)));
		string color = string.Empty;
		string text4 = string.Empty;
		switch (activeGameLoop.GetDifficulty())
		{
		case PsGameDifficulty.Easy:
			frame5 = PsState.m_uiSheet.m_atlas.GetFrame("menu_difficulty_casual");
			color = "#b7e112";
			text4 = "EASY";
			break;
		case PsGameDifficulty.Tricky:
			frame5 = PsState.m_uiSheet.m_atlas.GetFrame("menu_difficulty_tricky");
			color = "#ffbe10";
			text4 = "TRICKY";
			break;
		case PsGameDifficulty.Insane:
			frame5 = PsState.m_uiSheet.m_atlas.GetFrame("menu_difficulty_hardcore");
			color = "#f0723d";
			text4 = "HARD";
			break;
		case PsGameDifficulty.Impossible:
			frame5 = PsState.m_uiSheet.m_atlas.GetFrame("menu_difficulty_impossible");
			color = "#ff4e74";
			text4 = "INSANE";
			break;
		}
		float heightRatio = 0.13f;
		float num2 = 0.65f;
		float num3 = 0.05f;
		float num4 = -0.005f;
		float num5 = frame.width / (frame.height + frame2.height) * (1f - 2f * (num3 + num4));
		float num6 = frame.height / (frame.height + frame2.height);
		cpBB marginsBB = new cpBB(0f, num4, 0f, num3);
		cpBB marginsBB2 = new cpBB(0f, num3, 0f, num4);
		SetHeight(heightRatio, RelativeTo.ScreenHeight);
		base.SetWidth(num5 + num2, RelativeTo.OwnHeight);
		RemoveDrawHandler();
		UIFittedSprite uIFittedSprite = new UIFittedSprite(this, false, string.Empty, PsState.m_uiSheet, frame3);
		uIFittedSprite.SetHorizontalAlign(0f);
		uIFittedSprite.SetDepthOffset(-5f);
		uIFittedSprite.SetMargins(0.05f, 0.15f, 0f, -0.05f, RelativeTo.ParentHeight);
		if (!(PsState.m_activeGameLoop is PsGameLoopVersus) && !(activeGameLoop is PsGameLoopEditorTemplate))
		{
			UIFittedSprite uIFittedSprite2 = new UIFittedSprite(uIFittedSprite, false, string.Empty, PsState.m_uiSheet, frame4);
			uIFittedSprite2.SetVerticalAlign(0f);
			uIFittedSprite2.SetDepthOffset(-6f);
		}
		UICanvas uICanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
		uICanvas.SetHeight(num6, RelativeTo.ParentHeight);
		uICanvas.SetWidth(num5, RelativeTo.ParentHeight);
		uICanvas.SetVerticalAlign(1f);
		uICanvas.SetHorizontalAlign(1f);
		uICanvas.SetMargins(marginsBB, RelativeTo.ParentHeight);
		uICanvas.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite3 = new UIFittedSprite(uICanvas, false, string.Empty, PsState.m_uiSheet, frame);
		uIFittedSprite3.SetHorizontalAlign(0f);
		uIFittedSprite3.SetVerticalAlign(0f);
		uIFittedSprite3.SetMargins(1.35f, 0.9f, 0.15f, 0.2f, RelativeTo.ParentHeight);
		uIFittedSprite3.SetDepthOffset(-1f);
		UIFittedText uIFittedText = new UIFittedText(uIFittedSprite3, false, string.Empty, text2, "KGSecondChances_Font", true, "#ffffff", "#000000");
		uIFittedText.m_tmc.m_textMesh.fontStyle = FontStyle.Italic;
		uIFittedText.m_shadowtmc.m_textMesh.fontStyle = FontStyle.Italic;
		uIFittedText.SetShadowShift(new Vector2(1f, -1f).normalized, 0.04f);
		uIFittedText.SetHorizontalAlign(0f);
		if (frame5 != null)
		{
			UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
			uIVerticalList.SetWidth(0.6f, RelativeTo.ParentHeight);
			uIVerticalList.SetHorizontalAlign(1.03f);
			uIVerticalList.SetVerticalAlign(1f);
			uIVerticalList.RemoveDrawHandler();
			UIFittedSprite uIFittedSprite4 = new UIFittedSprite(uIVerticalList, false, string.Empty, PsState.m_uiSheet, frame5);
			uIFittedSprite4.SetDepthOffset(-3f);
			UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, string.Empty);
			uIHorizontalList.SetHeight(0.38f, RelativeTo.ParentWidth);
			uIHorizontalList.SetMargins(-1f, -1f, 0f, 0.05f, RelativeTo.ParentWidth);
			uIHorizontalList.RemoveDrawHandler();
			UIFittedText uIFittedText2 = new UIFittedText(uIHorizontalList, false, string.Empty, text4, "HurmeBold_Font", true, color);
		}
		UICanvas uICanvas2 = new UICanvas(this, false, string.Empty, null, string.Empty);
		uICanvas2.SetHeight(1f - num6, RelativeTo.ParentHeight);
		uICanvas2.SetWidth(num5, RelativeTo.ParentHeight);
		uICanvas2.SetVerticalAlign(0f);
		uICanvas2.SetHorizontalAlign(1f);
		uICanvas2.SetMargins(marginsBB2, RelativeTo.ParentHeight);
		uICanvas2.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite5 = new UIFittedSprite(uICanvas2, false, string.Empty, PsState.m_uiSheet, frame2);
		uIFittedSprite5.SetMargins(0.2f, RelativeTo.ParentHeight);
		uIFittedSprite5.SetHorizontalAlign(0f);
		uIFittedSprite5.SetVerticalAlign(1f);
		uIFittedSprite5.SetDepthOffset(0f);
		uIFittedSprite5.SetMargins(3.7f, 0.3f, 0.15f, 0.2f, RelativeTo.ParentHeight);
		m_creatorName = new UIFittedText(uIFittedSprite5, false, string.Empty, text, "KGSecondChances_Font", true, "#fffec6", "#000000");
		m_creatorName.SetShadowShift(new Vector2(1f, -1f).normalized, 0.04f);
		m_creatorName.SetHorizontalAlign(0f);
		m_creatorName.SetMargins(0f, -1f, 0f, 0f, RelativeTo.ParentHeight);
		if (!(activeGameLoop is PsGameLoopEditor))
		{
			if (string.IsNullOrEmpty(text3) || text3.Equals("No Description"))
			{
				if (activeGameLoop.m_gameMode is PsGameModeStarCollect)
				{
					text3 = PsStrings.Get(StringID.GAME_MODE_DESC_STARHUNT);
				}
				else if (activeGameLoop.m_gameMode is PsGameModeRace)
				{
					text3 = PsStrings.Get(StringID.GAME_MODE_DESC_RACING);
				}
			}
			m_description = new UITextbox(uIFittedSprite5, false, string.Empty, text3, "HurmeRegular_Font", 0.5f, RelativeTo.ParentHeight);
			m_description.SetWidth(0.84f, RelativeTo.ParentWidth);
			m_description.SetVerticalAlign(1f);
			m_description.SetMargins(0.15f, 0.15f, 0.05f, 0.15f, RelativeTo.ParentHeight);
			m_description.m_tmc.m_textMesh.color = DebugDraw.HexToColor("#000000");
			m_description.SetDrawHandler(PsUIDrawHandlers.DescriptionField);
			m_description.SetMinRows(1);
			m_description.SetMaxRows(4);
			m_description.SetDepthOffset(-6f);
		}
		PsUIProfileImage psUIProfileImage = new PsUIProfileImage(uICanvas2, false, string.Empty, facebookId, activeGameLoop.GetGamecenterId());
		psUIProfileImage.SetSize(1.15f, 1.15f, RelativeTo.ParentHeight);
		psUIProfileImage.SetDepthOffset(-7f);
		psUIProfileImage.SetHorizontalAlign(0.155f);
		TransformS.SetRotation(psUIProfileImage.m_TC, Vector3.forward * 4f);
	}

	public override void Update()
	{
		base.Update();
		if (m_description != null)
		{
			float x = m_creatorName.m_actualWidth / 2f + m_description.m_actualWidth * 1.25f;
			TransformS.SetPosition(m_description.m_TC, new Vector3(m_creatorName.m_TC.transform.position.x, m_description.m_TC.transform.localPosition.y, m_description.m_TC.transform.localPosition.z));
			TransformS.Move(m_description.m_TC, new Vector3(x, 0f, 0f));
		}
	}

	public new void SetWidth(float _width, RelativeTo _relativeTo)
	{
		Debug.LogWarning("Do not use PsUILevelHeader.SetWidth() method!");
	}

	public new void SetSize(float _width, float _height, RelativeTo _relativeTo)
	{
		Debug.LogWarning("Do not use PsUILevelHeader.SetSize() method!");
	}
}
