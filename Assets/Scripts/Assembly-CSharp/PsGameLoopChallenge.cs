using System;
using System.Collections;
using Server;
using UnityEngine;

public class PsGameLoopChallenge : PsTimedEventLoop
{
	public PsGameLoopChallenge(PsMinigameContext _context, string _timedEventId, string _minigameId, PsPlanetPath _path, int _id, int _levelNumber, int _score, int _timeleft, int _duration, string _endTime = null, bool _eventOver = false, bool _domeDestroyed = false)
		: base(_context, _timedEventId, _minigameId, _path, _id, _levelNumber, _score, _timeleft, _duration, _endTime, _eventOver, _domeDestroyed)
	{
		m_freeConsumableUnlock = "Rent";
	}

	public override string GetBannerString()
	{
		if (IsEventOver() && !m_eventOver)
		{
			m_eventOver = true;
			m_timeRunOut = true;
			HandleEventOver();
		}
		if (m_eventOver && !m_timeRunOut)
		{
			return "Completed";
		}
		if (m_timeRunOut)
		{
			return "Expired";
		}
		string text = "Daily Gems";
		if (SecondsLeft() < 28801)
		{
			return text + "\n" + PsMetagameManager.GetTimeStringFromSeconds(SecondsLeft());
		}
		return text;
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
		SaveProgression();
	}

	public override void SaveProgression(bool _save = true)
	{
		bool flag = false;
		if (m_scoreBest > m_scoreOld)
		{
			flag = true;
			if (m_scoreBest == 3)
			{
				m_eventOver = true;
			}
		}
		if (flag)
		{
			Hashtable progressionJSON = ClientTools.GenerateProgressionFloatingPathJson(m_path);
			PsMetagameManager.SaveProgression(progressionJSON, false);
		}
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

	public override void CreateGameMode()
	{
		m_gameMode = new PsGameModeDiamondChallenge(this);
	}

	protected override void LoadMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		m_loadingMetaData = false;
		m_minigameMetaData = _minigameMetaData;
		m_minigameId = _minigameMetaData.id;
		if (m_path.m_name != "FloatingPath")
		{
			Debug.LogError("WRONG PATH!!!!!");
		}
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
		ResumeElapsedTimer();
		PsState.m_activeMinigame.SetGamePaused(false);
	}

	public override void RestartMinigame()
	{
		base.RestartMinigame();
		EveryplayManager.StopRecording(0f);
		if (m_gameMode.m_waitForHighscoreAndGhost)
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

	public override void ExitMinigame()
	{
		base.ExitMinigame();
		EveryplayManager.StopRecording(0f);
		m_gameMode.m_exitingMinigame = true;
		new PsMinigameDialogueFlow(this, PsNodeEventTrigger.MinigameExit, 0f, delegate
		{
			PsIngameMenu.CloseAll();
			if (PsState.m_activeMinigame.m_gameStartCount > 0 && m_sendQuit)
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

	public override void BackAtMenu()
	{
		if (!m_backAtMenu && !m_released)
		{
			m_backAtMenu = true;
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
}
