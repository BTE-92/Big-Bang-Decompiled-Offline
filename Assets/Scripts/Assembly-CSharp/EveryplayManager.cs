public static class EveryplayManager
{
	private static bool m_isEnabled = true;

	private static bool m_isSupported;

	private static bool m_isRecording;

	private static bool m_hasRecordedVideo;

	private static float m_stopDelaySecs;

	private static bool m_startRecording;

	public static void Initialize()
	{
		if (ToolBox.GetDevicePerformanceClass() != PerformanceClass.SLOW)
		{
			Everyplay.ReadyForRecording += OnReadyForRecording;
			Everyplay.RecordingStopped += OnRecordingStopped;
			Everyplay.RecordingStarted += OnRecordingStarted;
			Everyplay.WasClosed += WasClosed;
			Everyplay.SetTargetFPS(60);
		}
	}

	public static void PlayLastRecording()
	{
		SoundS.SetPauseForAllSounds(true);
		Everyplay.PlayLastRecording();
	}

	private static void WasClosed()
	{
		SoundS.SetPauseForAllSounds(false);
	}

	private static void OnReadyForRecording(bool _enabled)
	{
		if (!m_isSupported && _enabled && Everyplay.IsSupported() && Everyplay.IsRecordingSupported())
		{
			m_isSupported = true;
		}
	}

	private static void OnRecordingStopped()
	{
		Debug.Log("Everyplay recording stopped.");
		m_isRecording = false;
	}

	private static void OnRecordingStarted()
	{
		Debug.Log("Everyplay recording started...");
		m_isRecording = true;
	}

	public static void SetEnabled(bool _enabled)
	{
		m_isEnabled = _enabled;
	}

	public static bool IsEnabled()
	{
		return m_isSupported && m_isEnabled;
	}

	public static bool IsSupported()
	{
		return m_isSupported;
	}

	public static bool HasRecordedVideo()
	{
		if (!IsEnabled())
		{
			return false;
		}
		return m_hasRecordedVideo;
	}

	public static void StartRecording()
	{
		if (IsEnabled())
		{
			m_startRecording = true;
			m_hasRecordedVideo = false;
			Debug.Log("Everyplay: Trying to start recording...");
		}
	}

	public static void StopRecording(float _delay = 0f)
	{
		if (!IsEnabled())
		{
			return;
		}
		m_startRecording = false;
		if (m_isRecording)
		{
			if (_delay <= 0f)
			{
				m_stopDelaySecs = 0f;
				Everyplay.StopRecording();
				Debug.Log("Everyplay: Trying to stop recording.");
			}
			else
			{
				m_stopDelaySecs = _delay;
				Debug.Log("Everyplay: Stop recording after " + _delay + " secs.");
			}
			m_hasRecordedVideo = true;
		}
	}

	public static void SetPaused(bool _paused)
	{
		if (IsEnabled() && Everyplay.IsRecording())
		{
			if (Everyplay.IsPaused() && !_paused)
			{
				Everyplay.ResumeRecording();
			}
			else if (!Everyplay.IsPaused() && _paused)
			{
				Everyplay.PauseRecording();
			}
		}
	}

	public static void ClearRecording()
	{
		if (IsEnabled())
		{
			m_hasRecordedVideo = false;
		}
	}

	public static void Update()
	{
		if (!IsEnabled())
		{
			return;
		}
		if (m_isRecording)
		{
			if (m_stopDelaySecs > 0f)
			{
				m_stopDelaySecs -= Main.m_gameDeltaTime;
				if (m_stopDelaySecs <= 0f)
				{
					StopRecording(0f);
				}
			}
		}
		else if (m_startRecording)
		{
			Everyplay.StartRecording();
			m_startRecording = false;
		}
	}
}
