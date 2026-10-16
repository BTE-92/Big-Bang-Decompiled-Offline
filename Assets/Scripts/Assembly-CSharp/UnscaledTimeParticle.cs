using UnityEngine;

public class UnscaledTimeParticle : MonoBehaviour
{
	private void Update()
	{
		if (Time.timeScale < 1f)
		{
			GetComponent<ParticleSystem>().Simulate(Main.m_dt, true, false);
		}
	}
}
