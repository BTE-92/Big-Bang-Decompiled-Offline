using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Server;
using UnityEngine;

[Serializable]
public class GeNode : GraphNode
{
	public MetagameNodeData m_metagameNodeData;

	public int m_parsingHelperNodeId;

	public GeNode(string _name, Vector3 _pos)
		: base(GraphNodeType.Normal, _name)
	{
		m_disabledAtStart = false;
		m_position = _pos;
		m_rotation = Vector3.zero;
		m_scale = Vector3.one;
		m_radius = 25f;
		m_isRect = false;
		m_parsingHelperNodeId = -1;
		m_metagameNodeData = new MetagameNodeData(string.Empty);
	}

	public GeNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_metagameNodeData = (MetagameNodeData)info.GetValue("nodeData", typeof(MetagameNodeData));
	}

	public override void Assemble()
	{
		base.Assemble();
		TextMeshC textMeshC = TextMeshS.AddComponent(m_TC, Vector3.zero, "HurmeSemiBold_Font", 2f, 2f, 100f, Align.Center, Align.Middle, CameraS.m_mainCamera, m_name);
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlockable || m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableUpgradeable)
		{
			m_name = PsStrings.Get(m_metagameNodeData.m_unlockableName);
		}
		else if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableCategory)
		{
			m_name = m_metagameNodeData.m_unlockableName;
		}
		else if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlock)
		{
			m_name = m_metagameNodeData.m_dataIdentifier;
		}
		TextMeshS.SetText(textMeshC, m_metagameNodeData.m_nodeDataType.ToString() + ((!(m_name != string.Empty)) ? string.Empty : ("\n" + m_name)));
		textMeshC.m_textMesh.characterSize = 1f;
		m_TAC = TouchAreaS.AddCircleArea(m_TC, "Select", m_radius, CameraS.m_mainCamera, m_TC);
		TouchAreaS.AddTouchEventListener(m_TAC, SelectionTouchHandler);
		Draw();
	}

	public override void Update()
	{
		if (!m_disabled)
		{
			base.Update();
			if (m_selected)
			{
				Draw();
			}
		}
	}

	public override void Select(bool _select)
	{
		base.Select(_select);
		if (!_select)
		{
			Draw();
		}
	}

	private void Draw()
	{
		DebugDraw.Clear(CameraS.m_mainCamera, m_TC);
		if (m_selected)
		{
			DebugDraw.defaultColor = Color.yellow;
		}
		DebugDraw.CreateCircle(CameraS.m_mainCamera, m_TC, Vector3.zero, 25f, false, 8);
		DebugDraw.defaultColor = Color.white;
	}

	protected void SelectionTouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchCount != 1)
		{
			return;
		}
		if (_touchPhase == TouchAreaPhase.ReleaseIn && !_touchIsSecondary)
		{
			List<GraphElement> selection = GizmoManager.GetSelection();
			if (m_selected)
			{
				GizmoManager.RemoveFromSelection(this);
			}
			else
			{
				GizmoManager.AddToSelection(this);
			}
			new SelectUndoAction(selection, GizmoManager.GetSelection());
			GizmoManager.Update();
		}
		List<GraphElement> selection2 = GizmoManager.GetSelection();
		if (selection2.Count == 1)
		{
			if (selection2[0].m_elementType == GraphElementType.Node)
			{
				GeNode node = selection2[0] as GeNode;
				GeBaseState.UpdateInfoUI(node);
			}
			else
			{
				GeBaseState.UpdateInfoUI(null);
			}
		}
		else if (selection2.Count == 0)
		{
			GeBaseState.UpdateInfoUI(null);
		}
	}

	public override void Copied()
	{
		GeBaseState.UpdateInfoUI(this);
	}

	public override void Dispose()
	{
		base.Dispose();
		GeBaseState.UpdateInfoUI(null);
	}

	public override void SetPropertyDefaults()
	{
		base.SetPropertyDefaults();
		m_isRemovable = true;
		m_isCopyable = true;
		m_isRotateable = false;
		m_isLinkable = true;
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			GeNode geNode = (GeNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(geNode, this);
			geNode.m_storedRotation = Vector3.zero;
			if (geNode.m_childElements.Count != 0)
			{
				for (int i = 0; i < geNode.m_childElements.Count; i++)
				{
					CopyAttributes(geNode.m_childElements[i], m_childElements[i]);
					geNode.m_childElements[i].m_storedRotation = Vector3.zero;
				}
			}
			return geNode;
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("nodeData", m_metagameNodeData);
	}

	public void ValueChangeDelegate(string _fieldName, object _newValue, object _oldValue)
	{
		if (_fieldName == "m_unlockableUpgradeSteps")
		{
			string[] array = new string[(int)_newValue];
			Array.Copy(m_metagameNodeData.m_unlockableUpgradePrices, array, ((int)_newValue <= (int)_oldValue) ? ((int)_newValue) : ((int)_oldValue));
			m_metagameNodeData.m_unlockableUpgradePrices = array;
		}
		else if (_fieldName == "m_levelMinigameMetaData")
		{
			if (_newValue == null)
			{
				m_metagameNodeData.m_levelMinigameId = string.Empty;
			}
			else
			{
				m_metagameNodeData.m_levelMinigameId = (_newValue as PsMinigameMetaData).id;
			}
		}
		m_name = string.Empty;
		Reset();
		GeBaseState.UpdateInfoUI(this);
	}

	public void UpdateInfoUI(UIVerticalList _parent)
	{
		UIPagedCanvas pagedCanvas = (UIPagedCanvas)_parent.m_parent.m_parent.m_parent;
		UIScrollableCanvas scrollableCanvas = (UIScrollableCanvas)_parent.m_parent;
		UIModel model = new UIModel(m_metagameNodeData, ValueChangeDelegate);
		UIText uIText = new UIText(_parent, false, string.Empty, m_name, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight, "#000000");
		new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Node Type", model, "m_nodeDataType", GetEnumArray<MetagameNodeDataType>());
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlock)
		{
			new UIStringPropertyButton(_parent, string.Empty, "Name", model, "m_unlockableName");
			new UIStringPropertyButton(_parent, string.Empty, "Description", model, "m_unlockableDescription");
			new UIStringPropertyButton(_parent, string.Empty, "Icon", model, "m_unlockableIcon");
			new UIStringPropertyButton(_parent, string.Empty, "Unique Identifier", model, "m_dataIdentifier");
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableCategory)
		{
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Category Type", model, "m_unlockableType", GetEnumArray<PsUnlockableType>());
			new UIStringPropertyButton(_parent, string.Empty, "Name", model, "m_unlockableName");
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Description", model, "m_unlockableDescription", GetUnitStrings());
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Icon", model, "m_unlockableIcon", GetItemIconNames());
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Unlockable)
		{
			new UIStringPropertyButton(_parent, string.Empty, "Name", model, "m_unlockableName");
			new UIStringPropertyButton(_parent, string.Empty, "Description", model, "m_unlockableDescription");
			new UIStringPropertyButton(_parent, string.Empty, "Icon", model, "m_unlockableIcon");
			new UIStringPropertyButton(_parent, string.Empty, "Graph Node Class Name", model, "m_unlockableGraphNodeClass");
			new UIStringPropertyButton(_parent, string.Empty, "Unique Identifier / Class name", model, "m_dataIdentifier");
			new UIIntPropertyButton(_parent, string.Empty, "Max Count", model, "m_unlockableMaxCount");
			new UIIntPropertyButton(_parent, string.Empty, "Complexity", model, "m_unlockableComplexity");
			new UIIntPropertyButton(_parent, string.Empty, "Item Level", model, "m_unlockableItemLevel");
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableSimple)
		{
			new UIStringPropertyButton(_parent, string.Empty, "Name", model, "m_unlockableName");
			new UIStringPropertyButton(_parent, string.Empty, "Description", model, "m_unlockableDescription");
			new UIStringPropertyButton(_parent, string.Empty, "Icon", model, "m_unlockableIcon");
			new UIStringPropertyButton(_parent, string.Empty, "Unique Identifier / Class name", model, "m_dataIdentifier");
			new UIIntPropertyButton(_parent, string.Empty, "Item Level", model, "m_unlockableItemLevel");
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.UnlockableUpgradeable)
		{
			new UIStringPropertyButton(_parent, string.Empty, "Name", model, "m_unlockableName");
			new UIStringPropertyButton(_parent, string.Empty, "Description", model, "m_unlockableDescription");
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Icon", model, "m_unlockableIcon", GetPlayerIconNames());
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Graph Node Type", model, "m_unlockableGraphNodeClass", GeScene.m_allGraphNodeSubclasses.ToArray());
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Unique Identifier / Class name", model, "m_dataIdentifier", GeScene.m_allUnitSubclasses.ToArray());
			new UIIntPropertyButton(_parent, string.Empty, "Max Count", model, "m_unlockableMaxCount");
			new UIIntPropertyButton(_parent, string.Empty, "Complexity", model, "m_unlockableComplexity");
			new UIIntPropertyButton(_parent, string.Empty, "Item Level", model, "m_unlockableItemLevel");
			new UIStringPropertyButton(_parent, string.Empty, "Rent Name", model, "m_unlockableRentName");
			new UIStringPropertyButton(_parent, string.Empty, "Rent Button", model, "m_unlockableRentButton");
			new UIIntPropertyButton(_parent, string.Empty, "Upgrade Steps", model, "m_unlockableUpgradeSteps", 2, 100);
			new UIStringArrayEditorPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Upgrade Prices", model, "m_unlockableUpgradePrices");
			if (m_metagameNodeData.m_dataIdentifier != null)
			{
				Vehicle vehicle = Activator.CreateInstance(Type.GetType(m_metagameNodeData.m_dataIdentifier), new string[0]) as Vehicle;
				List<string> nodeUpgradeNames = vehicle.GetNodeUpgradeNames();
				if (nodeUpgradeNames != null && nodeUpgradeNames.Count > 0)
				{
					int num = 5;
					string key = string.Empty;
					if (m_metagameNodeData.m_unlockableUpgradeValues.Count > nodeUpgradeNames.Count)
					{
						m_metagameNodeData.m_unlockableUpgradeValues.Clear();
					}
					if (m_metagameNodeData.m_unlockableUpgradeValues.Count > 0)
					{
						{
							IEnumerator enumerator = m_metagameNodeData.m_unlockableUpgradeValues.Keys.GetEnumerator();
							try
							{
								if (enumerator.MoveNext())
								{
									string text = (string)enumerator.Current;
									key = text;
									object[] array = (object[])m_metagameNodeData.m_unlockableUpgradeValues[text];
									num = array.Length;
								}
							}
							finally
							{
								IDisposable disposable = enumerator as IDisposable;
								if (disposable != null)
								{
									disposable.Dispose();
								}
							}
						}
					}
					for (int i = 0; i < nodeUpgradeNames.Count; i++)
					{
						if (!m_metagameNodeData.m_unlockableUpgradeValues.ContainsKey(nodeUpgradeNames[i]))
						{
							m_metagameNodeData.m_unlockableUpgradeValues.Add(nodeUpgradeNames[i], new object[num]);
							key = nodeUpgradeNames[i];
						}
					}
					new UIHashtableArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Upgrades", model, "m_unlockableUpgradeValues", key);
				}
				vehicle.Destroy();
			}
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Level)
		{
			UIListPropertyButton uIListPropertyButton = new UIListPropertyButton(_parent, string.Empty, "MinigameMetaData", model, "m_levelMinigameMetaData", typeof(GeMinigameSelectionPopup));
			if (m_metagameNodeData.m_levelMinigameMetaData != null)
			{
				uIListPropertyButton.m_value.SetText(m_metagameNodeData.m_levelMinigameMetaData.name);
			}
			else if (!string.IsNullOrEmpty(m_metagameNodeData.m_levelMinigameId))
			{
				uIListPropertyButton.m_value.SetText("loading...");
				LoadMinigameMetaData(m_metagameNodeData.m_levelMinigameId, uIListPropertyButton);
			}
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Difficulty", model, "m_levelDifficulty", GetEnumArray<PsGameDifficulty>());
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Player Unit", model, "m_levelPlayerUnit", GetVehicleClassNames());
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Subgenre", model, "m_levelSubgenre", GetEnumArray<PsSubgenre>());
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Game Mode", model, "m_levelGameMode", GetEnumArray<PsGameMode>());
			if (!string.IsNullOrEmpty(m_metagameNodeData.m_levelMinigameId))
			{
				GeBaseState.m_editButton = new PsUIGenericButton(_parent);
				GeBaseState.m_editButton.SetText("Edit");
				GeBaseState.m_editButton.SetGreenColors();
				GeBaseState.m_editButton.m_TAC.m_customObject = this;
				GeBaseState.m_testButton = new PsUIGenericButton(_parent);
				GeBaseState.m_testButton.SetText("Play");
				GeBaseState.m_testButton.SetGreenColors();
				GeBaseState.m_testButton.m_TAC.m_customObject = this;
			}
			new UIStringArrayEditorPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Medal Times", model, "m_levelMedalTimes");
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.EditorPuzzle)
		{
			UIListPropertyButton uIListPropertyButton2 = new UIListPropertyButton(_parent, string.Empty, "MinigameMetaData", model, "m_levelMinigameMetaData", typeof(GeMinigameSelectionPopup));
			if (m_metagameNodeData.m_levelMinigameMetaData != null)
			{
				uIListPropertyButton2.m_value.SetText(m_metagameNodeData.m_levelMinigameMetaData.name);
			}
			else if (!string.IsNullOrEmpty(m_metagameNodeData.m_levelMinigameId))
			{
				uIListPropertyButton2.m_value.SetText("loading...");
				LoadMinigameMetaData(m_metagameNodeData.m_levelMinigameId, uIListPropertyButton2);
			}
			new UIStringListSelectorPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Limited to Items", model, "m_levelItems", GetGrounds().ToArray(), GetGrounds(true).ToArray());
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Signal || m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Block)
		{
			UIListPropertyButton uIListPropertyButton3 = new UIListPropertyButton(_parent, string.Empty, "MinigameMetaData", model, "m_levelMinigameMetaData", typeof(GeMinigameSelectionPopup));
			if (m_metagameNodeData.m_levelMinigameMetaData != null)
			{
				uIListPropertyButton3.m_value.SetText(m_metagameNodeData.m_levelMinigameMetaData.name);
			}
			else if (!string.IsNullOrEmpty(m_metagameNodeData.m_levelMinigameId))
			{
				uIListPropertyButton3.m_value.SetText("loading...");
				LoadMinigameMetaData(m_metagameNodeData.m_levelMinigameId, uIListPropertyButton3);
			}
			if (!string.IsNullOrEmpty(m_metagameNodeData.m_levelMinigameId))
			{
				GeBaseState.m_editButton = new PsUIGenericButton(_parent);
				GeBaseState.m_editButton.SetText("Edit");
				GeBaseState.m_editButton.SetGreenColors();
				GeBaseState.m_editButton.m_TAC.m_customObject = this;
				GeBaseState.m_testButton = new PsUIGenericButton(_parent);
				GeBaseState.m_testButton.SetText("Play");
				GeBaseState.m_testButton.SetGreenColors();
				GeBaseState.m_testButton.m_TAC.m_customObject = this;
			}
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.VersusRace)
		{
			UIListPropertyButton uIListPropertyButton4 = new UIListPropertyButton(_parent, string.Empty, "MinigameMetaData", model, "m_levelMinigameMetaData", typeof(GeMinigameSelectionPopup));
			if (m_metagameNodeData.m_levelMinigameMetaData != null)
			{
				uIListPropertyButton4.m_value.SetText(m_metagameNodeData.m_levelMinigameMetaData.name);
			}
			else if (!string.IsNullOrEmpty(m_metagameNodeData.m_levelMinigameId))
			{
				uIListPropertyButton4.m_value.SetText("loading...");
				LoadMinigameMetaData(m_metagameNodeData.m_levelMinigameId, uIListPropertyButton4);
			}
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Block)
		{
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Visual Type", model, "m_blockType", GetEnumArray<PsPathBlockType>());
			new UIIntPropertyButton(_parent, string.Empty, "Required stars", model, "m_blockRequiredStars");
			new UIIntPropertyButton(_parent, string.Empty, "Coin Reward", model, "m_blockCoins");
			new UIIntPropertyButton(_parent, string.Empty, "Diamond Reward", model, "m_blockDiamonds");
			new UIIntPropertyButton(_parent, string.Empty, "Bolt Reward", model, "m_blockBolts");
			new UIIntPropertyButton(_parent, string.Empty, "Key Reward", model, "m_blockKeys");
			new UIStringPropertyButton(_parent, string.Empty, "Reward Cue", model, "m_dataIdentifier");
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.Dialogue)
		{
			new UIStringPropertyButton(_parent, string.Empty, "Identifier", model, "m_dataIdentifier");
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Trigger at", model, "m_dialogueTrigger", GetEnumArray<PsNodeEventTrigger>());
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Character", model, "m_dialogueCharacter", GetEnumArray<PsDialogueCharacter>());
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Position", model, "m_dialogueCharacterPosition", GetEnumArray<PsDialogueCharacterPosition>());
			new UIStringPropertyButton(_parent, string.Empty, "Text", model, "m_dialogueText");
			new UIStringPropertyButton(_parent, string.Empty, "Progress", model, "m_dialogueProceed");
			new UIStringPropertyButton(_parent, string.Empty, "Cancel", model, "m_dialogueCancel");
		}
		if (m_metagameNodeData.m_nodeDataType == MetagameNodeDataType.LevelTemplate)
		{
			new UIStringPropertyButton(_parent, string.Empty, "Identifier", model, "m_dataIdentifier");
			new UIListPropertyButton(_parent, string.Empty, "MinigameMetaData", model, "m_levelTemplateMinigames", typeof(GeTemplateSelectionPopup));
			if (m_metagameNodeData.m_levelTemplateMinigames.Count == 0)
			{
				new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Dome Size", model, "m_levelTemplateDomeSize", GetEnumArray<PsGameArea>());
			}
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Player Unit", model, "m_levelPlayerUnit", GetVehicleClassNames());
			new UIArrayPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Game Mode", model, "m_levelGameMode", GetEnumArray<PsGameMode>());
			new UIStringListSelectorPropertyButton(_parent, scrollableCanvas, pagedCanvas, string.Empty, "Limited to Items", model, "m_levelItems", GetGrounds().ToArray(), GetGrounds(true).ToArray());
		}
	}

	public List<string> GetUnitsNGrounds(bool _names = false)
	{
		List<string> list = new List<string>();
		GeNode[] nodes = GetNodes(MetagameNodeDataType.UnlockableCategory);
		for (int i = 0; i < nodes.Length; i++)
		{
			if (nodes[i].m_metagameNodeData.m_unlockableType != PsUnlockableType.GameMaterial && nodes[i].m_metagameNodeData.m_unlockableType != PsUnlockableType.Unit)
			{
				continue;
			}
			List<GraphConnection> outputs = nodes[i].m_outputs;
			for (int j = 0; j < outputs.Count; j++)
			{
				if (_names)
				{
					list.Add((outputs[j].m_end as GeNode).m_metagameNodeData.m_unlockableName);
				}
				else
				{
					list.Add((outputs[j].m_end as GeNode).m_metagameNodeData.m_dataIdentifier);
				}
			}
		}
		return list;
	}

	public List<string> GetGrounds(bool _names = false)
	{
		List<string> list = new List<string>();
		GeNode[] nodes = GetNodes(MetagameNodeDataType.UnlockableCategory);
		for (int i = 0; i < nodes.Length; i++)
		{
			if (nodes[i].m_metagameNodeData.m_unlockableType != PsUnlockableType.GameMaterial)
			{
				continue;
			}
			List<GraphConnection> outputs = nodes[i].m_outputs;
			for (int j = 0; j < outputs.Count; j++)
			{
				if (_names)
				{
					list.Add((outputs[j].m_end as GeNode).m_metagameNodeData.m_unlockableName);
				}
				else
				{
					list.Add((outputs[j].m_end as GeNode).m_metagameNodeData.m_dataIdentifier);
				}
			}
		}
		return list;
	}

	public object[] GetEnumArray<T>()
	{
		List<object> list = new List<object>();
		Array values = Enum.GetValues(typeof(T));
		for (int i = 0; i < values.Length; i++)
		{
			list.Add(values.GetValue(i));
		}
		return list.ToArray();
	}

	public object[] GetVehicleClassNames()
	{
		GeNode[] nodes = GetNodes(MetagameNodeDataType.UnlockableUpgradeable);
		string[] array = new string[nodes.Length + 1];
		array[0] = "Any";
		for (int i = 0; i < nodes.Length; i++)
		{
			array[i + 1] = nodes[i].m_metagameNodeData.m_dataIdentifier.ToString();
		}
		return array;
	}

	public GeNode[] GetNodes(MetagameNodeDataType _geNodeType)
	{
		List<GeNode> list = new List<GeNode>();
		for (int i = 0; i < m_parentElement.m_childElements.Count; i++)
		{
			GraphElement graphElement = m_parentElement.m_childElements[i];
			if (graphElement.m_elementType == GraphElementType.Node)
			{
				GeNode geNode = graphElement as GeNode;
				if (geNode.m_metagameNodeData.m_nodeDataType == _geNodeType)
				{
					list.Add(geNode);
				}
			}
		}
		return list.ToArray();
	}

	public object[] GetPlayerIconNames()
	{
		return PsState.m_uiSheet.m_atlas.GetFrameNames("item_player");
	}

	public object[] GetItemIconNames()
	{
		return PsState.m_uiSheet.m_atlas.GetFrameNames("item");
	}

	public object[] GetUnitStrings()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < StringTable.stringIDs.Length; i++)
		{
			if (StringTable.stringIDs[i].Contains("UNIT_"))
			{
				list.Add(StringTable.stringIDs[i]);
			}
		}
		return list.ToArray();
	}

	public object[] GetStrings()
	{
		return StringTable.stringIDs;
	}

	private void LoadMinigameMetaData(string _id, UIListPropertyButton _button)
	{
		HttpC httpC = MiniGame.Get(_id, LoadMetaDataSUCCESS, LoadMetaDataFAILED);
		httpC.objectData = _id;
		EntityManager.AddComponentToEntity(m_TC.p_entity, httpC);
	}

	private void LoadMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		m_metagameNodeData.m_levelMinigameMetaData = _minigameMetaData;
		List<GraphElement> selection = GizmoManager.GetSelection();
		if (selection.Count == 1 && selection[0] as GeNode == this)
		{
			GeBaseState.UpdateInfoUI(this);
		}
	}

	private void LoadMetaDataFAILED(HttpC _c)
	{
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, delegate
		{
			HttpC httpC = MiniGame.Get((string)_c.objectData, LoadMetaDataSUCCESS, LoadMetaDataFAILED);
			httpC.objectData = _c.objectData;
			EntityManager.AddComponentToEntity(m_TC.p_entity, httpC);
			return httpC;
		});
	}
}
