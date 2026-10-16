using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class MetagameNodeData : ISerializable
{
	public MetagameNodeDataType m_nodeDataType;

	public string m_dataIdentifier;

	public List<PsDialogue> m_dialogues;

	public List<MetagameNodeData> m_sidePathLevels;

	public byte[] m_levelMinigameBytes;

	public PsUnlockableType m_unlockableType;

	public string m_unlockableName;

	public string m_unlockableDescription;

	public string m_unlockableIcon;

	public string m_unlockableGraphNodeClass;

	public string m_unlockableRentName;

	public string m_unlockableRentButton;

	public int m_unlockableMaxCount;

	public int m_unlockableComplexity;

	public int m_unlockableOrderIndex;

	public int m_unlockableItemLevel;

	public Hashtable m_unlockableUpgradeValues;

	public int m_unlockableUpgradeSteps;

	public string[] m_unlockableUpgradePrices;

	public PsGameDifficulty m_levelDifficulty;

	public string m_levelPlayerUnit;

	public PsSubgenre m_levelSubgenre;

	public PsGameMode m_levelGameMode;

	public List<string> m_levelItems;

	public PsMinigameMetaData m_levelMinigameMetaData;

	public string m_levelMinigameId;

	public string[] m_levelMedalTimes;

	public int m_blockRequiredStars;

	public PsPathBlockType m_blockType;

	public int m_blockCoins;

	public int m_blockDiamonds;

	public int m_blockBolts;

	public int m_blockKeys;

	public string m_blockUnlocked;

	public string m_blockWillUnlock;

	public PsNodeEventTrigger m_dialogueTrigger;

	public string m_eventDialogueIdentifier;

	public PsDialogueCharacter m_dialogueCharacter;

	public PsDialogueCharacterPosition m_dialogueCharacterPosition;

	public string m_dialogueText;

	public string m_dialogueProceed;

	public string m_dialogueCancel;

	public string m_dialogueUnlockIdentifier;

	public PsGameArea m_levelTemplateDomeSize;

	public List<string> m_levelTemplateMinigames;

	public MetagameNodeData(string _uniqueName = "")
	{
		m_nodeDataType = MetagameNodeDataType.Undefined;
		m_dataIdentifier = string.Empty;
		m_dialogues = new List<PsDialogue>();
		m_sidePathLevels = null;
		m_unlockableType = PsUnlockableType.Undefined;
		m_unlockableName = _uniqueName;
		m_unlockableDescription = string.Empty;
		m_unlockableIcon = string.Empty;
		m_unlockableGraphNodeClass = string.Empty;
		m_unlockableRentName = string.Empty;
		m_unlockableRentButton = string.Empty;
		m_unlockableMaxCount = 0;
		m_unlockableComplexity = 0;
		m_unlockableItemLevel = 0;
		m_unlockableUpgradeValues = new Hashtable();
		m_unlockableUpgradeSteps = 10;
		m_unlockableUpgradePrices = new string[m_unlockableUpgradeSteps];
		m_levelMinigameMetaData = null;
		m_levelMinigameId = string.Empty;
		m_levelDifficulty = PsGameDifficulty.Any;
		m_levelPlayerUnit = "Any";
		m_levelSubgenre = PsSubgenre.Any;
		m_levelGameMode = PsGameMode.Any;
		m_levelItems = new List<string>();
		m_levelMedalTimes = new string[3];
		m_blockType = PsPathBlockType.Antenna;
		m_blockRequiredStars = 0;
		m_blockBolts = 0;
		m_blockCoins = 0;
		m_blockDiamonds = 0;
		m_blockKeys = 0;
		m_blockUnlocked = string.Empty;
		m_blockWillUnlock = string.Empty;
		m_dialogueTrigger = PsNodeEventTrigger.Manual;
		m_dialogueCharacter = PsDialogueCharacter.Scientist;
		m_dialogueCharacterPosition = PsDialogueCharacterPosition.Right;
		m_dialogueText = "What say you?";
		m_dialogueProceed = "Yes!";
		m_dialogueCancel = "No!";
		m_levelTemplateDomeSize = PsGameArea.Any;
		m_levelTemplateMinigames = new List<string>();
	}

	public MetagameNodeData(SerializationInfo info, StreamingContext ctxt)
	{
		try
		{
			m_nodeDataType = (MetagameNodeDataType)(int)Enum.Parse(typeof(MetagameNodeDataType), (string)info.GetValue("geNodeType", typeof(string)));
		}
		catch
		{
			m_nodeDataType = MetagameNodeDataType.Undefined;
		}
		m_dialogues = new List<PsDialogue>();
		if (m_nodeDataType == MetagameNodeDataType.Unlock)
		{
			m_dataIdentifier = (string)info.GetValue("itemClass", typeof(string));
			m_unlockableName = (string)info.GetValue("itemName", typeof(string));
			m_unlockableDescription = (string)info.GetValue("itemDescription", typeof(string));
			m_unlockableIcon = (string)info.GetValue("itemIcon", typeof(string));
		}
		if (m_nodeDataType == MetagameNodeDataType.UnlockableCategory)
		{
			m_unlockableType = (PsUnlockableType)(int)Enum.Parse(typeof(PsUnlockableType), (string)info.GetValue("itemType", typeof(string)));
			m_unlockableName = (string)info.GetValue("itemName", typeof(string));
			m_unlockableDescription = (string)info.GetValue("itemDescription", typeof(string));
			m_unlockableIcon = (string)info.GetValue("itemIcon", typeof(string));
		}
		if (m_nodeDataType == MetagameNodeDataType.Unlockable)
		{
			m_dataIdentifier = (string)info.GetValue("itemClass", typeof(string));
			m_unlockableName = (string)info.GetValue("itemName", typeof(string));
			m_unlockableDescription = (string)info.GetValue("itemDescription", typeof(string));
			m_unlockableIcon = (string)info.GetValue("itemIcon", typeof(string));
			m_unlockableGraphNodeClass = (string)info.GetValue("itemNodeType", typeof(string));
			m_unlockableMaxCount = (int)info.GetValue("itemMaxCount", typeof(int));
			m_unlockableComplexity = (int)info.GetValue("itemComplexity", typeof(int));
			try
			{
				m_unlockableItemLevel = (int)info.GetValue("itemLevel", typeof(int));
			}
			catch
			{
				m_unlockableItemLevel = 0;
			}
		}
		if (m_nodeDataType == MetagameNodeDataType.UnlockableSimple)
		{
			m_dataIdentifier = (string)info.GetValue("itemClass", typeof(string));
			m_unlockableName = (string)info.GetValue("itemName", typeof(string));
			m_unlockableDescription = (string)info.GetValue("itemDescription", typeof(string));
			m_unlockableIcon = (string)info.GetValue("itemIcon", typeof(string));
			try
			{
				m_unlockableItemLevel = (int)info.GetValue("itemLevel", typeof(int));
			}
			catch
			{
				m_unlockableItemLevel = 0;
			}
		}
		if (m_nodeDataType == MetagameNodeDataType.UnlockableUpgradeable)
		{
			m_dataIdentifier = (string)info.GetValue("itemClass", typeof(string));
			m_unlockableName = (string)info.GetValue("itemName", typeof(string));
			m_unlockableDescription = (string)info.GetValue("itemDescription", typeof(string));
			m_unlockableIcon = (string)info.GetValue("itemIcon", typeof(string));
			m_unlockableGraphNodeClass = (string)info.GetValue("itemNodeType", typeof(string));
			m_unlockableMaxCount = (int)info.GetValue("itemMaxCount", typeof(int));
			m_unlockableComplexity = (int)info.GetValue("itemComplexity", typeof(int));
			try
			{
				m_unlockableItemLevel = (int)info.GetValue("itemLevel", typeof(int));
			}
			catch
			{
				m_unlockableItemLevel = 0;
			}
			m_unlockableUpgradeValues = (Hashtable)info.GetValue("vehicleUpgradeValues", typeof(Hashtable));
			m_unlockableUpgradeSteps = (int)info.GetValue("vehicleUpgradeSteps", typeof(int));
			m_unlockableUpgradePrices = (string[])info.GetValue("vehicleUpgradePrices", typeof(string[]));
			try
			{
				m_unlockableRentName = (string)info.GetValue("rentName", typeof(string));
			}
			catch
			{
				m_unlockableRentName = "Rentzal";
			}
			try
			{
				m_unlockableRentButton = (string)info.GetValue("rentButton", typeof(string));
			}
			catch
			{
				m_unlockableRentButton = "Rentz this";
			}
		}
		if (m_nodeDataType == MetagameNodeDataType.Level)
		{
			try
			{
				m_levelMinigameId = (string)info.GetValue("levelMinigameId", typeof(string));
			}
			catch
			{
				m_levelMinigameId = string.Empty;
			}
			m_levelDifficulty = (PsGameDifficulty)(int)Enum.Parse(typeof(PsGameDifficulty), (string)info.GetValue("levelDifficulty", typeof(string)));
			m_levelPlayerUnit = (string)info.GetValue("levelPlayerUnit", typeof(string));
			m_levelSubgenre = (PsSubgenre)(int)Enum.Parse(typeof(PsSubgenre), (string)info.GetValue("levelSubgenre", typeof(string)));
			m_levelGameMode = (PsGameMode)(int)Enum.Parse(typeof(PsGameMode), (string)info.GetValue("levelGameMode", typeof(string)));
			m_levelItems = new List<string>((string[])info.GetValue("levelItems", typeof(string[])));
			try
			{
				m_levelMedalTimes = (string[])info.GetValue("levelMedalTimes", typeof(string[]));
			}
			catch
			{
				m_levelMedalTimes = new string[3];
			}
		}
		if (m_nodeDataType == MetagameNodeDataType.EditorPuzzle)
		{
			try
			{
				m_levelMinigameId = (string)info.GetValue("levelMinigameId", typeof(string));
			}
			catch
			{
				m_levelMinigameId = string.Empty;
			}
			m_levelItems = new List<string>((string[])info.GetValue("puzzleLimitedItems", typeof(string[])));
		}
		if (m_nodeDataType == MetagameNodeDataType.VersusRace)
		{
			try
			{
				m_levelMinigameId = (string)info.GetValue("levelMinigameId", typeof(string));
			}
			catch
			{
				m_levelMinigameId = string.Empty;
			}
		}
		if (m_nodeDataType == MetagameNodeDataType.Signal)
		{
			try
			{
				m_levelMinigameId = (string)info.GetValue("levelMinigameId", typeof(string));
			}
			catch
			{
				m_levelMinigameId = string.Empty;
			}
			m_blockUnlocked = info.GetString("blockUnlocked");
			try
			{
				m_blockWillUnlock = info.GetString("blockWillUnlock");
			}
			catch
			{
				m_blockWillUnlock = string.Empty;
			}
		}
		if (m_nodeDataType == MetagameNodeDataType.Block)
		{
			try
			{
				m_levelMinigameId = (string)info.GetValue("levelMinigameId", typeof(string));
			}
			catch
			{
				m_levelMinigameId = string.Empty;
			}
			m_blockRequiredStars = info.GetInt32("blockRequiredStars");
			m_blockType = (PsPathBlockType)(int)Enum.Parse(typeof(PsPathBlockType), info.GetString("blockType"));
			m_blockBolts = info.GetInt32("blockBolts");
			m_blockCoins = info.GetInt32("blockCoins");
			m_blockDiamonds = info.GetInt32("blockDiamonds");
			try
			{
				m_blockKeys = info.GetInt32("blockKeys");
			}
			catch
			{
				m_blockKeys = 0;
			}
			m_blockUnlocked = info.GetString("blockUnlocked");
			try
			{
				m_blockWillUnlock = info.GetString("blockWillUnlock");
			}
			catch
			{
				m_blockWillUnlock = string.Empty;
			}
			try
			{
				m_dataIdentifier = info.GetString("blockRewardCue");
			}
			catch
			{
				m_dataIdentifier = string.Empty;
			}
		}
		if (m_nodeDataType == MetagameNodeDataType.Dialogue)
		{
			m_dataIdentifier = (string)info.GetValue("dialogueIdentifier", typeof(string));
			try
			{
				m_dialogueTrigger = (PsNodeEventTrigger)(int)Enum.Parse(typeof(PsNodeEventTrigger), (string)info.GetValue("dialogueTrigger", typeof(string)));
			}
			catch
			{
				m_dialogueTrigger = PsNodeEventTrigger.MinigameLose;
			}
			m_dialogueCharacter = (PsDialogueCharacter)(int)Enum.Parse(typeof(PsDialogueCharacter), (string)info.GetValue("dialogueCharacter", typeof(string)));
			m_dialogueCharacterPosition = (PsDialogueCharacterPosition)(int)Enum.Parse(typeof(PsDialogueCharacterPosition), (string)info.GetValue("dialogueCharacterPosition", typeof(string)));
			m_dialogueText = (string)info.GetValue("dialogueText", typeof(string));
			m_dialogueProceed = (string)info.GetValue("dialogueProceed", typeof(string));
			m_dialogueCancel = (string)info.GetValue("dialogueCancel", typeof(string));
		}
		if (m_nodeDataType == MetagameNodeDataType.LevelTemplate)
		{
			m_dataIdentifier = (string)info.GetValue("levelTemplateIdentifier", typeof(string));
			m_levelPlayerUnit = (string)info.GetValue("levelPlayerUnit", typeof(string));
			m_levelGameMode = (PsGameMode)(int)Enum.Parse(typeof(PsGameMode), (string)info.GetValue("levelGameMode", typeof(string)));
			m_levelTemplateDomeSize = (PsGameArea)(int)Enum.Parse(typeof(PsGameArea), (string)info.GetValue("levelTemplateDomeSize", typeof(string)));
			m_levelItems = new List<string>((string[])info.GetValue("levelTemplateItems", typeof(string[])));
			m_levelTemplateMinigames = new List<string>((string[])info.GetValue("levelTemplateMinigames", typeof(string[])));
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		info.AddValue("geNodeType", m_nodeDataType.ToString());
		if (m_nodeDataType == MetagameNodeDataType.Unlock)
		{
			info.AddValue("itemClass", m_dataIdentifier);
			info.AddValue("itemName", m_unlockableName);
			info.AddValue("itemDescription", m_unlockableDescription);
			info.AddValue("itemIcon", m_unlockableIcon);
		}
		if (m_nodeDataType == MetagameNodeDataType.UnlockableCategory)
		{
			info.AddValue("itemType", m_unlockableType.ToString());
			info.AddValue("itemName", m_unlockableName);
			info.AddValue("itemDescription", m_unlockableDescription);
			info.AddValue("itemIcon", m_unlockableIcon);
		}
		if (m_nodeDataType == MetagameNodeDataType.Unlockable)
		{
			info.AddValue("itemClass", m_dataIdentifier);
			info.AddValue("itemName", m_unlockableName);
			info.AddValue("itemDescription", m_unlockableDescription);
			info.AddValue("itemIcon", m_unlockableIcon);
			info.AddValue("itemNodeType", m_unlockableGraphNodeClass);
			info.AddValue("itemMaxCount", m_unlockableMaxCount);
			info.AddValue("itemComplexity", m_unlockableComplexity);
			info.AddValue("itemLevel", m_unlockableItemLevel);
		}
		if (m_nodeDataType == MetagameNodeDataType.UnlockableSimple)
		{
			info.AddValue("itemClass", m_dataIdentifier);
			info.AddValue("itemName", m_unlockableName);
			info.AddValue("itemDescription", m_unlockableDescription);
			info.AddValue("itemIcon", m_unlockableIcon);
			info.AddValue("itemLevel", m_unlockableItemLevel);
		}
		if (m_nodeDataType == MetagameNodeDataType.UnlockableUpgradeable)
		{
			info.AddValue("itemClass", m_dataIdentifier);
			info.AddValue("itemName", m_unlockableName);
			info.AddValue("itemDescription", m_unlockableDescription);
			info.AddValue("itemIcon", m_unlockableIcon);
			info.AddValue("itemNodeType", m_unlockableGraphNodeClass);
			info.AddValue("itemMaxCount", m_unlockableMaxCount);
			info.AddValue("itemComplexity", m_unlockableComplexity);
			info.AddValue("itemLevel", m_unlockableItemLevel);
			info.AddValue("vehicleUpgradeValues", m_unlockableUpgradeValues);
			info.AddValue("vehicleUpgradeSteps", m_unlockableUpgradeSteps);
			info.AddValue("vehicleUpgradePrices", m_unlockableUpgradePrices);
			info.AddValue("rentName", m_unlockableRentName);
			info.AddValue("rentButton", m_unlockableRentButton);
		}
		if (m_nodeDataType == MetagameNodeDataType.Level)
		{
			if (m_levelMinigameMetaData != null)
			{
				info.AddValue("levelMinigameId", m_levelMinigameMetaData.id);
			}
			else
			{
				info.AddValue("levelMinigameId", m_levelMinigameId);
			}
			info.AddValue("levelDifficulty", m_levelDifficulty.ToString());
			info.AddValue("levelPlayerUnit", m_levelPlayerUnit);
			info.AddValue("levelSubgenre", m_levelSubgenre.ToString());
			info.AddValue("levelGameMode", m_levelGameMode.ToString());
			info.AddValue("levelItems", m_levelItems.ToArray());
			info.AddValue("levelMedalTimes", m_levelMedalTimes);
		}
		if (m_nodeDataType == MetagameNodeDataType.EditorPuzzle)
		{
			if (m_levelMinigameMetaData != null)
			{
				info.AddValue("levelMinigameId", m_levelMinigameMetaData.id);
			}
			else
			{
				info.AddValue("levelMinigameId", m_levelMinigameId);
			}
			info.AddValue("puzzleLimitedItems", m_levelItems.ToArray());
		}
		if (m_nodeDataType == MetagameNodeDataType.Signal)
		{
			if (m_levelMinigameMetaData != null)
			{
				info.AddValue("levelMinigameId", m_levelMinigameMetaData.id);
			}
			else
			{
				info.AddValue("levelMinigameId", m_levelMinigameId);
			}
			info.AddValue("blockUnlocked", m_blockUnlocked);
			info.AddValue("blockWillUnlock", m_blockWillUnlock);
		}
		if (m_nodeDataType == MetagameNodeDataType.VersusRace)
		{
			if (m_levelMinigameMetaData != null)
			{
				info.AddValue("levelMinigameId", m_levelMinigameMetaData.id);
			}
			else
			{
				info.AddValue("levelMinigameId", m_levelMinigameId);
			}
		}
		if (m_nodeDataType == MetagameNodeDataType.Block)
		{
			if (m_levelMinigameMetaData != null)
			{
				info.AddValue("levelMinigameId", m_levelMinigameMetaData.id);
			}
			else
			{
				info.AddValue("levelMinigameId", m_levelMinigameId);
			}
			info.AddValue("blockRequiredStars", m_blockRequiredStars);
			info.AddValue("blockType", m_blockType.ToString());
			info.AddValue("blockBolts", m_blockBolts);
			info.AddValue("blockCoins", m_blockCoins);
			info.AddValue("blockDiamonds", m_blockDiamonds);
			info.AddValue("blockKeys", m_blockKeys);
			info.AddValue("blockUnlocked", m_blockUnlocked);
			info.AddValue("blockWillUnlock", m_blockWillUnlock);
			info.AddValue("blockRewardCue", m_dataIdentifier);
		}
		if (m_nodeDataType == MetagameNodeDataType.Dialogue)
		{
			info.AddValue("dialogueIdentifier", m_dataIdentifier);
			info.AddValue("dialogueTrigger", m_dialogueTrigger.ToString());
			info.AddValue("dialogueCharacter", m_dialogueCharacter.ToString());
			info.AddValue("dialogueCharacterPosition", m_dialogueCharacterPosition.ToString());
			info.AddValue("dialogueText", m_dialogueText);
			info.AddValue("dialogueProceed", m_dialogueProceed);
			info.AddValue("dialogueCancel", m_dialogueCancel);
		}
		if (m_nodeDataType == MetagameNodeDataType.LevelTemplate)
		{
			info.AddValue("levelTemplateIdentifier", m_dataIdentifier);
			if (m_levelMinigameMetaData != null)
			{
				info.AddValue("levelMinigameId", m_levelMinigameMetaData.id);
			}
			else
			{
				info.AddValue("levelMinigameId", m_levelMinigameId);
			}
			info.AddValue("levelPlayerUnit", m_levelPlayerUnit);
			info.AddValue("levelGameMode", m_levelGameMode.ToString());
			info.AddValue("levelTemplateDomeSize", m_levelTemplateDomeSize.ToString());
			info.AddValue("levelTemplateItems", m_levelItems.ToArray());
			info.AddValue("levelTemplateMinigames", m_levelTemplateMinigames.ToArray());
		}
	}
}
