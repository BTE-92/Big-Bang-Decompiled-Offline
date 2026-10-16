using System.Collections.Generic;

public class ReplayItem
{
	public string m_className;

	public List<ReplaySnapshot> m_snapshots;

	public int m_currentSnapshotIndex;

	public List<ReplayEventFrame> m_events;

	public int m_currentEventIndex;

	public ReplayItem(string _className)
	{
		m_className = _className;
		m_snapshots = new List<ReplaySnapshot>();
		m_currentSnapshotIndex = 0;
		m_events = new List<ReplayEventFrame>();
		m_currentEventIndex = 0;
	}
}
