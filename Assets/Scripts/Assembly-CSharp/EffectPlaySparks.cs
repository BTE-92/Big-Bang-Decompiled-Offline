using UnityEngine;

public class EffectPlaySparks : MonoBehaviour
{
	public ParticleSystem activateSparks;

	public void PlaySparks()
	{
		activateSparks.Play();
	}
}
