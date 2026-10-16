public class GroundC : BasicComponent
{
	public Ground m_ground;

	public GroundC()
		: base((ComponentType)32)
	{
	}

	public override void Reset()
	{
		base.Reset();
		m_ground = null;
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
