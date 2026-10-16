public class TimerC : BasicComponent
{
	public static int m_componentCount;

	public string name;

	public double currentTime;

	public double startTime;

	public double duration;

	public double delay;

	public bool isDone;

	public bool destroyEntityOnTimeout;

	public IComponent customComponent;

	public object customObject;

	public TimerComponentDelegate timeoutHandler;

	public TimerC()
		: base(ComponentType.Timer)
	{
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
		customComponent = null;
		customObject = null;
	}

	~TimerC()
	{
		m_componentCount--;
	}
}
