using UnityEngine;

public class PsGameLoopOwnPublished : PsGameLoop
{
	public PsGameLoopOwnPublished(string _minigameId, PsMinigameMetaData _metaData)
		: base(PsMinigameContext.Published, _minigameId, null, -1, -1, 3, false)
	{
		m_minigameMetaData = _metaData;
		m_freeConsumableUnlock = "Rent";
	}

	public override void StartLoop()
	{
		if (PsState.m_activeGameLoop == null)
		{
			m_unlocked = true;
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopStart, 0f, delegate
			{
				SetAsActiveLoop();
				LoadMinigame();
			});
		}
	}

	public void FadeInLoadingScreen(TimerC _c)
	{
		LoadMinigame();
	}

	public override void LoopUnlocked()
	{
	}

	public override void LoadMinigame()
	{
		if (m_loaderEntity != null)
		{
			EntityManager.RemoveEntity(m_loaderEntity);
		}
		m_loaderEntity = EntityManager.AddEntity();
		m_loadingMetaData = false;
		if (m_gameMode == null)
		{
			CreateGameMode();
		}
		m_scoreCurrent = 0;
		m_timeScoreCurrent = int.MaxValue;
		m_timeScoreBest = m_minigameMetaData.timeScore;
		if (m_timeScoreBest == 0)
		{
			m_timeScoreBest = int.MaxValue;
		}
		m_timeScoreOld = m_timeScoreBest;
		m_scoreOld = m_scoreBest;
		m_rewardOld = m_scoreBest;
		Debug.LogWarning("------- BEST TIME: " + HighScores.TimeScoreToTime(m_timeScoreBest));
		Debug.LogWarning("------- BEST SCORE: " + m_scoreBest);
		m_loadMetadataCallback = null;
		LoadMinigameBytes();
		PreviewLoop();
	}

	public override void CreateGameMode()
	{
		if (m_minigameMetaData.gameMode == PsGameMode.Race)
		{
			m_gameMode = new PsGameModeRaceOwnPublished(this);
		}
		else if (m_minigameMetaData.gameMode == PsGameMode.StarCollect)
		{
			m_gameMode = new PsGameModeStarCollectOwnPublished(this);
		}
	}

	public override void PreviewLoop()
	{
		Main.m_currentGame.m_sceneManager.ChangeScene(new GameScene("GameScene"), new PsLevelLoadingScene());
	}

	protected override void MinigameDownloadOk(byte[] _levelData)
	{
		base.MinigameDownloadOk(_levelData);
		if (m_gameMode is PsGameModeRace)
		{
			(m_gameMode as PsGameModeRace).CalculateMedalTimes();
		}
		LoadGhosts();
	}

	public override void GhostsLoaded()
	{
		if (Main.m_currentGame.m_sceneManager.m_loadingScene is PsLevelLoadingScene)
		{
			(Main.m_currentGame.m_sceneManager.m_loadingScene as PsLevelLoadingScene).LevelLoaded();
		}
	}

	public override void EnterMinigame()
	{
		CameraS.m_updateComponents = true;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameEnter, 0.5f, delegate
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameStartState());
			m_gameMode.EnterMinigame();
		});
	}

	public override void InitializeMinigame()
	{
		base.InitializeMinigame();
		SetPlaybackGhostAndCoins();
	}

	public override void SetPlaybackGhostAndCoins()
	{
		if (m_gameMode.m_playbackGhosts.Count > 0)
		{
			m_gameMode.CreatePlaybackGhostVisuals();
		}
	}

	public override void StartMinigame()
	{
		PsState.m_activeMinigame.m_gameStarted = true;
		m_gameMode.StartMinigame();
		Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		PsState.m_physicsPaused = false;
		PsState.m_activeMinigame.m_gameOn = true;
		PsState.m_activeMinigame.m_gameStartCount++;
		PsState.m_activeMinigame.m_playerReachedGoal = false;
		CreatePlaybackGhosts();
		SoundS.PlaySingleShot("/InGame/GameStart", Vector3.zero);
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GamePlayState());
		EveryplayManager.StartRecording();
	}

	public override void PauseMinigame()
	{
		if (!PsState.m_activeMinigame.m_gamePaused && PsState.m_activeMinigame.m_gameStarted && !PsState.m_activeMinigame.m_gameEnded)
		{
			PsState.m_activeMinigame.SetGamePaused(true);
			PauseElapsedTimer();
			m_gameMode.PauseMinigame();
		}
	}

	public override void ResumeMinigame()
	{
		m_gameMode.CreatePlayMenu(RestartMinigame, PauseMinigame);
		ResumeElapsedTimer();
		PsState.m_activeMinigame.SetGamePaused(false);
	}

	public override void RestartMinigame()
	{
		base.RestartMinigame();
		EveryplayManager.StopRecording(0f);
		if (m_gameMode.m_waitForHighscoreAndGhost || m_gameMode.m_waitForNextGhost)
		{
			PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(PsUICenterWaitForHighscoresAndGhost), null, null, null, false);
			psUIBasePopup.SetAction("Done", RestartAction);
		}
		else
		{
			RestartAction();
		}
	}

	public virtual void RestartAction()
	{
		PsState.m_activeMinigame.SetGamePaused(false);
		ResumeElapsedTimer();
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		CameraS.RemoveAllTargetComponents();
		PsState.m_activeMinigame.m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
		InitializeMinigame();
		m_gameMode.EnterMinigame();
	}

	public override void LoseMinigame()
	{
		m_gameMode.StopRecordingGhost();
		PsIngameMenu.CloseAll();
		StopEveryplayAfterDelay();
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
		base.WinMinigame();
		m_gameMode.StopRecordingGhost();
		PsIngameMenu.CloseAll();
		StopEveryplayAfterDelay();
		PsState.m_activeMinigame.m_gameEnded = true;
		PsState.m_activeMinigame.m_playerReachedGoal = true;
		PsState.m_activeMinigame.m_gameOn = false;
		m_gameMode.m_exitingMinigame = false;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 1f, delegate
		{
			m_gameMode.WinMinigame();
		});
	}

	public override void ExitMinigame()
	{
		base.ExitMinigame();
		EveryplayManager.StopRecording(0f);
		m_gameMode.m_exitingMinigame = true;
		m_gameMode.m_waitForNextGhost = false;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameExit, 0f, delegate
		{
			PsIngameMenu.CloseAll();
			if (PsState.m_activeMinigame.m_gameStartCount > 0 && m_sendQuit)
			{
				PsMetagameManager.SendQuit(PsState.m_activeGameLoop);
			}
			PsMetagameManager.HideResources();
			Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
		});
	}

	public override void DestroyMinigame()
	{
		base.DestroyMinigame();
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		CameraS.RemoveAllTargetComponents();
		LevelManager.DestroyCurrentLevel();
		PsState.m_activeMinigame = null;
	}

	public override void BackAtMenu()
	{
		if (!m_backAtMenu && !m_released)
		{
			m_backAtMenu = true;
			ReleaseLoop();
		}
		m_backAtMenu = false;
	}

	public override void ReleaseLoop()
	{
		if (!m_released)
		{
			base.ReleaseLoop();
			m_scoreOld = m_scoreBest;
			m_rewardOld = m_scoreBest;
			PsUIBaseState psUIBaseState = null;
			psUIBaseState = ((!PsState.m_adminMode) ? new PsUIBaseState(typeof(PsUITabbedProfile), typeof(PsUIVersusTopBack)) : new PsUIBaseState(typeof(PsUICenterCreate), typeof(PsUIVersusTopBack), typeof(PsUITabbedProfile), typeof(PsUISideCreateAdmin)));
			psUIBaseState.SetAction("Exit", delegate
			{
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new PsMainMenuState());
			});
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(psUIBaseState);
			PsMenuScene.CreateBlur();
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
