using UnityEngine;

public class BoelbAI : MonoBehaviour
{
	public Animator animator;

	public SkinnedMeshRenderer bodySkin;

	public SkinnedMeshRenderer eyeSkin;

	public ParticleSystem deathSplurt;

	public Transform lookAtTarget;

	public Transform eyeTrans;

	private bool hasLookTarget;

	private float minLookDist;

	private float maxLookDist = 500f;

	public float minBlinkDensity = 0.25f;

	public float maxBlinkDensity = 2f;

	private float blinkTimer = 1f;

	private float jumpTimer = 2f;

	public void Jump()
	{
		animator.SetTrigger("Jump");
	}

	public void Blink()
	{
		animator.SetTrigger("Blink");
	}

	public void Kill()
	{
		bodySkin.gameObject.SetActive(false);
		eyeSkin.gameObject.SetActive(false);
		deathSplurt.Play();
	}

	public void LookAtTarget(Transform target)
	{
		Vector3 worldPosition = target.position + Vector3.forward * -200f;
		if (worldPosition.y < eyeTrans.position.y)
		{
			worldPosition.y = eyeTrans.position.y;
		}
		eyeTrans.LookAt(worldPosition);
		float num = 0f;
		num = Vector3.Distance(eyeTrans.position, target.position);
		num = Mathf.Clamp(num, minLookDist, maxLookDist);
		eyeSkin.SetBlendShapeWeight(0, num / maxLookDist * 100f);
	}

	private float GetDeltaTime()
	{
		return Main.m_gameDeltaTime;
	}

	private void Update()
	{
		blinkTimer -= GetDeltaTime();
		jumpTimer -= GetDeltaTime();
		if (blinkTimer <= 0f)
		{
			Blink();
			blinkTimer = Random.Range(minBlinkDensity, maxBlinkDensity);
		}
		if (jumpTimer <= 0f)
		{
			Jump();
			jumpTimer = Random.Range(2f, 5f);
		}
		if (lookAtTarget != null)
		{
			LookAtTarget(lookAtTarget);
		}
	}
}
