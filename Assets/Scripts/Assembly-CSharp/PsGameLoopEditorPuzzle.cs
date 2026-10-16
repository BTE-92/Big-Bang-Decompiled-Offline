using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsGameLoopEditorPuzzle : PsGameLoop
{
	private bool m_existingMinigame;

	public List<string> m_limitedItems;

	private PsUIBasePopup m_everyplayPopup;

	public PsGameLoopEditorPuzzle(List<string> _limitedItems, string _minigameId, PsPlanetPath _path, int _id, int _levelNumber, int _score, bool _unlocked)
		: base(PsMinigameContext.EditorPuzzle, _minigameId, _path, _id, _levelNumber, _score, _unlocked)
	{
		m_limitedItems = _limitedItems;
		if (m_limitedItems == null)
		{
			m_limitedItems = new List<string>();
		}
		if (m_minigameMetaData != null)
		{
			m_existingMinigame = true;
		}
	}

	public override void StartLoop()
	{
		m_winFirtTime = false;
		if (PsState.m_activeGameLoop == null && m_nodeId <= m_path.m_currentNodeId)
		{
			LoopUnlocked();
		}
	}

	public override void LoopUnlocked()
	{
		PsMetagameManager.ShowResources(null, false, true, true, true, true, 0.03f, true);
		if (m_nodeId == m_path.m_currentNodeId && !m_unlocked)
		{
			PsMetagameManager.ResetUnlockerTime();
			m_unlocked = true;
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
		}
		if (m_nodeId <= m_path.m_currentNodeId && !Input.GetKey(KeyCode.LeftShift) && (!PsState.m_adminMode || !PsState.m_adminModeSkipping))
		{
			m_unlocked = true;
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopStart, 0f, delegate
			{
				SetAsActiveLoop();
				if (m_nodeId < m_path.m_currentNodeId)
				{
					LoadMinigame();
				}
				else
				{
					if (m_node != null)
					{
						m_node.m_hideUI = true;
					}
					TimerS.AddComponent(EntityManager.AddEntity(), "Timer", 0.3f, 0f, true, FadeInLoadingScreen);
				}
			});
		}
		else if (Input.GetKey(KeyCode.LeftShift) || (PsState.m_adminMode && PsState.m_adminModeSkipping))
		{
			m_unlocked = true;
			m_backAtMenu = false;
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopStart, 0f, delegate
			{
				SetAsActiveLoop();
				SaveProgression();
				BackAtMenu();
			});
		}
	}

	public void FadeInLoadingScreen(TimerC _c)
	{
		LoadMinigame();
	}

	public override void LoadMinigame()
	{
		if (m_loadingMetaData)
		{
			Debug.LogWarning("------ ALREADY LOADING MINIGAME META DATA. ADDING TO CALLBACK");
			if (m_loadMetadataCallback != null)
			{
				m_loadMetadataCallback = (Action)Delegate.Combine(m_loadMetadataCallback, new Action(LoadMinigame));
			}
			else
			{
				m_loadMetadataCallback = LoadMinigame;
			}
		}
		else
		{
			base.LoadMinigame();
			PreviewLoop();
		}
	}

	protected override void LoadMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		base.LoadMetaDataSUCCESS(_minigameMetaData);
		m_minigameMetaData.gameMode = PsGameMode.StarCollect;
		m_rentedVehicle = true;
		LoadMinigameBytes();
	}

	public override void PreviewLoop()
	{
		Main.m_currentGame.m_sceneManager.ChangeScene(new EditorScene("EditorScene"), new PsLevelLoadingScene());
	}

	protected override void MinigameDownloadOk(byte[] _levelData)
	{
		base.MinigameDownloadOk(_levelData);
		if (Main.m_currentGame.m_sceneManager.m_loadingScene is PsLevelLoadingScene)
		{
			(Main.m_currentGame.m_sceneManager.m_loadingScene as PsLevelLoadingScene).LevelLoaded();
		}
	}

	public override void CreateMinigame()
	{
		Debug.LogWarning("GENERATE MINIGAME");
		bool flag = false;
		if (m_minigameBytes != null)
		{
			LevelManager.LoadLevel(FilePacker.UnZipBytes(m_minigameBytes), false);
			PsState.m_activeMinigame = LevelManager.m_currentLevel as Minigame;
			PsState.m_activeMinigame.m_editing = true;
			LevelManager.AssembleCurrentLevel();
			if (LevelManager.m_currentLevel != null)
			{
				PsState.m_activeMinigame.m_groundNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("LevelGround") as LevelGroundNode;
			}
			else
			{
				Debug.LogError("ERROR LOADING LEVEL FROM BYTES!!");
			}
			flag = true;
			PsState.m_editorCameraZoom = 1500f;
		}
		else
		{
			Debug.LogError("no minigame bytes");
		}
		GraphElement[] elements = LevelManager.m_currentLevel.m_currentLayer.GetElements("PlayerSpawnPoint");
		GraphElement[] elements2 = LevelManager.m_currentLevel.m_currentLayer.GetElements("StarSpawnPoint");
		GraphElement[] elements3 = LevelManager.m_currentLevel.m_currentLayer.GetElements("GoalSpawnPoint");
		if (elements.Length > 0)
		{
			int num = UnityEngine.Random.Range(0, elements.Length - 1);
			Vector3 position = elements[num].m_position;
			PsState.m_activeMinigame.m_playerNode.m_position = position;
			TransformS.SetPosition(PsState.m_activeMinigame.m_playerNode.m_TC, position);
			LevelPlayerNode playerNode = PsState.m_activeMinigame.m_playerNode;
			m_rentedVehicle = true;
			m_userHasSelectedVehicle = true;
			m_selectedVehicle = PsSelectedVehicle.CreatorVehicle;
			Vehicle vehicle = Activator.CreateInstance(Type.GetType(m_minigameMetaData.playerUnit), new string[0]) as Vehicle;
			PsState.m_activeGameLoop.m_rentUpgradeValues = vehicle.ParseUpgradeValues(m_minigameMetaData.creatorUpgrades);
			vehicle.Destroy();
			PsState.m_activeMinigame.m_playerNode = playerNode;
			PsState.m_activeMinigame.m_playerNode.Reset();
		}
		List<GraphElement> list = new List<GraphElement>();
		GraphElement[] elements4 = LevelManager.m_currentLevel.m_currentLayer.GetElements("CollectibleStar");
		if (elements4.Length == 0)
		{
			PsState.m_activeMinigame.CreateStars();
			elements4 = LevelManager.m_currentLevel.m_currentLayer.GetElements("CollectibleStar");
		}
		if (elements2.Length >= 3)
		{
			List<int> list2 = new List<int>();
			for (int i = 0; i < 3; i++)
			{
				int num2 = UnityEngine.Random.Range(0, elements2.Length);
				while (list2.Contains(num2))
				{
					num2 = UnityEngine.Random.Range(0, elements2.Length);
				}
				list2.Add(num2);
				Vector3 position2 = elements2[num2].m_position;
				elements4[i].m_position = position2;
				TransformS.SetPosition(elements4[i].m_TC, position2);
				elements4[i].Reset();
			}
		}
		list.AddRange(elements4);
		if (elements3.Length > 0)
		{
			int num3 = UnityEngine.Random.Range(0, elements3.Length - 1);
			Vector3 position3 = elements3[num3].m_position;
			PsState.m_activeMinigame.m_goalNode.m_position = position3;
			TransformS.SetPosition(PsState.m_activeMinigame.m_goalNode.m_TC, position3);
			PsState.m_activeMinigame.m_goalNode.Reset();
		}
		List<GraphElement> childElements = LevelManager.m_currentLevel.m_currentLayer.m_childElements;
		for (int num4 = childElements.Count - 1; num4 > -1; num4--)
		{
			if (childElements[num4].m_name == "PlayerSpawnPoint" || childElements[num4].m_name == "GoalSpawnPoint" || childElements[num4].m_name == "StarSpawnPoint")
			{
				childElements[num4].Clear(false);
				LevelManager.m_currentLevel.m_currentLayer.RemoveElement(childElements[num4]);
			}
			else
			{
				if (childElements[num4] == PsState.m_activeMinigame.m_playerNode || childElements[num4] == PsState.m_activeMinigame.m_goalNode || list.Contains(childElements[num4]))
				{
					childElements[num4].m_isSelectable = false;
					childElements[num4].m_isMoveable = false;
					childElements[num4].m_isRotateable = false;
				}
				childElements[num4].m_isRemovable = false;
				childElements[num4].m_isCopyable = false;
				if (childElements[num4].m_name == "Teleport")
				{
					List<GraphElement> childElements2 = (childElements[num4] as GraphNode).m_childElements;
					for (int j = 0; j < childElements2.Count; j++)
					{
						childElements2[j].m_isRemovable = false;
						childElements2[j].m_isCopyable = false;
					}
				}
			}
		}
		m_scoreCurrent = 0;
		m_timeScoreBest = m_minigameMetaData.timeScore;
		m_timeScoreCurrent = 0;
		CreateGameMode();
		m_gameMode.ApplySettings();
		if (flag)
		{
			AutoGeometryManager.UpdateMaxValueLookupTable(AutoGeometryManager.m_layers[PsState.m_drawLayer], LookupUpdateMode.LEVEL_LOADED, true);
		}
	}

	public override void EnteredMinigameScene()
	{
		StartElapsedTimer();
		CreateMinigame();
		CreateEnvironment();
		EnterEditor();
		InitializeEditor();
		if (!PsState.m_muteMusic)
		{
			Minigame minigame = LevelManager.m_currentLevel as Minigame;
			minigame.m_music = SoundS.AddComponent(minigame.m_environmentTC, "/Music/EditorMusic", 1f, true);
			SoundS.PlaySound(minigame.m_music, true);
		}
	}

	public override void CreateEnvironment()
	{
		if (PsState.m_activeMinigame.m_backgroundPrefab != null)
		{
			PrefabS.RemoveComponent(PsState.m_activeMinigame.m_backgroundPrefab);
			PsState.m_activeMinigame.m_backgroundPrefab = null;
		}
		int num = (int)PsState.m_activeMinigame.m_settings["domeSizeIndex"];
		GameObject gameObject = null;
		switch (num)
		{
		case 0:
			gameObject = ResourceManager.GetGameObject(RESOURCE.Map_Play_Small_GameObject);
			break;
		case 1:
			gameObject = ResourceManager.GetGameObject(RESOURCE.Map_Play_Medium_GameObject);
			break;
		case 2:
			gameObject = ResourceManager.GetGameObject(RESOURCE.Map_Play_Large_GameObject);
			break;
		}
		PsState.m_activeMinigame.m_backgroundPrefab = PrefabS.AddComponent(PsState.m_activeMinigame.m_environmentTC, new Vector3(0f, 30f, 70f), gameObject);
		GameObject gameObject2 = PsState.m_activeMinigame.m_backgroundPrefab.p_gameObject.transform.FindChild("Static").gameObject;
		if ((bool)gameObject2)
		{
			StaticBatchingUtility.Combine(gameObject2);
		}
	}

	public override void CreateGameMode()
	{
		m_gameMode = new PsGameModeEditorPuzzle(this);
	}

	public override void StartEditorRecording()
	{
		EveryplayManager.StartRecording();
	}

	public override void EnterEditor()
	{
		Debug.LogWarning("ENTER EDITOR");
		PsMetrics.PlayerEnteredEditor();
		PsGameLoop nodeInfo = m_path.GetNodeInfo(m_nodeId + 1);
		if (nodeInfo != null && nodeInfo.m_minigameMetaData == null)
		{
			nodeInfo.LoadMinigameMetaData();
		}
		PsState.m_activeMinigame.m_editing = true;
		PsState.m_physicsPaused = true;
		CameraS.m_updateComponents = true;
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		PsMetagameManager.HideResources();
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameEnter, 0.5f, delegate
		{
			m_gameMode.CreateEditMenu(delegate
			{
				ExitEditor(false);
			}, EnterMinigame);
		});
	}

	public override void InitializeEditor()
	{
		PsState.m_physicsPaused = true;
	}

	public override void EnterMinigame()
	{
		m_backAtMenu = false;
		Debug.LogWarning("ENTER MINIGAME");
		PsMetagameManager.HideResources();
		GizmoManager.ClearSelection();
		GizmoManager.Update();
		CreateGameMode();
		PsState.m_activeMinigame.m_editing = false;
		PsState.m_physicsPaused = true;
		Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorTestState());
		(LevelManager.m_currentLevel as Minigame).m_groundNode.SaveGroundBeforePlay();
		LevelManager.ResetCurrentLevel();
		InitializeMinigame();
		m_gameMode.CreatePlayMenu(RestartMinigame, ExitMinigame);
	}

	public override void InitializeMinigame()
	{
		base.InitializeMinigame();
	}

	public override void StartMinigame()
	{
		SoundS.PlaySingleShot("/InGame/GameStart", Vector3.zero);
		UnityEngine.Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		PsState.m_physicsPaused = false;
		PsState.m_activeMinigame.m_gameStarted = true;
		PsState.m_activeMinigame.m_gameStartCount++;
		PsState.m_activeMinigame.m_playerReachedGoal = false;
		PsIngameMenu.OpenController();
		EveryplayManager.SetPaused(false);
	}

	public override void PauseMinigame()
	{
		if (!PsState.m_activeMinigame.m_gamePaused && PsState.m_activeMinigame.m_gameStarted && !PsState.m_activeMinigame.m_gameEnded)
		{
			PsState.m_activeMinigame.SetGamePaused(true);
			PauseElapsedTimer();
		}
	}

	public override void ResumeMinigame()
	{
		PsState.m_activeMinigame.SetGamePaused(false);
		ResumeElapsedTimer();
	}

	public override void LoseMinigame()
	{
		m_gameMode.StopRecordingGhost();
		PsIngameMenu.CloseAll();
		PsState.m_activeMinigame.m_gameEnded = true;
		PsState.m_activeMinigame.m_gameOn = false;
		PsState.m_activeMinigame.m_gameDeathCount++;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameLose, 1f, delegate
		{
			m_gameMode.LoseMinigame();
		});
	}

	public override void WinMinigame()
	{
		Debug.LogWarning("WIN MINIGAME");
		base.WinMinigame();
		m_gameMode.StopRecordingGhost();
		PsIngameMenu.CloseAll();
		PsState.m_activeMinigame.m_gameEnded = true;
		PsState.m_activeMinigame.m_playerReachedGoal = true;
		if (GetGameMode() == PsGameMode.StarCollect)
		{
			m_scoreCurrent = PsState.m_activeMinigame.m_collectedStars;
		}
		m_gameMode.m_exitingMinigame = false;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 1f, delegate
		{
			m_gameMode.WinMinigame();
			SaveProgression();
		});
	}

	public override void SaveProgression(bool _save = true)
	{
		bool flag = false;
		bool sendMetrics = false;
		if (m_nodeId == m_path.m_currentNodeId)
		{
			m_path.m_currentNodeId = m_nodeId + 1;
			flag = true;
			sendMetrics = true;
		}
		if (m_sidePath != null && m_sidePath.m_currentNodeId < 1)
		{
			m_sidePath.m_currentNodeId = 1;
		}
		if (m_scoreBest > m_scoreOld)
		{
			flag = true;
		}
		if (flag)
		{
			if (m_path.m_name == "MainPath")
			{
				Hashtable progressionJSON = ClientTools.GenerateProgressionPathJson(this, m_path.m_currentNodeId);
				PsMetagameManager.SaveProgression(progressionJSON, sendMetrics);
			}
			else
			{
				Hashtable progressionJSON2 = ClientTools.GenerateProgressionPathJson(m_path);
				PsMetagameManager.SaveProgression(progressionJSON2, false);
			}
		}
	}

	public override void RestartMinigame()
	{
		base.RestartMinigame();
		EditorScene.ClearScreenshots();
		PsMetagameManager.HideResources();
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		CameraS.RemoveAllTargetComponents();
		PsState.m_activeMinigame.m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
		InitializeMinigame();
		PsIngameMenu.CloseAll();
		m_gameMode.CreatePlayMenu(RestartMinigame, ExitMinigame);
		PsIngameMenu.OpenController();
	}

	public override void ExitMinigame()
	{
		Debug.LogWarning("EXIT MINIGAME");
		base.ExitMinigame();
		PsIngameMenu.CloseAll();
		PsState.m_activeMinigame.m_editing = true;
		PsState.m_activeMinigame.m_gameStarted = false;
		PsState.m_physicsPaused = true;
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		PsState.m_activeMinigame.m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		m_gameMode.CreateEditMenu(delegate
		{
			ExitEditor(false);
		}, EnterMinigame);
	}

	public override void ExitEditor(bool _published)
	{
		Debug.LogWarning("EXIT EDITOR");
		GizmoManager.ClearSelection();
		GizmoManager.Update();
		PsMetrics.MinigameExited(PsState.m_activeGameLoop.m_context.ToString(), PsState.m_activeGameLoop.m_minigameMetaData, PsState.m_activeGameLoop.ElapsedTime());
		PsState.m_activeMinigame.RemoveTimeScale();
		EveryplayManager.StopRecording(0f);
		Everyplay.SetMetadata("minigamename", m_minigameMetaData.name);
		Everyplay.SetMetadata("username", PlayerPrefsX.GetUserName());
		m_gameMode.m_exitingMinigame = true;
		m_gameMode.m_waitForNextGhost = false;
		if (EveryplayManager.IsSupported() && EveryplayManager.IsEnabled())
		{
			m_everyplayPopup = new PsUIBasePopup(typeof(PsUICenterEveryplayConfirmation));
			m_everyplayPopup.SetAction("Proceed", ShowEveryplay);
			m_everyplayPopup.SetAction("Exit", ContinueExit);
		}
		else
		{
			ContinueExit();
		}
	}

	public void ShowEveryplay()
	{
		EveryplayManager.StopRecording(0f);
		EveryplayManager.PlayLastRecording();
		Everyplay.WasClosed += ContinueExit;
	}

	public void ContinueExit()
	{
		if (m_everyplayPopup != null)
		{
			m_everyplayPopup.Destroy();
		}
		m_everyplayPopup = null;
		PsIngameMenu.CloseAll();
		Everyplay.WasClosed -= ContinueExit;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameExit, 0f, delegate
		{
			if (PsState.m_activeMinigame.m_gameStartCount > 0 && m_sendQuit)
			{
				PsMetagameManager.SendQuit(PsState.m_activeGameLoop);
				PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
			}
			PsMetagameManager.HideResources();
			Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
		});
	}

	public override void DestroyMinigame()
	{
		base.DestroyMinigame();
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		LevelManager.DestroyCurrentLevel();
		PsState.m_activeMinigame = null;
		PsState.m_activeGameLoop.m_minigameMetaData = null;
		PsState.m_activeGameLoop.m_minigameBytes = null;
		PsIngameMenu.CloseAll();
	}

	public override void BackAtMenu()
	{
		if (!m_backAtMenu && !m_released)
		{
			m_backAtMenu = true;
			PsGameLoop currentNodeInfo = m_path.GetCurrentNodeInfo();
			if (currentNodeInfo != null && currentNodeInfo.m_node != null && !m_winFirtTime)
			{
				currentNodeInfo.m_node.SetActive();
			}
			if (m_sidePath != null)
			{
				PsGameLoop currentNodeInfo2 = m_sidePath.GetCurrentNodeInfo();
				if (currentNodeInfo2 != null && currentNodeInfo2.m_node != null && !m_winFirtTime)
				{
					currentNodeInfo2.m_node.SetActive();
				}
			}
			if (m_node != null && m_scoreBest != m_scoreOld)
			{
				m_node.GiveStars(StarsGiven);
			}
			else
			{
				ReleaseLoop();
			}
		}
		m_backAtMenu = false;
	}

	private void StarsGiven(TimerC _c)
	{
		ReleaseLoop();
	}

	public override void ReleaseLoop()
	{
		if (m_released)
		{
			return;
		}
		base.ReleaseLoop();
		m_scoreOld = m_scoreBest;
		m_rewardOld = m_scoreBest;
		if (m_nodeId != m_path.m_currentNodeId)
		{
			if (m_node != null)
			{
				m_node.RemoveHighlight();
			}
			if (m_nodeId + 1 == m_path.m_currentNodeId)
			{
				new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopReleased, 0f, delegate
				{
					LoopReleased();
				});
			}
			else
			{
				LoopReleased();
			}
		}
		else
		{
			SetActiveLoopNull();
		}
	}

	public void LoopReleased()
	{
		SetActiveLoopNull();
		if (m_winFirtTime)
		{
			ActivateCurrentLoop();
			m_winFirtTime = false;
		}
	}
}
