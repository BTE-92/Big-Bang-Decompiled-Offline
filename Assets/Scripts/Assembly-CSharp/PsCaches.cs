using UnityEngine;

public static class PsCaches
{
	public static ItemCache<byte[]> m_screenshots;

	public static ByteArrayDiskCache m_screenshotsDiskCache;

	public static ItemCache<Texture2D> m_profileImages;

	public static ListCache<PsMinigameMetaData> m_latestList;

	public static ListCache<PsMinigameMetaData> m_trendingList;

	public static ListCache<PsMinigameMetaData> m_popularList;

	public static ListCache<PsMinigameMetaData> m_unratedList;

	public static ListCache<PsMinigameMetaData> m_savedList;

	public static ListCache<PsMinigameMetaData> m_publishedList;

	public static ListCache<PsReseachMetaData> m_signalList;

	public static void Initialize()
	{
		m_screenshots = new ItemCache<byte[]>("SCREENSHOT_CACHE", 50, -1f);
		m_screenshotsDiskCache = new ByteArrayDiskCache("SCREENSHOT_DISKCACHE", 300);
		m_profileImages = new ItemCache<Texture2D>("PROFILE_IMAGES", 100, -1f);
		m_latestList = new ListCache<PsMinigameMetaData>("MINIGAMES_LATEST", 100f);
		m_trendingList = new ListCache<PsMinigameMetaData>("MINIGAMES_TRENDING", 100f);
		m_popularList = new ListCache<PsMinigameMetaData>("MINIGAMES_POPULAR", 100f);
		m_unratedList = new ListCache<PsMinigameMetaData>("MINIGAMES_NEW", 100f);
		m_savedList = new ListCache<PsMinigameMetaData>("MINIGAMES_SAVED", 100f);
		m_publishedList = new ListCache<PsMinigameMetaData>("MINIGAMES_PUBLISHED", 5f);
		m_signalList = new ListCache<PsReseachMetaData>("ITEMS_SIGNALS", 100f);
	}
}
