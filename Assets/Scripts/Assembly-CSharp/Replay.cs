using System.Collections;
using System.Collections.Generic;

public class Replay
{
	public string m_levelId;

	public Hashtable m_data;

	public List<ReplayItem> m_items;

	public Replay(string _levelId)
	{
		m_levelId = _levelId;
		m_data = new Hashtable();
		m_items = new List<ReplayItem>();
	}
}
