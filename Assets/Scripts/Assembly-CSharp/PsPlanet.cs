using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using UnityEngine;
using UnityEngine.Rendering;

public class PsPlanet
{
	public string m_identifier;

	public Entity m_spaceEntity;

	public TransformC m_alienPlanetTC;

	public TransformC m_cloudTC;

	public TransformC m_atmosphereTC;

	public TransformC m_floatingNodesTC;

	public TransformC m_rollTC;

	public TransformC m_backgroundTC;

	public TransformC m_planetCameraTC;

	public Camera m_planetCamera;

	public Camera m_blurCamera;

	public float m_currentAngle;

	public float m_minAngle;

	public float m_maxAngle;

	public int m_currentZeroIndex = -1;

	public int m_minRow;

	public int m_maxRow;

	public List<PlanetRow> m_nodeRows;

	public int m_nodeRowCount = 10;

	public float m_nodeRowAngleInterval = 10f;

	public float m_planetRadius = 101.1f;

	public float m_pathStartOffset = -80f;

	public float m_floaterOffset;

	public PsPlanetPath m_mainPath;

	public List<PsPlanetPath> m_sidePaths;

	public PsFloaterManager m_floaterManager;

	public List<PsPlanetFlyingNode> m_flyingNodes;

	public SpriteSheet m_planetSpriteSheet;

	public SpriteSheet m_planetSpriteSheetTutorial;

	private List<Transform> m_cloudSets;

	private List<Transform> m_cloudParticles;

	public Material m_blurMaterial;

	public bool m_forcedRoll;

	public float m_targetAngle;

	public int m_targetRow;

	private float m_rollStartTime;

	private float m_startAngle;

	private float m_rollDifference;

	private bool m_forcedRollActive;

	public List<PsUICheckpointBanner> m_banners;

	public PsPlanet()
	{
		PsPlanetManager.m_planets.Add(this);
		m_identifier = "Planet";
	}

	public int GetMainPathCurrentNodeId()
	{
		if (m_mainPath == null)
		{
			return -1;
		}
		return m_mainPath.m_currentNodeId;
	}

	public int GetLastNodeId()
	{
		if (m_mainPath != null)
		{
			return m_mainPath.m_nodeInfos.Count;
		}
		return 0;
	}

