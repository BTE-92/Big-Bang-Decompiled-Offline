using UnityEngine;

public class PsSplashScreen : UICanvas
{
	public Material m_splashMaterial;

	public Texture2D m_splashTexture;

	public PsSplashScreen(UIComponent _parent)
		: base(_parent, false, "SplashScreen", null, string.Empty)
	{
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		SetDrawHandler(SplashBackgroundDrawhandler);
		EntityManager.RemoveAllTagsFromEntity(m_TC.p_entity);
	}

	private void SplashBackgroundDrawhandler(UIComponent _c)
	{
		m_splashTexture = Resources.Load("PlaySomething/Menu/Textures/splash_universal") as Texture2D;
		m_splashMaterial = Object.Instantiate(ResourceManager.GetMaterial(RESOURCE.ProfileMat_Material));
		m_splashMaterial.mainTexture = m_splashTexture;
		float num = (float)Screen.width / (float)m_splashTexture.width;
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect((float)m_splashTexture.width * num, (float)m_splashTexture.height * num, Vector2.zero);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 5f, rect, DebugDraw.HexToUint("ffffff"), DebugDraw.HexToUint("ffffff"), m_splashMaterial, _c.m_camera, "Splash", UVRect.Normal());
	}

	public override void Destroy()
	{
		if (m_splashMaterial != null)
		{
			Object.Destroy(m_splashMaterial);
			m_splashMaterial = null;
		}
		Resources.UnloadAsset(m_splashTexture);
		m_splashTexture = null;
		base.Destroy();
	}
}
