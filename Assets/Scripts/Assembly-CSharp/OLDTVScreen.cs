using UnityEngine;

[ExecuteInEditMode]
public class OLDTVScreen : MonoBehaviour
{
	public Material tvMaterialScreen;

	public float screenSaturation;

	public Texture chromaticAberrationPattern;

	public float chromaticAberrationMagnetude = 0.015f;

	public Texture noisePattern;

	public float noiseMagnetude = 0.1f;

	public Texture staticPattern;

	public Texture staticMask;

	public float staticMagnetude = 0.015f;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		float num = 0f;
		bool flag = true;
		flag = true;
		tvMaterialScreen.SetFloat("_Saturation", screenSaturation);
		tvMaterialScreen.SetFloat("_ChromaticAberrationMagnetude", chromaticAberrationMagnetude);
		tvMaterialScreen.SetTextureOffset("_NoiseTex", new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)));
		tvMaterialScreen.SetFloat("_NoiseMagnetude", noiseMagnetude);
		tvMaterialScreen.SetTextureOffset("_StaticTex", new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)));
		tvMaterialScreen.SetFloat("_StaticMagnetude", staticMagnetude);
		Graphics.Blit(source, destination, tvMaterialScreen);
	}
}
