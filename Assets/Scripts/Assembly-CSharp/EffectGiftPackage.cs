using System.Collections;
using UnityEngine;

public class EffectGiftPackage : MonoBehaviour
{
	public GameObject packageNormal;

	public GameObject[] packageSides;

	public GameObject openingEffect;

	private Color currentColor;

	private Color targetColor = new Color(1f, 1f, 1f, 0f);

	public float fadeoutTime = 1f;

	public AnimationCurve fadeoutCurve;

	public bool debugEnabled;

	private void OpenGift()
	{
		packageNormal.SetActive(false);
		openingEffect.SetActive(true);
		for (int i = 0; i < packageSides.Length; i++)
		{
			packageSides[i].SetActive(true);
		}
		GetComponent<Animation>().Play("EffectGiftPackageOpen");
		StartCoroutine("Fade");
	}

	private IEnumerator Fade()
	{
		for (float f = 0f; f < fadeoutTime; f += Time.deltaTime)
		{
			float currentTime = f / fadeoutTime;
			currentColor.a = fadeoutCurve.Evaluate(currentTime);
			packageNormal.GetComponent<Renderer>().sharedMaterial.color = currentColor;
			yield return null;
		}
		currentColor.a = 0f;
		packageNormal.GetComponent<Renderer>().sharedMaterial.color = currentColor;
	}

	private void Start()
	{
		packageNormal.GetComponent<Renderer>().sharedMaterial.color = new Color(1f, 1f, 1f, 1f);
		currentColor = packageNormal.GetComponent<Renderer>().sharedMaterial.color;
	}

	private void Update()
	{
		if (Input.GetKeyDown("space") && debugEnabled)
		{
			openingEffect.SetActive(false);
			OpenGift();
		}
	}
}
