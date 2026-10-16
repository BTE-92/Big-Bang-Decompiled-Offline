using UnityEngine;

public class PsBlackScene : IScene, IStatedObject
{
	private string _name;

	private StateMachine _stateMachine;

	private bool _initComplete;

	private int m_counter;

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

	public PsBlackScene(string _sceneName, bool _restart = false)
	{
	}

	public void Load()
	{
		Initialize();
	}

	public IState GetCurrentState()
	{
		return m_stateMachine.GetCurrentState();
	}

	public void Reset()
	{
		Destroy();
		Load();
	}

	public void Update()
	{
		if (m_counter != 60)
		{
			m_counter++;
			if (m_counter == 60)
			{
				Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLoadingScene(Color.black));
			}
		}
	}

	public void Destroy()
	{
	}

	public void Initialize()
	{
		_initComplete = true;
	}
}
