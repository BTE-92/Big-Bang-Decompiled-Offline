using UnityEngine;

public class DebugEffectChain : MonoBehaviour
{
	private ParticleSystem currentSystem;

	public ParticleSystem[] particleSystems;

	[Range(0f, 1f)]
	public float range;

	private void EvaluateEffect()
	{
		int num = particleSystems.Length;
		float num2 = 1f / (float)num;
		for (int i = 0; i < particleSystems.Length; i++)
		{
			if (currentSystem != particleSystems[i] && num2 * (float)i <= range && num2 * (float)(i + 1) > range)
			{
				if (currentSystem != null)
				{
					currentSystem.Stop();
				}
				particleSystems[i].Play();
				Debug.Log("Playing effect " + particleSystems[i]);
				currentSystem = particleSystems[i];
				break;
			}
		}
	}

	private void Update()
	{
		EvaluateEffect();
	}
}
