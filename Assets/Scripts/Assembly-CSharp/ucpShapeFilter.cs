public struct ucpShapeFilter
{
	public uint group;

	public uint categories;

	public uint mask;

	public void ucpShapeFilterAll()
	{
		group = 0u;
		categories = uint.MaxValue;
		mask = uint.MaxValue;
	}

	public void ucpShapeFilterNone()
	{
		group = 0u;
		categories = 0u;
		mask = 0u;
	}
}
