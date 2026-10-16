using System.Collections.Generic;

public static class PsMetagameData
{
	public static List<PsUnlockableCategory> m_units;

	public static List<PsUnlockableCategory> m_playerUnits;

	public static List<PsUnlockableCategory> m_gameModes;

	public static List<PsUnlockableCategory> m_gameAreas;

	public static List<PsUnlockableCategory> m_gameEnvironments;

	public static List<PsUnlockableCategory> m_gameMaterials;

	public static List<PsUnlockableCategory> m_menus;

	public static List<PsDialogue> m_dialogues;

	public static List<PsUnlock> m_unlocks;

	public static List<PsLevelTemplate> m_levelTemplates;

	public static List<string> m_unlockedTutorials;

	public static void Initialize()
	{
		m_units = new List<PsUnlockableCategory>();
		m_playerUnits = new List<PsUnlockableCategory>();
		m_gameModes = new List<PsUnlockableCategory>();
		m_gameAreas = new List<PsUnlockableCategory>();
		m_gameEnvironments = new List<PsUnlockableCategory>();
		m_gameMaterials = new List<PsUnlockableCategory>();
		m_menus = new List<PsUnlockableCategory>();
		m_dialogues = new List<PsDialogue>();
		m_unlocks = new List<PsUnlock>();
		m_levelTemplates = new List<PsLevelTemplate>();
		m_unlockedTutorials = new List<string>();
	}

	public static PsLevelTemplate AddLevelTemplate(PsMetagameDataHelper _helper)
	{
		PsLevelTemplate levelTemplateByIdentifier = GetLevelTemplateByIdentifier(_helper.m_metagameNodeData.m_dataIdentifier);
		if (levelTemplateByIdentifier != null)
		{
			return levelTemplateByIdentifier;
		}
		levelTemplateByIdentifier = new PsLevelTemplate();
		m_levelTemplates.Add(levelTemplateByIdentifier);
		levelTemplateByIdentifier.m_identifier = _helper.m_metagameNodeData.m_dataIdentifier;
		levelTemplateByIdentifier.m_gameArea = _helper.m_metagameNodeData.m_levelTemplateDomeSize;
		levelTemplateByIdentifier.m_gameMode = _helper.m_metagameNodeData.m_levelGameMode;
		levelTemplateByIdentifier.m_limitedItems = _helper.m_metagameNodeData.m_levelItems;
		levelTemplateByIdentifier.m_playerUnit = _helper.m_metagameNodeData.m_levelPlayerUnit;
		levelTemplateByIdentifier.m_templateMinigames = _helper.m_metagameNodeData.m_levelTemplateMinigames;
		for (int i = 0; i < _helper.m_inputs.Count; i++)
		{
			if (_helper.m_inputs[i].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Dialogue)
			{
				if (levelTemplateByIdentifier.m_dialogueIdentifiers == null)
				{
					levelTemplateByIdentifier.m_dialogueIdentifiers = new List<string>();
				}
				PsDialogue psDialogue = AddDialogue(_helper.m_inputs[i]);
				levelTemplateByIdentifier.m_dialogueIdentifiers.Add(psDialogue.m_identifier);
				Debug.LogWarning(levelTemplateByIdentifier.m_identifier + " has dialogue: " + psDialogue.m_identifier);
			}
		}
		return levelTemplateByIdentifier;
	}

