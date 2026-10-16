using System;

public class GameScene : IScene, IStatedObject
{
	private string _name;

	private StateMachine _stateMachine = new StateMachine();

	private bool _initComplete;

	public static Type m_returnSceneType;

	private int mainTicker;

	public static bool m_resetting;

	public string m_name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public StateMachine m_stateMachine
	{
		get
		{
			return _stateMachine;
		}
		set
		{
			_stateMachine = value;
		}
	}

	public bool m_initComplete
	{
		get
		{
			return _initComplete;
		}
	}

	public GameScene(string _sceneName, Type _returnSceneType = null)
	{
		PsState.m_UIComponentsHidden = false;
		m_returnSceneType = _returnSceneType;
		m_name = _sceneName;
	}

	public void Load()
	{
		PsCaches.m_publishedList.Clear();
		PsResources.LoadGameResources();
		Initialize();
		EveryplayManager.ClearRecording();
	}

	public static void InitGame()
	{
	}

	public static void ResetGame()
	{
	}

	public IState GetCurrentState()
	{
		return m_stateMachine.GetCurrentState();
	}

	public void Reset()
	{
	}

	public void Initialize()
	{
		if (PsState.m_activeGameLoop != null)
		{
			PsState.m_activeGameLoop.EnteredMinigameScene();
		}
		_initComplete = true;
	}

	public void Update()
	{
		mainTicker++;
		m_stateMachine.Update();
		if (PsState.m_activeGameLoop.m_context == PsMinigameContext.Signal)
		{
			GameSceneEffectManager.Update();
		}
	}

	public void Destroy()
	{
		PsState.m_activeGameLoop.DestroyMinigame();
		PsIngameMenu.CloseAll();
		PsMetagameManager.HideResources();
		m_stateMachine.Destroy();
		PsResources.UnloadGameResources();
	}

	~GameScene()
	{
	}
}
