using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using UnityEngine;

public class PsGameLoopEditorTemplate : PsGameLoop
{
	public string m_templateIdentifier;

	public bool m_removedFromPath;

	public bool m_published;

	public PsUIBasePopup m_everyplayPopup;

	public PsGameLoopEditorTemplate(string _templateIdentifier, string _minigameId, PsPlanetPath _path, int _id, int _levelNumber, int _score, bool _unlocked, bool _removedFromPath)
		: base(PsMinigameContext.EditorTemplate, _minigameId, _path, _id, _levelNumber, _score, _unlocked)
	{
		m_templateIdentifier = _templateIdentifier;
		m_removedFromPath = _removedFromPath;
		m_autosave = true;
	}

	public override void StartLoop()
	{
		if (PsState.m_activeGameLoop == null && !m_removedFromPath && m_nodeId <= m_path.m_currentNodeId)
		{
			LoopUnlocked();
		}
	}

	public override void LoadMinigameMetaData(Action _callback)
	{
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
		if (m_nodeId <= m_path.m_currentNodeId && !Input.GetKey(KeyCode.LeftShift))
		{
			m_unlocked = true;
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopStart, 0f, delegate
			{
				SetAsActiveLoop();
				if (m_nodeId < m_path.m_currentNodeId)
				{
					LoadMinigame();
					Main.m_currentGame.m_sceneManager.ChangeScene(new EditorScene("EditorScene"), new PsLevelLoadingScene());
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
		else if (Input.GetKey(KeyCode.LeftShift))
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
		Main.m_currentGame.m_sceneManager.ChangeScene(new EditorScene("EditorScene"), new PsLevelLoadingScene());
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
			Debug.LogWarning("minigame in memory");
			MinigameDownloadOk(m_minigameBytes);
		}
		else if (m_minigameMetaData != null)
		{
			Debug.LogWarning("minigame metadata in memory, load level bytes");
			LoadMinigameBytes();
		}
		else if (!string.IsNullOrEmpty(m_minigameId))
		{
			Debug.LogWarning("minigame id known, load metadata");
			m_loadingMetaData = true;
			HttpC httpC = MiniGame.Get(m_minigameId, LoadMetaDataSUCCESS, LoadMetaDataFAILED);
			httpC.objectData = m_minigameId;
		}
		else
		{
			Debug.LogWarning("no minigame id, or metadata, create minigame from template");
			m_loadingMetaData = true;
			PsLevelTemplate levelTemplateByIdentifier = PsMetagameData.GetLevelTemplateByIdentifier(m_templateIdentifier);
			PsMetagameManager.GiveLevelTemplate(levelTemplateByIdentifier, LoadMetaDataSUCCESS);
		}
	}

	protected override void LoadMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		_minigameMetaData.template = false;
		base.LoadMetaDataSUCCESS(_minigameMetaData);
		LoadMinigameBytes();
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
			int num = UnityEngine.Random.Range(0, elements.Length);
			Vector3 position = elements[num].m_position;
			PsState.m_activeMinigame.m_playerNode.m_position = position;
			TransformS.SetPosition(PsState.m_activeMinigame.m_playerNode.m_TC, position);
			PsState.m_activeMinigame.m_playerNode.Reset();
		}
		List<GraphElement> list = new List<GraphElement>();
		if (m_minigameMetaData.gameMode == PsGameMode.StarCollect)
		{
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
					int num3 = 0;
					while (list2.Contains(num2) && num3 < 1000)
					{
						num2 = UnityEngine.Random.Range(0, elements2.Length);
						num3++;
					}
					list2.Add(num2);
					Vector3 position2 = elements2[num2].m_position;
					elements4[i].m_position = position2;
					TransformS.SetPosition(elements4[i].m_TC, position2);
					elements4[i].Reset();
				}
			}
			list.AddRange(elements4);
		}
		if (elements3.Length > 0)
		{
			int num4 = UnityEngine.Random.Range(0, elements3.Length);
			Vector3 position3 = elements3[num4].m_position;
			PsState.m_activeMinigame.m_goalNode.m_position = position3;
			TransformS.SetPosition(PsState.m_activeMinigame.m_goalNode.m_TC, position3);
			PsState.m_activeMinigame.m_goalNode.Reset();
		}
		List<GraphElement> childElements = LevelManager.m_currentLevel.m_currentLayer.m_childElements;
		for (int num5 = childElements.Count - 1; num5 > -1; num5--)
		{
			if (!PsState.m_adminMode && (childElements[num5].m_name == "PlayerSpawnPoint" || childElements[num5].m_name == "GoalSpawnPoint" || childElements[num5].m_name == "StarSpawnPoint" || (childElements[num5].m_name == "CollectibleStar" && m_minigameMetaData.gameMode != PsGameMode.StarCollect)))
			{
				childElements[num5].Clear(false);
				LevelManager.m_currentLevel.m_currentLayer.RemoveElement(childElements[num5]);
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
		GameObject gameObject2 = PsState.m_activeMinigame.m_backgroundPrefab.p_gameObject.transform.Find("Static").gameObject;
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

	public override void StartEditorRecording()
	{
		EveryplayManager.StartRecording();
	}

	public override void EnterEditor()
	{
		Debug.LogWarning("ENTER EDITOR");
		PsMetrics.PlayerEnteredEditor();
		m_backAtMenu = false;
		PsState.m_activeMinigame.m_editing = true;
		PsState.m_physicsPaused = true;
		CameraS.m_updateComponents = true;
		if (string.IsNullOrEmpty(m_minigameMetaData.id))
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorPlayer(EditorSelectorContext.START_WIZARD));
		}
		else
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
		if (PsLoadingScreen.m_active)
		{
			PsLoadingScreen.FadeOut();
		}
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

	public override void ExitEditor(bool _published)
	{
		GizmoManager.ClearSelection();
		GizmoManager.Update();
		PsIngameMenu.CloseAll();
		PsMetrics.MinigameExited(PsState.m_activeGameLoop.m_context.ToString(), PsState.m_activeGameLoop.m_minigameMetaData, PsState.m_activeGameLoop.ElapsedTime());
		PsState.m_activeMinigame.RemoveTimeScale();
		EveryplayManager.StopRecording(0f);
		Everyplay.SetMetadata("minigamename", m_minigameMetaData.name);
		Everyplay.SetMetadata("username", PlayerPrefsX.GetUserName());
		m_gameMode.m_exitingMinigame = true;
		m_gameMode.m_waitForNextGhost = false;
		m_published = _published;
		if (!_published)
		{
			SaveMinigame(true);
		}
		else if (_published)
		{
			if (m_nodeId == m_path.m_currentNodeId)
			{
				m_path.m_currentNodeId = m_nodeId + 1;
			}
			if (m_sidePath != null && m_sidePath.m_currentNodeId < 1)
			{
				m_sidePath.m_currentNodeId = 1;
			}
			m_removedFromPath = true;
			new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameExit, 0f, delegate
			{
				ExitAfterEveryplay();
			});
		}
	}

	public void ExitAfterEveryplay()
	{
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
		Everyplay.WasClosed -= ContinueExit;
		if (m_everyplayPopup != null)
		{
			m_everyplayPopup.Destroy();
		}
		m_everyplayPopup = null;
		Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
	}

	public override void EnterMinigame()
	{
		EveryplayManager.SetPaused(false);
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
		UnityEngine.Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		PsState.m_physicsPaused = false;
		PsState.m_activeMinigame.m_gameStarted = true;
		PsState.m_activeMinigame.m_gameStartCount++;
		PsState.m_activeMinigame.m_playerReachedGoal = false;
		PsIngameMenu.OpenController();
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
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterLoseEditor), typeof(PsUITopLoseRace), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", ExitMinigame);
		PsIngameMenu.m_popupMenu.SetAction("Restart", RestartMinigame);
	}

	public override void WinMinigame()
	{
		base.WinMinigame();
		m_gameMode.StopRecordingGhost();
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
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 0f, delegate
		{
			PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterWinEditor), typeof(PsUITopWin), null, null, false);
			PsIngameMenu.m_popupMenu.SetAction("Win", WinMinigame);
		});
	}

	public override void RestartMinigame()
	{
		base.RestartMinigame();
		EditorScene.ClearScreenshots();
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
			ExitEditor(false);
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
			if (m_published)
			{
				m_published = false;
				LaunchToSpace();
			}
			else
			{
				ReleaseLoop();
			}
		}
	}

	public void LaunchToSpace()
	{
		if (m_node != null)
		{
			m_node.RemoveHighlight();
			m_node.m_checkpointEffect.Hide();
			Entity entity = EntityManager.AddEntity();
			TransformC transformC = TransformS.AddComponent(entity);
			TransformS.ParentComponent(transformC, PsPlanetManager.GetCurrentPlanet().m_alienPlanetTC);
			transformC.transform.position = m_node.m_tc.transform.position;
			transformC.transform.rotation = m_node.m_tc.transform.rotation;
			transformC.transform.localScale = m_node.m_tc.transform.localScale;
			PrefabC c = PrefabS.AddComponent(transformC, Vector3.up * 2.35f, ResourceManager.GetGameObject(RESOURCE.FloatingPlatformFreshAndFreePrefab_GameObject));
			PrefabS.SetCamera(c, PsPlanetManager.GetCurrentPlanet().m_planetCamera);
			TweenC tweenComponent = TweenS.AddTransformTween(transformC, TweenedProperty.Position, TweenStyle.ExpoOut, transformC.transform.localPosition + transformC.parent.transform.worldToLocalMatrix.MultiplyVector(transformC.transform.up) * 6f, 1f, 0.45f, true);
			TweenS.AddTweenEndEventListener(tweenComponent, FlyerPlopEventHandler);
			TweenC c2 = TweenS.AddTransformTween(transformC, TweenedProperty.Scale, TweenStyle.QuadIn, new Vector3(0.85f, 1.75f, 0.85f), 0.25f, 0.35f, true);
			TweenS.SetAdditionalTweenProperties(c2, 0, true, TweenStyle.QuadOut);
		}
		else
		{
			ReleaseLoop();
		}
	}

	public void FlyerPlopEventHandler(TweenC _c)
	{
		Vector3 endValue = _c.p_TC.transform.localPosition + _c.p_TC.parent.transform.worldToLocalMatrix.MultiplyVector(_c.p_TC.transform.up) * 100f;
		TweenC tweenComponent = TweenS.AddTransformTween(_c.p_TC, TweenedProperty.Position, TweenStyle.CubicIn, endValue, 1.5f, 0f, false);
		TweenS.AddTweenEndEventListener(tweenComponent, LaunchedToSpace);
	}

	public void LaunchedToSpace(TweenC _c)
	{
		EntityManager.RemoveEntity(_c.p_entity);
		if (PsState.m_activeGameLoop == this)
		{
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
}