	public static PsDialogue AddDialogue(PsMetagameDataHelper _helper)
	{
		PsDialogue dialogueByIdentifier = GetDialogueByIdentifier(_helper.m_metagameNodeData.m_dataIdentifier);
		if (dialogueByIdentifier != null)
		{
			return dialogueByIdentifier;
		}
		dialogueByIdentifier = new PsDialogue(_helper.m_metagameNodeData.m_dataIdentifier, _helper.m_metagameNodeData.m_dialogueTrigger);
		m_dialogues.Add(dialogueByIdentifier);
		PsMetagameDataHelper psMetagameDataHelper = _helper;
		while (psMetagameDataHelper != null)
		{
			PsDialogueStep psDialogueStep = new PsDialogueStep();
			dialogueByIdentifier.m_steps.Add(psDialogueStep);
			psDialogueStep.m_character = psMetagameDataHelper.m_metagameNodeData.m_dialogueCharacter;
			psDialogueStep.m_characterPosition = psMetagameDataHelper.m_metagameNodeData.m_dialogueCharacterPosition;
			psDialogueStep.m_charactertext = psMetagameDataHelper.m_metagameNodeData.m_dialogueText;
			psDialogueStep.m_proceedText = psMetagameDataHelper.m_metagameNodeData.m_dialogueProceed;
			psDialogueStep.m_cancelText = psMetagameDataHelper.m_metagameNodeData.m_dialogueCancel;
			PsMetagameDataHelper psMetagameDataHelper2 = null;
			for (int i = 0; i < psMetagameDataHelper.m_outputs.Count; i++)
			{
				if (psMetagameDataHelper.m_outputs[i].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Dialogue)
				{
					psMetagameDataHelper2 = psMetagameDataHelper.m_outputs[i];
					break;
				}
			}
			psMetagameDataHelper = psMetagameDataHelper2;
		}
		return dialogueByIdentifier;
	}

	public static void AddUnlocks(PsMetagameDataHelper _helper)
	{
		PsUnlock psUnlock = new PsUnlock(_helper.m_metagameNodeData.m_dataIdentifier);
		m_unlocks.Add(psUnlock);
		for (int i = 0; i < _helper.m_inputs.Count; i++)
		{
			if (_helper.m_inputs[i].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlockable || _helper.m_inputs[i].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableSimple || _helper.m_inputs[i].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableUpgradeable)
			{
				string dataIdentifier = _helper.m_inputs[i].m_metagameNodeData.m_dataIdentifier;
				PsUnlockable unlockableByIdentifier = GetUnlockableByIdentifier(dataIdentifier);
				if (unlockableByIdentifier != null)
				{
					psUnlock.m_unlockables.Add(unlockableByIdentifier);
				}
				else
				{
					Debug.LogWarning("couldn't find requested unlockable item: " + dataIdentifier);
				}
			}
		}
		PsMetagameDataHelper psMetagameDataHelper = null;
		for (int j = 0; j < _helper.m_outputs.Count; j++)
		{
			if (_helper.m_outputs[j].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlock)
			{
				psMetagameDataHelper = _helper.m_outputs[j];
			}
			else
			{
				if (_helper.m_outputs[j].m_metagameNodeData.m_nodeDataType != MetagameNodeDataType.Level && _helper.m_outputs[j].m_metagameNodeData.m_nodeDataType != MetagameNodeDataType.EditorPuzzle && _helper.m_outputs[j].m_metagameNodeData.m_nodeDataType != MetagameNodeDataType.Signal)
				{
					continue;
				}
				PsMetagameDataHelper psMetagameDataHelper2 = _helper.m_outputs[j];
				while (psMetagameDataHelper2 != null)
				{
					psUnlock.m_levels.Add(psMetagameDataHelper2.m_metagameNodeData);
					for (int k = 0; k < psMetagameDataHelper2.m_inputs.Count; k++)
					{
						if (psMetagameDataHelper2.m_inputs[k].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Dialogue)
						{
							PsDialogue item = AddDialogue(psMetagameDataHelper2.m_inputs[k]);
							psMetagameDataHelper2.m_metagameNodeData.m_dialogues.Add(item);
						}
					}
					PsMetagameDataHelper psMetagameDataHelper3 = null;
					for (int l = 0; l < psMetagameDataHelper2.m_outputs.Count; l++)
					{
						if (psMetagameDataHelper2.m_outputs[l].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Level || psMetagameDataHelper2.m_outputs[l].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.EditorPuzzle || psMetagameDataHelper2.m_outputs[l].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.VersusRace || psMetagameDataHelper2.m_outputs[l].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.LevelTemplate || psMetagameDataHelper2.m_outputs[l].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Block || psMetagameDataHelper2.m_outputs[l].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Signal)
						{
							psMetagameDataHelper3 = psMetagameDataHelper2.m_outputs[l];
						}
						else if (psMetagameDataHelper2.m_outputs[l].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.SidePath)
						{
							psMetagameDataHelper2.m_metagameNodeData.m_sidePathLevels = new List<MetagameNodeData>();
							PsMetagameDataHelper psMetagameDataHelper4 = psMetagameDataHelper2.m_outputs[l];
							if (psMetagameDataHelper4.m_outputs.Count > 0)
							{
								psMetagameDataHelper4 = psMetagameDataHelper4.m_outputs[0];
							}
							while (psMetagameDataHelper4 != null)
							{
								psMetagameDataHelper2.m_metagameNodeData.m_sidePathLevels.Add(psMetagameDataHelper4.m_metagameNodeData);
								for (int m = 0; m < psMetagameDataHelper4.m_inputs.Count; m++)
								{
									if (psMetagameDataHelper4.m_inputs[m].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Dialogue)
									{
										PsDialogue psDialogue = AddDialogue(psMetagameDataHelper4.m_inputs[m]);
										Debug.Log(psDialogue.m_identifier);
										psMetagameDataHelper4.m_metagameNodeData.m_dialogues.Add(psDialogue);
									}
								}
								psMetagameDataHelper4 = ((psMetagameDataHelper4.m_outputs.Count <= 0) ? null : psMetagameDataHelper4.m_outputs[0]);
							}
						}
						else if (psMetagameDataHelper2.m_outputs[l].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlock)
						{
							psMetagameDataHelper2.m_metagameNodeData.m_blockWillUnlock = psMetagameDataHelper2.m_outputs[l].m_metagameNodeData.m_dataIdentifier;
						}
					}
					psMetagameDataHelper2 = psMetagameDataHelper3;
				}
			}
		}
		if (psMetagameDataHelper != null)
		{
			AddUnlocks(psMetagameDataHelper);
		}
	}

