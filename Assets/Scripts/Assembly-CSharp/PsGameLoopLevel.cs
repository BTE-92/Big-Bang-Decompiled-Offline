using System;
using System.Collections;
using UnityEngine;

public class PsGameLoopLevel : PsGameLoop
{
	private PsUIBasePopup m_waitingPopup;

	private bool m_reportPerformance;

	public PsGameLoopLevel(string _minigameId, MinigameSearchParametres _searchParametres, PsPlanetPath _path, int _id, int _levelNumber, int _score, bool _unlocked, string[] _medalTimes = null)
		: base(PsMinigameContext.Level, _minigameId, _path, _id, _levelNumber, _score, _unlocked, _medalTimes)
	{
		m_minigameSearchParametres = _searchParametres;
		m_minigameSearchParametres.m_playerUnitFilter = "Motorcycle";
		m_minigameSearchParametres.m_gameMode = PsGameMode.Race;
	}

	public PsGameLoopLevel(PsMinigameContext _context, string _minigameId, MinigameSearchParametres _searchParametres, PsPlanetPath _path, int _id, int _levelNumber, int _score, bool _unlocked, string[] _medalTimes = null)
		: base(_context, _minigameId, _path, _id, _levelNumber, _score, _unlocked, _medalTimes)
	{
		m_minigameSearchParametres = _searchParametres;
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
		LoadMinigameBytes();
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
		Debug.LogWarning("ENTER MINIGAME");
		PsGameLoop nodeInfo = m_path.GetNodeInfo(m_nodeId + 1);
		if (nodeInfo != null && nodeInfo.m_minigameMetaData == null)
		{
			nodeInfo.LoadMinigameMetaData();
		}
		CameraS.m_updateComponents = true;
		CameraS.m_zoomMultipler = 2f;
		CameraS.m_zoomNeutralizer = 0.985f;
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

	public override void StartMinigame()
	{
		PsState.m_activeMinigame.m_gameStarted = true;
		m_gameMode.StartMinigame();
		UnityEngine.Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		PsState.m_physicsPaused = false;
		PsState.m_activeMinigame.m_gameOn = true;
		PsState.m_activeMinigame.m_gameStartCount++;
		PsState.m_activeMinigame.m_playerReachedGoal = false;
		CreatePlaybackGhosts();
		SoundS.PlaySingleShot("/InGame/GameStart", Vector3.zero);
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GamePlayState());
		EveryplayManager.StartRecording();
		CameraS.m_zoomNeutralizer = 0.97f;
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
		ResumeElapsedTimer();
		PsState.m_activeMinigame.SetGamePaused(false);
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
		if ((m_timeScoreBest == 0 || m_timeScoreBest == int.MaxValue) && m_scoreOld == 0)
		{
			m_reportPerformance = true;
		}
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
			SaveProgression();
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
				PsMetagameManager.SetPlayerData(new Hashtable(), false, PsMetagameManager.PlayerDataSetSUCCEED, PsMetagameManager.PlayerDataSetFAILED);
			}
			PsMetagameManager.HideResources();
			Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLevelEndLoadingScene(Color.black, this));
		});
	}

	public override void SkipMinigame()
	{
		base.SkipMinigame();
		PsIngameMenu.CloseAll();
		SaveSkipProgression();
		Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLoadingScene(Color.black));
	}

	public void CloseGameCard()
	{
		PsIngameMenu.CloseAll();
		Main.m_currentGame.m_currentScene.m_stateMachine.RevertToPreviousState();
		ReleaseLoop();
	}

	public void SaveSkipProgression()
	{
		m_scoreBest = (m_scoreCurrent = 1);
		m_path.m_currentNodeId = m_nodeId + 1;
		if (m_sidePath != null && m_sidePath.m_currentNodeId < 1)
		{
			m_sidePath.m_currentNodeId = 1;
		}
		if (m_path.m_name == "MainPath")
		{
			Hashtable progressionJSON = ClientTools.GenerateProgressionPathJson(this, m_path.m_currentNodeId);
			PsMetagameManager.SaveProgression(progressionJSON, true);
		}
		else
		{
			Hashtable progressionJSON2 = ClientTools.GenerateProgressionPathJson(m_path);
			PsMetagameManager.SaveProgression(progressionJSON2, false);
		}
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
		if (!flag)
		{
			return;
		}
		if (m_path.m_name == "MainPath")
		{
			Hashtable progressionJSON = ClientTools.GenerateProgressionPathJson(this, m_path.m_currentNodeId);
			PsMetagameManager.SaveProgression(progressionJSON, sendMetrics);
			if (m_nodeId == 2 && m_reportPerformance)
			{
				PsMetrics.ReportPerformance(PsState.m_activeMinigame.m_gameTicks / 60f / PsState.m_activeMinigame.m_realTimeSpent);
				m_reportPerformance = false;
			}
		}
		else
		{
			Hashtable progressionJSON2 = ClientTools.GenerateProgressionPathJson(m_path);
			PsMetagameManager.SaveProgression(progressionJSON2, false);
		}
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
		TimerS.RemoveComponent(_c);
		ReleaseLoop();
	}

	public override void ReleaseLoop()
	{
		if (m_released)
		{
			return;
		}
		base.ReleaseLoop();
		Debug.LogStackTrace();
		PsMetagameManager.ShowRateAppIfPossible(this);
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
