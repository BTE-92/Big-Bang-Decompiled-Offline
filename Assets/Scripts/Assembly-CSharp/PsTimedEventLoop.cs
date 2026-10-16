using System;
using System.Globalization;

public class PsTimedEventLoop : PsGameLoop
{
	public PsTimedEventType m_type;

	public double m_eventLastStartTime;

	public int m_eventTimeLeft;

	public int m_eventDuration;

	public DateTime m_endTime;

	public bool m_eventOver;

	public bool m_domeDestroyed;

	public bool m_timeRunOut;

	public string m_timedEventId;

	public PsTimedEventLoop(PsMinigameContext _context, string _timedEventId, string _minigameId, PsPlanetPath _path, int _id, int _levelNumber, int _score, int _timeleft, int _duration = -1, string _endTime = null, bool _eventOver = false, bool _domeDestroyed = false)
		: base(_context, _minigameId, _path, _id, _levelNumber, _score, true)
	{
		m_timedEventId = _timedEventId;
		m_eventTimeLeft = _timeleft;
		m_eventDuration = _duration;
		m_eventOver = _eventOver;
		m_domeDestroyed = _domeDestroyed;
		if (_endTime != null)
		{
			m_endTime = DateTime.ParseExact(_endTime, "O", CultureInfo.InvariantCulture);
		}
		else
		{
			m_endTime = DateTime.Now.AddSeconds(m_eventTimeLeft);
		}
		SetEventState();
	}

	public virtual void SetEventState()
	{
		if (!m_eventOver)
		{
			m_eventOver = IsEventOver();
		}
		m_timeRunOut = IsEventOver();
	}

	public void UpdateInfo(int _timeLeftSeconds)
	{
		m_eventTimeLeft = _timeLeftSeconds;
		m_endTime = DateTime.Now.AddSeconds(m_eventTimeLeft);
	}

	public bool IsEventOver()
	{
		return SecondsLeft() < 0;
	}

	public int SecondsLeft()
	{
		DateTime now = DateTime.Now;
		return Convert.ToInt32(m_endTime.Subtract(now).TotalSeconds);
	}

	public virtual string GetBannerString()
	{
		if (IsEventOver() && !m_eventOver)
		{
			m_eventOver = true;
			m_timeRunOut = true;
			HandleEventOver();
		}
		if (m_eventOver && !m_timeRunOut)
		{
			return "Completed";
		}
		if (m_timeRunOut)
		{
			return "Expired";
		}
		return PsMetagameManager.GetTimeStringFromSeconds(SecondsLeft());
	}

	public virtual void HandleEventOver()
	{
		if (!m_domeDestroyed)
		{
			m_domeDestroyed = true;
			(m_node as PsFloatingPlanetNode).DestroyNode();
			PsPlanetManager.m_timedEvents = null;
			m_planet.GetTimedEventsFromServer();
		}
	}
}
