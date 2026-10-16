using System.Collections.Generic;

public class ListCache<T> : ICache
{
	public string m_name;

	public bool m_expires;

	public double m_expireDurationMins;

	public double m_expirationTime;

	private List<CacheItem<T>> m_items;

	public ListCache(string _name, float _cacheExpireTimeMins = -1f)
	{
		CacheManager.AddCache(this);
		m_name = _name;
		m_expireDurationMins = _cacheExpireTimeMins;
		m_expires = ((m_expireDurationMins > 0.0) ? true : false);
		if (m_expires)
		{
			m_expirationTime = Main.m_gameTimeSinceAppStarted + m_expireDurationMins * 60.0;
		}
		m_items = new List<CacheItem<T>>();
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
		m_items.Add(_item);
		Refresh();
	}

	public void AddItems(string[] _keys, T[] _items)
	{
		for (int i = 0; i < _items.Length; i++)
		{
			AddItem(_keys[i], _items[i]);
		}
	}

	public CacheItem<T> GetItem(string _key)
	{
		for (int i = 0; i < m_items.Count; i++)
		{
			if (m_items[i].m_key == _key)
			{
				return m_items[i];
			}
		}
		return null;
	}

	public int GetItemCount()
	{
		return m_items.Count;
	}

	public T GetContent(int _index)
	{
		CacheItem<T> cacheItem = m_items[_index];
		if (cacheItem == null)
		{
			return default(T);
		}
		return cacheItem.m_content;
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

	public T[] GetContents()
	{
		if (m_items.Count == 0)
		{
			return null;
		}
		T[] array = new T[m_items.Count];
		for (int i = 0; i < m_items.Count; i++)
		{
			array[i] = m_items[i].m_content;
		}
		return array;
	}

	public void RemoveItem(int _index)
	{
		RemoveItem(m_items[_index]);
	}

	public void RemoveItem(string _key)
	{
		for (int i = 0; i < m_items.Count; i++)
		{
			if (m_items[i].m_key == _key)
			{
				RemoveItem(m_items[i]);
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
