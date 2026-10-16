using System.Collections;
using UnityEngine;

public class DecorationPyroChickoon : MonoBehaviour
{
	public Renderer[] chickoonParts;

	public Renderer bucketRenderer;

	public Animator bucketAnimator;

	public ParticleSystem particleLid;

	public ParticleSystem particleFeather;

	public ParticleSystem chickoonExplosion;

	public ParticleSystem chickoonSmokesplosion;

	public ParticleSystem greaseUp;

	public ParticleSystem greaseExplosion;

	public ParticleSystem smoke;

	public GameObject chickoonOffsetGo;

	public GameObject chickoonGo;

	public AnimationCurve flyLeft;

	public AnimationCurve flyRight;

	private float flyTime;

	public void PlayEffect()
	{
		chickoonOffsetGo.transform.localPosition = new Vector3(0f, 0f, 0f);
		chickoonGo.transform.rotation = chickoonOffsetGo.transform.rotation;
		bucketAnimator.SetInteger("Randomizer", Random.Range(0, 101));
		bucketAnimator.SetTrigger("Bucket");
	}

	private void PopLid()
	{
		bucketRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.5f, 0f));
		particleLid.Play();
		if (bucketAnimator.GetInteger("Randomizer") > 0)
		{
			particleFeather.Play();
			smoke.Play();
			SoundS.PlaySingleShot("/Ingame/Units/Decorations/DecorationPyrotechnicsChickoon_ShootChicken", base.transform.position);
		}
		else if (bucketAnimator.GetInteger("Randomizer") == 0)
		{
			SoundS.PlaySingleShot("/Ingame/Units/Decorations/DecorationPyrotechnicsChickoon_ShootBroiler", base.transform.position);
			greaseUp.Play();
		}
	}

	private void ExplodeChickoon()
	{
		if (bucketAnimator.GetInteger("Randomizer") > 0)
		{
			chickoonExplosion.transform.position = chickoonGo.transform.position;
			chickoonExplosion.Play();
			chickoonSmokesplosion.transform.position = chickoonGo.transform.position;
			chickoonSmokesplosion.Play();
			SoundS.PlaySingleShot("/Ingame/Units/Decorations/DecorationPyrotechnicsChickoon_ChickenExplosion", base.transform.position);
		}
		else if (bucketAnimator.GetInteger("Randomizer") == 0)
		{
			greaseExplosion.Play();
			SoundS.PlaySingleShot("/Ingame/Units/Decorations/DecorationPyrotechnicsChickoon_BroilerExplosion", base.transform.position);
		}
	}

	private void startFlyRoutine()
	{
		StartCoroutine("FlyRoutine");
	}

	private IEnumerator FlyRoutine()
	{
		flyTime = 0f;
		float randomDirection = Random.Range(0f, 1f);
		float randomRotation = Random.Range(0f, 1f);
		while (flyTime <= 2f)
		{
			if (PsState.m_physicsPaused)
			{
				yield return null;
				continue;
			}
			flyTime += Main.m_gameDeltaTime;
			Vector3 oldPosition = chickoonGo.transform.position;
			float xOffset = Mathf.Lerp(flyLeft.Evaluate(flyTime), flyRight.Evaluate(flyTime), randomDirection);
			chickoonOffsetGo.transform.Translate(new Vector3(xOffset, flyLeft.Evaluate(flyTime) * 2f, 0f));
			Vector3 movementDirection = (chickoonGo.transform.position - oldPosition).normalized;
			movementDirection.y = 2f;
			chickoonGo.transform.rotation = Quaternion.Slerp(chickoonGo.transform.rotation, Quaternion.LookRotation(movementDirection, new Vector3(0f, -1f, 0f)), Time.deltaTime * 4f);
			yield return null;
		}
	}
}
