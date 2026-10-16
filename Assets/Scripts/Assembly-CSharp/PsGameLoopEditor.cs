using Server;
using UnityEngine;

public class PsGameLoopEditor : PsGameLoop
{
	private bool m_existingMinigame;

	public PsGameLoopEditor(string _minigameId, PsMinigameMetaData _minigameMetaData, bool _autosave = true)
		: base(PsMinigameContext.Editor, _minigameId, null, -1, -1, -1, false)
	{
		m_minigameMetaData = _minigameMetaData;
		if (m_minigameMetaData != null)
		{
			m_existingMinigame = true;
		}
		m_autosave = _autosave;
		Debug.LogWarning("AUTOSAVE: " + m_autosave);
	}

	public override void StartLoop()
	{
		if (PsState.m_activeGameLoop == null)
		{
			SetAsActiveLoop();
			Debug.LogWarning("AUTOSAVE: " + m_autosave);
			PsLoadingScreen.FadeIn(delegate
			{
				LoadMinigame();
			});
		}
	}

	public override void LoadMinigame()
	{
		if (m_loaderEntity != null)
		{
			EntityManager.RemoveEntity(m_loaderEntity);
		}
		m_loaderEntity = EntityManager.AddEntity();
		if (m_minigameBytes != null)
		{
			MinigameDownloadOk(m_minigameBytes);
		}
		else if (m_minigameMetaData != null)
		{
			LoadMinigameBytes();
		}
		else if (!string.IsNullOrEmpty(m_minigameId))
		{
			m_loadingMetaData = true;
			HttpC httpC = MiniGame.Get(m_minigameId, LoadMetaDataSUCCESS, LoadMetaDataFAILED);
			httpC.objectData = m_minigameId;
		}
		else
		{
			Main.m_currentGame.m_sceneManager.ChangeScene(new EditorScene("EditorScene"));
		}
	}

