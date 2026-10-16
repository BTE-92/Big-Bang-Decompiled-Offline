using System;
using System.Collections;
using MiniJSON;
using Server;
using UnityEngine;

public class PsGameLoopVersus : PsTimedEventLoop
{
	public bool m_roundInfoSent;

	public bool m_versusWon;

	public bool m_nextRound;

	public int m_tryCount;

	public int m_setTriesRetryCount;

	private PsUIBasePopup m_outOfTriesPopup;

	private PsUIBasePopup m_forfeitPopup;

	private PsUIBasePopup m_exitPopup;

	public int m_oldCupAmount;

	public int m_oldRank;

	public bool m_removeFromPath;

	public bool m_removedFromPath;

	public new bool m_createFlyingNode;

	public bool m_addToFloaters;

	public PsGameLoopVersus(PsMinigameContext _context, VersusMetaData _versusData, string _timedEventId, string _minigameId, PsPlanetPath _path, int _id, int _levelNumber, int _score, int _timeleft, bool _removedFromPath, int _duration = -1, string _endTime = null, bool _eventOver = false, bool _domeDestroyed = false)
		: base(_context, _timedEventId, _minigameId, _path, _id, _levelNumber, _score, _timeleft, _duration, _endTime, _eventOver, _domeDestroyed)
	{
		m_removedFromPath = _removedFromPath;
		m_versusData = _versusData;
		SetUp();
	}

	public virtual void SetUp()
	{
		m_removeFromPath = false;
		if (m_versusData != null)
		{
			m_minigameMetaData = m_versusData.gameMetaData;
			m_gameMode = new PsGameModeRaceVersus(this);
			m_tryCount = m_versusData.GetOwnInfo().tries;
			m_removeFromPath = false;
		}
		else
		{
			m_removeFromPath = true;
			m_unlocked = false;
		}
		m_setTriesRetryCount = 0;
		m_nextRound = false;
		m_createFlyingNode = (m_addToFloaters = false);
	}

	public override void SetEventState()
	{
	}

	public override string GetBannerString()
	{
		PsFloatingVersusNode psFloatingVersusNode = m_node as PsFloatingVersusNode;
		if (m_versusData == null)
		{
			psFloatingVersusNode.AddGreyScale();
			return "Loading...";
		}
		if (!string.IsNullOrEmpty(m_versusData.winner))
		{
			psFloatingVersusNode.RemoveGreyScale();
			if (m_versusData.winner == PlayerPrefsX.GetUserId())
			{
				return "You won!";
			}
			return "You lost";
		}
		if (string.IsNullOrEmpty(m_versusData.currentPlayer))
		{
			psFloatingVersusNode.AddGreyScale();
			return "Searching\nopponent";
		}
		if (m_versusData.currentPlayer != PlayerPrefsX.GetUserId() || PsState.m_versusChallengeTryAmount <= m_versusData.GetOwnInfo().tries)
		{
			psFloatingVersusNode.AddGreyScale();
			return "Opponent's\nturn";
		}
		psFloatingVersusNode.RemoveGreyScale();
		int num = SecondsLeft();
		string text = ((num >= 3601 || num < 0) ? string.Empty : ("\n" + PsMetagameManager.GetTimeStringFromSeconds(num)));
		return "Your turn!" + text;
	}

	public bool UpdateInfo(VersusMetaData _data)
	{
		bool result = false;
		if (m_versusData.currentPlayer != _data.currentPlayer)
		{
			result = true;
		}
		m_versusData = _data;
		m_tryCount = ((m_versusData.GetOwnInfo() != null) ? m_versusData.GetOwnInfo().tries : 0);
		UpdateInfo(m_versusData.timeLeft);
		return result;
	}

	public override void CreateGameMode()
	{
		m_gameMode = new PsGameModeRaceVersus(this);
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
		m_timeScoreCurrent = HighScores.TicksToTimeScore(Mathf.RoundToInt(PsState.m_activeMinigame.m_gameTicks), true);
		if (m_versusData.GetOpponentInfo() == null || m_versusData.GetOpponentInfo().score.time == 0 || m_timeScoreCurrent < m_versusData.GetOpponentInfo().score.time)
		{
			new PsMinigameDialogueFlow(this, PsNodeEventTrigger.VersusFinishWin, 1f, delegate
			{
				m_gameMode.WinMinigame();
			});
		}
		else if (m_versusData.GetOpponentInfo() != null && m_versusData.GetOpponentInfo().score.time != 0 && m_timeScoreCurrent > m_versusData.GetOpponentInfo().score.time)
		{
			new PsMinigameDialogueFlow(this, PsNodeEventTrigger.VersusFinishLose, 1f, delegate
			{
				m_gameMode.WinMinigame();
			});
		}
		else
		{
			m_gameMode.WinMinigame();
		}
	}

