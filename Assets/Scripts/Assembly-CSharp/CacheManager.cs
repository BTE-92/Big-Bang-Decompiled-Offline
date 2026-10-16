using System.Collections.Generic;

public static class CacheManager
{
	private static List<ICache> m_caches;

	public static void Initialize()
	{
		m_caches = new List<ICache>();
	}

	public static void AddCache(ICache _cache)
	{
		m_caches.Add(_cache);
	}

	public static ICache GetCache(string _name)
	{
		foreach (ICache cache in m_caches)
		{
			if (cache.GetName().Equals(_name))
			{
				return cache;
			}
		}
		return null;
	}

	public static void Update()
	{
		for (int i = 0; i < m_caches.Count; i++)
		{
			ICache cache = m_caches[i];
			cache.Update();
		}
	}
}
