using System.Collections.Generic;

public static class ReplayManager
{
	public static List<Replay> m_playbackReplays;

	public static Replay m_recordingReplay;

	public static List<Replay> m_recordedReplays;

	public static void Initialize()
	{
		m_playbackReplays = new List<Replay>();
		m_recordedReplays = new List<Replay>();
	}

	public static void CreateRecordingReplay(string _levelId)
	{
		if (m_recordingReplay != null)
		{
			m_recordedReplays.Add(m_recordingReplay);
		}
		m_recordingReplay = new Replay(_levelId);
	}

	public static void CreatePlaybackReplay(Replay _replay)
	{
		m_playbackReplays.Add(_replay);
	}

	public static void ClearPlaybackReplays()
	{
		m_playbackReplays.Clear();
	}

	public static void Update()
	{
	}
}
