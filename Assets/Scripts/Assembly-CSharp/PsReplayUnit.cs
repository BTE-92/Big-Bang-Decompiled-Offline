public class PsReplayUnit : ReplayItem
{
	public Unit m_unit;

	public PsReplayUnit(Unit _unit)
		: base(_unit.GetType().ToString())
	{
		m_unit = _unit;
	}
}
