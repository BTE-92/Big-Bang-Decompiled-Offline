public interface IState
{
	string m_tag { get; set; }

	StateMachine p_stateMachine { get; set; }

	void Enter(IStatedObject _parent);

	void Execute();

	void Exit();
}