	public static void Unlock(string _unlockIdentifier)
	{
		Debug.Log("m_unlocks COUNT: " + m_unlocks.Count);
		for (int i = 0; i < m_unlocks.Count; i++)
		{
			if (m_unlocks[i].m_name == _unlockIdentifier)
			{
				m_unlocks[i].m_unlocked = true;
				Debug.LogWarning("UNLOCK: " + _unlockIdentifier);
				List<PsUnlockable> unlockables = m_unlocks[i].m_unlockables;
				for (int j = 0; j < unlockables.Count; j++)
				{
					Debug.LogWarning("UNLOCKABLE: " + unlockables[j].m_name);
					unlockables[j].m_unlocked = true;
				}
				break;
			}
		}
	}

	public static int UnlocksOnLevel(string _unlockIdentifier)
	{
		int num = 0;
		for (int i = 0; i < m_unlocks.Count; i++)
		{
			for (int j = 0; j < m_unlocks[i].m_unlockables.Count; j++)
			{
				if (m_unlocks[i].m_unlockables[j].m_identifier == _unlockIdentifier)
				{
					return num + 1;
				}
			}
			for (int k = 0; k < m_unlocks[i].m_levels.Count; k++)
			{
				if (m_unlocks[i].m_levels[k].m_nodeDataType == MetagameNodeDataType.Level)
				{
					num++;
				}
			}
		}
		return -1;
	}

	public static void LockAll()
	{
		for (int i = 0; i < m_unlocks.Count; i++)
		{
			m_unlocks[i].m_unlocked = false;
			List<PsUnlockable> unlockables = m_unlocks[i].m_unlockables;
			for (int j = 0; j < unlockables.Count; j++)
			{
				unlockables[j].m_unlocked = false;
			}
		}
	}

	public static void TemporarilyUnlockUnlockable(string _unlockIdentifier)
	{
		PsUnlockable unlockableByIdentifier = GetUnlockableByIdentifier(_unlockIdentifier);
		if (unlockableByIdentifier != null)
		{
			if (PsState.m_activeGameLoop != null)
			{
				PsState.m_activeGameLoop.m_temporaryUnlock = _unlockIdentifier;
			}
			unlockableByIdentifier.m_unlocked = true;
		}
	}

	public static void Unlock(PsUnlock _unlock)
	{
		_unlock.m_unlocked = true;
		List<PsUnlockable> unlockables = _unlock.m_unlockables;
		for (int i = 0; i < unlockables.Count; i++)
		{
			unlockables[i].m_unlocked = true;
		}
	}

