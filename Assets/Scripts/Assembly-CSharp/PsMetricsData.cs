public static class PsMetricsData
{
	public static int m_beamersUsed;

	public static int m_testersUsed;

	public static int m_refillsUsed;

	public static int m_timeInEditor;

	public static int m_lastGhostSize;

	public static bool m_isSendingGhost;

	public static bool m_isNewGhost;

	public static void Reset()
	{
		m_testersUsed = 0;
		m_beamersUsed = 0;
		m_refillsUsed = 0;
	}
}
