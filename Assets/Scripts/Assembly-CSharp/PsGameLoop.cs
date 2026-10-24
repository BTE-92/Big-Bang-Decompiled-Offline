using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using UnityEngine;

public class PsGameLoop
{
	public PsPlanet m_planet;

	public PsMinigameContext m_context;

	public PsGameLoop m_previousLoop;

	public bool m_createFlyingNode;

	public PsPlanetPath m_path;

	public int m_nodeId;

	public int m_levelNumber;

	public PsPlanetPath m_sidePath;

	public bool m_unlocked;

	public bool m_sendQuit;

	public bool m_freeUnlock;

	public bool m_adRent;

	public bool m_activated;

	public PsGameMode m_gameModeFromProgressionData;

	public string m_minigameId;

	public MinigameSearchParametres m_minigameSearchParametres;

	public PsMinigameMetaData m_minigameMetaData;

	public byte[] m_minigameBytes;

	public VersusMetaData m_versusData;

	public string[] m_forcedMedalTimes;

	public PsGameModeBase m_gameMode;

	public int m_scoreCurrent;

	public int m_scoreBest;

	public int m_scoreOld;

	public int m_rewardOld;

	public int m_currentRunScore;

	public int m_timeScoreCurrent;

	public int m_timeScoreBest;

	public int m_timeScoreOld;

	public bool m_boosterUsed;

	public bool m_winFirtTime;

	public Hashtable m_dialogues;

	public string m_temporaryUnlock = string.Empty;

	private bool m_unlockLoopAfterLoading;

	public bool m_loadingMetaData;

	public bool m_loadingMinigameBytes;

	public bool m_rentedVehicle;

	public List<int> m_rentUpgradeValues;

	public bool m_userHasSelectedVehicle;

	public PsSelectedVehicle m_selectedVehicle;

	public string m_freeConsumableUnlock = string.Empty;

	public PsPlanetNode m_node;

	public bool m_keepNodeHidden;

	public float m_roadDrawStart;

	public bool m_drawRoad;

	public float m_roadScaleStart;

	public bool m_scaleRoad;

	public Action m_loadMetadataCallback;

	protected DateTime m_startTime;

	protected long m_elapsedtime;

	public bool m_backAtMenu;

	public bool m_released;

	protected bool m_autosave;

	public Entity m_loaderEntity;

	public List<PsUnlockable> m_unlockableList;

	public PsGameLoop(PsMinigameContext _context, string _minigameId, PsPlanetPath _path, int _id, int _levelNumber, int _score, bool _unlocked, string[] _medalTimes = null)
	{
		m_planet = PsPlanetManager.GetCurrentPlanet();
		m_context = _context;
		m_minigameId = _minigameId;
		m_path = _path;
		m_nodeId = _id;
		m_levelNumber = _levelNumber;
		m_scoreBest = _score;
		m_scoreCurrent = m_scoreBest;
		m_scoreOld = m_scoreBest;
		m_rewardOld = m_scoreBest;
		m_unlocked = _unlocked;
		m_currentRunScore = 0;
		m_activated = false;
		if (m_unlocked)
		{
			m_activated = true;
		}
		m_forcedMedalTimes = _medalTimes;
		m_sidePath = null;
		m_loadingMetaData = false;
		m_sendQuit = true;
		m_startTime = DateTime.UtcNow;
		m_backAtMenu = false;
		m_released = false;
		m_boosterUsed = false;
		m_freeUnlock = false;
		m_adRent = false;
	}

	public virtual void SetPreviousLoop(PsGameLoop _loop)
	{
		m_previousLoop = _loop;
	}

	public virtual void SetUnlockedProgressionName(string _name)
	{
	}

	public virtual string GetUnlockedProgressionName()
	{
		return string.Empty;
	}

	public virtual string GetWillUnlockProgressionName()
	{
		return string.Empty;
	}

	public virtual void StartLoop()
	{
		m_winFirtTime = false;
		if (PsState.m_activeGameLoop == null)
		{
			SetAsActiveLoop();
		}
	}

	public virtual void LoopUnlocked()
	{
	}

	public virtual void StartCancelled()
	{
		PsMetagameManager.ShowResources(null, false, true, true, true, true, 0.03f, true);
	}

	public virtual float GiveCheckpointReward()
	{
		return 0f;
	}

