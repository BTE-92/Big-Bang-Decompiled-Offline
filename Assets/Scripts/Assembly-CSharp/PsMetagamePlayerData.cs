using System.Collections;

public static class PsMetagamePlayerData
{
	public static Hashtable m_playerData = new Hashtable();

	public static bool m_playerDataUpdated = true;

	public static void UpdatePlayerData(Hashtable _newData)
	{
		if (_newData == null)
		{
			return;
		}
		foreach (string key in _newData.Keys)
		{
			if (m_playerData.ContainsKey(key))
			{
				m_playerData[key] = _newData[key];
			}
			else
			{
				m_playerData.Add(key, _newData[key]);
			}
		}
		m_playerDataUpdated = true;
	}
}
