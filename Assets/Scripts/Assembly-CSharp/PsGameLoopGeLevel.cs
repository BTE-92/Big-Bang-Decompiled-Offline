using System;
using UnityEngine;

public class PsGameLoopGeLevel : PsGameLoop
{
	public PsGameLoopGeLevel(string _minigameId, MinigameSearchParametres _searchParametres)
		: base(PsMinigameContext.Admin, _minigameId, null, -1, -1, 0, false)
	{
		m_minigameSearchParametres = _searchParametres;
	}

	public override void StartLoop()
	{
		if (PsState.m_activeGameLoop == null)
		{
			SetAsActiveLoop();
			PsLoadingScreen.FadeIn(delegate
			{
				LoadMinigame();
			});
		}
	}

	public override void LoadMinigame()
	{
		if (m_loadingMetaData)
		{
			m_loadMetadataCallback = (Action)Delegate.Combine(m_loadMetadataCallback, new Action(LoadMinigame));
			return;
		}
		base.LoadMinigame();
		LoadMinigameBytes();
	}

	protected override void LoadMetaDataSUCCESS(PsMinigameMetaData _minigameMetaData)
	{
		m_loadingMetaData = false;
		m_minigameMetaData = _minigameMetaData;
		m_minigameId = _minigameMetaData.id;
		if (_minigameMetaData.gameMode == PsGameMode.Race)
		{
			m_gameMode = new PsGameModeRace(this);
		}
		else if (_minigameMetaData.gameMode == PsGameMode.StarCollect)
		{
			m_gameMode = new PsGameModeStarCollect(this);
		}
		m_scoreCurrent = 0;
		m_timeScoreCurrent = int.MaxValue;
		m_timeScoreBest = _minigameMetaData.timeScore;
		if (m_timeScoreBest == 0)
		{
			m_timeScoreBest = int.MaxValue;
		}
		m_timeScoreOld = m_timeScoreBest;
		m_scoreBest = _minigameMetaData.score;
		m_scoreOld = m_scoreBest;
		m_rewardOld = m_scoreBest;
		Debug.LogWarning("BEST TIME: " + HighScores.TimeScoreToTime(m_timeScoreBest));
		Debug.LogWarning("BEST SCORE: " + m_scoreBest);
		LoadMinigameBytes();
	}

	protected override void MinigameDownloadOk(byte[] _levelData)
	{
		base.MinigameDownloadOk(_levelData);
		Main.m_currentGame.m_sceneManager.ChangeScene(new GameScene("GameScene"));
	}

	public override void EnterMinigame()
	{
		m_backAtMenu = false;
		CameraS.m_updateComponents = true;
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GameStartState());
		PsLoadingScreen.FadeOut();
	}

	public override void InitializeMinigame()
	{
		base.InitializeMinigame();
		PsIngameMenu.OpenController();
	}

	public override void StartMinigame()
	{
		UnityEngine.Random.seed = (int)(Main.m_gameTimeSinceAppStarted * 60.0);
		PsState.m_physicsPaused = false;
		PsState.m_activeMinigame.m_gameStarted = true;
		PsState.m_activeMinigame.m_gameStartCount++;
		PsState.m_activeMinigame.m_playerReachedGoal = false;
		CreatePlaybackGhosts();
		SoundS.PlaySingleShot("/InGame/GameStart", Vector3.zero);
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new GamePlayState());
	}

	public override void PauseMinigame()
	{
		PsState.m_activeMinigame.SetGamePaused(true);
		PauseElapsedTimer();
		PsIngameMenu.CloseAll();
	}

	public override void ResumeMinigame()
	{
		PsState.m_activeMinigame.SetGamePaused(false);
		ResumeElapsedTimer();
	}

	public override void RestartMinigame()
	{
		base.RestartMinigame();
		EntityManager.RemoveEntitiesByTag("GTAG_AUTODESTROY");
		CameraS.RemoveAllTargetComponents();
		(LevelManager.m_currentLevel as Minigame).m_groundNode.RevertGroundFromPlay();
		LevelManager.ResetCurrentLevel();
		InitializeMinigame();
		CameraS.SnapMainCameraFrame();
	}

	public override void LoseMinigame()
	{
		StopEveryplayAfterDelay();
		PsIngameMenu.CloseAll();
		PsState.m_activeMinigame.m_gameEnded = true;
		PsState.m_activeMinigame.m_gameDeathCount++;
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new PsGameCardState(ExitMinigameAction, null));
	}

	public override void WinMinigame()
	{
		base.WinMinigame();
		PsIngameMenu.CloseAll();
		PsState.m_activeMinigame.m_gameEnded = true;
		PsState.m_activeMinigame.m_playerReachedGoal = true;
		m_gameMode.m_exitingMinigame = false;
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new PsGameCardState(ExitMinigameAction, null));
	}

	public override void SaveProgression(bool _save = true)
	{
	}

	private void ExitMinigameAction()
	{
		ExitMinigame();
	}

	public override void ExitMinigame()
	{
		base.ExitMinigame();
		EveryplayManager.StopRecording(0f);
		m_gameMode.m_exitingMinigame = true;
		Main.m_currentGame.m_sceneManager.ChangeScene(new GeScene("NodeEditorScene"), new FadeLoadingScene(Color.black));
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
		m_backAtMenu = true;
	}
}
