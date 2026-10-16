public class CustomObjectC : BasicComponent
{
	public object m_customObject;

	public CustomObjectC()
		: base((ComponentType)31)
	{
	}

	public override void Reset()
	{
		base.Reset();
		m_customObject = null;
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
