public class AgTileC : BasicComponent
{
	public AgTile m_tile;

	public AgTileC()
		: base(ComponentType.None)
	{
		Reset();
	}

	public override void Reset()
	{
		base.Reset();
	}

	public override void Destroy()
	{
		base.Destroy();
		m_tile = null;
	}
}
