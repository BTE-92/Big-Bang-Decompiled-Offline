public class MonoBehaviourC : BasicComponent
{
	public static int m_componentCount;

	public IMonoBehaviour m_monoBehaviour;

	public MonoBehaviourC()
		: base(ComponentType.MonoBehaviour)
	{
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
		m_monoBehaviour = null;
	}

	~MonoBehaviourC()
	{
		m_componentCount--;
	}
}
