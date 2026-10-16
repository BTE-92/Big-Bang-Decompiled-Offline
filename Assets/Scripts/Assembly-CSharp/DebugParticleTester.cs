using UnityEngine;

public class DebugParticleTester : MonoBehaviour
{
	public ParticleSystem currentPSystem;

	public void Play()
	{
		if (currentPSystem != null)
		{
			currentPSystem.Play();
		}
	}
}
