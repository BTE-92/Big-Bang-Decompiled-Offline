public class CacheItem<T>
{
	public string m_key;

	public T m_content;

	private bool m_expires;

	private double m_expireDurationMins;

	private double m_expirationTime;

	public CacheItem(string _key, T _content, float _expireDurationMins = -1f)
	{
		m_key = _key;
		m_content = _content;
		m_expireDurationMins = _expireDurationMins;
		m_expires = ((m_expireDurationMins > 0.0) ? true : false);
		if (m_expires)
		{
			m_expirationTime = Main.m_gameTimeSinceAppStarted + m_expireDurationMins * 60.0;
		}
	}

	public T GetContent()
	{
		if (m_expires)
		{
			m_expirationTime = Main.m_gameTimeSinceAppStarted + m_expireDurationMins * 60.0;
		}
		return m_content;
	}

	public void SetContent(T _content)
	{
		if (m_expires)
		{
			m_expirationTime = Main.m_gameTimeSinceAppStarted + m_expireDurationMins * 60.0;
		}
		m_content = _content;
	}

	public bool IsExpired()
	{
		if (!m_expires)
		{
			return false;
		}
		return (Main.m_gameTimeSinceAppStarted > m_expirationTime) ? true : false;
	}

	public void Refresh()
	{
		if (m_expires)
		{
			m_expirationTime = Main.m_gameTimeSinceAppStarted + m_expireDurationMins * 60.0;
		}
	}
}