	public virtual void LoadMinigameMetaData(Action _callback = null)
	{
		m_loadMetadataCallback = (Action)Delegate.Combine(m_loadMetadataCallback, _callback);
		Debug.LogWarning("------- LOAD ONLY MINIGAME METADATA: " + m_minigameId);
		if (m_minigameId != string.Empty && m_minigameMetaData == null)
		{
			m_loadingMetaData = true;
			HttpC httpC = MiniGame.Get(m_minigameId, LoadOnlyMetaDataSUCCESS, LoadOnlyMetaDataFAILED);
			httpC.objectData = m_minigameId;
			Debug.LogWarning("------- LOAD METADATA: " + m_minigameId);
		}
		else if (string.IsNullOrEmpty(m_minigameId))
		{
			m_loadingMetaData = true;
			if (m_minigameSearchParametres == null)
			{
				m_minigameSearchParametres = new MinigameSearchParametres();
			}
			HttpC httpC2 = MiniGame.SearchMinigames(m_minigameSearchParametres, SearchOnlyMetaDataSUCCESS, SearchOnlyMetaDataFAILED, null, 1);
			Debug.LogWarning("------- SEARCH METADATA");
		}
	}

	protected void SearchOnlyMetaDataFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => MiniGame.SearchMinigames((MinigameSearchParametres)_c.objectData, SearchOnlyMetaDataSUCCESS, SearchOnlyMetaDataFAILED, null, 1));
	}

	protected void LoadOnlyMetaDataFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => MiniGame.Get((string)_c.objectData, LoadOnlyMetaDataSUCCESS, LoadOnlyMetaDataFAILED));
	}

	protected virtual void SearchOnlyMetaDataSUCCESS(PsMinigameMetaData[] _minigameMetaData)
	{
		if (_minigameMetaData.Length > 0)
		{
			LoadOnlyMetaDataSUCCESS(_minigameMetaData[0]);
		}
		else
		{
			Debug.LogError("------- SERVER RETURNED EMPTY LEVEL LIST");
		}
	}

	protected virtual void LoadOnlyMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		m_loadingMetaData = false;
		m_minigameMetaData = _minigameMetaData;
		m_minigameId = _minigameMetaData.id;
		m_gameModeFromProgressionData = m_minigameMetaData.gameMode;
		if (m_gameMode == null)
		{
			CreateGameMode();
		}
		m_scoreCurrent = 0;
		m_timeScoreCurrent = int.MaxValue;
		m_timeScoreBest = _minigameMetaData.timeScore;
		if (m_timeScoreBest == 0)
		{
			m_timeScoreBest = int.MaxValue;
		}
		m_timeScoreOld = m_timeScoreBest;
		m_scoreOld = m_scoreBest;
		m_rewardOld = m_scoreBest;
		if (m_path.m_name == "MainPath")
		{
			Hashtable progressionJSON = ClientTools.GenerateProgressionPathJson(this, m_path.m_currentNodeId);
			PsMetagameManager.SaveProgression(progressionJSON, false);
		}
		else
		{
			Hashtable progressionJSON2 = ClientTools.GenerateProgressionPathJson(m_path);
			PsMetagameManager.SaveProgression(progressionJSON2, false);
		}
		if (m_loadMetadataCallback != null)
		{
			Debug.LogWarning("------- CALLING CALLBACKS:");
			Delegate[] invocationList = m_loadMetadataCallback.GetInvocationList();
			Delegate[] array = invocationList;
			foreach (Delegate obj in array)
			{
				Debug.LogWarning(obj.Method.Name);
			}
			m_loadMetadataCallback();
		}
		m_loadMetadataCallback = null;
	}

	public virtual void LoadMinigame()
	{
		if (m_loaderEntity != null)
		{
			EntityManager.RemoveEntity(m_loaderEntity);
		}
		m_loaderEntity = EntityManager.AddEntity();
		if (m_minigameId != string.Empty && m_minigameMetaData == null)
		{
			m_loadingMetaData = true;
			HttpC httpC = MiniGame.Get(m_minigameId, LoadMetaDataSUCCESS, LoadMetaDataFAILED);
			httpC.objectData = m_minigameId;
			EntityManager.AddComponentToEntity(m_loaderEntity, httpC);
			Debug.LogWarning("------- LOAD METADATA");
		}
		else if (string.IsNullOrEmpty(m_minigameId))
		{
			m_loadingMetaData = true;
			if (m_minigameSearchParametres == null)
			{
				m_minigameSearchParametres = new MinigameSearchParametres();
			}
			HttpC httpC2 = MiniGame.SearchMinigames(m_minigameSearchParametres, SearchMetaDataSUCCESS, SearchMetaDataFAILED, null, 1);
			httpC2.objectData = m_minigameSearchParametres;
			EntityManager.AddComponentToEntity(m_loaderEntity, httpC2);
			Debug.LogWarning("------- SEARCH METADATA");
		}
		else if (m_minigameId != string.Empty && m_minigameMetaData != null)
		{
			LoadMinigameBytes();
		}
	}

	protected virtual void SearchMetaDataFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => MiniGame.SearchMinigames((MinigameSearchParametres)_c.objectData, SearchMetaDataSUCCESS, SearchMetaDataFAILED, null, 1));
	}

	protected virtual void LoadMetaDataFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => MiniGame.Get((string)_c.objectData, LoadMetaDataSUCCESS, LoadMetaDataFAILED));
	}

	protected virtual void SearchMetaDataSUCCESS(PsMinigameMetaData[] _minigameMetaData)
	{
		if (_minigameMetaData.Length > 0)
		{
			LoadMetaDataSUCCESS(_minigameMetaData[0]);
		}
		else
		{
			Debug.LogError("------- SERVER RETURNED EMPTY LEVEL LIST");
		}
	}

	protected virtual void LoadMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		m_loadingMetaData = false;
		m_minigameMetaData = _minigameMetaData;
		m_minigameId = _minigameMetaData.id;
		Debug.LogWarning(m_minigameId + " " + m_levelNumber + " " + m_path.m_name);
		if (m_gameMode == null)
		{
			CreateGameMode();
		}
		m_scoreCurrent = 0;
		m_timeScoreCurrent = int.MaxValue;
		m_timeScoreBest = _minigameMetaData.timeScore;
		if (m_timeScoreBest == 0)
		{
			m_timeScoreBest = int.MaxValue;
		}
		m_timeScoreOld = m_timeScoreBest;
		m_scoreOld = m_scoreBest;
		m_rewardOld = m_scoreBest;
		if (m_path.m_name == "MainPath")
		{
			Hashtable progressionJSON = ClientTools.GenerateProgressionPathJson(this, m_path.m_currentNodeId);
			PsMetagameManager.SaveProgression(progressionJSON, false);
		}
		else
		{
			Hashtable progressionJSON2 = ClientTools.GenerateProgressionPathJson(m_path);
			PsMetagameManager.SaveProgression(progressionJSON2, false);
		}
		Debug.LogWarning("------- BEST TIME: " + HighScores.TimeScoreToTime(m_timeScoreBest));
		Debug.LogWarning("------- BEST SCORE: " + m_scoreBest);
		if (m_loadMetadataCallback != null)
		{
			Debug.LogWarning("------- CALLING CALLBACKS:");
			Delegate[] invocationList = m_loadMetadataCallback.GetInvocationList();
			Delegate[] array = invocationList;
			foreach (Delegate obj in array)
			{
				Debug.LogWarning(obj.Method.Name);
			}
			m_loadMetadataCallback();
		}
		m_loadMetadataCallback = null;
	}

	protected virtual void LoadMinigameBytes()
	{
		if (m_minigameBytes == null && !m_loadingMinigameBytes)
		{
			PsMetrics.LevelLoadStart();
			Debug.LogWarning("------- LOAD BYTES " + m_minigameId);
			HttpC levelData = MiniGame.GetLevelData(m_minigameMetaData.id, MinigameDownloadOk, MinigameDownloadFailed);
			if (m_loaderEntity == null)
			{
				m_loaderEntity = EntityManager.AddEntity();
			}
			EntityManager.AddComponentToEntity(m_loaderEntity, levelData);
			m_loadingMinigameBytes = true;
		}
		else if (m_minigameBytes != null)
		{
			MinigameDownloadOk(m_minigameBytes);
		}
		else
		{
			Debug.LogWarning("TRYING TO LOAD MINIGAMEBYTES, BUT NO LUCK");
		}
	}

	protected virtual void MinigameDownloadFailed(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => MiniGame.GetLevelData(m_minigameMetaData.id, MinigameDownloadOk, MinigameDownloadFailed));
	}

	protected virtual void MinigameDownloadOk(byte[] _levelData)
	{
		m_loadingMinigameBytes = false;
		PsMetrics.LevelLoadFinished();
		if (_levelData.Length > 0)
		{
			m_minigameBytes = _levelData;
		}
		if (m_gameMode == null)
		{
			CreateGameMode();
		}
	}

	protected virtual void SaveMinigame(bool _exiting = false)
	{
		Debug.Log(m_autosave);
		if (_exiting && true && m_autosave)
		{
			new PsUIBasePopup(typeof(PsUICenterEditorSave), null, null, null, true, false);
		}
		else if (!PsState.m_activeMinigame.m_changed)
		{
			Debug.LogWarning("level not changed");
			if (!_exiting)
			{
				return;
			}
			new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameExit, 0f, delegate
			{
				if (this is PsGameLoopEditorTemplate)
				{
					(this as PsGameLoopEditorTemplate).ExitAfterEveryplay();
				}
				else
				{
					Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
				}
			});
		}
		else if (m_autosave)
		{
			Debug.LogWarning("level changed, saving");
			PsState.m_activeMinigame.m_changed = false;
			string[] array = new string[PsState.m_activeMinigame.m_itemCount.Keys.Count];
			int num = 0;
			foreach (string key in PsState.m_activeMinigame.m_itemCount.Keys)
			{
				array[num] = key;
				num++;
			}
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
			if (_exiting)
			{
				Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorSaveState());
			}
			else
			{
				SendLevelToServer();
			}
		}
		else if (_exiting)
		{
			if (this is PsGameLoopEditorTemplate)
			{
				(this as PsGameLoopEditorTemplate).ExitAfterEveryplay();
			}
			else
			{
				Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
			}
		}
	}

	protected HttpC SendLevelToServer()
	{
		if (PsState.m_activeGameLoop.m_minigameMetaData.published)
		{
			PsState.m_activeGameLoop.m_minigameMetaData.id = string.Empty;
		}
		PsState.m_activeMinigame.SetLayerItems();
		string creationTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
		if (PsState.m_activeGameLoop.m_minigameMetaData.template ? true : false)
		{
			return Template.Save(PsState.m_activeGameLoop.m_minigameMetaData.id, LevelSendToServerOk, LevelSendToServerFailed);
		}
		return MiniGame.Save(PsState.m_activeGameLoop, null, LevelSendToServerOk, LevelSendToServerFailed, false, creationTime);
	}

	protected void LevelSendToServerOk(PsMinigameMetaData _metaData)
	{
		PsState.m_activeGameLoop.m_minigameMetaData = _metaData;
		PsState.m_activeGameLoop.m_minigameId = _metaData.id;
		PsMetrics.LevelSaved();
		Debug.Log("MINIGAME SAVED");
		Debug.Log("TIME EDITED: " + _metaData.timeSpentEditing);
		PsMetricsData.m_timeInEditor += PsState.m_activeMinigame.GetTimeSinceInit();
		PsCaches.m_savedList.Clear();
	}

	protected void LevelSendToServerFailed(HttpC _c)
	{
		Debug.LogError("MINIGAME SAVE FAILED");
		ServerManager.ThrowServerErrorException("Connection error", _c.www, SendLevelToServer);
	}

	public virtual void CreateGameMode()
	{
		if (m_minigameMetaData.gameMode == PsGameMode.Race)
		{
			m_gameMode = new PsGameModeRace(this);
		}
		else if (m_minigameMetaData.gameMode == PsGameMode.StarCollect)
		{
			m_gameMode = new PsGameModeStarCollect(this);
		}
	}

	public virtual void LoadGhosts()
	{
		Debug.LogWarning("LOAD GHOST");
		m_gameMode.LoadGhosts();
	}

	public virtual void GhostsLoaded()
	{
	}

	public virtual void CreatePlaybackGhosts()
	{
		Debug.LogWarning("CREATE PLAYBACK GHOST VISUALS");
		m_gameMode.CreatePlaybackGhostVisuals();
	}

	public virtual void PreviewLoop()
	{
		Debug.LogError("OVERRIDE!");
	}

	public void StartElapsedTimer()
	{
		m_startTime = DateTime.UtcNow;
		m_elapsedtime = 0L;
	}

	public void PauseElapsedTimer()
	{
		m_elapsedtime = ElapsedTime();
	}

	public void ResumeElapsedTimer()
	{
		m_startTime = DateTime.UtcNow;
	}

	public virtual void EnteredMinigameScene()
	{
		PsMetagameManager.HideResources();
		StartElapsedTimer();
		CreateMinigame();
		CreateEnvironment();
		InitializeMinigame();
		PsMetrics.MinigameStarted(PsState.m_activeGameLoop);
		if (Main.m_currentGame.m_sceneManager.m_loadingScene == null)
		{
			EnterMinigame();
		}
	}

	public virtual void CreateMinigame()
	{
		LevelManager.LoadLevel(FilePacker.UnZipBytes(m_minigameBytes));
		PsState.m_activeMinigame = LevelManager.m_currentLevel as Minigame;
		if (LevelManager.m_currentLevel != null)
		{
			PsState.m_activeMinigame.m_groundNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("LevelGround") as LevelGroundNode;
			m_gameMode.ApplySettings();
		}
		else
		{
			Debug.LogError("ERROR LOADING LEVEL FROM BYTES!!");
		}
	}

	public virtual void CreateEnvironment()
	{
		m_gameMode.CreateEnvironment();
	}

	public virtual void EnterEditor()
	{
	}

	public virtual void InitializeEditor()
	{
	}

	public virtual void ExitEditor(bool _published = false)
	{
	}

	public virtual void EnterMinigame()
	{
		Debug.LogError("OVERRIDE!");
	}

	public virtual void StartEditorRecording()
	{
	}

	public virtual void InitializeMinigame()
	{
		PsState.m_physicsPaused = true;
		PsState.m_activeMinigame.m_gameOn = false;
		PsState.m_activeMinigame.m_gameTicks = 0f;
		PsState.m_activeMinigame.m_realTimeSpent = 0f;
		PsState.m_activeMinigame.m_gameEnded = false;
		PsState.m_activeMinigame.m_gameStarted = false;
		PsState.m_activeMinigame.m_gamePaused = false;
		PsState.m_activeMinigame.m_gameTicksFreezed = false;
		PsState.m_activeMinigame.m_playerBeamingOut = false;
		PsState.m_activeMinigame.m_collectedStars = 0;
		CameraS.SnapMainCameraFrame();
	}

	public virtual void SetPlaybackGhostAndCoins()
	{
		m_gameMode.CreatePlaybackGhostVisuals();
		if (m_gameMode.m_playbackGhosts.Count > 0)
		{
			PsMetagameManager.UpdateCoinValueBooster();
			m_gameMode.GenerateCollectableCoins();
			m_gameMode.PlaceCoins();
			Debug.LogWarning("PLACE COINS");
		}
	}

	public virtual void StartMinigame()
	{
		Debug.LogError("OVERRIDE!");
	}

	public virtual void PauseMinigame()
	{
		Debug.LogError("OVERRIDE!");
	}

	public virtual void ResumeMinigame()
	{
		Debug.LogError("OVERRIDE!");
	}

	public virtual void RestartMinigame()
	{
		m_sendQuit = true;
		m_boosterUsed = false;
		PsState.m_activeMinigame.RemoveTimeScale();
	}

	public virtual void LoseMinigame()
	{
		Debug.LogError("OVERRIDE!");
	}

	public virtual void WinMinigame()
	{
		if ((m_timeScoreBest == 0 || m_timeScoreBest == int.MaxValue) && m_scoreOld == 0)
		{
			m_winFirtTime = true;
		}
		m_sendQuit = false;
		PsState.m_activeMinigame.RemoveTimeScale();
	}

	public virtual void SaveProgression(bool _save = true)
	{
		Debug.LogError("OVERRIDE!");
	}

	public virtual void ExitMinigame()
	{
		m_gameMode.CheckAchievementsOnEnd();
		m_loadingMinigameBytes = false;
		m_loadingMetaData = false;
		Exit();
	}

	private void Exit()
	{
		PsState.m_activeMinigame.RemoveTimeScale();
	}

	public virtual void SkipMinigame()
	{
		m_winFirtTime = true;
		PsMetrics.MinigameSkipped(PsState.m_activeGameLoop.m_context.ToString(), PsState.m_activeGameLoop.m_minigameMetaData, PsState.m_activeGameLoop.ElapsedTime());
	}

	public virtual void DestroyMinigame()
	{
		GhostManager.Free();
	}

	public virtual void BackAtMenu()
	{
		Debug.LogError("OVERRIDE!");
	}

	public void IncreaseItemLevel(string _unlockIdentifier)
	{
		if (string.IsNullOrEmpty(_unlockIdentifier))
		{
			return;
		}
		PsUnlock unlockByIdentifier = PsMetagameData.GetUnlockByIdentifier(_unlockIdentifier);
		if (unlockByIdentifier == null)
		{
			return;
		}
		PsMetagameData.Unlock(unlockByIdentifier);
		int num = 0;
		m_unlockableList = new List<PsUnlockable>();
		if (unlockByIdentifier.m_unlockables != null)
		{
			for (int i = 0; i < unlockByIdentifier.m_unlockables.Count; i++)
			{
				Debug.LogWarning(unlockByIdentifier.m_unlockables[i].m_name + ", ITEM LEVEL: " + unlockByIdentifier.m_unlockables[i].m_itemLevel);
				if (unlockByIdentifier.m_unlockables[i].m_itemLevel > num)
				{
					num = unlockByIdentifier.m_unlockables[i].m_itemLevel;
				}
				if (unlockByIdentifier.m_unlockables[i].m_container.m_type != PsUnlockableType.Menu)
				{
					Debug.LogWarning("TO UNLOCK SCREEN: " + unlockByIdentifier.m_unlockables[i].m_name);
					m_unlockableList.Add(unlockByIdentifier.m_unlockables[i]);
				}
			}
		}
		Debug.LogWarning("ITEM LEVEL: " + num + ", OLD ITEM LEVEL: " + PsMetagameManager.m_playerStats.itemLevel);
		if (num > PsMetagameManager.m_playerStats.itemLevel)
		{
			PsMetagameManager.m_playerStats.itemLevel = num;
			Debug.LogWarning("SETTING ITEM LEVEL TO: " + num);
		}
		if (m_unlockableList.Count > 0)
		{
			PsMenuScene.m_lastState = "PsResearchUnlockState";
		}
	}

	public void CleanLoop()
	{
		m_released = true;
		if (m_gameMode != null)
		{
			m_gameMode.Destroy();
			m_gameMode = null;
		}
		m_minigameBytes = null;
	}

	public virtual void ReleaseLoop()
	{
		Debug.LogWarning("LOOP RELEASED");
		CleanLoop();
	}

	public virtual void SetAsActiveLoop()
	{
		if (PsState.m_activeGameLoop != null)
		{
			Debug.LogError(string.Concat("trying to set ", this, " as active loop, but active loop is already set: ", PsState.m_activeGameLoop));
		}
		Debug.LogWarning(string.Concat(this, ": is PsState.m_activeLoop"));
		PsState.m_activeGameLoop = this;
		PsState.m_activeGameLoop.m_released = false;
		m_userHasSelectedVehicle = false;
		m_selectedVehicle = PsSelectedVehicle.None;
	}

	public virtual void SetActiveLoopNull()
	{
		if (PsState.m_activeGameLoop == null)
		{
			Debug.LogError("trying to set active loop to null, but it already is null");
		}
		Debug.LogWarning(string.Concat(this, ": setting PsState.m_activeLoop to null"));
		PsState.m_activeGameLoop = null;
	}

	public virtual void ExitLoadingScene()
	{
		m_loadingMetaData = false;
		m_loadMetadataCallback = null;
		Main.m_currentGame.m_sceneManager.StopSceneChange();
		EntityManager.RemoveEntity(m_loaderEntity);
		m_loadingMinigameBytes = false;
		m_loaderEntity = null;
		ReleaseLoop();
	}

	public virtual void ActivateCurrentLoop()
	{
		PsGameLoop currentNodeInfo = m_path.GetCurrentNodeInfo();
		if (currentNodeInfo != null && currentNodeInfo.m_node != null)
		{
			currentNodeInfo.m_node.Activate();
		}
		if (m_sidePath != null)
		{
			PsGameLoop currentNodeInfo2 = m_sidePath.GetCurrentNodeInfo();
			if (currentNodeInfo2 != null && currentNodeInfo2.m_node != null)
			{
				currentNodeInfo2.m_node.Activate();
			}
		}
		if (m_winFirtTime && currentNodeInfo != null && !currentNodeInfo.m_activated)
		{
			currentNodeInfo.SaveAsActivated();
			new PsMenuDialogueFlow(currentNodeInfo, PsNodeEventTrigger.LoopActivated, 0.1f);
		}
	}

	public virtual void SaveAsActivated()
	{
		if (!m_activated)
		{
			m_activated = true;
		}
	}

	public virtual int GetLevelRequirement()
	{
		return m_minigameMetaData.levelRequirement;
	}

	public virtual string GetCreatorName()
	{
		return m_minigameMetaData.creatorName;
	}

	public virtual string GetCreatorId()
	{
		return m_minigameMetaData.creatorId;
	}

	public virtual string GetName()
	{
		return m_minigameMetaData.name;
	}

	public virtual string GetFacebookId()
	{
		return m_minigameMetaData.creatorFacebookId;
	}

	public virtual string GetGamecenterId()
	{
		return m_minigameMetaData.creatorGameCenterId;
	}

	public virtual string GetPlayerUnit()
	{
		return m_minigameMetaData.playerUnit;
	}

	public virtual PsRating GetRating()
	{
		return m_minigameMetaData.rating;
	}

	public virtual PsGameDifficulty GetDifficulty()
	{
		return m_minigameMetaData.difficulty;
	}

	public virtual string GetGameId()
	{
		if (string.IsNullOrEmpty(m_minigameMetaData.id))
		{
			Debug.LogError("Minigame ID should not be null!");
		}
		return m_minigameMetaData.id;
	}

	public virtual string GetTournamentId()
	{
		return null;
	}

	public virtual string GetDescription()
	{
		return m_minigameMetaData.description;
	}

	public virtual void SetRating(PsRating _newRating)
	{
		m_minigameMetaData.rating = _newRating;
	}

	public virtual int GetTime()
	{
		return m_minigameMetaData.timeScore;
	}

	public virtual PsGameMode GetGameMode()
	{
		return m_minigameMetaData.gameMode;
	}

	public virtual bool IsGameMode(PsGameMode _gameMode)
	{
		if (m_minigameMetaData.gameMode == _gameMode)
		{
			return true;
		}
		return false;
	}

	public virtual int GetScore()
	{
		return m_scoreBest;
	}

	protected void StopEveryplayAfterDelay()
	{
		EveryplayManager.StopRecording(10f);
		Everyplay.SetMetadata("minigamename", m_minigameMetaData.name);
		Everyplay.SetMetadata("username", PlayerPrefsX.GetUserName());
	}

	public long ElapsedTime()
	{
		return m_elapsedtime + Convert.ToInt64((DateTime.UtcNow - m_startTime).TotalSeconds);
	}

	public void SwitchToVersusChallenge(VersusMetaData _versusData)
	{
		PsGameLoopFriendly psGameLoopFriendly = new PsGameLoopFriendly(PsMinigameContext.Friendly, _versusData, _versusData.timedEventId, _versusData.gameId, null, -1, -1, m_scoreBest, _versusData.timeLeft, false);
		psGameLoopFriendly.SetPreviousLoop(this);
		psGameLoopFriendly.m_minigameBytes = m_minigameBytes;
		psGameLoopFriendly.m_minigameMetaData = m_minigameMetaData;
		Exit();
		EveryplayManager.StopRecording(0f);
		m_gameMode.m_exitingMinigame = true;
		m_gameMode.m_waitForNextGhost = false;
		PsState.m_activeGameLoop = null;
		psGameLoopFriendly.StartFriendlyLoop();
	}

	public virtual Hashtable GetRentUpgradeValues()
	{
		return m_minigameMetaData.creatorUpgrades;
	}

	public virtual Type GetRentVehicleType()
	{
		return Type.GetType(m_minigameMetaData.playerUnit);
	}

	public virtual List<int> GetRentVehicleValues()
	{
		return PsState.m_activeMinigame.m_playerUnit.ParseUpgradeValues(PsState.m_activeGameLoop.GetRentUpgradeValues());
	}
}
