using UnityEngine;

public class UIStarInflate : MonoBehaviour
{
	public SkinnedMeshRenderer blendMesh;

	public ParticleSystem starExplosion;

	public Color endColor;

	public bool exploded;

	private Material iMaterial;

	private void Start()
	{
		iMaterial = blendMesh.material;
	}

	public void Reset()
	{
		blendMesh.enabled = true;
		blendMesh.SetBlendShapeWeight(0, 0f);
		iMaterial.SetColor("_Color", Color.white);
		if (starExplosion != null)
		{
			starExplosion.Stop();
		}
	}

	public void SetInflationAmount(float amount)
	{
		float num = amount * 100f;
		if (num < 0f)
		{
			num = 0f;
		}
		if (num >= 100f)
		{
			blendMesh.enabled = false;
			exploded = true;
			if (starExplosion != null)
			{
				starExplosion.Play();
				SoundS.PlaySingleShot("/Ingame/Events/LoseStar", Vector2.zero);
			}
		}
		else
		{
			blendMesh.SetBlendShapeWeight(0, num);
			if (iMaterial != null)
			{
				iMaterial.SetColor("_Color", Color.Lerp(new Color(1f, 1f, 1f, 1f), endColor, amount));
			}
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(iMaterial);
	}
}
