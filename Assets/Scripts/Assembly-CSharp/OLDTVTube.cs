using UnityEngine;

[ExecuteInEditMode]
public class OLDTVTube : MonoBehaviour
{
	public delegate void RepaintAction();

	public Material tvMaterialTube;

	public Texture mask;

	public Texture reflex;

	public float reflexMagnetude = 0.5f;

	public float radialDistortion = 0.2f;

	public float screenSaturation;

	public Texture chromaticAberrationPattern;

	public float chromaticAberrationMagnetude = 0.015f;

	public Texture noisePattern;

	public float noiseMagnetude = 0.1f;

	public Texture staticPattern;

	public Texture staticMask;

	public float staticMagnetude = 0.015f;

	public event RepaintAction WantRepaint;

	private void Repaint()
	{
		if (WantRepaint != null)
		{
			WantRepaint();
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		float value = 1f;
		bool flag = false;
		flag = true;
		if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
		{
			if (flag)
			{
				value = 0f;
			}
			tvMaterialTube.SetFloat("_ScreenHorizontal", value);
		}
		else
		{
			if (flag)
			{
				value = 1f;
			}
			tvMaterialTube.SetFloat("_ScreenHorizontal", value);
		}
		tvMaterialTube.SetFloat("_ReflexMagnetude", reflexMagnetude);
		tvMaterialTube.SetFloat("_Distortion", radialDistortion);
		tvMaterialTube.SetFloat("_Saturation", screenSaturation);
		tvMaterialTube.SetFloat("_ChromaticAberrationMagnetude", chromaticAberrationMagnetude);
		tvMaterialTube.SetTextureOffset("_NoiseTex", new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)));
		tvMaterialTube.SetFloat("_NoiseMagnetude", noiseMagnetude);
		tvMaterialTube.SetTextureOffset("_StaticTex", new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)));
		tvMaterialTube.SetFloat("_StaticMagnetude", staticMagnetude);
		Graphics.Blit(source, destination, tvMaterialTube);
	}
}
