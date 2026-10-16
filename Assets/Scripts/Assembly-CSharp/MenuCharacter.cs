using System.Collections;
using UnityEngine;

public class MenuCharacter : MonoBehaviour
{
	public Animator charAnimator;

	public GameObject eyeLids;

	public GameObject eyeLidsHalf;

	public GameObject eyesForward;

	public GameObject eyesCamera;

	public ParticleSystem sleepZ;

	private bool isSleeping;

	private float blinkTimer = 1f;

	private float minBlink = 3f;

	private float maxBlink = 6f;

	public virtual void Talk()
	{
		charAnimator.SetTrigger("Talk");
	}

	public virtual void Poke()
	{
		charAnimator.SetTrigger("Poke");
	}

	public void LookAtCamera()
	{
		if (eyeLids != null)
		{
			StartCoroutine("LookAtCameraCR");
		}
	}

	public void LookAwayFromCamera()
	{
		if (eyeLids != null)
		{
			StartCoroutine("LookAwayFromCameraCR");
		}
	}

	public void Activate()
	{
		sleepZ.Stop();
		charAnimator.SetTrigger("Poke");
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			if (meshRenderer.gameObject.name != "AlphaMask")
			{
				meshRenderer.material.shader = Shader.Find("WOE/Units/Transparent-Unlit");
			}
		}
		if (eyeLids != null)
		{
			eyeLids.SetActive(false);
		}
		if (eyeLidsHalf != null)
		{
			eyeLidsHalf.SetActive(true);
		}
	}

	public void Deactivate()
	{
		if (sleepZ != null)
		{
			sleepZ.Play();
		}
		isSleeping = true;
		if (eyeLids != null)
		{
			eyeLids.SetActive(true);
		}
		if (eyeLidsHalf != null)
		{
			eyeLidsHalf.SetActive(false);
		}
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			if (meshRenderer.gameObject.name != "AlphaMask")
			{
				meshRenderer.material.shader = Shader.Find("WOE/Fx/GreyscaleUnlitAlphaZ");
			}
		}
	}

	private IEnumerator BlinkCR()
	{
		float blinkCounter = 0.1f;
		eyeLids.SetActive(true);
		while (blinkCounter >= 0f)
		{
			blinkCounter -= GetDeltaTime();
			yield return null;
		}
		eyeLids.SetActive(false);
	}

	private IEnumerator LookAtCameraCR()
	{
		float blinkCounter = 0.1f;
		eyeLids.SetActive(true);
		eyesForward.SetActive(false);
		eyesCamera.SetActive(true);
		while (blinkCounter >= 0f)
		{
			blinkCounter -= GetDeltaTime();
			yield return null;
		}
		eyeLids.SetActive(false);
	}

	private IEnumerator LookAwayFromCameraCR()
	{
		float blinkCounter = 0.1f;
		eyeLids.SetActive(true);
		eyesForward.SetActive(true);
		eyesCamera.SetActive(false);
		while (blinkCounter >= 0f)
		{
			blinkCounter -= GetDeltaTime();
			yield return null;
		}
		eyeLids.SetActive(false);
	}

	public float GetDeltaTime()
	{
		return Main.m_gameDeltaTime;
	}

	private void Update()
	{
		if (eyeLids != null && !isSleeping)
		{
			if (blinkTimer <= 0f)
			{
				blinkTimer = Random.Range(minBlink, maxBlink);
				StartCoroutine("BlinkCR");
			}
			else
			{
				blinkTimer -= GetDeltaTime();
			}
		}
	}
}
