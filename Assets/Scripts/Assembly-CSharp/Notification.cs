public class Notification : PsUIPopup
{
	public double m_started;

	public double m_duration;

	public Notification()
	{
		m_duration = 1.0;
		NotificationManager.AddNotification(this);
	}

	public virtual void Start()
	{
		m_started = Main.m_gameTimeSinceAppStarted;
	}

	public virtual void End()
	{
		NotificationManager.RemoveNotification(this);
	}
}
