using UnityEngine;

public class SplashLoad : MonoBehaviour
{
	public Material m_fadeMaterial;

	private float m_timer;

	private float m_fadeTimer = 0.5f;

	private bool m_triggered;

	private AsyncOperation loadOperation;

	private void Start()
	{
		EveryplayManager.SetEnabled(PsState.m_everyplayEnabled);
		EveryplayManager.Initialize();
		Color color = m_fadeMaterial.color;
		color.a = 0f;
		m_fadeMaterial.color = color;
		m_triggered = false;
		loadOperation = Application.LoadLevelAsync("MainScene");
		loadOperation.allowSceneActivation = false;
	}

	private void Update()
	{
		m_timer += Time.deltaTime;
		if (!(m_timer >= 3.5f))
		{
			return;
		}
		if (m_fadeTimer <= 0f)
		{
			m_fadeTimer = 0f;
			if (!m_triggered)
			{
				loadOperation.allowSceneActivation = true;
				m_triggered = true;
			}
		}
		float a = (0.5f - m_fadeTimer) / 0.5f;
		Color color = m_fadeMaterial.color;
		color.a = a;
		m_fadeMaterial.color = color;
		m_fadeTimer -= Time.deltaTime;
		m_timer = 3.5f;
	}
}
