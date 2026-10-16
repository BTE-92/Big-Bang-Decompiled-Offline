using System.Collections;
using FMOD.Studio;

public class SoundC : BasicComponent
{
	public static int m_componentCount;

	public bool isCombineSound;

	public string combineSoundKey;

	public TransformC p_TC;

	public string name;

	public bool isPlaying;

	public bool isPaused;

	public bool forceAtListenerPosition;

	public EventInstance eventInstance;

	public Hashtable parameterInstances;

	public SoundC()
		: base(ComponentType.Sound)
	{
		m_componentCount++;
	}

	public override void Reset()
	{
		base.Reset();
		isPlaying = false;
		isPaused = false;
		forceAtListenerPosition = false;
		eventInstance = null;
		parameterInstances = null;
		p_TC = null;
		name = null;
		isCombineSound = false;
		combineSoundKey = string.Empty;
	}

	~SoundC()
	{
		m_componentCount--;
	}
}
