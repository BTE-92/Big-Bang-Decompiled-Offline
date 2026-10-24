using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class HttpS
{
	public static double m_localTimeOut = 20.0;

	public static DynamicArray<HttpC> m_components;

	private static List<HttpC> m_removeComponentList;

	public static bool m_gatesFreeze;

	public static void Initialize()
	{
		m_components = new DynamicArray<HttpC>();
		m_removeComponentList = new List<HttpC>();
	}

	public static HttpC AddGetComponent(string _url, string _tag, string[] _queueTags = null, bool _destroyAfterDone = true, Hashtable _headers = null)
	{
		return AddComponent(null, _url, _tag, null, _headers, _queueTags, _destroyAfterDone);
	}

	public static HttpC AddPostComponent(string _url, string _tag, Hashtable _postHeader = null, string[] _queueTags = null, bool _destroyAfterDone = true)
	{
		if (_postHeader == null)
		{
			_postHeader = defaultHeaders("application/octet-stream");
		}
		return AddComponent(null, _url, _tag, new byte[1], _postHeader, _queueTags, _destroyAfterDone);
	}

	public static HttpC AddPostComponent(string _url, string _tag, string _JSON, Hashtable _postHeader = null, string[] _queueTags = null, bool _destroyAfterDone = true)
	{
		if (_postHeader == null)
		{
			_postHeader = defaultHeaders("application/json");
		}
		return AddComponent(null, _url, _tag, Encoding.UTF8.GetBytes(_JSON), _postHeader, _queueTags, _destroyAfterDone);
	}

	public static HttpC AddPostComponent(string _url, string _tag, byte[] _postData = null, Hashtable _postHeader = null, string[] _queueTags = null, bool _destroyAfterDone = true, bool _disposeWhenDestroyed = true)
	{
		if (_postHeader == null)
		{
			_postHeader = defaultHeaders("application/octet-stream");
		}
		return AddComponent(null, _url, _tag, _postData, _postHeader, _queueTags, _destroyAfterDone, _disposeWhenDestroyed);
	}

	private static HttpC AddComponent(Entity _entity, string _url, string _tag, byte[] _postData = null, Hashtable _postHeader = null, string[] _queueTags = null, bool _destroyAfterDone = true, bool _disposeWhenDestroyed = true)
	{
		HttpC httpC = m_components.AddItem();
		httpC.tag = _tag;
		httpC.queueTags = _queueTags;
		httpC.url = _url;
		httpC.destroyAfterDone = _destroyAfterDone;
		httpC.disposeWhenDestroyed = _disposeWhenDestroyed;
		httpC.timeOut = Main.m_gameTimeSinceAppStarted + m_localTimeOut;
		httpC.wwwUrl = _url;
		httpC.wwwData = _postData;
		if (_postHeader != null)
		{
			httpC.wwwHeaders = new Dictionary<string, string>();
			foreach (string key in _postHeader.Keys)
			{
				httpC.wwwHeaders.Add(key, (string)_postHeader[key]);
			}
		}
		else
		{
			httpC.wwwHeaders = null;
		}
		if (_postData == null)
		{
			httpC.www = new GameWWW(_url, null, httpC.wwwHeaders);
		}
		else
		{
			httpC.www = new GameWWW(_url, _postData, httpC.wwwHeaders);
		}
		if (_entity != null)
		{
			EntityManager.AddComponentToEntity(_entity, httpC);
		}
		else
		{
			httpC.m_active = true;
			httpC.m_wasActive = true;
		}
		return httpC;
	}

	public static void RemoveComponent(HttpC _c)
	{
		if (_c.www != null)
		{
			_c.www = null;
		}
		EntityManager.RemoveComponentFromEntity(_c);
		m_components.RemoveItem(_c);
	}

	public static bool QueueTagsExist(string[] _tags)
	{
		if (m_gatesFreeze || ((_tags == null || _tags.Length == 0) && m_components.m_aliveCount > 0))
		{
			int num = m_components.m_array.Length - 1;
			if (num > -1)
			{
				HttpC httpC = m_components.m_array[num];
				if (httpC == null || !httpC.m_active || httpC.ignoreInQueue)
				{
					return false;
				}
			}
			return true;
		}
		if ((_tags == null || _tags.Length == 0) && m_components.m_aliveCount == 0)
		{
			return false;
		}
		for (int num2 = m_components.m_array.Length - 1; num2 > -1; num2--)
		{
			HttpC httpC2 = m_components.m_array[num2];
			if (httpC2 != null && httpC2.m_active && httpC2.queueTags != null && !httpC2.ignoreInQueue)
			{
				for (int i = 0; i < httpC2.queueTags.Length; i++)
				{
					for (int j = 0; j < _tags.Length; j++)
					{
						if (httpC2.queueTags[i].IndexOf(_tags[j]) >= 0)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public static void Update()
	{
		double gameTimeSinceAppStarted = Main.m_gameTimeSinceAppStarted;
		int num = m_components.m_aliveCount - 1;
		while (num > -1 && m_components.m_aliveCount > 0)
		{
			HttpC httpC = m_components.m_array[m_components.m_aliveIndices[num]];
			if (httpC.m_active && httpC.www != null)
			{
				if (httpC.www.isDone)
				{
					if (!string.IsNullOrEmpty(httpC.www.error) && !httpC.executed)
					{
						Debug.LogWarning("WWWRequest error " + httpC.www.error + " from " + httpC.url);
						httpC.RequestFailed();
					}
					else if (httpC.debugKeyCode != KeyCode.None)
					{
						if (Input.GetKey(httpC.debugKeyCode))
						{
							httpC.RequestComplete();
							httpC.destroyAfterDone = true;
						}
					}
					else
					{
						httpC.RequestComplete();
					}
					if (httpC.destroyAfterDone && httpC.canRemove && !m_gatesFreeze && m_components.m_aliveCount > 0)
					{
						m_removeComponentList.Add(httpC);
					}
				}
				else if (!(httpC.www.progress > 0f) && gameTimeSinceAppStarted > httpC.timeOut)
				{
					if (!httpC.executed)
					{
						Debug.LogWarning("WWWRequest locally timed out from " + httpC.url);
					}
					httpC.RequestFailed();
					if (httpC.destroyAfterDone && httpC.canRemove && !m_gatesFreeze)
					{
						m_removeComponentList.Add(httpC);
					}
				}
			}
			num--;
		}
		while (m_removeComponentList.Count > 0)
		{
			int index = m_removeComponentList.Count - 1;
			RemoveComponent(m_removeComponentList[index]);
			m_removeComponentList.RemoveAt(index);
		}
		m_components.Update();
	}

	public static void ClearAllRequests(HttpC _exception = null)
	{
		for (int num = m_components.m_aliveCount - 1; num > -1; num--)
		{
			HttpC httpC = m_components.m_array[m_components.m_aliveIndices[num]];
			if ((_exception == null || httpC != _exception) && httpC.m_active && httpC.www != null)
			{
				m_removeComponentList.Add(httpC);
			}
		}
		while (m_removeComponentList.Count > 0)
		{
			int index = m_removeComponentList.Count - 1;
			RemoveComponent(m_removeComponentList[index]);
			m_removeComponentList.RemoveAt(index);
		}
		m_components.Update();
	}

	private static Hashtable defaultHeaders(string _contentType)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("Content-Type", _contentType);
		return hashtable;
	}
}
