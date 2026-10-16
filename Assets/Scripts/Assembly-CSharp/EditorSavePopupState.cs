public class EditorSavePopupState : BasicState
{
	public UIModel m_model;

	public string m_levelName;

	public string m_levelDescription;

	private UIScrollableCanvas m_savePopupContainer;

	private UITextArea m_nameField;

	private UITextArea m_descriptionField;

	private UISavePopupFooter m_footer;

	public override void Enter(IStatedObject _parent)
	{
		if (string.IsNullOrEmpty(PsState.m_activeGameLoop.m_minigameMetaData.id))
		{
			PsState.m_activeMinigame.GenerateDefaultNameAndDescription();
		}
		m_levelName = PsState.m_activeGameLoop.m_minigameMetaData.name;
		m_levelDescription = PsState.m_activeGameLoop.m_minigameMetaData.description;
		m_model = new UIModel(this);
		m_savePopupContainer = new UIScrollableCanvas(null, "SavePopupContainer");
		m_savePopupContainer.SetWidth(1f, RelativeTo.ScreenWidth);
		m_savePopupContainer.SetHeight(1f, RelativeTo.ScreenHeight);
		m_savePopupContainer.SetMargins(0.125f, 0.125f, 0f, 0f);
		m_savePopupContainer.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_savePopupContainer, "SavePopup");
		uIVerticalList.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList, "SavePopupHeader", "save", "Enter a name and press Save");
		UIPopupContentArea parent = new UIPopupContentArea(uIVerticalList, "ContentArea");
		m_nameField = new UITextArea(parent, "SavePopupNameField", "Track name", string.Empty, null, m_model, "m_levelName");
		m_descriptionField = new UITextArea(parent, "SavePopupDescriptionField", "Track description", string.Empty, null, m_model, "m_levelDescription", 3);
		m_footer = new UISavePopupFooter(uIVerticalList, "SavePopupFooter");
		m_savePopupContainer.Update();
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
		if (m_footer.m_saveButton.m_hit)
		{
			PsState.m_activeGameLoop.m_minigameMetaData.name = m_levelName;
			PsState.m_activeGameLoop.m_minigameMetaData.description = m_levelDescription;
			PsState.m_activeGameLoop.m_minigameMetaData.complexity = PsState.m_activeMinigame.m_complexity;
			PsState.m_activeGameLoop.m_minigameMetaData.levelRequirement = PsState.m_activeMinigame.m_levelRequirement;
			PsState.m_activeGameLoop.m_minigameMetaData.playerUnit = PsState.m_activeMinigame.m_playerUnitName;
			string[] array = new string[PsState.m_activeMinigame.m_itemCount.Keys.Count];
			int num = 0;
			foreach (string key in PsState.m_activeMinigame.m_itemCount.Keys)
			{
				array[num] = key;
				num++;
			}
			PsState.m_activeGameLoop.m_minigameMetaData.itemsUsed = array;
			PsState.m_activeGameLoop.m_minigameMetaData.timeSpentEditing += PsState.m_activeMinigame.GetTimeSinceInit();
			PsState.m_activeMinigame.InitializeTimer();
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorSaveState());
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
		else if (m_footer.m_cancelButton.m_hit)
		{
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
	}

	public override void Exit()
	{
		if (m_savePopupContainer != null)
		{
			m_savePopupContainer.Destroy();
			m_savePopupContainer = null;
		}
	}
}