	public static PsUnlock GetNextUnlock(bool _unlock = true)
	{
		for (int i = 0; i < m_unlocks.Count; i++)
		{
			PsUnlock psUnlock = m_unlocks[i];
			if (!psUnlock.m_unlocked)
			{
				if (_unlock)
				{
					Unlock(psUnlock);
				}
				return psUnlock;
			}
		}
		if (m_unlocks.Count > 0)
		{
			return m_unlocks[m_unlocks.Count - 1];
		}
		PsUnlock psUnlock2 = new PsUnlock("error");
		MetagameNodeData metagameNodeData = new MetagameNodeData(string.Empty);
		metagameNodeData.m_nodeDataType = MetagameNodeDataType.Level;
		psUnlock2.m_levels.Add(metagameNodeData);
		MetagameNodeData metagameNodeData2 = new MetagameNodeData(string.Empty);
		metagameNodeData2.m_nodeDataType = MetagameNodeDataType.Block;
		psUnlock2.m_levels.Add(metagameNodeData2);
		return psUnlock2;
	}

	public static PsUnlock GetNextUnlockFromIdentifier(string _unlockIdentifier)
	{
		for (int i = 0; i < m_unlocks.Count; i++)
		{
			PsUnlock psUnlock = m_unlocks[i];
			if (psUnlock.m_name == _unlockIdentifier && i + 1 < m_unlocks.Count)
			{
				return m_unlocks[i + 1];
			}
		}
		if (m_unlocks.Count > 0)
		{
			return m_unlocks[m_unlocks.Count - 1];
		}
		PsUnlock psUnlock2 = new PsUnlock("error");
		MetagameNodeData metagameNodeData = new MetagameNodeData(string.Empty);
		metagameNodeData.m_nodeDataType = MetagameNodeDataType.Level;
		psUnlock2.m_levels.Add(metagameNodeData);
		MetagameNodeData metagameNodeData2 = new MetagameNodeData(string.Empty);
		metagameNodeData2.m_nodeDataType = MetagameNodeDataType.Block;
		psUnlock2.m_levels.Add(metagameNodeData2);
		return psUnlock2;
	}

