using System;
using Server;
using UnityEngine;

public class PsGameLoopFresh : PsTimedEventLoop
{
	public PsGameLoopFresh(PsMinigameContext _context, string _minigameId, PsPlanetPath _path, int _id, int _levelNumber, int _score, int _timeleft, int _duration, string _endTime = null, bool _eventOver = false, bool _domeDestroyed = false)
		: base(_context, _minigameId, _minigameId, _path, _id, _levelNumber, _score, _timeleft, _duration, _endTime, _eventOver, _domeDestroyed)
	{
	}

	public override void CreateGameMode()
	{
		if (m_minigameMetaData.gameMode == PsGameMode.Race)
		{
			m_gameMode = new PsGameModeRaceFresh(this);
		}
		else if (m_minigameMetaData.gameMode == PsGameMode.StarCollect)
		{
			m_gameMode = new PsGameModeStarCollectFresh(this);
		}
	}

	public override void WinMinigame()
	{
		base.WinMinigame();
		PsIngameMenu.CloseAll();
		m_gameMode.StopRecordingGhost();
		StopEveryplayAfterDelay();
		PsState.m_activeMinigame.m_gameEnded = true;
		PsState.m_activeMinigame.m_playerReachedGoal = true;
		PsState.m_activeMinigame.m_gameOn = false;
		m_gameMode.m_exitingMinigame = false;
		PsIngameMenu.CloseAll();
		m_gameMode.WinMinigame();
	}

	public override void StartLoop()
	{
		if (PsState.m_activeGameLoop == null && !m_eventOver)
		{
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopStart, 0f, delegate
			{
				SetAsActiveLoop();
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
		if (m_minigameId != string.Empty && m_minigameMetaData == null)
		{
			m_loadingMetaData = true;
			HttpC httpC = MiniGame.Get(m_minigameId, LoadMetaDataSUCCESS, LoadMetaDataFAILED);
			httpC.objectData = m_minigameId;
			EntityManager.AddComponentToEntity(m_loaderEntity, httpC);
			Debug.LogWarning("LOAD METADATA");
		}
		else if (m_minigameId != string.Empty && m_minigameMetaData != null)
		{
			LoadMinigameBytes();
		}
		PreviewLoop();
	}

	public override void PreviewLoop()
	{
		Main.m_currentGame.m_sceneManager.ChangeScene(new GameScene("GameScene"), new PsLevelLoadingScene());
	}

	public void CloseGameCard()
	{
		PsIngameMenu.CloseAll();
		ReleaseLoop();
		Main.m_currentGame.m_currentScene.m_stateMachine.RevertToPreviousState();
	}

	protected override void LoadMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		m_loadingMetaData = false;
		m_minigameMetaData = _minigameMetaData;
		m_minigameId = _minigameMetaData.id;
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
		Debug.LogWarning("BEST TIME: " + HighScores.TimeScoreToTime(m_timeScoreBest));
		Debug.LogWarning("BEST SCORE: " + m_scoreBest);
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
		LoadMinigameBytes();
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
		m_eventOver = true;
		CameraS.m_updateComponents = true;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameEnter, 0.5f, delegate
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameStartState());
			PsIngameMenu.CloseAll();
			PsIngameMenu.m_popupMenu = new PsUIBasePopup(null, typeof(PsUITopStartFresh), null, null, false);
			PsIngameMenu.m_popupMenu.SetAction("Exit", ExitMinigame);
			PsIngameMenu.OpenController(true);
		});
	}

	public override void InitializeMinigame()
	{
		base.InitializeMinigame();
		SetPlaybackGhostAndCoins();
	}

	public override void CreateMinigame()
	{
		m_rentedVehicle = true;
		m_userHasSelectedVehicle = true;
		m_selectedVehicle = PsSelectedVehicle.CreatorVehicle;
		Vehicle vehicle = Activator.CreateInstance(Type.GetType(m_minigameMetaData.playerUnit), new string[0]) as Vehicle;
		PsState.m_activeGameLoop.m_rentUpgradeValues = vehicle.ParseUpgradeValues(m_minigameMetaData.creatorUpgrades);
		vehicle.Destroy();
		base.CreateMinigame();
	}

	public override void StartMinigame()
	{
		m_gameMode.StartMinigame();
		UnityEngine.Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		PsState.m_physicsPaused = false;
		PsState.m_activeMinigame.m_gameOn = true;
		PsState.m_activeMinigame.m_gameStarted = true;
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
		PsState.m_activeMinigame.SetGamePaused(false);
		ResumeElapsedTimer();
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
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameStartState());
		PsIngameMenu.CloseAll();
		PsIngameMenu.m_popupMenu = new PsUIBasePopup(null, typeof(PsUITopStartFresh), null, null, false);
		PsIngameMenu.m_popupMenu.SetAction("Exit", ExitMinigame);
		PsIngameMenu.OpenController(true);
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

	public override void ExitMinigame()
	{
		base.ExitMinigame();
		EveryplayManager.StopRecording(0f);
		PsIngameMenu.CloseAll();
		PsMetagameManager.HideResources();
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 1f, delegate
		{
			PsIngameMenu.m_popupMenu = new PsUIBasePopup(typeof(PsUICenterRateGame), typeof(PsUITopBanner), null, null, false);
			PsIngameMenu.m_popupMenu.SetAction("Exit", ExitRating);
		});
	}

	public void ExitRating()
	{
		PsIngameMenu.CloseAll();
		m_gameMode.m_exitingMinigame = true;
		m_gameMode.m_waitForNextGhost = false;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameExit, 0f, delegate
		{
			PsIngameMenu.CloseAll();
			if (m_sendQuit)
			{
				PsMetagameManager.SendQuit(PsState.m_activeGameLoop);
			}
			PsPlanetManager.m_timedEvents = null;
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
		ReleaseLoop();
	}

	public override void ReleaseLoop()
	{
		if (!m_released)
		{
			base.ReleaseLoop();
			m_scoreOld = m_scoreBest;
			m_rewardOld = m_scoreBest;
			SetActiveLoopNull();
		}
	}

	public override string GetBannerString()
	{
		return "Play & Rate";
	}
}
