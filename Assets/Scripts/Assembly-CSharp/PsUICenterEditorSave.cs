using UnityEngine;

public class PsUICenterEditorSave : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_saveButton;

	private UITextArea m_nameField;

	private UITextArea m_missionField;

	private PsUILevelNameField m_levelName;

	private PsUILevelDescriptionField m_levelDescription;

	private new UIModel m_model;

	public PsUISwitch m_saveAsTemplate;

	public PsUICenterEditorSave(UIComponent _parent)
		: base(_parent, string.Empty, false)
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
		UIText parent = new UIText(uIHorizontalList, false, string.Empty, "SAVE LEVEL", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
		UICanvas uICanvas = new UICanvas(parent, false, string.Empty, null, string.Empty);
		uICanvas.SetHorizontalAlign(0f);
		uICanvas.SetMargins(-1.5f, 0f, -0.125f, -0.125f, RelativeTo.ParentHeight);
		uICanvas.RemoveDrawHandler();
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_header_save");
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
		SetWidth(0.7f, RelativeTo.ScreenWidth);
		SetVerticalAlign(0.5f);
		UIVerticalList uIVerticalList = new UIVerticalList(_parent, "ContentArea");
		uIVerticalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIVerticalList.RemoveDrawHandler();
		m_levelName = new PsUILevelNameField(uIVerticalList);
		m_levelName.SetText(PsState.m_activeGameLoop.m_minigameMetaData.name);
		m_levelDescription = new PsUILevelDescriptionField(uIVerticalList);
		m_levelDescription.SetText(PsState.m_activeGameLoop.m_minigameMetaData.description);
		m_levelDescription.SetHorizontalAlign(0f);
		m_saveButton = new PsUIGenericButton(this);
		m_saveButton.SetText("SAVE", 0.04f);
		m_saveButton.SetIcon("hud_icon_header_save");
		m_saveButton.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		m_saveButton.SetGreenColors();
		m_saveButton.SetDepthOffset(-4f);
		m_saveButton.SetAlign(0.85f, -0.25f);
	}

	public override void Step()
	{
		if (m_saveButton.m_hit)
		{
			PsIngameMenu.CloseAll();
			EditorBaseState.RemoveTransformGizmo();
			UndoManager.Purge();
			EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
			if (!PsState.m_activeMinigame.m_editing)
			{
				(LevelManager.m_currentLevel as Minigame).m_groundNode.RevertGroundFromPlay();
			}
			LevelManager.ResetCurrentLevel();
			PsState.m_activeMinigame.SetLayerItems();
			string[] array = new string[PsState.m_activeMinigame.m_itemCount.Keys.Count];
			int num = 0;
			foreach (string key in PsState.m_activeMinigame.m_itemCount.Keys)
			{
				array[num] = key;
				num++;
			}
			if (m_levelName.GetText() != PsState.m_activeGameLoop.m_minigameMetaData.name || m_levelDescription.GetText() != PsState.m_activeGameLoop.m_minigameMetaData.description)
			{
				PsState.m_activeMinigame.m_changed = true;
			}
			PsState.m_activeGameLoop.m_minigameMetaData.name = m_levelName.GetText();
			PsState.m_activeGameLoop.m_minigameMetaData.description = m_levelDescription.GetText();
			PsState.m_activeGameLoop.m_minigameMetaData.template = m_saveAsTemplate != null && m_saveAsTemplate.m_enabled;
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
			GetRoot().Destroy();
			if (PsState.m_activeMinigame.m_changed)
			{
				Debug.LogWarning("level changed, saving");
				Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorSaveState());
			}
			else
			{
				Debug.LogWarning("level not changed, just exit");
				if (PsState.m_activeGameLoop is PsGameLoopEditorTemplate)
				{
					(PsState.m_activeGameLoop as PsGameLoopEditorTemplate).ExitAfterEveryplay();
				}
				else
				{
					Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLoadingScene(Color.black));
				}
			}
		}
		base.Step();
	}

	private void NameDoneCallback(string _value)
	{
		Debug.Log("DONE: " + _value);
		m_nameField.SetValue(_value);
	}

	private void MissionDoneCallback(string _value)
	{
		Debug.Log("DONE: " + _value);
		m_missionField.SetValue(_value);
	}

	private void KeyPressedCallback(string _value)
	{
		Debug.Log("PRESSED: " + _value);
	}

	private void CancelCallback()
	{
		Debug.Log("CANCEL");
	}
}