	protected override void LoadMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		m_loadingMetaData = false;
		m_minigameMetaData = _minigameMetaData;
		m_minigameId = _minigameMetaData.id;
		LoadMinigameBytes();
	}

	protected override void MinigameDownloadOk(byte[] _levelData)
	{
		if (_levelData.Length > 0)
		{
			m_minigameBytes = _levelData;
		}
		Main.m_currentGame.m_sceneManager.ChangeScene(new EditorScene("EditorScene"));
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
			if (LevelManager.m_currentLevel != null)
			{
				PsState.m_activeMinigame.m_groundNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("LevelGround") as LevelGroundNode;
			}
			else
			{
				Debug.LogError("ERROR LOADING LEVEL FROM BYTES!!");
			}
			LevelManager.AssembleCurrentLevel();
			flag = true;
			CreateGameMode();
			m_gameMode.ApplySettings();
		}
		else
		{
			m_minigameMetaData = new PsMinigameMetaData();
			PsState.m_activeMinigame = DefaultMinigame.Assemble();
			PsState.m_activeMinigame.GenerateDefaultNameAndDescription();
			PsState.m_activeMinigame.m_editing = true;
			DefaultMinigame.ApplyWizard(PsState.m_activeMinigame);
			PsState.m_activeMinigame.m_groundNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("LevelGround") as LevelGroundNode;
			flag = true;
			CreateGameMode();
			m_gameMode.ApplySettings();
			CameraS.SnapMainCameraPos(Vector2.zero);
		}
		m_scoreCurrent = 0;
		m_timeScoreBest = m_minigameMetaData.timeScore;
		m_timeScoreCurrent = 0;
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
		if (m_minigameMetaData.gameMode == PsGameMode.Race)
		{
			m_gameMode = new PsGameModeRaceEditor(this);
		}
		else if (m_minigameMetaData.gameMode == PsGameMode.StarCollect)
		{
			m_gameMode = new PsGameModeStarCollectEditor(this);
		}
	}

	public override void EnterEditor()
	{
		Debug.LogWarning("ENTER EDITOR");
		PsMetrics.PlayerEnteredEditor();
		PsState.m_activeMinigame.m_editing = true;
		PsState.m_physicsPaused = true;
		CameraS.m_updateComponents = true;
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		m_gameMode.CreateEditMenu(delegate
		{
			ExitEditor();
		}, EnterMinigame);
		if (PsLoadingScreen.m_active)
		{
			PsLoadingScreen.FadeOut();
		}
	}

	public override void InitializeEditor()
	{
		PsState.m_physicsPaused = true;
	}

	public override void ExitEditor(bool _published = false)
	{
		PsIngameMenu.CloseAll();
		GizmoManager.ClearSelection();
		GizmoManager.Update();
		if (!_published)
		{
			SaveMinigame(true);
			return;
		}
		PsMetagameManager.HideResources();
		Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
	}

	public override void EnterMinigame()
	{
		m_backAtMenu = false;
		Debug.LogWarning("ENTER MINIGAME");
		GizmoManager.ClearSelection();
		GizmoManager.Update();
		CreateGameMode();
		if (m_gameMode.GetType() == typeof(PsGameModeRaceEditor))
		{
			(m_gameMode as PsGameModeRaceEditor).CalculateMedalTimes();
		}
		PsState.m_activeMinigame.m_editing = false;
		PsState.m_physicsPaused = true;
		Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorTestState());
		(LevelManager.m_currentLevel as Minigame).m_groundNode.SaveGroundBeforePlay();
		LevelManager.ResetCurrentLevel();
		InitializeMinigame();
		m_gameMode.CreatePlayMenu(RestartMinigame, ExitMinigame);
		SaveMinigame();
	}

	public override void InitializeMinigame()
	{
		base.InitializeMinigame();
	}

	public override void StartMinigame()
	{
		SoundS.PlaySingleShot("/InGame/GameStart", Vector3.zero);
		Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		PsState.m_physicsPaused = false;
		PsState.m_activeMinigame.m_gameStarted = true;
		PsState.m_activeMinigame.m_gameStartCount++;
		PsState.m_activeMinigame.m_playerReachedGoal = false;
		PsIngameMenu.OpenController();
		EveryplayManager.StartRecording();
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
		PsState.m_activeMinigame.m_gameDeathCount++;
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterLoseRace), typeof(PsUITopLoseRace), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", RestartMinigame);
	}

	public override void WinMinigame()
	{
		base.WinMinigame();
		m_gameMode.StopRecordingGhost();
		EveryplayManager.StopRecording(0f);
		PsIngameMenu.CloseAll();
		PsState.m_activeMinigame.m_gameEnded = true;
		PsState.m_activeMinigame.m_playerReachedGoal = true;
		Debug.LogWarning(PsState.m_activeMinigame.m_gameTicks);
		if (GetGameMode() == PsGameMode.StarCollect)
		{
			m_scoreCurrent = PsState.m_activeMinigame.m_collectedStars;
		}
		else if (GetGameMode() == PsGameMode.Race)
		{
			m_scoreCurrent = 3;
		}
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinEditor), typeof(PsUITopWin), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Win", WinMinigame);
	}

	public override void RestartMinigame()
	{
		base.RestartMinigame();
		EditorScene.ClearScreenshots();
		EveryplayManager.StopRecording(0f);
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
		base.ExitMinigame();
		EveryplayManager.StopRecording(0f);
		PsIngameMenu.CloseAll();
		PsState.m_activeMinigame.m_editing = true;
		PsState.m_activeMinigame.m_gameStartCount = 0;
		PsState.m_activeMinigame.m_gameStarted = false;
		PsState.m_physicsPaused = true;
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		PsState.m_activeMinigame.m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		m_gameMode.CreateEditMenu(delegate
		{
			ExitEditor();
		}, EnterMinigame);
	}

	public override void DestroyMinigame()
	{
		base.DestroyMinigame();
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		LevelManager.DestroyCurrentLevel();
		PsState.m_activeMinigame = null;
		PsState.m_activeGameLoop.m_minigameMetaData = null;
		PsState.m_activeGameLoop.m_minigameBytes = null;
	}

	public override void BackAtMenu()
	{
		if (!m_backAtMenu && !m_released)
		{
			m_backAtMenu = true;
			PsUIBaseState psUIBaseState = null;
			psUIBaseState = ((!PsState.m_adminMode) ? new PsUIBaseState(typeof(PsUICenterCreate), typeof(PsUITopBackButton)) : new PsUIBaseState(typeof(PsUICenterCreate), typeof(PsUITopBackButton), typeof(PsUITabbedProfile), typeof(PsUISideCreateAdmin)));
			psUIBaseState.SetAction("Exit", delegate
			{
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new PsMainMenuState());
			});
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(psUIBaseState);
			PsMenuScene.CreateBlur();
			ReleaseLoop();
		}
	}

	public override void ReleaseLoop()
	{
		if (!m_released)
		{
			base.ReleaseLoop();
			SetActiveLoopNull();
			ActivateCurrentLoop();
		}
	}

	public override void ActivateCurrentLoop()
	{
		PsGameLoop currentNodeInfo = m_planet.m_mainPath.GetCurrentNodeInfo();
		if (currentNodeInfo != null && currentNodeInfo.m_node != null)
		{
			currentNodeInfo.m_node.Activate();
		}
		if (m_winFirtTime && currentNodeInfo != null && !currentNodeInfo.m_activated)
		{
			currentNodeInfo.SaveAsActivated();
			new PsMenuDialogueFlow(currentNodeInfo, PsNodeEventTrigger.LoopActivated, 0.1f);
		}
	}
}
