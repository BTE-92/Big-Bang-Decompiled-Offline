public static class DebugTimer
{
	private static double time;

	public static void Start()
	{
		time = Main.m_EPOCHSeconds;
	}

	public static void Stop()
	{
		Debug.Log(Main.m_EPOCHSeconds - time);
	}
}
