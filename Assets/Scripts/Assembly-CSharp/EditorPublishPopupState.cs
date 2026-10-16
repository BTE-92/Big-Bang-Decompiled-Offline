public class EditorPublishPopupState : BasicState
{
	public UIModel m_model;

	public string m_levelName;

	public string m_levelDescription;

	private UITextArea m_nameField;

	private UITextArea m_descriptionField;

	private UICanvas m_publishPopupContainer;

	private UIPublishPopupFooter m_footer;

	private UIEditorPublishScreenshotSelect m_screenshotSelector;

	public override void Enter(IStatedObject _parent)
	{
		if (string.IsNullOrEmpty(PsState.m_activeGameLoop.m_minigameMetaData.id))
		{
			PsState.m_activeMinigame.GenerateDefaultNameAndDescription();
		}
		m_levelName = PsState.m_activeGameLoop.GetName();
		m_levelDescription = PsState.m_activeGameLoop.GetDescription();
		m_model = new UIModel(this);
		m_publishPopupContainer = new UIScrollableCanvas(null, "PublishPopupContainer");
		m_publishPopupContainer.SetWidth(1f, RelativeTo.ScreenWidth);
		m_publishPopupContainer.SetHeight(1f, RelativeTo.ScreenHeight);
		m_publishPopupContainer.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_publishPopupContainer, "PublishPopup");
		uIVerticalList.SetMargins(0.05f, 0.05f, 0f, 0f);
		uIVerticalList.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList, "PublishPopupHeader", "GO LIVE!", "Enter a name and press 'Go Live!'");
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList, "hList");
		uIHorizontalList.SetDrawHandler(UIDrawHandlers.EditorPopupContentArea);
		m_screenshotSelector = new UIEditorPublishScreenshotSelect(uIHorizontalList, "Screenshot area");
		m_screenshotSelector.SetScreenshot(EditorScene.SwitchScreenshot(true));
		UIVerticalList uIVerticalList2 = new UIVerticalList(uIHorizontalList, "vList");
		uIVerticalList2.SetMargins(0.075f, 0.075f, 0.05f, 0.05f);
		uIVerticalList2.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIVerticalList2.SetWidth(0.6f, RelativeTo.ScreenWidth);
		uIVerticalList2.RemoveDrawHandler();
		m_nameField = new UITextArea(uIVerticalList2, "PublishPopupNameField", "Track name", string.Empty, null, m_model, "m_levelName");
		m_descriptionField = new UITextArea(uIVerticalList2, "SavePopupDescriptionField", "Track description", string.Empty, null, m_model, "m_levelDescription", 3);
		m_footer = new UIPublishPopupFooter(uIVerticalList, "PublishPopupFooter");
		m_publishPopupContainer.Update();
	}

	private void NameDoneCallback(string _value)
	{
		Debug.Log("DONE: " + _value);
		m_nameField.SetValue(_value);
	}

	private void DescriptionDoneCallback(string _value)
	{
		Debug.Log("DONE: " + _value);
		m_descriptionField.SetValue(_value);
	}

	private void KeyPressedCallback(string _value)
	{
		Debug.Log("PRESSED: " + _value);
	}

	private void CancelCallback()
	{
		Debug.Log("CANCEL");
	}

	public override void Execute()
	{
		if (m_footer.m_publishButton.m_hit)
		{
			PsState.m_activeGameLoop.m_minigameMetaData.name = m_levelName;
			PsState.m_activeGameLoop.m_minigameMetaData.description = m_levelDescription;
			PsState.m_activeGameLoop.m_minigameMetaData.complexity = PsState.m_activeMinigame.m_complexity;
			PsState.m_activeGameLoop.m_minigameMetaData.levelRequirement = PsState.m_activeMinigame.m_levelRequirement;
			PsState.m_activeGameLoop.m_minigameMetaData.playerUnit = PsState.m_activeMinigame.m_playerUnitName;
			Debug.LogWarning("PUBLISH UPGRADES: " + PsState.m_activeGameLoop.m_minigameMetaData.creatorUpgrades);
			string[] array = new string[PsState.m_activeMinigame.m_itemCount.Keys.Count];
			int num = 0;
			foreach (string key in PsState.m_activeMinigame.m_itemCount.Keys)
			{
				array[num] = key;
				num++;
			}
			PsState.m_activeGameLoop.m_minigameMetaData.itemsUsed = array;
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorPublishState());
			PsState.m_activeGameLoop.m_minigameMetaData.timeSpentEditing += PsState.m_activeMinigame.GetTimeSinceInit();
			PsState.m_activeMinigame.InitializeTimer();
		}
		else if (m_footer.m_cancelButton.m_hit)
		{
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorTestEndState());
		}
		else if (m_nameField.m_hit)
		{
			UITextInput uITextInput = new UITextInput("Track name", NameDoneCallback, CancelCallback);
			uITextInput.SetText((string)m_nameField.GetValue());
			uITextInput.SetMaxCharacterCount(32);
			uITextInput.SetMinCharacterCount(3);
			uITextInput.Update();
		}
		else if (m_descriptionField.m_hit)
		{
			UITextInput uITextInput2 = new UITextInput("Track description", DescriptionDoneCallback, CancelCallback);
			uITextInput2.SetText((string)m_descriptionField.GetValue());
			uITextInput2.Update();
		}
	}

	public override void Exit()
	{
		if (m_publishPopupContainer != null)
		{
			m_publishPopupContainer.Destroy();
			m_publishPopupContainer = null;
		}
	}
}