	public static void AddCategory(PsMetagameDataHelper _helper, PsUnlockableCategory _container = null)
	{
		if (_helper.m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableCategory)
		{
			PsUnlockableCategory psUnlockableCategory = new PsUnlockableCategory(_helper.m_metagameNodeData.m_unlockableName);
			psUnlockableCategory.m_description = _helper.m_metagameNodeData.m_unlockableDescription;
			psUnlockableCategory.m_type = _helper.m_metagameNodeData.m_unlockableType;
			psUnlockableCategory.m_name = _helper.m_metagameNodeData.m_unlockableName;
			if (_container != null)
			{
				psUnlockableCategory.m_container = _container;
				_container.m_items.Add(psUnlockableCategory);
			}
			if (_helper.m_metagameNodeData.m_unlockableType == PsUnlockableType.Unit)
			{
				m_units.Add(psUnlockableCategory);
			}
			else if (_helper.m_metagameNodeData.m_unlockableType == PsUnlockableType.PlayerUnit)
			{
				m_playerUnits.Add(psUnlockableCategory);
			}
			else if (_helper.m_metagameNodeData.m_unlockableType == PsUnlockableType.GameArea)
			{
				m_gameAreas.Add(psUnlockableCategory);
			}
			else if (_helper.m_metagameNodeData.m_unlockableType == PsUnlockableType.GameEnvironment)
			{
				m_gameEnvironments.Add(psUnlockableCategory);
			}
			else if (_helper.m_metagameNodeData.m_unlockableType == PsUnlockableType.GameMode)
			{
				m_gameModes.Add(psUnlockableCategory);
			}
			else if (_helper.m_metagameNodeData.m_unlockableType == PsUnlockableType.GameMaterial)
			{
				m_gameMaterials.Add(psUnlockableCategory);
			}
			else if (_helper.m_metagameNodeData.m_unlockableType == PsUnlockableType.Menu)
			{
				m_menus.Add(psUnlockableCategory);
			}
			for (int i = 0; i < _helper.m_outputs.Count; i++)
			{
				AddCategory(_helper.m_outputs[i], psUnlockableCategory);
			}
		}
		else if (_helper.m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlockable)
		{
			PsEditorItem psEditorItem = new PsEditorItem(_helper.m_metagameNodeData.m_unlockableName);
			psEditorItem.m_description = _helper.m_metagameNodeData.m_unlockableDescription;
			psEditorItem.m_iconImage = _helper.m_metagameNodeData.m_unlockableIcon;
			psEditorItem.m_identifier = _helper.m_metagameNodeData.m_dataIdentifier;
			psEditorItem.m_graphNodeClassName = _helper.m_metagameNodeData.m_unlockableGraphNodeClass;
			psEditorItem.m_maxAmount = _helper.m_metagameNodeData.m_unlockableMaxCount;
			psEditorItem.m_complexity = _helper.m_metagameNodeData.m_unlockableComplexity;
			psEditorItem.m_itemLevel = _helper.m_metagameNodeData.m_unlockableItemLevel;
			AddLevelTemplates(_helper, psEditorItem);
			psEditorItem.m_container = _container;
			_container.m_items.Add(psEditorItem);
		}
		else if (_helper.m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableSimple)
		{
			PsEditorItem psEditorItem2 = new PsEditorItem(_helper.m_metagameNodeData.m_unlockableName);
			psEditorItem2.m_description = _helper.m_metagameNodeData.m_unlockableDescription;
			psEditorItem2.m_iconImage = _helper.m_metagameNodeData.m_unlockableIcon;
			psEditorItem2.m_identifier = _helper.m_metagameNodeData.m_dataIdentifier;
			psEditorItem2.m_itemLevel = _helper.m_metagameNodeData.m_unlockableItemLevel;
			AddLevelTemplates(_helper, psEditorItem2);
			psEditorItem2.m_container = _container;
			_container.m_items.Add(psEditorItem2);
		}
		else if (_helper.m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableUpgradeable)
		{
			PsUpgradeableEditorItem psUpgradeableEditorItem = new PsUpgradeableEditorItem(_helper.m_metagameNodeData.m_unlockableName);
			psUpgradeableEditorItem.m_description = _helper.m_metagameNodeData.m_unlockableDescription;
			psUpgradeableEditorItem.m_iconImage = _helper.m_metagameNodeData.m_unlockableIcon;
			psUpgradeableEditorItem.m_identifier = _helper.m_metagameNodeData.m_dataIdentifier;
			psUpgradeableEditorItem.m_graphNodeClassName = _helper.m_metagameNodeData.m_unlockableGraphNodeClass;
			psUpgradeableEditorItem.m_maxAmount = _helper.m_metagameNodeData.m_unlockableMaxCount;
			psUpgradeableEditorItem.m_complexity = _helper.m_metagameNodeData.m_unlockableComplexity;
			psUpgradeableEditorItem.m_itemLevel = _helper.m_metagameNodeData.m_unlockableItemLevel;
			psUpgradeableEditorItem.m_upgradeValues = _helper.m_metagameNodeData.m_unlockableUpgradeValues;
			psUpgradeableEditorItem.m_upgradePrices = _helper.m_metagameNodeData.m_unlockableUpgradePrices;
			psUpgradeableEditorItem.m_upgradeSteps = _helper.m_metagameNodeData.m_unlockableUpgradeSteps;
			psUpgradeableEditorItem.m_rentName = _helper.m_metagameNodeData.m_unlockableRentName;
			psUpgradeableEditorItem.m_rentButton = _helper.m_metagameNodeData.m_unlockableRentButton;
			AddLevelTemplates(_helper, psUpgradeableEditorItem);
			psUpgradeableEditorItem.m_container = _container;
			_container.m_items.Add(psUpgradeableEditorItem);
		}
	}

	public static void AddLevelTemplates(PsMetagameDataHelper _helper, PsUnlockable _item)
	{
		for (int i = 0; i < _helper.m_inputs.Count; i++)
		{
			if (_helper.m_inputs[i].m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.LevelTemplate)
			{
				_item.m_tutorialIdentifier = _helper.m_inputs[i].m_metagameNodeData.m_dataIdentifier;
				Debug.LogWarning(_item.m_identifier + " has tutorial: " + _item.m_tutorialIdentifier);
			}
		}
	}

	public static PsUnlock GetUnlockByIdentifier(string _identifier)
	{
		for (int i = 0; i < m_unlocks.Count; i++)
		{
			if (m_unlocks[i].m_name == _identifier)
			{
				return m_unlocks[i];
			}
		}
		return null;
	}

