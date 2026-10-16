using UnityEngine;

public class PsUICenterGameCard : UICanvas
{
	public UICanvas m_profileCanvas;

	public PsUIGenericButton m_playButton;

	protected string m_colorBoxUp = "0072d7";

	protected string m_colorBoxDown = "005cb5";

	protected string m_colorStroke = "1fb7f6";

	protected string m_colorBevelUp = "0072d7";

	protected string m_colorBevelDown = "0072d7";

	protected bool m_metadataLoaded;

	protected UIFittedSprite m_unitIcon;

	protected UIFittedText m_contextText;

	protected UIFittedSprite m_scoreIcon;

	protected PsUIScreenshot m_screenshot;

	protected PsUIProfileImage m_profileIcon;

	protected UIVerticalList m_nameList;

	protected UITextbox m_description;

	protected PsUILoadingAnimation m_animation;

	public PsUICenterGameCard(UIComponent _parent)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		PsMenuScene.CreateBlur();
		m_metadataLoaded = !PsState.m_activeGameLoop.m_loadingMetaData;
		SetWidth(0.7f, RelativeTo.ScreenWidth);
		SetHeight(0.85f, RelativeTo.ParentHeight);
		SetVerticalAlign(0f);
		RemoveDrawHandler();
		CreateHeader();
		CreateMinigameCard();
		CreateButtons();
	}

	public override void Step()
	{
		if (!m_metadataLoaded && !PsState.m_activeGameLoop.m_loadingMetaData)
		{
			SetMetadataInfo();
		}
		if (m_playButton.m_hit)
		{
			SoundS.PlaySingleShot("/UI/ButtonNormal", Vector3.zero);
			(GetRoot() as PsUIBasePopup).CallAction("Play");
			(Main.m_currentGame.m_currentScene.GetCurrentState() as PsUIBaseState).DestroyCanvas();
		}
		base.Step();
	}

	public virtual void SetMetadataInfo()
	{
		m_animation.Destroy();
		m_animation = null;
		m_screenshot.m_gameId = PsState.m_activeGameLoop.GetGameId();
		m_screenshot.LoadPicture();
		m_profileIcon.m_facebookId = PsState.m_activeGameLoop.GetFacebookId();
		m_profileIcon.LoadPicture();
		CreateLevelAndCreatorNames(m_nameList);
		m_description.SetText(PsState.m_activeGameLoop.GetDescription());
		m_playButton.SetGreenColors();
		m_playButton.EnableTouchAreas();
		m_metadataLoaded = true;
		(GetRoot() as PsUIBasePopup).Update();
		Update();
	}

	public virtual void CreateHeader()
	{
	}

	public virtual void CreateHeaderIcon(UICanvas _parent)
	{
		string text = null;
		if (!m_metadataLoaded || PsState.m_activeGameLoop.GetPlayerUnit() == "OffroadCar")
		{
			text = "item_player_monster_car_icon";
		}
		else if (PsState.m_activeGameLoop.GetPlayerUnit() == "Motorcycle")
		{
			text = "item_player_motorcycle_icon";
		}
		if (text != null)
		{
			m_unitIcon = new UIFittedSprite(_parent, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(text));
			if (!m_metadataLoaded)
			{
				m_unitIcon.SetOverrideShader(Shader.Find("WOE/Fx/GreyscaleUnlitAlpha"));
			}
			m_unitIcon.SetHorizontalAlign(0f);
		}
	}

	public virtual void CreateHeaderText(UICanvas _parent)
	{
		string text = "Path Level " + PsState.m_activeGameLoop.m_levelNumber;
		m_contextText = new UIFittedText(_parent, false, string.Empty, text, "KGSecondChances_Font", true, "#ffffff", "#5c3a0a");
		m_contextText.SetHorizontalAlign(0f);
	}

	public virtual void CreateHeaderScore(UICanvas _parent)
	{
		int num = 0;
		if (m_metadataLoaded)
		{
			num = PsState.m_activeGameLoop.m_scoreBest;
		}
		string frameName = "menu_mode_star_" + num;
		m_scoreIcon = new UIFittedSprite(_parent, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(frameName));
		m_scoreIcon.SetAlign(1f, 1f);
	}

	public virtual void CreateMinigameCard()
	{
		UICanvas uICanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
		uICanvas.SetDrawHandler(MiddleCardDrawHandler);
		uICanvas.SetHeight(0.5f, RelativeTo.ScreenHeight);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uICanvas, string.Empty);
		uIHorizontalList.SetMargins(0.035f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetAlign(0f, 1f);
		uIHorizontalList.RemoveDrawHandler();
		m_screenshot = new PsUIScreenshot(uIHorizontalList, false, "Screenshot", Vector2.zero, PsState.m_activeGameLoop, m_metadataLoaded);
		m_screenshot.SetSize(0.2f, 0.2f, RelativeTo.ScreenWidth);
		m_screenshot.SetVerticalAlign(1f);
		m_screenshot.SetDepthOffset(-10f);
		UIVerticalList uIVerticalList = new UIVerticalList(uIHorizontalList, string.Empty);
		uIVerticalList.SetMargins(0f, 0f, 0.025f, 0f, RelativeTo.ScreenShortest);
		uIVerticalList.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIVerticalList.RemoveDrawHandler();
		m_profileCanvas = new UICanvas(uIVerticalList, true, "GameCardProfileCanvas", null, string.Empty);
		m_profileCanvas.SetWidth(0.4f, RelativeTo.ScreenWidth);
		m_profileCanvas.SetHeight(0.05f, RelativeTo.ScreenWidth);
		m_profileCanvas.SetHorizontalAlign(0f);
		m_profileCanvas.RemoveDrawHandler();
		m_profileCanvas.m_TAC.m_letTouchesThrough = true;
		UIHorizontalList uIHorizontalList2 = new UIHorizontalList(m_profileCanvas, string.Empty);
		uIHorizontalList2.SetHorizontalAlign(0f);
		uIHorizontalList2.SetSpacing(0.025f, RelativeTo.ScreenShortest);
		uIHorizontalList2.RemoveDrawHandler();
		string facebookId = string.Empty;
		string gameCenterId = string.Empty;
		if (m_metadataLoaded)
		{
			facebookId = PsState.m_activeGameLoop.GetFacebookId();
			gameCenterId = PsState.m_activeGameLoop.GetGamecenterId();
		}
		m_profileIcon = new PsUIProfileImage(uIHorizontalList2, false, string.Empty, facebookId, gameCenterId, -1, m_metadataLoaded);
		m_profileIcon.SetSize(0.05f, 0.05f, RelativeTo.ScreenWidth);
		m_nameList = new UIVerticalList(uIHorizontalList2, string.Empty);
		m_nameList.SetMargins(0f, 0f, -0.01f, 0f, RelativeTo.ScreenShortest);
		m_nameList.SetVerticalAlign(1f);
		m_nameList.RemoveDrawHandler();
		if (m_metadataLoaded)
		{
			CreateLevelAndCreatorNames(m_nameList);
		}
		string text = "Receiving data, hold still!";
		if (m_metadataLoaded)
		{
			text = PsState.m_activeGameLoop.GetDescription();
		}
		m_description = new UITextbox(uIVerticalList, false, string.Empty, text, "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest);
		m_description.SetWidth(0.5f, RelativeTo.ParentWidth);
		m_description.SetAlign(0f, 1f);
		if (!m_metadataLoaded)
		{
			m_animation = new PsUILoadingAnimation(this, false);
		}
		uIVerticalList.SetVerticalAlign(1f);
	}

	public virtual void CreateLevelAndCreatorNames(UIComponent _parent)
	{
		UIText uIText = new UIText(_parent, false, string.Empty, PsState.m_activeGameLoop.GetName(), "KGSecondChances_Font", 0.02f, RelativeTo.ScreenWidth, "#a0ff00");
		uIText.SetHorizontalAlign(0f);
		UIText uIText2 = new UIText(_parent, false, string.Empty, "by " + PsState.m_activeGameLoop.GetCreatorName(), "HurmeSemiBold_Font", 0.0175f, RelativeTo.ScreenWidth, "#fffec6");
		uIText2.SetHorizontalAlign(0f);
	}

	public virtual void CreateButtons()
	{
		m_playButton = new PsUIGenericButton(this);
		m_playButton.m_camera = m_camera;
		m_playButton.SetGreenColors();
		m_playButton.SetText("PLAY");
		m_playButton.SetAlign(0.8f, 0.15f);
		if (!m_metadataLoaded)
		{
			m_playButton.SetGreenColors(false);
			m_playButton.DisableTouchAreas();
		}
	}

	public void HeaderDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, (float)Screen.width * 0.025f, 8, Vector2.zero);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 5f, roundedRect, DebugDraw.HexToUint(m_colorBoxDown), DebugDraw.HexToUint(m_colorBoxUp), ResourceManager.GetMaterial(RESOURCE.MenuMetalLightMat_Material), _c.m_camera, "BG");
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 4f, roundedRect, (float)Screen.height * 0.0125f, DebugDraw.HexToColor(m_colorStroke), ResourceManager.GetMaterial("Framework/Line16Mat"), _c.m_camera, Position.Center, true);
	}

	public void MiddleCardDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		string hex = "008ce5";
		string hex2 = "0061a3";
		string hex3 = "46acec";
		string hex4 = "003b64";
		Color color = DebugDraw.HexToColor(hex4);
		color.a = 0.5f;
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, (float)Screen.width * 0.1125f, 16, Vector2.zero);
		Vector2[] roundedRect2 = DebugDraw.GetRoundedRect(_c.m_actualWidth - (float)Screen.height * 0.02f, _c.m_actualHeight - (float)Screen.height * 0.02f, (float)Screen.width * 0.1075f, 16, Vector2.zero);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 5f, roundedRect, DebugDraw.HexToUint(hex2), DebugDraw.HexToUint(hex), ResourceManager.GetMaterial(RESOURCE.MenuMetalMat_Material), _c.m_camera, "BG");
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 4f, roundedRect, (float)Screen.height * 0.005f, DebugDraw.HexToColor(hex4), DebugDraw.HexToColor(hex3), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 4f, roundedRect2, (float)Screen.height * 0.0075f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
	}
}
