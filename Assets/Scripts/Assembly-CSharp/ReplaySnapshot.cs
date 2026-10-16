using System.Collections.Generic;

public class ReplaySnapshot
{
	public int m_tick;

	public List<ReplayState> m_states;

	public bool m_alive;

	public bool m_electrified;

	public int m_boostTicks;

	public ReplaySnapshot()
	{
		m_states = new List<ReplayState>();
	}
}
