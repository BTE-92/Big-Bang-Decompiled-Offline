using UnityEngine;

public static class PsLoadingScreen
{
	public static UICanvas m_bg;

	public static Entity m_entity;

	public static TransformC m_tc;

	public static Camera m_camera;

	public static SpriteSheet m_sheet;

	private static TweenEventDelegate m_fadeCallback;

	public static bool m_active;

	public static void Create()
	{
		m_entity = EntityManager.AddEntity("LoadingScreen");
		m_entity.m_persistent = true;
		m_tc = TransformS.AddComponent(m_entity, Vector3.zero);
		m_active = false;
	}

	public static void FadeIn(TweenEventDelegate _fadeInCallback = null)
	{
		TouchAreaS.Disable();
		m_fadeCallback = _fadeInCallback;
		if (m_entity == null)
		{
			Create();
		}
		m_camera = CameraS.AddCamera("Loading Screen Camera", true);
		m_bg = new UICanvas(null, false, string.Empty, null, string.Empty);
		m_bg.SetCamera(m_camera, true, false);
		m_bg.SetWidth(1f, RelativeTo.ScreenWidth);
		m_bg.SetHeight(1f, RelativeTo.ScreenHeight);
		m_bg.SetDepthOffset(-240f);
		m_bg.SetDrawHandler(BackgroundDrawhandler);
		m_bg.Update();
		TweenC tweenC = TweenS.AddTransformTween(m_bg.m_TC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.zero, Vector3.one, 0.5f, 0f, true);
		TweenS.SetTweenAlphaProperties(tweenC, false, true, false);
		TweenS.AddTweenEndEventListener(tweenC, FadeInCallback);
		m_active = true;
	}

	private static void BackgroundDrawhandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth, _c.m_actualHeight, Vector2.zero);
		PrefabC prefabC = PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, rect, DebugDraw.HexToUint("#000000"), DebugDraw.HexToUint("#000000"), ResourceManager.GetMaterial(RESOURCE.MenuPopupBackgroundMat_Material), _c.m_camera, string.Empty);
		prefabC.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find("WOE/Unlit/ColorUnlitTransparentBg");
		prefabC.p_gameObject.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 1f);
	}

	private static void FadeInCallback(TweenC _c)
	{
		new PsUILoadingText(m_bg);
		m_bg.Update();
		if (m_fadeCallback != null)
		{
			Debug.Log("FADE IN CALLBACK");
			m_fadeCallback(_c);
		}
	}

	public static void FadeOut(TweenEventDelegate _fadeOutCallback = null)
	{
		if (m_bg != null)
		{
			m_bg.DestroyChildren();
		}
		m_fadeCallback = _fadeOutCallback;
		TweenC tweenC = TweenS.AddTransformTween(m_bg.m_TC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.one, Vector3.zero, 0.5f, 0f, true);
		TweenS.SetTweenAlphaProperties(tweenC, false, true, false);
		TweenS.AddTweenEndEventListener(tweenC, FadeOutCallback);
	}

	private static void FadeOutCallback(TweenC _c)
	{
		m_active = false;
		TouchAreaS.Enable();
		if (m_fadeCallback != null)
		{
			m_fadeCallback(_c);
		}
		Destroy();
	}

	public static void Destroy()
	{
		if (m_bg != null)
		{
			m_bg.Destroy();
			m_bg = null;
		}
		CameraS.RemoveCamera(m_camera);
		EntityManager.RemoveEntity(m_entity);
		m_entity = null;
		m_tc = null;
		m_camera = null;
		m_sheet = null;
		m_fadeCallback = null;
	}
}
