using System;

public class ServerQueueItem
{
	private Action m_serverCall;

	private Action m_proceed;

	private string[] m_tags;

	public bool done;

	public ServerQueueItem(string[] _tags, Action _proceed, Action _serverCall)
	{
		m_proceed = _proceed;
		m_serverCall = _serverCall;
		m_tags = _tags;
	}

	public void Update()
	{
		if (!HttpS.QueueTagsExist(m_tags))
		{
			if (m_serverCall != null)
			{
				m_serverCall();
			}
			if (m_proceed != null)
			{
				m_proceed();
			}
			done = true;
		}
	}

	~ServerQueueItem()
	{
	}
}
