using UnityEngine;

public class PlayParticleEffect : MonoBehaviour
{
	public void PlayEffect()
	{
		if (GetComponent<ParticleSystem>() != null)
		{
			GetComponent<ParticleSystem>().Play();
		}
	}
}
