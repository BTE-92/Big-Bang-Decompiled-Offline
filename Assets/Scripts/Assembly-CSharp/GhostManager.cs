using System;

public static class GhostManager
{
	public static bool m_ghostLoadComplete;

	private static Action m_proceedCallback;

	private static Entity m_loaderEntity;

	private static int m_ghostRetryCounter;

	public static SpriteSheet m_ghostSpriteSheet;

	public static void Initialize()
	{
	}

	public static void Free()
	{
		if (m_ghostSpriteSheet != null)
		{
			SpriteS.RemoveSpriteSheet(m_ghostSpriteSheet);
			m_ghostSpriteSheet = null;
		}
	}
}