	public override void StartLoop()
	{
		m_oldCupAmount = PsMetagameManager.m_playerStats.cups;
		m_oldRank = PsMetagameManager.m_playerStats.rank;
		m_versusWon = false;
		if (PsState.m_activeGameLoop != null || (m_path != null && m_nodeId > m_path.m_currentNodeId && m_path.m_name != "FloatingPath"))
		{
			return;
		}
		if (!m_eventOver)
		{
			if (m_versusData != null)
			{
				if (!string.IsNullOrEmpty(m_versusData.winner))
				{
					bool flag = m_versusData.winner == PlayerPrefsX.GetUserId();
					Debug.Log("Winner: " + ((!flag) ? "opponent" : "player"));
					ClaimVersus(flag);
					return;
				}
				if (string.IsNullOrEmpty(m_versusData.currentPlayer))
				{
					Debug.LogWarning("Searching opponent");
					return;
				}
				if (m_versusData.currentPlayer != PlayerPrefsX.GetUserId() || PsState.m_versusChallengeTryAmount <= m_versusData.GetOwnInfo().tries)
				{
					Debug.LogWarning("Opponents turn");
					return;
				}
			}
			new PsMenuDialogueFlow(this, PsNodeEventTrigger.LoopStart, 0f, delegate
			{
				SetAsActiveLoop();
				LoadMinigame();
			});
		}
		else
		{
			Debug.LogWarning("Event was already over");
		}
	}

	public virtual void ClaimVersus(bool _playerWon)
	{
		PsMetagameManager.VersusChallengeRewards(_playerWon);
		m_eventOver = true;
		m_domeDestroyed = true;
		if (m_path.m_nodeInfos.Contains(this))
		{
			m_path.m_nodeInfos.Remove(this);
		}
		string pathJson = Json.Serialize(ClientTools.GenerateProgressionFloatingPathJson(m_path));
		Versus.ClaimAndSave(m_timedEventId, pathJson, ClaimAndSaveSUCCEED, ClaimAndSaveFAILED);
		if (_playerWon)
		{
			PsMainMenuState.m_versusButton.Win(DestroyNode);
		}
		else
		{
			PsMainMenuState.m_versusButton.Lose(DestroyNode);
		}
	}

	public void ClaimAndSaveSUCCEED(HttpC _c)
	{
		Debug.Log("ClaimAndSave succeed");
		PsPlanetSerializer.SaveProgressionLocally(PsPlanetManager.GetCurrentPlanet());
	}

	public void ClaimAndSaveFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", _c.www, delegate
		{
			string pathJson = Json.Serialize(ClientTools.GenerateProgressionFloatingPathJson(PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingPath));
			HttpC httpC = Versus.ClaimAndSave(m_timedEventId, pathJson, ClaimAndSaveSUCCEED, ClaimAndSaveFAILED);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}

	public void DestroyNode()
	{
		if (m_node != null)
		{
			(m_node as PsFloatingPlanetNode).DestroyNode();
		}
	}

	public override void LoadMinigame()
	{
		if (m_loaderEntity != null)
		{
			EntityManager.RemoveEntity(m_loaderEntity);
		}
		m_loaderEntity = EntityManager.AddEntity();
		if (m_versusData == null)
		{
			m_loadingMetaData = true;
			string minigameId = m_minigameId;
			HttpC c = Versus.Create(CreateVersusChallengeSUCCEED, CreateVersusChallengeFAILED, minigameId);
			EntityManager.AddComponentToEntity(m_loaderEntity, c);
		}
		else if (m_minigameId != string.Empty && m_minigameMetaData == null)
		{
			m_loadingMetaData = true;
			HttpC httpC = MiniGame.Get(m_minigameId, LoadMetaDataSUCCESS, LoadMetaDataFAILED);
			httpC.objectData = m_minigameId;
			EntityManager.AddComponentToEntity(m_loaderEntity, httpC);
			Debug.LogWarning("LOAD METADATA");
		}
		else
		{
			LoadMetaDataSUCCESS(m_minigameMetaData);
		}
		PreviewLoop();
	}

