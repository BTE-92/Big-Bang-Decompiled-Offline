public class StartupScene : IScene, IStatedObject
{
	private string _name;

	private StateMachine _stateMachine;

	private bool _initComplete;

	private PsUILoadingText m_loadingText;

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

	public StartupScene(string _sceneName)
	{
		m_name = _sceneName;
	}

	public IState GetCurrentState()
	{
		return m_stateMachine.GetCurrentState();
	}

	public void Load()
	{
		if (PsState.m_activeGameLoop != null)
		{
			PsState.m_activeGameLoop.CleanLoop();
		}
		PsState.m_activeGameLoop = null;
		PsState.m_activeMinigame = null;
		PsState.m_UIComponentsHidden = false;
		for (int num = PsState.m_openPopups.Count - 1; num >= 0; num--)
		{
			PsState.m_openPopups[num].Destroy();
		}
		for (int num2 = PsState.m_dialogues.Count - 1; num2 >= 0; num2--)
		{
			PsState.m_dialogues[num2].Destroy();
		}
		ServerManager.m_connectingPopup = null;
		Initialize();
	}

	public void Initialize()
	{
		PsState.m_restarting = false;
		if ((KeyChain.GetUserId() != null || PlayerPrefsX.GetUserId() != null) && KeyChain.GetUserName() != null)
		{
			m_loadingText = new PsUILoadingText(null);
			m_loadingText.Update();
		}
		m_stateMachine = new StateMachine(this);
		m_stateMachine.ChangeState(new UserLoginState());
		_initComplete = true;
	}

	public void Reset()
	{
		Destroy();
		Load();
	}

	public void Update()
	{
		m_stateMachine.Update();
	}

	public void Destroy()
	{
		PsMetagameManager.DestroySplashBG();
		if (m_loadingText != null)
		{
			m_loadingText.Destroy();
			m_loadingText = null;
		}
		m_stateMachine.Destroy();
		EntityManager.RemoveAllEntities();
	}

	~StartupScene()
	{
	}
}
