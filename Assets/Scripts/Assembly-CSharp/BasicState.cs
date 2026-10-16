public class BasicState : IState
{
	private StateMachine _stateMachine;

	private string _tag = string.Empty;

	public StateMachine p_stateMachine
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

	public string m_tag
	{
		get
		{
			return _tag;
		}
		set
		{
			_tag = value;
		}
	}

	public virtual void Enter(IStatedObject _parent)
	{
	}

	public virtual void Execute()
	{
	}

	public virtual void Exit()
	{
	}
}
