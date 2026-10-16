using UnityEngine;

public class EffectPyroFlame : MonoBehaviour
{
	public float animationLength = 1f;

	public Renderer tubeRenderer;

	public ParticleSystem confetti;

	public ParticleSystem confettiB;

	public ParticleSystem confettiC;

	public ParticleSystem confettiGrouped;

	public ParticleSystem lid;

	public ParticleSystem smoke;

	public Animator tubeAnimator;

	public void PlayEffect()
	{
		tubeRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.5f, 0f));
		tubeAnimator.SetTrigger("Tube");
		confetti.Play();
		confettiB.Play();
		confettiC.Play();
		confettiGrouped.Play();
		lid.Play();
		smoke.Play();
		SoundS.PlaySingleShot("/Ingame/Units/Decorations/DecorationPyrotechnicsConfetti", base.transform.position);
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Space))
		{
			PlayEffect();
		}
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
		{
			tubeRenderer.material.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
		}
	}
}
