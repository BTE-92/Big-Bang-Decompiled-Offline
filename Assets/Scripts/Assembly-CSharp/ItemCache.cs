using System.Collections.Generic;

public class ItemCache<T> : ICache
{
	public string m_name;

	public int m_maxItems;

	public bool m_expires;

	public double m_expireDurationMins;

	public double m_expirationTime;

	public LinkedList<CacheItem<T>> m_items;

	public ItemCache(string _name, int _maxItems = int.MaxValue, float _cacheExpireTimeMins = -1f)
	{
		CacheManager.AddCache(this);
		m_name = _name;
		m_maxItems = _maxItems;
		m_expireDurationMins = _cacheExpireTimeMins;
		m_expires = ((m_expireDurationMins > 0.0) ? true : false);
		if (m_expires)
		{
			m_expirationTime = Main.m_gameTimeSinceAppStarted + m_expireDurationMins * 60.0;
		}
		m_items = new LinkedList<CacheItem<T>>();
	}

	public string GetName()
	{
		return m_name;
	}

	public CacheItem<T> AddItem(string _key, T _content)
	{
		CacheItem<T> cacheItem = new CacheItem<T>(_key, _content, -1f);
		AddItem(_key, cacheItem);
		return cacheItem;
	}

	public void AddItem(string _key, CacheItem<T> _item)
	{
		RemoveItem(_key);
		if (m_items.Count >= m_maxItems)
		{
			m_items.RemoveFirst();
		}
		m_items.AddLast(_item);
	}

	public CacheItem<T> GetItem(string _key)
	{
		CacheItem<T> cacheItem = null;
		foreach (CacheItem<T> item in m_items)
		{
			if (item.m_key.Equals(_key))
			{
				cacheItem = item;
				break;
			}
		}
		if (cacheItem != null)
		{
			m_items.Remove(cacheItem);
			m_items.AddLast(cacheItem);
			cacheItem.Refresh();
		}
		return cacheItem;
	}

	public int GetItemCount()
	{
		return m_items.Count;
	}

	public T GetContent(string _key)
	{
		CacheItem<T> item = GetItem(_key);
		if (item == null)
		{
			return default(T);
		}
		return item.GetContent();
	}

	public void RemoveItem(string _key)
	{
		foreach (CacheItem<T> item in m_items)
		{
			if (item.m_key.Equals(_key))
			{
				RemoveItem(item);
				break;
			}
		}
	}

	public void RemoveItem(CacheItem<T> _item)
	{
		m_items.Remove(_item);
	}

	public void Clear()
	{
		m_items.Clear();
	}

	public void Refresh()
	{
		if (m_expires)
		{
			m_expirationTime = Main.m_gameTimeSinceAppStarted + m_expireDurationMins * 60.0;
		}
	}

	public void Update()
	{
		if (m_expires && Main.m_gameTimeSinceAppStarted > m_expirationTime)
		{
			Clear();
			Refresh();
		}
	}
}
