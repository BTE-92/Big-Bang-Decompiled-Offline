using Server;

public class UserLoginState : BasicState
{
	private StartupScene m_parent;

	public bool m_loginComplete;

	public override void Enter(IStatedObject _parent)
	{
		m_parent = _parent as StartupScene;
		PsMetagameData.Initialize();
		if (PsPlanetManager.GetCurrentPlanet() == null)
		{
			PsPlanetManager.AddPlanet(new PsPlanet());
		}
		PsPlanetManager.GetCurrentPlanet().LoadPersistentPathData();
		m_loginComplete = false;
		ServerManager.Login();
	}

	public override void Execute()
	{
		if (ServerManager.m_playerAuthenticated && LoginFlow.isCompleted && !m_loginComplete)
		{
			m_loginComplete = true;
			PsState.m_inStartupSequence = false;
			m_parent.m_stateMachine.ChangeState(new InitializeMetagameState());
		}
	}

	public override void Exit()
	{
	}
}
