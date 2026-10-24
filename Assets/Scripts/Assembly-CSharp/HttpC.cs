using System;
using System.Collections.Generic;
using UnityEngine;

public class HttpC : BasicComponent
{
	public static int m_componentCount;

	public GameWWW www;

	public string url;

	public string tag;

	public string[] queueTags;

	public bool destroyAfterDone;

	public bool canRemove;

	public bool disposeWhenDestroyed;

	public bool executed;

	public bool ignoreInQueue;

	public double timeOut;

	public object objectData;

	public int retryCount;

	public KeyCode debugKeyCode;

	public string wwwUrl;

	public byte[] wwwData;

	public Dictionary<string, string> wwwHeaders;

	public event Action<HttpC> requestComplete;

	public event Action<HttpC> requestFailed;

	public HttpC()
		: base(ComponentType.Http)
	{
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
		www = null;
		url = null;
		tag = null;
		queueTags = null;
		requestComplete = null;
		requestFailed = null;
		disposeWhenDestroyed = true;
		canRemove = false;
		ignoreInQueue = false;
		timeOut = 0.0;
		retryCount = 3;
		objectData = null;
		executed = false;
		debugKeyCode = KeyCode.None;
		wwwUrl = null;
		wwwData = null;
		wwwHeaders = null;
	}

	public void SetDebug(KeyCode _executeKeyCode)
	{
		debugKeyCode = _executeKeyCode;
		destroyAfterDone = false;
	}

	public void RequestComplete()
	{
		if (requestComplete != null && !executed)
		{
			requestComplete(this);
		}
		executed = true;
		canRemove = true;
	}

	public void RequestFailed()
	{
		if (retryCount > 0)
		{
			retryCount--;
			timeOut = Main.m_gameTimeSinceAppStarted + HttpS.m_localTimeOut;
			www = new GameWWW(wwwUrl, wwwData, wwwHeaders);
			return;
		}
		if (requestFailed != null && !executed)
		{
			requestFailed(this);
		}
		executed = true;
		canRemove = true;
	}

	public override void Destroy()
	{
		base.Destroy();
		requestComplete = null;
		requestFailed = null;
	}

	~HttpC()
	{
		m_componentCount--;
	}
}
