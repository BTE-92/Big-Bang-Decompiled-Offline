using System.Collections.Generic;

public class PsUnlock
{
	public string m_name;

	public List<PsUnlockable> m_unlockables;

	public List<MetagameNodeData> m_levels;

	public MetagameNodeData m_introLevel;

	public bool m_unlocked;

	public PsUnlock(string _identifier)
	{
		m_name = _identifier;
		m_unlockables = new List<PsUnlockable>();
		m_levels = new List<MetagameNodeData>();
	}
}
