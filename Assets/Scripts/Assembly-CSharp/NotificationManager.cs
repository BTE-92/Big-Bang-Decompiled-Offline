using System.Collections.Generic;

public static class NotificationManager
{
	private static List<Notification> m_notificationQueue;

	private static Notification m_currentNotification;

	private static bool m_paused;

	private static double m_lastNotificationEnded;

	public static double m_waitBetween = 0.25;

	public static void Initialize()
	{
		m_notificationQueue = new List<Notification>();
	}

	public static void AddNotification(Notification _notification)
	{
		m_notificationQueue.Add(_notification);
	}

	public static void RemoveNotification(Notification _notification)
	{
		m_notificationQueue.Remove(_notification);
	}

	public static void Pause()
	{
		m_paused = true;
	}

	public static void Resume()
	{
		m_paused = false;
		m_lastNotificationEnded = Main.m_gameTimeSinceAppStarted + 1.0;
	}

	public static void Update()
	{
		if ((!m_paused || m_currentNotification != null) && m_notificationQueue.Count > 0)
		{
			if (m_currentNotification == null && m_lastNotificationEnded + m_waitBetween < Main.m_gameTimeSinceAppStarted)
			{
				m_currentNotification = m_notificationQueue[0];
				m_currentNotification.Start();
			}
			if (m_currentNotification != null && m_currentNotification.m_started + m_currentNotification.m_duration < Main.m_gameTimeSinceAppStarted)
			{
				m_currentNotification.End();
				m_currentNotification = null;
				m_lastNotificationEnded = Main.m_gameTimeSinceAppStarted;
			}
		}
	}
}
