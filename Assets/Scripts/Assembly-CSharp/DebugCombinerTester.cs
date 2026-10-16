using UnityEngine;

public class DebugCombinerTester : MonoBehaviour
{
	public Texture2D baseTexture;

	public Texture2D baseColorMask;

	public Texture2D patternColorMask;

	public Color32 baseColor;

	public Color32 patternColor;

	private Material testMaterial;

	private void Awake()
	{
		testMaterial = GetComponent<Renderer>().material;
		ChangeTexture();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			ChangeTexture();
		}
	}

	private void ChangeTexture()
	{
		testMaterial.SetTexture("_MainTex", TextureCombiner.CombineVehicleTextures(baseTexture, baseColorMask, patternColorMask, baseColor, patternColor));
	}
}
