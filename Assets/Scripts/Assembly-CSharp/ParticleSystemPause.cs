using UnityEngine;

public class ParticleSystemPause : MonoBehaviour
{
	private ParticleSystem pSystem;

	private bool pSystemPaused;

	private void Start()
	{
		pSystem = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (PsState.m_physicsPaused && !pSystemPaused)
		{
			pSystem.Pause();
			pSystemPaused = true;
		}
		else if (!PsState.m_physicsPaused && pSystemPaused)
		{
			pSystem.Play();
			pSystemPaused = false;
		}
	}
}
