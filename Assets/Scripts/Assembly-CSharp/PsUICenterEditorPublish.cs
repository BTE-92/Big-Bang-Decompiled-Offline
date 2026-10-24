using UnityEngine;

public class PsUICenterEditorPublish : PsUIHeaderedCanvas
{
	private new UIModel m_model;

	private PsUIGenericButton m_goLiveButton;

	private UIEditorPublishScreenshotSelect m_screenshotSelector;

	private PsUILevelNameField m_levelName;

	private PsUILevelDescriptionField m_levelDescription;

	public PsUISwitch m_publishHidden;

	public PsUICenterEditorPublish(UIComponent _parent)
		: base(_parent, string.Empty)
	{
		GetRoot().SetDrawHandler(PsUIDrawHandlers.DebriefBackground);
		SetWidth(0.8f, RelativeTo.ScreenWidth);
		SetHeight(0.75f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		CreateContent(this);
		CreateHeaderContent(m_header);
		m_model = new UIModel(this);
	}

	public void CreateHeaderContent(UIComponent _parent)
	{
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText parent = new UIText(uIHorizontalList, false, string.Empty, "PUBLISH LEVEL", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		UICanvas uICanvas = new UICanvas(parent, false, string.Empty, null, string.Empty);
		uICanvas.SetHorizontalAlign(0f);
		uICanvas.SetMargins(-1.5f, 0f, -0.125f, -0.125f, RelativeTo.ParentHeight);
		uICanvas.RemoveDrawHandler();
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_header_publish");
		UISprite uISprite = new UISprite(uICanvas, false, string.Empty, PsState.m_uiSheet, frame);
		uISprite.SetSize(1f, 1f * (frame.height / frame.width), RelativeTo.ParentHeight);
		uISprite.SetHorizontalAlign(0f);
		uISprite.SetColor(DebugDraw.HexToColor("#95e5ff") * Color.gray);
	}

	public void CreateContent(UIComponent _parent)
	{
		PsState.m_activeMinigame.GenerateDefaultNameAndDescription();
		SetMargins(0.03f, 0.03f, 0f, 0.05f);
		SetHeight(0.65f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.5f);
		UICanvas uICanvas = new UICanvas(_parent, false, string.Empty, null, string.Empty);
		uICanvas.SetWidth(0.4f, RelativeTo.ParentWidth);
		uICanvas.SetHeight(1f, RelativeTo.ParentHeight);
		uICanvas.RemoveDrawHandler();
		uICanvas.SetAlign(0f, 0f);
		m_screenshotSelector = new UIEditorPublishScreenshotSelect(uICanvas, "Screenshot area");
		UICanvas uICanvas2 = new UICanvas(_parent, false, string.Empty, null, string.Empty);
		uICanvas2.SetWidth(0.6f, RelativeTo.ParentWidth);
		uICanvas2.SetHeight(1f, RelativeTo.ParentHeight);
		uICanvas.SetMargins(0.03f);
		uICanvas2.SetAlign(1f, 0f);
		uICanvas2.RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(uICanvas2, "vList");
		uIVerticalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		m_levelName = new PsUILevelNameField(uIVerticalList);
		m_levelName.SetText(PsState.m_activeGameLoop.m_minigameMetaData.name);
		m_levelDescription = new PsUILevelDescriptionField(uIVerticalList);
		m_levelDescription.SetText(PsState.m_activeGameLoop.m_minigameMetaData.description);
		m_levelDescription.SetHorizontalAlign(0f);
		m_goLiveButton = new PsUIGenericButton(this);
		m_goLiveButton.SetDepthOffset(-4f);
		m_goLiveButton.SetText("GO LIVE!", 0.04f);
		m_goLiveButton.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		m_goLiveButton.SetIcon("hud_icon_header_publish");
		m_goLiveButton.SetGreenColors();
		m_goLiveButton.SetAlign(0.85f, -0.25f);
	}

	public override void Step()
	{
		if (m_goLiveButton.m_hit)
		{
			PsIngameMenu.CloseAll();
			EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
			(LevelManager.m_currentLevel as Minigame).m_groundNode.RevertGroundFromPlay();
			PsState.m_activeGameLoop.m_minigameMetaData.creatorUpgrades = (PsState.m_activeMinigame.m_playerUnit as Vehicle).GetUpgradeValues();
			Debug.LogWarning("PUBLISH UPGRADES: " + PsState.m_activeGameLoop.m_minigameMetaData.creatorUpgrades);
			string[] array = new string[PsState.m_activeMinigame.m_itemCount.Keys.Count];
			int num = 0;
			foreach (string key in PsState.m_activeMinigame.m_itemCount.Keys)
			{
				array[num] = key;
				num++;
			}
			PsState.m_activeGameLoop.m_minigameMetaData.name = m_levelName.GetText();
			PsState.m_activeGameLoop.m_minigameMetaData.description = m_levelDescription.GetText();
			PsState.m_activeGameLoop.m_minigameMetaData.hidden = m_publishHidden != null && m_publishHidden.m_enabled;
			PsState.m_activeGameLoop.m_minigameMetaData.complexity = PsState.m_activeMinigame.m_complexity;
			PsState.m_activeGameLoop.m_minigameMetaData.levelRequirement = PsState.m_activeMinigame.m_levelRequirement;
			PsState.m_activeGameLoop.m_minigameMetaData.playerUnit = PsState.m_activeMinigame.m_playerUnitName;
			PsState.m_activeGameLoop.m_minigameMetaData.itemsUsed = array;
			PsState.m_activeGameLoop.m_minigameMetaData.timeSpentEditing += PsState.m_activeMinigame.GetTimeSinceInit();
			PsState.m_activeGameLoop.m_minigameMetaData.timeSpentInEditMode += PsState.m_activeMinigame.GetEditTimeSinceInit();
			PsState.m_activeGameLoop.m_minigameMetaData.lastPlaySessionStartCount = PsState.m_activeMinigame.m_gameStartCount;
			PsState.m_activeGameLoop.m_minigameMetaData.editSessionCount += PsState.m_activeMinigame.m_editSessionCount;
			PsState.m_activeGameLoop.m_minigameMetaData.groundsModificationCount += PsState.m_activeMinigame.m_groundsModificationCount;
			PsState.m_activeGameLoop.m_minigameMetaData.itemsModificationCount += PsState.m_activeMinigame.m_itemsModificationCount;
			PsState.m_activeMinigame.InitializeTimer();
			PsState.m_activeMinigame.ResetEditDateCounters();
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorPublishState());
		}
		base.Step();
	}
}
