using System.Collections.Generic;
using FMOD.Studio;

public class CombineSound
{
	public EventInstance eventInstance;

	public List<SoundC> components;

	public bool isPlaying;

	public bool isPaused;

	public bool isOutOfRange;

	public float volume;

	public float minDist;

	public float maxDist;

	public SoundC nearestSoundC;
}
