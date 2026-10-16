public class UnitC : BasicComponent
{
	public Unit m_unit;

	public UnitC()
		: base((ComponentType)30)
	{
	}

	public override void Reset()
	{
		base.Reset();
		m_unit = null;
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