	public void CreateVersusChallengeSUCCEED(VersusMetaData _versusData)
	{
		m_versusData = _versusData;
		m_minigameId = m_versusData.gameId;
		m_timedEventId = m_versusData.timedEventId;
		m_minigameMetaData = m_versusData.gameMetaData;
		m_gameMode = new PsGameModeRaceVersus(this);
		m_tryCount = m_versusData.GetOwnInfo().tries;
		HttpC httpC = MiniGame.Get(m_minigameId, LoadMetaDataSUCCESS, LoadMetaDataFAILED);
		httpC.objectData = m_minigameId;
		EntityManager.AddComponentToEntity(m_loaderEntity, httpC);
		Debug.LogWarning("LOAD METADATA");
	}

	public void CreateVersusChallengeFAILED(HttpC _c)
	{
		string forcedMinigameId = m_minigameId;
		ServerManager.ThrowServerErrorException("Connection error", _c.www, () => Versus.Create(CreateVersusChallengeSUCCEED, CreateVersusChallengeFAILED, forcedMinigameId));
	}

	public override void PreviewLoop()
	{
		if (Main.m_currentGame.m_sceneManager.m_currentScene is PsMenuScene)
		{
			Main.m_currentGame.m_sceneManager.ChangeScene(new GameScene("GameScene"), new PsLevelLoadingScene());
			return;
		}
		if (m_previousLoop != null)
		{
			m_previousLoop.DestroyMinigame();
		}
		EveryplayManager.ClearRecording();
		EnteredMinigameScene();
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
		m_minigameId = m_versusData.gameId;
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
		if (m_removeFromPath && !m_removedFromPath && m_path != null)
		{
			m_createFlyingNode = true;
			m_removedFromPath = true;
			m_removeFromPath = false;
			Hashtable progressionJSON = ClientTools.GenerateProgressionPathJson(m_path);
			PsMetagameManager.SaveProgression(progressionJSON, false);
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
		CameraS.m_updateComponents = true;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameEnter, 0.5f, delegate
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameStartState());
			m_gameMode.EnterMinigame();
			if (m_versusData.GetOwnInfo().score.time > 0)
			{
				(m_gameMode as PsGameModeRaceVersus).m_bestVersusTime = m_versusData.GetOwnInfo().score.time;
				(m_gameMode as PsGameModeRaceVersus).m_previousBest = (m_gameMode as PsGameModeRaceVersus).m_bestVersusTime;
			}
			else
			{
				(m_gameMode as PsGameModeRaceVersus).m_bestVersusTime = int.MaxValue;
				(m_gameMode as PsGameModeRaceVersus).m_previousBest = (m_gameMode as PsGameModeRaceVersus).m_bestVersusTime;
			}
		});
	}

	public override void InitializeMinigame()
	{
		base.InitializeMinigame();
		SetPlaybackGhostAndCoins();
	}

	public override void SetPlaybackGhostAndCoins()
	{
		m_gameMode.CreatePlaybackGhostVisuals();
	}

	public override void CreateMinigame()
	{
		m_selectedVehicle = PsSelectedVehicle.None;
		Vehicle vehicle = Activator.CreateInstance(Type.GetType(m_minigameMetaData.playerUnit), new string[0]) as Vehicle;
		PsState.m_activeGameLoop.m_rentUpgradeValues = vehicle.ParseUpgradeValues(m_minigameMetaData.creatorUpgrades);
		vehicle.Destroy();
		base.CreateMinigame();
	}

	public override void StartMinigame()
	{
		m_roundInfoSent = false;
		PsState.m_activeMinigame.m_gameStarted = true;
		m_tryCount++;
		m_gameMode.StartMinigame();
		m_setTriesRetryCount = 0;
		SetTries();
		UnityEngine.Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		PsState.m_physicsPaused = false;
		PsState.m_activeMinigame.m_gameOn = true;
		PsState.m_activeMinigame.m_gameStartCount++;
		PsState.m_activeMinigame.m_playerReachedGoal = false;
		CreatePlaybackGhosts();
		SoundS.PlaySingleShot("/InGame/GameStart", Vector3.zero);
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GamePlayState());
		EveryplayManager.StartRecording();
	}

	protected virtual void SetTries()
	{
		new PsServerQueueFlow(delegate
		{
			Versus.SetTries(m_versusData.timedEventId, m_tryCount, SetTriesSUCCEED, SetTriesFAILED);
		}, null, new string[1] { "setTries" });
	}

	private void SetTriesSUCCEED(HttpC _c)
	{
		Debug.Log("SET TRIES SUCCEED");
	}

	private void SetTriesFAILED(HttpC _c)
	{
		Debug.Log("SET TRIES FAILED");
		if (m_setTriesRetryCount < 3)
		{
			Versus.SetTries(m_versusData.timedEventId, m_tryCount, SetTriesSUCCEED, SetTriesFAILED);
		}
		else
		{
			ServerManager.ThrowServerErrorException("Connection error", _c.www, () => Versus.SetTries(m_versusData.timedEventId, m_tryCount, SetTriesSUCCEED, SetTriesFAILED));
		}
		m_setTriesRetryCount++;
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
		(m_gameMode as PsGameModeRaceVersus).m_previousBest = (m_gameMode as PsGameModeRaceVersus).m_bestVersusTime;
		EveryplayManager.StopRecording(0f);
		PauseMinigame();
		if (m_tryCount >= PsState.m_versusChallengeTryAmount && m_outOfTriesPopup == null)
		{
			m_outOfTriesPopup = new PsUIBasePopup(typeof(PsUIVersusOutOfTriesPopup));
			m_outOfTriesPopup.SetAction("Continue", NextRound);
			m_outOfTriesPopup.SetAction("Forfeit", SendForfeit);
		}
		else if (m_gameMode.m_waitForHighscoreAndGhost || m_gameMode.m_waitForNextGhost)
		{
			PsUIBasePopup psUIBasePopup = new PsUIBasePopup(typeof(PsUICenterWaitForHighscoresAndGhost));
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
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		CameraS.RemoveAllTargetComponents();
		PsState.m_activeMinigame.m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
		InitializeMinigame();
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameStartState());
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

	public virtual void NextRound()
	{
		base.ExitMinigame();
		Cancel();
		PsState.m_activeMinigame.m_gameStarted = false;
		EveryplayManager.StopRecording(0f);
		PsIngameMenu.CloseAll();
		PsMetagameManager.HideResources();
		bool winTrue = m_versusWon || (m_versusData.GetOpponentInfo() == null && (m_versusData.GetOwnInfo().score.time > 0 || (m_gameMode as PsGameModeRaceVersus).m_bestGhostBlob.data != null));
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 1f, delegate
		{
			if (!(m_gameMode as PsGameModeRaceVersus).m_totalWin && !m_roundInfoSent)
			{
				SendQuit(winTrue);
			}
			PsPlanetManager.m_timedEvents = null;
			PsMetagameManager.HideResources();
			m_versusData.currentPlayer = ((m_versusData.GetOpponentInfo() == null) ? string.Empty : "opponent");
			Action action = delegate
			{
				Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
			};
			action();
		});
	}

	protected virtual void SendQuit(bool _winTrue)
	{
		Versus.Quit(m_timedEventId, m_minigameId, PsState.m_activeMinigame.m_gameStartCount, _winTrue, m_tryCount >= PsState.m_versusChallengeTryAmount && !_winTrue, VersusQuitSUCCEED, VersusQuitFAILED);
	}

	public void ForfeitChallenge()
	{
		Cancel();
		if (m_forfeitPopup == null)
		{
			PsState.m_activeMinigame.SetGamePaused(true);
			PauseElapsedTimer();
			m_forfeitPopup = new PsUIBasePopup(typeof(PsUIVersusForfeitPopup));
			m_forfeitPopup.SetAction("Forfeit", SendForfeit);
			m_forfeitPopup.SetAction("Cancel", Cancel);
		}
	}

	public override void ExitMinigame()
	{
		if (m_exitPopup == null)
		{
			m_exitPopup = new PsUIBasePopup(typeof(PsUIVersusExitPopup));
			m_exitPopup.SetAction("Exit", ExitMinigameVersus);
			m_exitPopup.SetAction("Continue", NextRound);
			m_exitPopup.SetAction("Forfeit", ForfeitChallenge);
		}
	}

	public void Cancel()
	{
		if (m_forfeitPopup != null)
		{
			m_forfeitPopup.Destroy();
			m_forfeitPopup = null;
		}
		if (m_exitPopup != null)
		{
			m_exitPopup.Destroy();
			m_exitPopup = null;
		}
		if (m_outOfTriesPopup != null)
		{
			m_outOfTriesPopup.Destroy();
			m_outOfTriesPopup = null;
		}
	}

	public virtual void SendForfeit()
	{
		base.ExitMinigame();
		Cancel();
		PsState.m_activeMinigame.m_gameStarted = false;
		EveryplayManager.StopRecording(0f);
		PsIngameMenu.CloseAll();
		PsMetagameManager.HideResources();
		PsMetagameManager.VersusChallengeRewards(false);
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 1f, delegate
		{
			if (m_versusData.GetOpponentInfo() == null)
			{
				Versus.Claim(m_timedEventId, ClaimSUCCEED, ClaimFAILED);
			}
			else
			{
				Versus.Forfeit(m_timedEventId, m_minigameId, PsState.m_activeMinigame.m_gameStartCount, ForfeitSUCCEED, ForfeitFAILED);
			}
			PsPlanetManager.m_timedEvents = null;
			PsMetagameManager.HideResources();
			Action action = delegate
			{
				Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
			};
			m_eventOver = true;
			CreateClaimPopup(action);
		});
	}

	public virtual void ExitMinigameVersus()
	{
		base.ExitMinigame();
		Cancel();
		PsState.m_activeMinigame.m_gameStarted = false;
		EveryplayManager.StopRecording(0f);
		PsIngameMenu.CloseAll();
		PsMetagameManager.HideResources();
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameWin, 1f, delegate
		{
			if (m_sendQuit)
			{
				Versus.Quit(m_timedEventId, m_minigameId, PsState.m_activeMinigame.m_gameStartCount, false, false, VersusQuitSUCCEED, VersusQuitFAILED);
			}
			if (m_createFlyingNode)
			{
				m_addToFloaters = true;
			}
			PsPlanetManager.m_timedEvents = null;
			PsMetagameManager.HideResources();
			Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
		});
	}

	private void CreateClaimPopup(Action _continue)
	{
		m_versusData.winner = "opponent";
		PsUIBasePopup popup = new PsUIBasePopup(typeof(PsUICenterVersusClaimCurrent));
		popup.SetAction("Exit", delegate
		{
			popup.Destroy();
			_continue();
		});
	}

	private void ClaimSUCCEED(HttpC _c)
	{
		Debug.Log("Claim succeed");
	}

	private void ClaimFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", _c.www, delegate
		{
			HttpC httpC = Versus.Claim((string)_c.objectData, ClaimSUCCEED, ClaimFAILED);
			httpC.objectData = _c.objectData;
			return httpC;
		});
	}

	private void ForfeitSUCCEED(HttpC _c)
	{
		Debug.Log("FORFEIT SUCCEED");
	}

	private void ForfeitFAILED(HttpC _c)
	{
		Debug.Log("FORFEIT FAILED");
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Versus.Forfeit(m_timedEventId, m_minigameId, PsState.m_activeMinigame.m_gameStartCount, ForfeitSUCCEED, ForfeitFAILED));
	}

	private void VersusQuitSUCCEED(HttpC _c)
	{
		Debug.Log("VERSUS QUIT SUCCEED");
	}

	private void VersusQuitFAILED(HttpC _c)
	{
		ServerManager.ThrowServerErrorException("Connection error", ServerErrors.GetNetworkError(_c.www.error), () => Versus.Quit(m_timedEventId, m_minigameId, PsState.m_activeMinigame.m_gameStartCount, m_versusWon, m_tryCount >= PsState.m_versusChallengeTryAmount && !m_versusWon, VersusQuitSUCCEED, VersusQuitFAILED));
	}

	public override void DestroyMinigame()
	{
		base.DestroyMinigame();
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		CameraS.RemoveAllTargetComponents();
		LevelManager.DestroyCurrentLevel();
		PsState.m_activeMinigame = null;
		if (!m_createFlyingNode)
		{
			ReleaseLoop();
		}
	}

	public override void BackAtMenu()
	{
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
			if (m_previousLoop != null)
			{
				m_previousLoop.SetAsActiveLoop();
			}
			m_previousLoop = null;
		}
	}
}
