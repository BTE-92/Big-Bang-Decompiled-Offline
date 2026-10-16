using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLogo : MonoBehaviour
{
	[HideInInspector]
	public List<Material> magentaMaterials;

	[HideInInspector]
	public List<Material> yellowMaterials;

	public float minStartDelay;

	public float maxStartDelay = 0.25f;

	public float minFlashTime = 0.2f;

	public float maxFlashTime = 0.5f;

	public List<Gradient> magentaGradients;

	public List<Gradient> yellowGradients;

	private void Awake()
	{
		magentaMaterials.Clear();
		yellowMaterials.Clear();
		foreach (Transform item in base.transform)
		{
			if (item.name.StartsWith("Block"))
			{
				Material material = Object.Instantiate(item.GetComponent<Renderer>().material);
				item.GetComponent<MeshRenderer>().material = material;
				yellowMaterials.Add(material);
			}
			else if (item.name.StartsWith("Letter"))
			{
				Material material2 = Object.Instantiate(item.GetComponent<Renderer>().material);
				item.GetComponent<MeshRenderer>().material = material2;
				magentaMaterials.Add(material2);
			}
		}
	}

	private void Start()
	{
		foreach (Material magentaMaterial in magentaMaterials)
		{
			StartCoroutine("FlashColors", new FlasherData
			{
				mat = magentaMaterial,
				grad = magentaGradients
			});
		}
		foreach (Material yellowMaterial in yellowMaterials)
		{
			StartCoroutine("FlashColors", new FlasherData
			{
				mat = yellowMaterial,
				grad = yellowGradients
			});
		}
	}

	private IEnumerator FlashColors(FlasherData fData)
	{
		float startCounter = Random.Range(minStartDelay, maxStartDelay);
		float flashTime = Random.Range(minFlashTime, maxFlashTime);
		float flashStartTime = flashTime;
		Gradient grad = fData.grad[Random.Range(0, fData.grad.Count)];
		Material mat = fData.mat;
		while (startCounter > 0f)
		{
			startCounter -= GetDeltaTime();
			yield return null;
		}
		while (flashTime > 0f)
		{
			flashTime -= GetDeltaTime();
			mat.SetColor("_Color", grad.Evaluate(1f - flashTime / flashStartTime));
			yield return null;
		}
	}

	private float GetDeltaTime()
	{
		return Time.deltaTime;
	}

	private void OnDestroy()
	{
		foreach (Material magentaMaterial in magentaMaterials)
		{
			Object.Destroy(magentaMaterial);
		}
		foreach (Material yellowMaterial in yellowMaterials)
		{
			Object.Destroy(yellowMaterial);
		}
	}
}