	public static PsDialogue GetDialogueByIdentifier(string _identifier)
	{
		foreach (PsDialogue dialogue in m_dialogues)
		{
			if (_identifier.Equals(dialogue.m_identifier))
			{
				return dialogue;
			}
		}
		return null;
	}

	public static PsLevelTemplate GetLevelTemplateByIdentifier(string _identifier)
	{
		foreach (PsLevelTemplate levelTemplate in m_levelTemplates)
		{
			if (_identifier.Equals(levelTemplate.m_identifier))
			{
				return levelTemplate;
			}
		}
		return null;
	}

	public static PsEditorItem GetUnlockableByIdentifier(string _identifier)
	{
		foreach (PsUnlockableCategory playerUnit in m_playerUnits)
		{
			foreach (PsUpgradeableEditorItem item in playerUnit.m_items)
			{
				if (_identifier.Equals(item.m_identifier))
				{
					return item;
				}
			}
		}
		foreach (PsUnlockableCategory unit in m_units)
		{
			foreach (PsEditorItem item2 in unit.m_items)
			{
				if (_identifier.Equals(item2.m_identifier))
				{
					return item2;
				}
			}
		}
		foreach (PsUnlockableCategory gameArea in m_gameAreas)
		{
			foreach (PsEditorItem item3 in gameArea.m_items)
			{
				if (_identifier.Equals(item3.m_identifier))
				{
					return item3;
				}
			}
		}
		foreach (PsUnlockableCategory gameEnvironment in m_gameEnvironments)
		{
			foreach (PsEditorItem item4 in gameEnvironment.m_items)
			{
				if (_identifier.Equals(item4.m_identifier))
				{
					return item4;
				}
			}
		}
		foreach (PsUnlockableCategory gameMode in m_gameModes)
		{
			foreach (PsEditorItem item5 in gameMode.m_items)
			{
				if (_identifier.Equals(item5.m_identifier))
				{
					return item5;
				}
			}
		}
		foreach (PsUnlockableCategory gameMaterial in m_gameMaterials)
		{
			foreach (PsEditorItem item6 in gameMaterial.m_items)
			{
				if (_identifier.Equals(item6.m_identifier))
				{
					return item6;
				}
			}
		}
		foreach (PsUnlockableCategory menu in m_menus)
		{
			foreach (PsEditorItem item7 in menu.m_items)
			{
				if (_identifier.Equals(item7.m_identifier))
				{
					return item7;
				}
			}
		}
		return null;
	}

	public static bool CanPlaceUnit(string _unitClassName)
	{
		if (Main.m_currentGame.m_currentScene.m_name != "EditorScene")
		{
			return true;
		}
		Minigame minigame = LevelManager.m_currentLevel as Minigame;
		if (minigame != null)
		{
			PsEditorItem unlockableByIdentifier = GetUnlockableByIdentifier(_unitClassName);
			if (unlockableByIdentifier == null)
			{
				return true;
			}
			Debug.Log("Current complexity: " + minigame.m_complexity + "/" + minigame.m_maxComplexity);
			if (minigame.m_complexity + unlockableByIdentifier.m_complexity > minigame.m_maxComplexity)
			{
				PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(UIEditorMaxComplexityPopup), null, null, null, false);
				return false;
			}
			if (!minigame.m_itemCount.Contains(_unitClassName))
			{
				return true;
			}
			if (unlockableByIdentifier != null)
			{
				if (unlockableByIdentifier.m_maxAmount > (int)minigame.m_itemCount[_unitClassName])
				{
					return true;
				}
				string text = PsStrings.Get(StringID.EDITOR_PROMPT_SINGLE_ITEM_LIMIT_REACHED);
				text = text.Replace("%1", unlockableByIdentifier.m_name);
				text = text.Replace("%2", unlockableByIdentifier.m_maxAmount.ToString());
				PsUIBasePopup psUIBasePopup2 = new PsUIBasePopup(typeof(UIEditorMaxComplexityPopup), null, null, null, false);
				(psUIBasePopup2.m_mainContent as UIEditorMaxComplexityPopup).m_box.SetText(text);
				psUIBasePopup2.Update();
				return false;
			}
		}
		return false;
	}
}
