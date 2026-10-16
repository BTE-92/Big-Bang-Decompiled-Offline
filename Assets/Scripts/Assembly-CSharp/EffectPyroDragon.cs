using UnityEngine;

public class EffectPyroDragon : MonoBehaviour
{
	public float animationLength = 1f;

	public Renderer flameRenderer;

	public Renderer tubeRenderer;

	public ParticleSystem sparks;

	public ParticleSystem smoke;

	public ParticleSystem fireBall;

	public Animator dragonAnimator;

	public void PlayEffect()
	{
		tubeRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.5f, 0f));
		dragonAnimator.SetTrigger("Dragon");
		sparks.Play();
		smoke.Play();
		fireBall.Play();
		SoundS.PlaySingleShot("/Ingame/Units/Decorations/DecorationPyrotechnicsFire", base.transform.position);
	}
}
