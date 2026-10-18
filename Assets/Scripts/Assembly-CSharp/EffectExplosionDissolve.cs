using UnityEngine;

public class EffectExplosionDissolve : MonoBehaviour
{
    public ParticleSystemRenderer pRenderer;

	public float dissolveSpeed = 1f;

	private float dissolveTimer;

	private Material tempMat;

	private void Start()
	{
		tempMat = pRenderer.material;
	}

	private void Update()
	{
		if (!PsState.m_activeMinigame.m_gamePaused && pRenderer != null && dissolveTimer < dissolveSpeed)
		{
			dissolveTimer += Time.deltaTime;
			float value = dissolveTimer / dissolveSpeed;
			tempMat.SetFloat("_AlphaCutoff", value);
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(tempMat);
	}
}
