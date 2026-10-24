using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Minigame : Level
{
	public const int DOME_WORLD_YOFFSET = 30;

	public bool m_editing;

	public bool m_gamePaused;

	public bool m_gameOn;

	public int m_gameDeathCount;

	public Unit m_playerUnit;

	public LevelPlayerNode m_playerNode;

	public GraphNode m_goalNode;

	public TransformC m_playerTC;

	public Type m_playerControllerType;

	public bool m_playerReachedGoal;

	public int m_playerReachedGoalCount;

	public bool m_gameStarted;

	public bool m_gameEnded;

	public bool m_gameTicksFreezed;

	public float m_gameTicks;

	public float m_realTimeSpent;

	public int m_gameStartCount;

	public bool m_playerBeamingOut;

	public bool m_highscoresSent;

	public Ghost m_lastLoadedGhost;

	public int m_lastSentTimeScore;

	public int m_lastSentScore;

	public bool m_isGamePlayedThrough;

	public int m_collectedStars;

	public Entity m_environmentEntity;

	public Entity m_domeEntity;

	public TransformC m_environmentTC;

	public PrefabC m_backgroundPrefab;

	public LevelGroundNode m_groundNode;

	public ChipmunkBodyC m_groundBody;

	public GroundC m_groundC;

	public SoundC m_music;

	public Hashtable m_itemCount;

	public int m_maxComplexity;

	public int m_complexity;

	public int m_levelRequirement;

	public string m_playerUnitName;

	public bool m_generateDefaultName;

	public TutorialSign m_currentSign;

	public SpriteSheet m_miniGameSpriteSheet;

	public int[] m_layerIndexTable;

	public double m_timeElapsed;

	public double m_timeElapsedInEditor;

	private bool m_newEditSession = true;

	public int m_editSessionCount;

	public int m_itemsModificationCount;

	public int m_groundsModificationCount;

	private float m_timeScaleStart;

	private float m_timeScaleTarget;

	private TweenStyle m_timeScaleTweenStyle;

	private float m_timeScaleDuration;

	private float m_timeScaleCurrentTime;

	private bool m_timeScaling;

	private bool m_waitingForTimeScaleCallback;

	private Action m_timeScaleCallback;

	private float m_timeScaleCallbackDelay;

	public Minigame(int _width, int _height)
		: base(_width, _height)
	{
		m_itemCount = new Hashtable();
		m_complexity = 0;
		m_levelRequirement = 0;
		m_playerUnitName = null;
		m_generateDefaultName = true;
		m_changed = true;
		CreateMiniGameSpriteSheet();
	}

	public Minigame(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		m_generateDefaultName = false;
		m_changed = false;
		CreateMiniGameSpriteSheet();
	}

	public void CreateMiniGameSpriteSheet()
	{
		if (m_miniGameSpriteSheet == null)
		{
			m_miniGameSpriteSheet = SpriteS.AddSpriteSheet(CameraS.m_mainCamera, ResourceManager.GetMaterial(RESOURCE.IngameAtlasMat_Material), ResourceManager.GetTextAsset(RESOURCE.UiAtlas_TextAsset), 1f);
		}
	}

	public override void AddItem(string _key)
	{
		if (m_itemCount == null)
		{
			m_itemCount = new Hashtable();
		}
		PsEditorItem unlockableByIdentifier = PsMetagameData.GetUnlockableByIdentifier(_key);
		if (unlockableByIdentifier != null)
		{
			if (unlockableByIdentifier.m_graphNodeClassName == "LevelPlayerNode")
			{
				m_playerUnitName = unlockableByIdentifier.m_identifier;
			}
			if (m_itemCount.ContainsKey(_key))
			{
				m_itemCount[_key] = (int)m_itemCount[_key] + 1;
			}
			else
			{
				m_itemCount.Add(_key, 1);
			}
			m_complexity += unlockableByIdentifier.m_complexity;
			if (unlockableByIdentifier.m_itemLevel > m_levelRequirement)
			{
				m_levelRequirement = unlockableByIdentifier.m_itemLevel;
			}
		}
	}

	public override void RemoveItem(string _key)
	{
		if (m_itemCount == null)
		{
			m_itemCount = new Hashtable();
		}
		PsEditorItem unlockableByIdentifier = PsMetagameData.GetUnlockableByIdentifier(_key);
		if (unlockableByIdentifier == null || !m_itemCount.ContainsKey(_key))
		{
			return;
		}
		m_itemCount[_key] = (int)m_itemCount[_key] - 1;
		if ((int)m_itemCount[_key] <= 0)
		{
			m_itemCount.Remove(_key);
			if (m_levelRequirement != 0 && m_levelRequirement == unlockableByIdentifier.m_itemLevel)
			{
				m_levelRequirement = 0;
				foreach (string key in m_itemCount.Keys)
				{
					PsEditorItem unlockableByIdentifier2 = PsMetagameData.GetUnlockableByIdentifier(key);
					if (unlockableByIdentifier2.m_itemLevel > m_levelRequirement)
					{
						m_levelRequirement = unlockableByIdentifier2.m_itemLevel;
					}
				}
			}
		}
		m_complexity -= unlockableByIdentifier.m_complexity;
	}

	public override void ClearItems()
	{
		if (m_itemCount != null)
		{
			m_itemCount.Clear();
		}
		m_complexity = 0;
		m_levelRequirement = 0;
		m_playerUnitName = null;
	}

	public override void Destroy()
	{
		if (m_environmentEntity != null)
		{
			if (m_environmentEntity != null)
			{
				EntityManager.RemoveEntity(m_environmentEntity);
			}
			if (m_domeEntity != null)
			{
				EntityManager.RemoveEntity(m_domeEntity);
			}
			m_domeEntity = null;
			m_environmentEntity = null;
		}
		m_groundBody = null;
		m_groundC = null;
		m_itemCount.Clear();
		m_itemCount = null;
		m_complexity = 0;
		m_levelRequirement = 0;
		m_playerUnitName = null;
		if (m_miniGameSpriteSheet != null)
		{
			SpriteS.RemoveSpriteSheet(m_miniGameSpriteSheet);
			m_miniGameSpriteSheet = null;
		}
		base.Destroy();
		AutoGeometryManager.Free();
	}

	public void ApplySettings(bool _createAreaLimits = true, bool _updateDomeGfx = true)
	{
		Debug.LogWarning("APPLY SETTINGS: " + _createAreaLimits);
		LevelPlayerNode levelPlayerNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("Player") as LevelPlayerNode;
		if (levelPlayerNode != null)
		{
			PsState.m_editorCameraPos = levelPlayerNode.m_position;
		}
		if (m_environmentEntity == null)
		{
			m_environmentEntity = EntityManager.AddEntity("MinigameEnvironmentEntity");
			m_environmentTC = TransformS.AddComponent(m_environmentEntity, "MinigameEnvironmentTC", new Vector3(0f, (float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f, 0f));
		}
		int num = 2;
		if (m_settings.Contains("domeSizeIndex"))
		{
			Debug.LogWarning("HAS DOME SIZE: " + (int)m_settings["domeSizeIndex"]);
			num = (int)m_settings["domeSizeIndex"];
		}
		else
		{
			Debug.LogWarning("DOESN'T HAVE DOME SIZE");
			LevelLayer currentLayer = LevelManager.m_currentLevel.m_currentLayer;
			int num2 = currentLayer.m_layerWidth / 2048;
			Debug.Log("OLD LEVEL - CALCULATING DOME SIZE AUTOMATICALLY: " + num2);
			for (int i = 0; i < PsState.m_domeSizes.Length; i++)
			{
				if (PsState.m_domeSizes[i].x >= (float)num2)
				{
					num = i;
					break;
				}
			}
		}
		Debug.Log(num);
		if (_createAreaLimits)
		{
			UpdateAreaLimits(num, false, _updateDomeGfx);
		}
	}

	public override void SetLayerItems()
	{
		if (m_groundNode == null)
		{
			m_groundNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("LevelGround") as LevelGroundNode;
		}
		bool flag = false;
		Debug.LogWarning(m_groundNode != null);
		for (int i = 0; i < m_groundNode.m_AGLayer.Length; i++)
		{
			if (m_groundNode.m_AGLayer[i].m_tileHash.Count <= 0)
			{
				if (!m_itemCount.ContainsKey(m_groundNode.m_AGLayer[i].m_groundC.m_ground.m_groundItemIdentifier))
				{
					continue;
				}
				m_itemCount.Remove(m_groundNode.m_AGLayer[i].m_groundC.m_ground.m_groundItemIdentifier);
				if (!flag)
				{
					PsEditorItem unlockableByIdentifier = PsMetagameData.GetUnlockableByIdentifier(m_groundNode.m_AGLayer[i].m_groundC.m_ground.m_groundItemIdentifier);
					if (m_levelRequirement != 0 && m_levelRequirement == unlockableByIdentifier.m_itemLevel)
					{
						flag = true;
					}
				}
			}
			else
			{
				if (m_itemCount.ContainsKey(m_groundNode.m_AGLayer[i].m_groundC.m_ground.m_groundItemIdentifier))
				{
					m_itemCount[m_groundNode.m_AGLayer[i].m_groundC.m_ground.m_groundItemIdentifier] = m_groundNode.m_AGLayer[i].m_tileHash.Count;
				}
				else
				{
					m_itemCount.Add(m_groundNode.m_AGLayer[i].m_groundC.m_ground.m_groundItemIdentifier, m_groundNode.m_AGLayer[i].m_tileHash.Count);
				}
				PsEditorItem unlockableByIdentifier2 = PsMetagameData.GetUnlockableByIdentifier(m_groundNode.m_AGLayer[i].m_groundC.m_ground.m_groundItemIdentifier);
				if (m_levelRequirement < unlockableByIdentifier2.m_itemLevel)
				{
					m_levelRequirement = unlockableByIdentifier2.m_itemLevel;
				}
			}
		}
		if (flag)
		{
			m_levelRequirement = 0;
			foreach (string key in m_itemCount.Keys)
			{
				PsEditorItem unlockableByIdentifier3 = PsMetagameData.GetUnlockableByIdentifier(key);
				if (unlockableByIdentifier3.m_itemLevel > m_levelRequirement)
				{
					m_levelRequirement = unlockableByIdentifier3.m_itemLevel;
				}
			}
		}
		Debug.Log("Levelrequirement: " + m_levelRequirement);
	}

	public void InitializeDrawMaterials()
	{
		int count = PsMetagameData.m_gameMaterials[0].m_items.Count;
		m_layerIndexTable = new int[count];
		for (int i = 0; i < count; i++)
		{
			PsEditorItem psEditorItem = PsMetagameData.m_gameMaterials[0].m_items[i] as PsEditorItem;
			m_layerIndexTable[i] = GetLayerIndexForMaterial(psEditorItem.m_identifier);
		}
	}

	public int GetLayerIndexForMaterial(string _matIdentifier)
	{
		for (int i = 0; i < AutoGeometryManager.m_layers.Count; i++)
		{
			if (AutoGeometryManager.m_layers[i].m_groundC.m_ground.m_groundItemIdentifier.Equals(_matIdentifier))
			{
				return i;
			}
		}
		return -1;
	}

	public int GetMatIndexFromLayerIndex(int _layerId)
	{
		for (int i = 0; i < m_layerIndexTable.Length; i++)
		{
			if (m_layerIndexTable[i] == _layerId)
			{
				return i;
			}
		}
		return -1;
	}

	public void UpdateAreaLimits(int _domeIndex, bool _updateLookupTables = true, bool _updateDomeGfx = true)
	{
		Vector2 vector = PsState.m_domeSizes[_domeIndex];
		m_settings["domeSizeIndex"] = _domeIndex;
		Debug.LogWarning("UPDATING AREA LIMITS - SETTING DOME SIZE INDEX TO " + _domeIndex);
		if (m_domeEntity != null)
		{
			EntityManager.RemoveEntity(m_domeEntity);
		}
		PsEditorItem psEditorItem = PsMetagameData.m_gameAreas[0].m_items[_domeIndex] as PsEditorItem;
		if (psEditorItem != null)
		{
			if (psEditorItem.m_identifier == "AreaSmall")
			{
				m_maxComplexity = 1000;
			}
			else if (psEditorItem.m_identifier == "AreaMedium")
			{
				m_maxComplexity = 1250;
			}
			else if (psEditorItem.m_identifier == "AreaLarge")
			{
				m_maxComplexity = 1500;
			}
		}
		else
		{
			m_maxComplexity = 1000;
		}
		vector = vector * 2048f / 16f;
		AutoGeometryManager.SetDomeSize((int)vector.x, (int)vector.y, 30f);
		Debug.Log("DOME WORLD SIZE: " + vector * 16f);
		float num = AutoGeometryManager.m_domeDiameter.y / AutoGeometryManager.m_domeDiameter.x;
		LevelLayer currentLayer = LevelManager.m_currentLevel.m_currentLayer;
		m_domeEntity = EntityManager.AddEntity();
		TransformC transformComponent = TransformS.AddComponent(m_domeEntity, Vector2.zero);
		int num2 = 16 + (int)((float)AutoGeometryManager.m_width / 500f);
		float num3 = -90f;
		float num4 = 180f / (float)(num2 - 1);
		Vector2 vector2 = AutoGeometryManager.m_domeDiameter * 16f;
		Vector2[] array = new Vector2[num2];
		for (int i = 0; i < num2; i++)
		{
			float x = (vector2.x * 0.5f + 80f) * Mathf.Sin(num3 * ((float)Math.PI / 180f));
			float y = (float)(-AutoGeometryManager.m_height) * 0.5f + (vector2.y * 0.5f + 80f * num + 30f) * Mathf.Cos(num3 * ((float)Math.PI / 180f));
			array[i] = new Vector2(x, y);
			num3 += num4;
		}
		ucpSegmentShape[] array2 = new ucpSegmentShape[num2];
		Vector2 offset = new Vector2(0f, (float)(-currentLayer.m_layerHeight) * 0.5f);
		for (int j = 1; j < num2; j++)
		{
			array2[j - 1] = new ucpSegmentShape(array[j - 1], array[j], 32f, Vector2.zero, 257u, 0f, 0.5f, 0.8f, (ucpCollisionType)2);
		}
		Vector2 a = new Vector2((float)currentLayer.m_layerWidth * -0.5f, 0f);
		Vector2 b = new Vector2((float)currentLayer.m_layerWidth * 0.5f, 0f);
		array2[num2 - 1] = new ucpSegmentShape(a, b, 30f, offset, 257u, 0f, 0.5f, 0.8f, (ucpCollisionType)2);
		m_groundBody = ChipmunkProS.AddStaticBody(transformComponent, array2);
		m_groundC = PsS.AddGround(m_domeEntity, new SandGround(m_groundNode));
		m_groundBody.customComponent = m_groundC;
		int num5 = 213735;
		if (m_settings.Contains("bgSeed"))
		{
			num5 = (int)m_settings["bgSeed"];
		}
		if (_updateLookupTables)
		{
			AutoGeometryManager.UpdateMaxValueLookupTable(AutoGeometryManager.m_layers[PsState.m_drawLayer], LookupUpdateMode.LEVEL_LOADED, true);
		}
	}

	public void GenerateDefaultNameAndDescription()
	{
		if (m_generateDefaultName)
		{
			PsState.m_activeGameLoop.m_minigameMetaData.name = GetRandomName();
			PsState.m_activeGameLoop.m_minigameMetaData.description = GetDefaultDescription(PsState.m_activeGameLoop.m_minigameMetaData.gameMode);
			Debug.Log("Changing minigame default name to: " + PsState.m_activeGameLoop.m_minigameMetaData.name);
			m_generateDefaultName = false;
		}
	}

	public static string GetRandomName()
	{
		UnityEngine.Random.seed = UnityEngine.Random.Range(0, (int)(Main.m_gameTimeSinceAppStarted * 72.0));
		int max = PsResources.LevelNameGeneratorBaseStrings.Length;
		return PsResources.LevelNameGeneratorBaseStrings[UnityEngine.Random.Range(0, max)];
	}

	public static string GetDefaultDescription(PsGameMode _gameMode)
	{
		return PsStrings.Get((StringID)(9 + _gameMode - 1));
	}

	public void SetGameMode(string _gameModeName, Vector2 _pos, bool _firstSelect)
	{
		PsState.m_activeGameLoop.m_minigameMetaData.gameMode = (PsGameMode)(int)Enum.Parse(typeof(PsGameMode), _gameModeName);
		GraphElement[] elements = LevelManager.m_currentLevel.m_currentLayer.GetElements("CollectibleStar");
		switch (PsState.m_activeGameLoop.m_minigameMetaData.gameMode)
		{
		case PsGameMode.Race:
		{
			for (int i = 0; i < elements.Length; i++)
			{
				elements[i].Dispose();
			}
			break;
		}
		case PsGameMode.StarCollect:
			if (elements.Length == 0)
			{
				CreateStars();
			}
			break;
		}
	}

	public void CreateStars()
	{
		for (int i = 0; i < 3; i++)
		{
			int num = (int)m_settings["domeSizeIndex"];
			float num2 = PsState.m_domeSizes[num].x * 2048f;
			float num3 = PsState.m_domeSizes[num].y * 2048f;
			float num4 = (float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f;
			float x = num2 * 0.1f * (float)(i - 1);
			float y = num4 + num3 * 0.35f;
			GraphElement graphElement = new GraphNode(GraphNodeType.Normal, Type.GetType("CollectibleStar"), "CollectibleStar", new Vector3(x, y, 0f), Vector3.zero, Vector3.one);
			LevelManager.m_currentLevel.m_currentLayer.AddElement(graphElement);
			graphElement.Assemble();
		}
	}

	public void SetPlayer(Unit _player, TransformC _playerTC, Type _controllerType)
	{
		m_playerUnit = _player;
		m_playerNode = _player.m_graphElement as LevelPlayerNode;
		m_playerTC = _playerTC;
		m_playerControllerType = _controllerType;
	}

	public void SetPlayerNode(Unit _player)
	{
		m_playerNode = _player.m_graphElement as LevelPlayerNode;
	}

	public void SetGoalNode(Unit _goal)
	{
		m_goalNode = _goal.m_graphElement as GraphNode;
	}

	public void RemovePlayer()
	{
		m_playerTC = null;
		m_playerUnit = null;
		m_playerNode = null;
		m_playerControllerType = null;
		SoundS.SetListener(CameraS.m_mainCamera.gameObject);
	}

	public override void ItemChanged()
	{
		base.ItemChanged();
		m_itemsModificationCount++;
	}

	public override void Update()
	{
		base.Update();
		UpdateTimer();
		if (m_editing && m_changed && m_newEditSession)
		{
			m_newEditSession = false;
			m_editSessionCount++;
		}
		else if (!m_newEditSession && m_gameStarted)
		{
			m_newEditSession = true;
		}
		if (m_gameStarted && !m_gamePaused && PsState.m_activeGameLoop.m_gameMode != null)
		{
			PsState.m_activeGameLoop.m_gameMode.UpdateGhosts();
		}
		if (!m_timeScaling && !m_waitingForTimeScaleCallback)
		{
			return;
		}
		m_timeScaleCurrentTime += Main.m_dt;
		if (m_timeScaling)
		{
			if (m_timeScaleCurrentTime >= m_timeScaleDuration)
			{
				Main.SetTimeScale(m_timeScaleTarget);
				m_timeScaling = false;
				if (!m_waitingForTimeScaleCallback && m_timeScaleCallback != null)
				{
					m_timeScaleCallback();
				}
			}
			else
			{
				float timeScale = TweenS.tween(m_timeScaleTweenStyle, m_timeScaleCurrentTime, m_timeScaleDuration, m_timeScaleStart, m_timeScaleTarget - m_timeScaleStart);
				Main.SetTimeScale(timeScale);
			}
		}
		else if (m_timeScaleCurrentTime >= m_timeScaleDuration + m_timeScaleCallbackDelay)
		{
			if (m_timeScaleCallback != null)
			{
				m_timeScaleCallback();
			}
			m_waitingForTimeScaleCallback = false;
			m_timeScaleCallback = null;
		}
	}

	public virtual void TweenTimeScale(float _targetTimeScale, TweenStyle _tweenStyle, float _duration, Action _callback = null, float _callbackDelay = 0f)
	{
		m_timeScaleStart = Main.m_timeScale;
		m_timeScaleTarget = _targetTimeScale;
		m_timeScaleTweenStyle = _tweenStyle;
		m_timeScaleCurrentTime = 0f;
		m_timeScaleDuration = _duration;
		m_timeScaleCallback = _callback;
		m_timeScaleCallbackDelay = _callbackDelay;
		m_timeScaling = true;
		if (m_timeScaleCallback != null && m_timeScaleCallbackDelay > 0f)
		{
			m_waitingForTimeScaleCallback = true;
		}
	}

	public virtual void RemoveTimeScale()
	{
		m_timeScaling = false;
		m_waitingForTimeScaleCallback = false;
		m_timeScaleCallback = null;
		Main.SetTimeScale(1f);
	}

	public virtual void InitializeTimer()
	{
		m_timeElapsed = 0.0;
		m_timeElapsedInEditor = 0.0;
	}

	public void ResetEditDateCounters()
	{
		m_editSessionCount = 0;
		m_itemsModificationCount = 0;
		m_groundsModificationCount = 0;
	}

	public virtual int GetTimeSinceInit()
	{
		return Convert.ToInt32(Math.Round(m_timeElapsed));
	}

	public virtual int GetEditTimeSinceInit()
	{
		return Convert.ToInt32(Math.Round(m_timeElapsedInEditor));
	}

	protected virtual void UpdateTimer()
	{
		if (m_editing)
		{
			m_timeElapsedInEditor += Time.unscaledDeltaTime;
		}
		m_timeElapsed += Time.unscaledDeltaTime;
	}

	public void SetGamePaused(bool _pause)
	{
		if (_pause == PsState.m_physicsPaused)
		{
			return;
		}
		if (_pause)
		{
			SoundS.SetPauseForAllSounds(true);
			EntityManager.SetActivityOfEntitiesWithTag("GTAG_AUTODESTROY", false, false, false);
			EntityManager.SetActivityOfEntitiesWithTag("GTAG_UNIT", false, false, false);
			CameraS.m_updateComponents = false;
			PsState.m_physicsPaused = true;
			m_gamePaused = true;
			m_gameOn = false;
			if (m_currentSign != null)
			{
				m_currentSign.Hide();
			}
		}
		else
		{
			SoundS.SetPauseForAllSounds(false);
			EntityManager.SetActivityOfEntitiesWithTag("GTAG_AUTODESTROY", true, false, false);
			EntityManager.SetActivityOfEntitiesWithTag("GTAG_UNIT", true, false, false);
			PsState.m_physicsPaused = false;
			CameraS.m_updateComponents = true;
			m_gamePaused = false;
			m_gameOn = true;
			if (m_currentSign != null)
			{
				m_currentSign.Show();
			}
		}
		EveryplayManager.SetPaused(_pause);
	}
}
