public class PreloadingScene : IScene, IStatedObject
{
	private string _name = "PreloadingScene";

	private StateMachine _stateMachine;

	private bool _initComplete;

	private IScene m_toScene;

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

	public PreloadingScene(IScene _toScene)
	{
		m_toScene = _toScene;
	}

	public IState GetCurrentState()
	{
		return m_stateMachine.GetCurrentState();
	}

	public void Load()
	{
		PsState.m_UIComponentsHidden = false;
		Initialize();
	}

	public void Initialize()
	{
		PsMetagameManager.CreateSplashBG();
		m_stateMachine = new StateMachine(this);
		m_stateMachine.ChangeState(new PsUIBaseState(typeof(PsUIPreloaderCenter)));
		_initComplete = true;
	}

	public void NextScene()
	{
		SoundS.Initialize(CameraS.m_mainCamera.gameObject);
		Main.m_currentGame.m_sceneManager.ChangeScene(m_toScene);
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
		m_stateMachine.Destroy();
		EntityManager.RemoveAllEntities();
	}

	~PreloadingScene()
	{
	}
}