	public void CreateSpaceScene()
	{
		Debug.LogWarning("CREATING SPACE SCENE");
		InitializePathData();
		m_flyingNodes = new List<PsPlanetFlyingNode>();
		m_spaceEntity = EntityManager.AddEntity("PlanetUI");
		m_alienPlanetTC = TransformS.AddComponent(m_spaceEntity);
		m_rollTC = TransformS.AddComponent(m_spaceEntity, new Vector3(0f, 0f, 200f));
		m_backgroundTC = TransformS.AddComponent(m_spaceEntity);
		m_backgroundTC.transform.Rotate(Vector3.up * 180f);
		CameraS.m_mainCamera.enabled = false;
		m_planetCameraTC = TransformS.AddComponent(m_spaceEntity);
		m_planetCamera = CameraS.AddCamera("PlanetCamera", false);
		m_planetCamera.transform.parent = m_planetCameraTC.transform;
		m_planetCamera.transform.localPosition = Vector3.zero;
		m_planetCamera.transform.localRotation = Quaternion.identity;
		m_planetCamera.depth = -1f;
		m_planetCamera.clearFlags = CameraClearFlags.Color;
		m_planetCamera.fieldOfView = 26f;
		m_planetCamera.backgroundColor = Color.black;
		m_planetCamera.transform.localPosition = new Vector3(-20f, 25f, -237f);
		m_planetCamera.transform.eulerAngles = new Vector3(349f, 357f, 12f);
		m_planetCamera.cullingMask |= 0x200;
		m_blurCamera = CameraS.AddCamera("BlurCamera", true);
		CameraS.MoveToBehindOther(m_blurCamera, CameraS.m_mainCamera);
		m_blurCamera.transform.parent = m_planetCameraTC.transform;
		m_blurCamera.transform.localPosition = Vector3.zero;
		m_blurCamera.transform.localRotation = Quaternion.identity;
		m_blurCamera.depth = 0f;
		m_blurCamera.cullingMask = 4096;
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
		gameObject.name = "BlurCanvas";
		gameObject.layer = 12;
		gameObject.transform.parent = m_blurCamera.transform;
		Vector3 localScale = new Vector3(Screen.width, Screen.height, 0f);
		gameObject.transform.localScale = localScale;
		gameObject.transform.localPosition += Vector3.forward * 2f;
		gameObject.GetComponent<Collider>().enabled = false;
		Renderer component = gameObject.GetComponent<Renderer>();
		component.useLightProbes = false;
		component.receiveShadows = false;
		component.shadowCastingMode = ShadowCastingMode.Off;
		component.material = new Material(Shader.Find("WOE/Unlit/SimpleUnlit"));
		m_blurMaterial = component.material;
		m_blurCamera.enabled = false;
		m_planetSpriteSheet = SpriteS.AddSpriteSheet(m_planetCamera, ResourceManager.GetMaterial(RESOURCE.UIPlanetAtlasMat_Material), ResourceManager.GetTextAsset(RESOURCE.UiAtlas_TextAsset), 1f);
		m_planetSpriteSheetTutorial = SpriteS.AddSpriteSheet(m_planetCamera, ResourceManager.GetMaterial(RESOURCE.UIPlanetAtlasMat_Material), ResourceManager.GetTextAsset(RESOURCE.UiAtlas_TextAsset), 1f);
		PrefabC prefabC = PrefabS.AddComponent(m_alienPlanetTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.PlanetSliceDesertAPrefab_GameObject));
		prefabC.p_gameObject.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
		PrefabC prefabC2 = PrefabS.AddComponent(m_alienPlanetTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.PlanetSliceDesertAPrefab_GameObject));
		prefabC2.p_gameObject.transform.rotation = Quaternion.Euler(90f, 0f, -90f);
		PrefabC prefabC3 = PrefabS.AddComponent(m_alienPlanetTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.PlanetSliceDesertAPrefab_GameObject));
		prefabC3.p_gameObject.transform.rotation = Quaternion.Euler(180f, 0f, -90f);
		PrefabC prefabC4 = PrefabS.AddComponent(m_alienPlanetTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.PlanetSliceDesertAPrefab_GameObject));
		prefabC4.p_gameObject.transform.rotation = Quaternion.Euler(270f, 0f, -90f);
		PrefabC c = PrefabS.AddComponent(m_backgroundTC, ResourceManager.GetGameObject(RESOURCE.PlanetBackroundPrefab_GameObject), string.Empty);
		PrefabS.SetCamera(c, m_planetCamera);
		PrefabC c2 = PrefabS.AddComponent(m_backgroundTC, ResourceManager.GetGameObject(RESOURCE.PlanetStarskyPrefab_GameObject), string.Empty);
		PrefabS.SetCamera(c2, m_planetCamera);
		m_cloudTC = TransformS.AddComponent(m_spaceEntity, Vector3.zero);
		PrefabC prefabC5 = PrefabS.AddComponent(m_cloudTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.CloudsA_GameObject));
		prefabC5.p_gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		PrefabC prefabC6 = PrefabS.AddComponent(m_cloudTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.CloudsB_GameObject));
		prefabC6.p_gameObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		PrefabC prefabC7 = PrefabS.AddComponent(m_cloudTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.CloudsC_GameObject));
		prefabC7.p_gameObject.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
		PrefabC prefabC8 = PrefabS.AddComponent(m_cloudTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.CloudsB_GameObject));
		prefabC8.p_gameObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
		PrefabS.SetCamera(prefabC5, m_planetCamera);
		PrefabS.SetCamera(prefabC6, m_planetCamera);
		PrefabS.SetCamera(prefabC7, m_planetCamera);
		PrefabS.SetCamera(prefabC8, m_planetCamera);
		m_cloudSets = new List<Transform>();
		m_cloudParticles = new List<Transform>();
		ParseClouds(prefabC5.p_gameObject);
		ParseClouds(prefabC6.p_gameObject);
		ParseClouds(prefabC7.p_gameObject);
		ParseClouds(prefabC8.p_gameObject);
		m_atmosphereTC = TransformS.AddComponent(m_spaceEntity, Vector3.zero);
		PrefabC c3 = PrefabS.AddComponent(m_atmosphereTC, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.PlanetAtmosphere_GameObject));
		PrefabS.SetCamera(c3, m_planetCamera);
		m_floatingNodesTC = TransformS.AddComponent(m_spaceEntity, Vector3.zero);
		InitializePlanetNodes();
		m_floaterManager.LoadFloatingPath();
		PsPlanetManager.m_timedEvents = null;
		GetTimedEventsFromServer();
		TransformS.SetRotation(m_cloudTC, Vector3.right * m_currentAngle);
		TransformS.SetRotation(m_atmosphereTC, Vector3.right * m_currentAngle);
		TransformS.SetRotation(m_floatingNodesTC, Vector3.right * m_pathStartOffset);
		CreateBanners();
		if (m_banners != null && m_banners.Count > 0)
		{
			m_banners[0].Show();
		}
	}

	private void ParseClouds(GameObject _cloudSlice)
	{
		for (int i = 0; i < _cloudSlice.transform.childCount; i++)
		{
			Transform child = _cloudSlice.transform.GetChild(i);
			m_cloudSets.Add(child);
			for (int j = 0; j < child.childCount; j++)
			{
				Transform child2 = child.GetChild(j);
				m_cloudParticles.Add(child2);
			}
		}
	}

	public void HideSpaceScene()
	{
		EntityManager.SetVisibilityOfEntity(m_spaceEntity, false);
		m_planetCamera.enabled = false;
		if (PsMenuScene.m_blur != null)
		{
			m_blurCamera.enabled = true;
		}
	}

	public void ShowSpaceScene()
	{
		EntityManager.SetVisibilityOfEntity(m_spaceEntity, true);
		m_planetCamera.enabled = true;
		m_blurCamera.enabled = false;
	}

	public void DestroySpaceScene()
	{
		Debug.LogWarning("DESTROYING SPACE SCENE");
		UnityEngine.Object.DestroyImmediate(m_blurCamera.transform.GetChild(0).gameObject);
		CameraS.RemoveCamera(m_blurCamera);
		CameraS.RemoveCamera(m_planetCamera);
		SpriteS.RemoveSpriteSheet(m_planetSpriteSheet);
		SpriteS.RemoveSpriteSheet(m_planetSpriteSheetTutorial);
		if (m_spaceEntity != null)
		{
			EntityManager.RemoveEntity(m_spaceEntity);
		}
		m_spaceEntity = null;
		ClearRows();
		m_floaterManager.ClearFloatingNodes();
		ClearFlyingNodes();
		DestroyBanners();
	}

	public void ClearFlyingNodes()
	{
		int num = m_flyingNodes.Count - 1;
		for (int num2 = num; num2 > -1; num2--)
		{
			m_flyingNodes[num2].Destroy();
		}
	}

	public void LoadPersistentPathData()
	{
		try
		{
			m_mainPath = PsPlanetSerializer.LoadProgression(this, "MainPath");
		}
		catch
		{
			m_mainPath = null;
		}
		if (m_mainPath == null)
		{
			m_mainPath = new PsPlanetPath("MainPath");
		}
		m_floaterManager = new PsFloaterManager(this);
	}

	public void GetTimedEventsFromServer()
	{
		if (m_spaceEntity != null)
		{
			if (PsPlanetManager.m_timedEvents == null)
			{
				TimedEvents.Initialize(m_floaterManager.LoadTimedEvents);
			}
			else
			{
				m_floaterManager.LoadTimedEvents();
			}
		}
	}

	public void UpdateLocalPaths(List<PsPlanetPath> _serverPaths)
	{
		PsPlanetPath psPlanetPath = null;
		PsPlanetPath psPlanetPath2 = null;
		List<PsPlanetPath> list = new List<PsPlanetPath>();
		for (int i = 0; i < _serverPaths.Count; i++)
		{
			if (_serverPaths[i].m_name == "MainPath")
			{
				psPlanetPath = _serverPaths[i];
			}
			else if (_serverPaths[i].m_name == "FloatingPath")
			{
				psPlanetPath2 = _serverPaths[i];
			}
			else
			{
				list.Add(_serverPaths[i]);
			}
		}
		bool flag = false;
		if (psPlanetPath != null && psPlanetPath.m_nodeInfos.Count > 0)
		{
			flag = true;
			Debug.Log("UPDATE PERSISTENT PATH");
			Debug.Log("CURRENT NODE ID: " + m_mainPath.m_currentNodeId);
			Debug.Log("CURRENT NODE COUNT: " + m_mainPath.m_nodeInfos.Count);
			List<PsGameLoop> nodeInfos = psPlanetPath.m_nodeInfos;
			List<PsGameLoop> nodeInfos2 = m_mainPath.m_nodeInfos;
			nodeInfos.Sort((PsGameLoop s1, PsGameLoop s2) => s1.m_nodeId.CompareTo(s2.m_nodeId));
			for (int num = 0; num < nodeInfos.Count; num++)
			{
				if (nodeInfos[num].m_nodeId <= nodeInfos2.Count)
				{
					int index = nodeInfos[num].m_nodeId - 1;
					Debug.Log("UPDATE NODE ID: " + nodeInfos[num].m_nodeId);
					PsGameLoop psGameLoop = nodeInfos2[index];
					if (psGameLoop != PsState.m_activeGameLoop)
					{
						nodeInfos[num].m_path = psGameLoop.m_path;
						nodeInfos2[index] = nodeInfos[num];
					}
				}
				else
				{
					Debug.Log("ADD NODE ID: " + nodeInfos[num].m_nodeId);
					nodeInfos[num].m_path = m_mainPath;
					nodeInfos2.Add(nodeInfos[num]);
				}
			}
			m_mainPath.m_currentNodeId = psPlanetPath.m_currentNodeId;
		}
		if (psPlanetPath2 != null)
		{
			m_floaterManager.ReplacePathWithServerPath(psPlanetPath2);
		}
		for (int num2 = 0; num2 < list.Count; num2++)
		{
			flag = true;
			List<PsGameLoop> nodeInfos3 = list[num2].m_nodeInfos;
			nodeInfos3.Sort((PsGameLoop s1, PsGameLoop s2) => s1.m_nodeId.CompareTo(s2.m_nodeId));
			PsGameLoop nodeInfo = m_mainPath.GetNodeInfo(list[num2].m_startNodeId);
			PsPlanetPath psPlanetPath3 = nodeInfo.m_sidePath;
			if (psPlanetPath3 != null)
			{
				if (psPlanetPath3.m_nodeInfos != null)
				{
					psPlanetPath3.m_nodeInfos = new List<PsGameLoop>(nodeInfo.m_sidePath.m_nodeInfos);
				}
				else
				{
					psPlanetPath3.m_nodeInfos = new List<PsGameLoop>();
				}
			}
			if (psPlanetPath3 != null && psPlanetPath3 != list[num2])
			{
				for (int num3 = 0; num3 < nodeInfos3.Count; num3++)
				{
					if (nodeInfos3[num3].m_nodeId <= psPlanetPath3.m_nodeInfos.Count)
					{
						int index2 = nodeInfos3[num3].m_nodeId - 1;
						Debug.Log("UPDATE SIDE NODE ID: " + nodeInfos3[num3].m_nodeId);
						psPlanetPath3.m_nodeInfos[index2] = nodeInfos3[num3];
						psPlanetPath3.m_nodeInfos[index2].m_path = psPlanetPath3;
					}
					else
					{
						Debug.Log("ADD SIDE NODE ID: " + nodeInfos3[num3].m_nodeId);
						psPlanetPath3.m_nodeInfos.Add(nodeInfos3[num3]);
						psPlanetPath3.m_nodeInfos[nodeInfos3[num3].m_nodeId - 1].m_path = psPlanetPath3;
					}
				}
			}
			else
			{
				psPlanetPath3 = list[num2];
			}
			psPlanetPath3.m_currentNodeId = list[num2].m_currentNodeId;
			nodeInfo.m_sidePath = psPlanetPath3;
			psPlanetPath3.m_parentPath = nodeInfo.m_path;
		}
		if (Main.m_currentGame.m_currentScene.m_name == "MenuScene" && Main.m_currentGame.m_currentScene.m_initComplete)
		{
			ClearRows();
			Update();
		}
		if (flag)
		{
			PsPlanetSerializer.SaveProgressionLocally(this);
		}
		SetRowLimits(0, GetLastNodeId());
	}

	public void InitializePathData()
	{
		m_nodeRows = new List<PlanetRow>();
		if (m_mainPath == null)
		{
			m_mainPath = new PsPlanetPath("MainPath");
		}
	}

	public void ResetLocalProgression()
	{
		if (m_nodeRows != null)
		{
			while (m_nodeRows.Count > 0)
			{
				int index = m_nodeRows.Count - 1;
				PlanetRow planetRow = m_nodeRows[index];
				while (planetRow.m_nodes.Count > 0)
				{
					int index2 = planetRow.m_nodes.Count - 1;
					planetRow.m_nodes[index2].Destroy();
					planetRow.m_nodes.RemoveAt(index2);
				}
				m_nodeRows.RemoveAt(index);
			}
		}
		m_floaterManager.ClearFloatingNodes();
		m_mainPath = new PsPlanetPath("MainPath");
		m_floaterManager.m_floatingPath = new PsPlanetPath("FloatingPath");
		PsMetagameData.LockAll();
		PsPlanetSerializer.SaveProgressionLocally(this);
		PlayerPrefsX.SetLastSync("0");
	}

	public void ResetServerProgression()
	{
		PsServerRequest.ServerResetProgression(PsMetagameManager.ResetProgressionSUCCEED, PsMetagameManager.ResetProgressionFAILED);
	}

	public void InitializePlanetNodes(PsGameLoop _targetNode = null)
	{
		SetRowLimits(0, GetLastNodeId());
		int num = m_mainPath.m_currentNodeId;
		if (PsState.m_activeGameLoop != null && (PsState.m_activeGameLoop.m_context == PsMinigameContext.Level || PsState.m_activeGameLoop.m_context == PsMinigameContext.Signal || PsState.m_activeGameLoop.m_context == PsMinigameContext.EditorPuzzle || PsState.m_activeGameLoop.m_context == PsMinigameContext.EditorTemplate || PsState.m_activeGameLoop.m_context == PsMinigameContext.Versus || PsState.m_activeGameLoop.m_context == PsMinigameContext.Block))
		{
			num = PsState.m_activeGameLoop.m_nodeId + PsState.m_activeGameLoop.m_path.m_startNodeId + 1;
		}
		m_currentAngle = (float)(num - 2) * (0f - m_nodeRowAngleInterval);
		PsMainMenuState.m_currentScroll = m_currentAngle;
		Update();
	}

	public void SetRowLimits(int _min, int _max)
	{
		m_minRow = _min - 1;
		m_maxRow = _max - 2;
		m_minAngle = (float)m_minRow * m_nodeRowAngleInterval;
		m_maxAngle = (float)m_maxRow * m_nodeRowAngleInterval;
		Debug.LogWarning(m_minRow + " - " + m_maxAngle);
	}

	public List<PsGameLoop> GetRow(int _rowId)
	{
		List<PsGameLoop> list = new List<PsGameLoop>();
		PsGameLoop nodeInfo = m_mainPath.GetNodeInfo(_rowId);
		if (nodeInfo != null)
		{
			list.Add(nodeInfo);
		}
		int num = 0;
		int num2 = m_mainPath.GetPrevBlockId(_rowId) - 5;
		for (int num3 = _rowId - 1; num3 > num2 - 1; num3--)
		{
			num++;
			PsGameLoop nodeInfo2 = m_mainPath.GetNodeInfo(num3);
			if (nodeInfo2 != null && nodeInfo2.m_sidePath != null)
			{
				PsGameLoop nodeInfo3 = nodeInfo2.m_sidePath.GetNodeInfo(num);
				if (nodeInfo3 != null)
				{
					list.Add(nodeInfo3);
				}
			}
		}
		return list;
	}

	public List<PsGameLoop> GeneratePersistentPathStrip()
	{
		List<PsGameLoop> list = new List<PsGameLoop>();
		List<PsUnlock> list2 = new List<PsUnlock>();
		int i = 0;
		string unlockIdentifier = string.Empty;
		PsUnlock psUnlock;
		for (; i < 15; i += psUnlock.m_levels.Count)
		{
			psUnlock = null;
			psUnlock = ((i != 0) ? PsMetagameData.GetNextUnlockFromIdentifier(unlockIdentifier) : PsMetagameData.GetNextUnlock());
			if (psUnlock == null)
			{
				Debug.LogError("WHAT!?");
				return list;
			}
			unlockIdentifier = psUnlock.m_name;
			list2.Add(psUnlock);
		}
		int num = 0;
		int _levelNumber = 0;
		if (m_mainPath.m_nodeInfos.Count > 0)
		{
			num = m_mainPath.GetLastBlockId();
			PsGameLoop nodeInfo = m_mainPath.GetNodeInfo(m_mainPath.GetLastLevelId());
			if (nodeInfo != null)
			{
				_levelNumber = nodeInfo.m_levelNumber;
			}
		}
		Debug.LogWarning("START ID: " + num);
		Debug.LogWarning("START LEVEL NUMBER: " + num);
		int num2 = 0;
		for (int j = 0; j < list2.Count; j++)
		{
			PsUnlock psUnlock2 = list2[j];
			Debug.LogWarning("LEVEL COUNT: " + psUnlock2.m_levels.Count);
			for (int k = 0; k < psUnlock2.m_levels.Count; k++)
			{
				MetagameNodeData metagameNodeData = psUnlock2.m_levels[k];
				num++;
				PsGameLoop psGameLoop = CreateLoop(m_mainPath, metagameNodeData, num, ref _levelNumber, psUnlock2.m_name);
				if (metagameNodeData.m_sidePathLevels != null)
				{
					List<PsGameLoop> row = GetRow(num + 1);
					bool flag = false;
					bool flag2 = false;
					for (int l = 0; l < row.Count; l++)
					{
						if (row[l].m_path.m_lane == -1)
						{
							flag = true;
						}
						else if (row[l].m_path.m_lane == 1)
						{
							flag2 = true;
						}
					}
					if (!flag || !flag2)
					{
						PsPlanetPath psPlanetPath = new PsPlanetPath(num + "_SidePath");
						float xPosition = PsPlanetNode.GetXPosition(psGameLoop, 1);
						float xPosition2 = PsPlanetNode.GetXPosition(psGameLoop, 2);
						if (xPosition2 < 0f)
						{
							psPlanetPath.m_lane = 1;
						}
						else
						{
							psPlanetPath.m_lane = -1;
						}
						if (psPlanetPath.m_lane == -1 && flag)
						{
							psPlanetPath.m_lane = 1;
						}
						if (psPlanetPath.m_lane == 1 && flag2)
						{
							psPlanetPath.m_lane = -1;
						}
						psPlanetPath.m_startNodeId = psGameLoop.m_nodeId;
						psPlanetPath.m_parentPath = psGameLoop.m_path;
						psPlanetPath.m_currentNodeId = 0;
						psGameLoop.m_sidePath = psPlanetPath;
						int num3 = 0;
						int _levelNumber2 = 0;
						for (int m = 0; m < metagameNodeData.m_sidePathLevels.Count; m++)
						{
							MetagameNodeData metagameNodeData2 = metagameNodeData.m_sidePathLevels[m];
							num3++;
							PsGameLoop psGameLoop2 = CreateLoop(psPlanetPath, metagameNodeData2, num3, ref _levelNumber2, string.Empty);
							psPlanetPath.m_nodeInfos.Add(psGameLoop2);
							if (metagameNodeData2.m_dialogues == null)
							{
								continue;
							}
							psGameLoop2.m_dialogues = new Hashtable();
							for (int n = 0; n < metagameNodeData2.m_dialogues.Count; n++)
							{
								string text = metagameNodeData2.m_dialogues[n].m_trigger.ToString();
								if (!psGameLoop2.m_dialogues.Contains(text))
								{
									psGameLoop2.m_dialogues.Add(text, metagameNodeData2.m_dialogues[n].m_identifier);
								}
								else
								{
									Debug.LogError("DUPLICATE DIALOGUE IDENTIFIER " + text + " AT " + psUnlock2.m_name);
								}
							}
						}
					}
					else
					{
						Debug.LogError("Tried to add sidepath, but all lanes were already in use: " + num);
					}
				}
				if (metagameNodeData.m_dialogues != null)
				{
					psGameLoop.m_dialogues = new Hashtable();
					for (int num4 = 0; num4 < metagameNodeData.m_dialogues.Count; num4++)
					{
						string text2 = metagameNodeData.m_dialogues[num4].m_trigger.ToString();
						if (!psGameLoop.m_dialogues.Contains(text2))
						{
							psGameLoop.m_dialogues.Add(text2, metagameNodeData.m_dialogues[num4].m_identifier);
						}
						else
						{
							Debug.LogError("DUPLICATE DIALOGUE IDENTIFIER " + text2 + " AT " + psUnlock2.m_name);
						}
					}
				}
				list.Add(psGameLoop);
				m_mainPath.m_nodeInfos.Add(psGameLoop);
				string unlockedProgressionName = psGameLoop.GetUnlockedProgressionName();
				if (unlockedProgressionName != string.Empty)
				{
					psGameLoop.SetUnlockedProgressionName(psUnlock2.m_name);
				}
			}
		}
		SetRowLimits(0, GetLastNodeId());
		return list;
	}

	public PsGameLoop CreateLoop(PsPlanetPath _path, MetagameNodeData _data, int _id, ref int _levelNumber, string _unlockName)
	{
		PsGameLoop result = null;
		if (_data.m_nodeDataType == MetagameNodeDataType.Level)
		{
			_levelNumber++;
			MinigameSearchParametres minigameSearchParametres = new MinigameSearchParametres();
			minigameSearchParametres.m_difficulty = _data.m_levelDifficulty;
			minigameSearchParametres.m_features = ((_data.m_levelSubgenre == PsSubgenre.Any) ? null : new string[1] { _data.m_levelSubgenre.ToString() });
			minigameSearchParametres.m_gameMode = _data.m_levelGameMode;
			minigameSearchParametres.m_items = _data.m_levelItems.ToArray();
			minigameSearchParametres.m_playerUnitFilter = _data.m_levelPlayerUnit;
			result = new PsGameLoopLevel(_data.m_levelMinigameId, minigameSearchParametres, _path, _id, _levelNumber, 0, false, _data.m_levelMedalTimes);
		}
		else if (_data.m_nodeDataType == MetagameNodeDataType.EditorPuzzle)
		{
			_levelNumber++;
			Debug.LogWarning("PUZZLE MINIGAME ID: " + _data.m_levelMinigameId);
			result = new PsGameLoopEditorPuzzle(_data.m_levelItems, _data.m_levelMinigameId, _path, _id, _levelNumber, 0, false);
		}
		else if (_data.m_nodeDataType == MetagameNodeDataType.LevelTemplate)
		{
			_levelNumber++;
			result = new PsGameLoopEditorTemplate(_data.m_dataIdentifier, string.Empty, _path, _id, _levelNumber, 0, false, false);
		}
		else if (_data.m_nodeDataType == MetagameNodeDataType.VersusRace)
		{
			_levelNumber++;
			result = new PsGameLoopVersus(PsMinigameContext.Versus, null, null, _data.m_levelMinigameId, _path, _id, _levelNumber, 0, -1, false);
		}
		else if (_data.m_nodeDataType == MetagameNodeDataType.Signal || _data.m_nodeDataType == MetagameNodeDataType.Block)
		{
			result = ((_data.m_blockType != PsPathBlockType.Antenna) ? new PsGameLoopBlockLevel(PsMinigameContext.Block, _data.m_levelMinigameId, _data.m_blockType, _data.m_blockRequiredStars, _unlockName, _data.m_blockWillUnlock, _path, _id, 0, _data.m_blockCoins, _data.m_blockDiamonds, _data.m_blockBolts, _data.m_blockKeys, false, _data.m_dataIdentifier) : (string.IsNullOrEmpty(_data.m_levelMinigameId) ? ((PsGameLoop)new PsGameLoopBlockSignalWithoutLevel(PsMinigameContext.Signal, _data.m_blockType, _data.m_blockRequiredStars, _unlockName, _data.m_blockWillUnlock, _path, _id, 0, _data.m_blockCoins, _data.m_blockDiamonds, _data.m_blockBolts, _data.m_blockKeys, false, _data.m_dataIdentifier)) : ((PsGameLoop)new PsGameLoopBlockSignal(PsMinigameContext.Signal, _data.m_levelMinigameId, _data.m_blockType, _data.m_blockRequiredStars, _unlockName, _data.m_blockWillUnlock, _path, _id, 0, _data.m_blockCoins, _data.m_blockDiamonds, _data.m_blockBolts, _data.m_blockKeys, false, _data.m_dataIdentifier))));
		}
		return result;
	}

	public void OpenUnlocks()
	{
		Debug.LogWarning("UNLOCKING:");
		Debug.LogWarning("node count :" + m_mainPath.m_nodeInfos.Count);
		Debug.LogWarning("current node id :" + m_mainPath.m_currentNodeId);
		int num = m_mainPath.GetCurrentBlockId();
		if (num == -1)
		{
			num = m_mainPath.m_nodeInfos.Count - 1;
		}
		Debug.LogWarning("check until id :" + num);
		if (PsState.m_activeGameLoop != null && !string.IsNullOrEmpty(PsState.m_activeGameLoop.m_temporaryUnlock))
		{
			PsMetagameData.TemporarilyUnlockUnlockable(PsState.m_activeGameLoop.m_temporaryUnlock);
		}
		for (int i = 0; i < m_mainPath.m_nodeInfos.Count && num >= m_mainPath.m_nodeInfos[i].m_nodeId; i++)
		{
			string unlockedProgressionName = m_mainPath.m_nodeInfos[i].GetUnlockedProgressionName();
			if (unlockedProgressionName != string.Empty)
			{
				PsMetagameData.Unlock(unlockedProgressionName);
			}
		}
	}

	public void RollToRowId(int _rowId)
	{
		m_forcedRoll = true;
		m_startAngle = m_currentAngle;
		m_targetRow = _rowId - 1;
		m_targetAngle = (float)m_targetRow * (0f - m_nodeRowAngleInterval);
		m_rollStartTime = Main.m_resettingGameTime;
		m_rollDifference = m_targetAngle - m_startAngle;
	}

	public void Update()
	{
		m_currentZeroIndex = Mathf.FloorToInt((0f - m_currentAngle) / m_nodeRowAngleInterval);
		if (m_forcedRoll)
		{
			float num = Main.m_resettingGameTime - m_rollStartTime;
			if (num < 1.5f)
			{
				float currentAngle = TweenS.tween(TweenStyle.CubicInOut, num, 1.5f, m_startAngle, m_rollDifference);
				m_currentAngle = currentAngle;
				m_forcedRollActive = true;
			}
			else if (m_forcedRollActive)
			{
				m_forcedRoll = false;
				m_forcedRollActive = false;
			}
		}
		for (int i = 0; i < m_nodeRowCount; i++)
		{
			if (m_nodeRows.Count > i)
			{
				int rowId = m_nodeRows[0].m_rowId;
				if (rowId < m_currentZeroIndex)
				{
					int num2 = m_currentZeroIndex - rowId;
					for (int j = 0; j < num2; j++)
					{
						int rowId2 = m_nodeRows[m_nodeRows.Count - 1].m_rowId;
						PlanetRow planetRow = new PlanetRow(rowId2 + 1);
						m_nodeRows.Add(planetRow);
						List<PsGameLoop> row = GetRow(rowId2 + 1);
						for (int k = 0; k < row.Count; k++)
						{
							PsPlanetNode item = CreatePlanetNode(row[k]);
							planetRow.m_nodes.Add(item);
						}
					}
					while (m_nodeRows.Count > m_nodeRowCount)
					{
						int index = 0;
						PlanetRow planetRow2 = m_nodeRows[index];
						while (planetRow2.m_nodes.Count > 0)
						{
							int index2 = planetRow2.m_nodes.Count - 1;
							if (planetRow2.m_nodes[index2] != null)
							{
								planetRow2.m_nodes[index2].Destroy();
							}
							planetRow2.m_nodes.RemoveAt(index2);
						}
						m_nodeRows.RemoveAt(index);
					}
				}
				else
				{
					if (rowId <= m_currentZeroIndex)
					{
						continue;
					}
					int num3 = rowId - m_currentZeroIndex;
					for (int num4 = num3 - 1; num4 > -1; num4--)
					{
						PlanetRow planetRow3 = new PlanetRow(m_currentZeroIndex + num4);
						m_nodeRows.Insert(0, planetRow3);
						List<PsGameLoop> row2 = GetRow(m_currentZeroIndex + num4);
						for (int l = 0; l < row2.Count; l++)
						{
							PsPlanetNode item2 = CreatePlanetNode(row2[l]);
							planetRow3.m_nodes.Add(item2);
						}
					}
					while (m_nodeRows.Count > m_nodeRowCount)
					{
						int index3 = m_nodeRows.Count - 1;
						PlanetRow planetRow4 = m_nodeRows[index3];
						while (planetRow4.m_nodes.Count > 0)
						{
							int index4 = planetRow4.m_nodes.Count - 1;
							planetRow4.m_nodes[index4].Destroy();
							planetRow4.m_nodes.RemoveAt(index4);
						}
						m_nodeRows.RemoveAt(index3);
					}
				}
			}
			else
			{
				PlanetRow planetRow5 = new PlanetRow(m_currentZeroIndex + i);
				m_nodeRows.Add(planetRow5);
				List<PsGameLoop> row3 = GetRow(m_currentZeroIndex + i);
				for (int m = 0; m < row3.Count; m++)
				{
					PsPlanetNode item3 = CreatePlanetNode(row3[m]);
					planetRow5.m_nodes.Add(item3);
				}
			}
		}
		TransformS.SetRotation(m_alienPlanetTC, Vector3.right * m_currentAngle);
		float num5 = Math.Max(0, m_floaterManager.m_floatingNodeList.Count - 3);
		float num6 = num5 * m_nodeRowAngleInterval;
		if (num6 > 0f)
		{
			m_floaterOffset = Mathf.Max(0f, Mathf.Min(1f, (m_floaterOffset * num6 - PsMainMenuState.m_rollInertia) / num6));
		}
		m_floatingNodesTC.transform.Rotate(Vector3.right * PsMainMenuState.m_rollInertia);
		Quaternion localRotation = Quaternion.Lerp(m_floatingNodesTC.transform.rotation, Quaternion.Euler(Vector3.right * (m_pathStartOffset - m_floaterOffset * num6)), 0.025f);
		m_floatingNodesTC.transform.localRotation = localRotation;
		for (int n = 0; n < m_flyingNodes.Count; n++)
		{
			m_flyingNodes[n].Update();
		}
		Quaternion localRotation2 = Quaternion.Lerp(m_cloudTC.transform.localRotation, m_alienPlanetTC.transform.localRotation, 0.1f);
		m_cloudTC.transform.localRotation = localRotation2;
		Quaternion localRotation3 = Quaternion.Lerp(m_atmosphereTC.transform.localRotation, m_alienPlanetTC.transform.localRotation, 0.035f);
		m_atmosphereTC.transform.localRotation = localRotation3;
		for (int num7 = 0; num7 < m_cloudSets.Count; num7++)
		{
			m_cloudSets[num7].RotateAround(Vector3.zero, Vector3.right, (Mathf.Sin(Main.m_resettingGameTime * 0.25f + (float)(num7 * 10)) * 0.75f + 0.75f) * -0.025f - 0.025f);
		}
		for (int num8 = 0; num8 < m_cloudParticles.Count; num8++)
		{
			m_cloudParticles[num8].LookAt(m_planetCamera.transform.position);
			m_cloudParticles[num8].Rotate(Vector3.right * -90f);
		}
		for (int num9 = 0; num9 < m_nodeRows.Count; num9++)
		{
			PlanetRow planetRow6 = m_nodeRows[num9];
			for (int num10 = 0; num10 < planetRow6.m_nodes.Count; num10++)
			{
				planetRow6.m_nodes[num10].Update();
			}
		}
		m_floaterManager.Update();
		if (m_banners == null || m_banners.Count <= 0)
		{
			return;
		}
		for (int num11 = 0; num11 < m_banners.Count; num11++)
		{
			PsUICheckpointBanner psUICheckpointBanner = m_banners[num11];
			PsGameLoop gameLoop = psUICheckpointBanner.m_gameLoop;
			float num12 = (float)(gameLoop.m_nodeId - 2) * m_nodeRowAngleInterval + m_currentAngle;
			if (num12 < 38f && (psUICheckpointBanner.m_state == PsUICheckpointBanner.State.STAND || psUICheckpointBanner.m_state == PsUICheckpointBanner.State.UPDATE))
			{
				psUICheckpointBanner.m_state = PsUICheckpointBanner.State.FOLLOW;
				psUICheckpointBanner.SetArrowShape(0f);
				int num13 = gameLoop.m_nodeId + gameLoop.m_path.m_startNodeId;
				float xPosition = PsPlanetNode.GetXPosition(gameLoop);
				float num14 = 0f;
				float num15 = (float)num13 + num14;
				float num16 = Mathf.Cos((m_pathStartOffset + num15 * m_nodeRowAngleInterval) * ((float)Math.PI / 180f)) * m_planetRadius;
				float num17 = Mathf.Sin((m_pathStartOffset + num15 * m_nodeRowAngleInterval) * ((float)Math.PI / 180f)) * m_planetRadius;
				Vector3 childLocalPos = new Vector3(xPosition, num16, num17).normalized * m_planetRadius;
				float x = Mathf.Atan2(0f - num16, num17) * 57.29578f + 180f;
				float y = Mathf.Atan2(m_planetRadius, xPosition) * 57.29578f + 90f;
				TransformS.ParentComponent(psUICheckpointBanner.m_checkpointTC, m_alienPlanetTC, childLocalPos);
				TransformS.SetRotation(psUICheckpointBanner.m_checkpointTC, new Vector3(x, 0f, 0f));
				psUICheckpointBanner.m_checkpointTC.transform.Rotate(new Vector3(90f, y, 0f), Space.Self);
			}
			else if (num12 >= 38f && (psUICheckpointBanner.m_state == PsUICheckpointBanner.State.FOLLOW || psUICheckpointBanner.m_state == PsUICheckpointBanner.State.UPDATE))
			{
				psUICheckpointBanner.m_state = PsUICheckpointBanner.State.STAND;
				psUICheckpointBanner.SetArrowShape((num12 - 38f) / 10f);
				float xPosition2 = PsPlanetNode.GetXPosition(gameLoop);
				float num18 = Mathf.Cos((0f - m_currentAngle - 22f) * ((float)Math.PI / 180f)) * m_planetRadius;
				float num19 = Mathf.Sin((0f - m_currentAngle - 22f) * ((float)Math.PI / 180f)) * m_planetRadius;
				Vector3 childLocalPos2 = new Vector3(xPosition2, num18, num19).normalized * m_planetRadius;
				float x2 = Mathf.Atan2(0f - num18, num19) * 57.29578f + 180f;
				float y2 = Mathf.Atan2(m_planetRadius, xPosition2) * 57.29578f + 90f;
				TransformS.ParentComponent(psUICheckpointBanner.m_checkpointTC, m_alienPlanetTC, childLocalPos2);
				TransformS.SetRotation(psUICheckpointBanner.m_checkpointTC, new Vector3(x2, 0f, 0f));
				psUICheckpointBanner.m_checkpointTC.transform.Rotate(new Vector3(90f, y2, 0f), Space.Self);
				psUICheckpointBanner.m_checkpointTC.transform.SetParent(null, true);
			}
			else if (num12 >= 38f && num12 < 48f)
			{
				psUICheckpointBanner.SetArrowShape((num12 - 38f) / 10f);
			}
			if (num11 > 0)
			{
				float num20 = (float)(m_banners[num11 - 1].m_gameLoop.m_nodeId - 2) * m_nodeRowAngleInterval + m_currentAngle;
				if (num20 <= 7f && m_banners[num11].m_hidden)
				{
					m_banners[num11].Show();
				}
				else if (num20 > 7f && !m_banners[num11].m_hidden)
				{
					m_banners[num11].Hide();
				}
			}
			TransformS.SetGlobalPosition(psUICheckpointBanner.m_bannerTC, psUICheckpointBanner.m_checkpointTC.transform.position + psUICheckpointBanner.m_checkpointTC.transform.up * 7f);
		}
	}

	public void DestroyBanners()
	{
		if (m_banners != null && m_banners.Count != 0)
		{
			for (int i = 0; i < m_banners.Count; i++)
			{
				m_banners[i].Destroy();
				m_banners[i] = null;
			}
			m_banners.Clear();
			m_banners = null;
		}
	}

	public void CreateBanners()
	{
		DestroyBanners();
		if (m_banners == null)
		{
			m_banners = new List<PsUICheckpointBanner>();
		}
		for (int i = 0; i < m_mainPath.m_nodeInfos.Count; i++)
		{
			PsGameLoop psGameLoop = m_mainPath.m_nodeInfos[i];
			if (m_mainPath.m_currentNodeId <= psGameLoop.m_nodeId && (psGameLoop.m_context == PsMinigameContext.Block || psGameLoop.m_context == PsMinigameContext.Signal))
			{
				int num = psGameLoop.m_nodeId + psGameLoop.m_path.m_startNodeId;
				float xPosition = PsPlanetNode.GetXPosition(psGameLoop);
				float num2 = 0f;
				float num3 = (float)num + num2;
				float num4 = Mathf.Cos((m_pathStartOffset + num3 * m_nodeRowAngleInterval) * ((float)Math.PI / 180f)) * m_planetRadius;
				float num5 = Mathf.Sin((m_pathStartOffset + num3 * m_nodeRowAngleInterval) * ((float)Math.PI / 180f)) * m_planetRadius;
				Vector3 childLocalPos = new Vector3(xPosition, num4, num5).normalized * m_planetRadius;
				float x = Mathf.Atan2(0f - num4, num5) * 57.29578f + 180f;
				float y = Mathf.Atan2(m_planetRadius, xPosition) * 57.29578f + 90f;
				TransformC transformC = TransformS.AddComponent(m_alienPlanetTC.p_entity, "Checkpoint TC");
				TransformS.ParentComponent(transformC, m_alienPlanetTC, childLocalPos);
				TransformS.SetRotation(transformC, new Vector3(x, 0f, 0f));
				transformC.transform.Rotate(new Vector3(90f, y, 0f), Space.Self);
				TransformC transformC2 = TransformS.AddComponent(m_alienPlanetTC.p_entity, "Banner TC");
				m_banners.Add(new PsUICheckpointBanner(psGameLoop, transformC2, transformC));
				TransformS.SetRotation(transformC2, m_planetCamera.transform.rotation.eulerAngles);
			}
		}
	}

	public void ClearRows()
	{
		if (m_nodeRows == null)
		{
			return;
		}
		while (m_nodeRows.Count > 0)
		{
			int index = m_nodeRows.Count - 1;
			PlanetRow planetRow = m_nodeRows[index];
			while (planetRow.m_nodes.Count > 0)
			{
				int index2 = planetRow.m_nodes.Count - 1;
				planetRow.m_nodes[index2].Destroy();
				planetRow.m_nodes.RemoveAt(index2);
			}
			m_nodeRows.RemoveAt(index);
		}
	}

	private PsPlanetNode CreatePlanetNode(PsGameLoop _info, bool _tutorial = false)
	{
		PsPlanetNode result = null;
		if (_info.m_context == PsMinigameContext.Signal)
		{
			result = new PsPlanetSignalNode(_info, _tutorial);
		}
		else if (_info.m_context == PsMinigameContext.Block)
		{
			result = new PsPlanetBlockNode(_info, _tutorial);
		}
		else if (_info.m_context == PsMinigameContext.Level)
		{
			result = new PsPlanetLevelNode(_info, _tutorial);
		}
		else if (_info.m_context == PsMinigameContext.EditorPuzzle)
		{
			result = new PsPlanetEditorPuzzleNode(_info, _tutorial);
		}
		else if (_info.m_context == PsMinigameContext.EditorTemplate)
		{
			result = new PsPlanetEditorTemplateNode(_info, _tutorial);
		}
		else if (_info.m_context == PsMinigameContext.VersusPath || _info.m_context == PsMinigameContext.Versus)
		{
			result = new PsPlanetVersusNode(_info, _tutorial);
		}
		return result;
	}

	public PsFloatingPlanetNode CreateFloatingNode(PsTimedEventLoop _timedEvent)
	{
		PsFloatingPlanetNode psFloatingPlanetNode = new PsFloatingPlanetNode(_timedEvent);
		psFloatingPlanetNode.SetState();
		return psFloatingPlanetNode;
	}

	public List<PsGameLoop> GiveNextStrip(PsGameLoop _loop)
	{
		if (m_nodeRows != null)
		{
			int count = m_nodeRows.Count;
			int num = count - 1;
			while (num > -1 && m_nodeRows[num].m_rowId != _loop.m_nodeId)
			{
				if (m_nodeRows[num].m_nodes.Count == 0)
				{
					m_nodeRows.RemoveAt(num);
				}
				num--;
			}
		}
		List<PsGameLoop> list = GeneratePersistentPathStrip();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].m_keepNodeHidden = true;
			if (list[i].m_sidePath != null)
			{
				List<PsGameLoop> nodeInfos = list[i].m_sidePath.m_nodeInfos;
				for (int j = 0; j < nodeInfos.Count; j++)
				{
					nodeInfos[j].m_keepNodeHidden = true;
				}
			}
		}
		return list;
	}

	public void StartRevealSquence(List<PsGameLoop> _addedNodes)
	{
		Update();
		CreateBanners();
		float num = 0.125f;
		TimerS.AddComponent(m_spaceEntity, string.Empty, (float)(_addedNodes.Count + 1) * num + 1f, 0f, false, delegate(TimerC _c)
		{
			m_banners[0].Show();
			TimerS.RemoveComponent(_c);
		});
		for (int num2 = 0; num2 < _addedNodes.Count; num2++)
		{
			TimerC timerC = TimerS.AddComponent(m_spaceEntity, string.Empty, (float)(num2 + 1) * num + 1f, 0f, false, RevealPlanetNode);
			timerC.customObject = _addedNodes[num2];
			if (_addedNodes[num2].m_sidePath != null)
			{
				List<PsGameLoop> nodeInfos = _addedNodes[num2].m_sidePath.m_nodeInfos;
				for (int num3 = 0; num3 < nodeInfos.Count; num3++)
				{
					timerC = TimerS.AddComponent(m_spaceEntity, string.Empty, (float)(num2 + num3 + 2) * num + 1f, 0f, false, RevealPlanetNode);
					timerC.customObject = nodeInfos[num3];
				}
			}
		}
		TimerC timerC2 = TimerS.AddComponent(m_spaceEntity, string.Empty, (float)(_addedNodes.Count + 1) * num + 2f, 0f, false, StartRevealUnlockSequence);
		timerC2.customObject = _addedNodes[0];
	}

	private void RevealPlanetNode(TimerC _c)
	{
		TimerS.RemoveComponent(_c);
		PsGameLoop psGameLoop = _c.customObject as PsGameLoop;
		if (psGameLoop.m_node != null)
		{
			psGameLoop.m_node.Reveal();
		}
		else
		{
			psGameLoop.m_keepNodeHidden = false;
		}
	}

	private void StartRevealUnlockSequence(TimerC _c)
	{
		TimerS.RemoveComponent(_c);
		PsState.m_activeGameLoop.ReleaseLoop();
	}

	public void FastForward()
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Timer, m_spaceEntity);
		Debug.LogWarning("fastforwarding");
		Debug.LogWarning("planet timers: " + componentsByEntity.Count);
		for (int i = 0; i < componentsByEntity.Count; i++)
		{
			TimerC timerC = componentsByEntity[i] as TimerC;
			if (timerC.timeoutHandler != null)
			{
				timerC.timeoutHandler(timerC);
			}
		}
		if (PsState.m_activeGameLoop != null)
		{
			PsState.m_activeGameLoop.ReleaseLoop();
		}
	}
}
