using Server;
using UnityEngine;

public class PsUIScreenshot : UIComponent
{
	public Texture2D m_texture;

	public string m_gameId;

	public PsGameLoop m_loop;

	public bool m_fetchingScreenshot;

	public Material m_material;

	public Vector2 m_offset;

	private float m_cornerSize;

	public bool m_useTutorialTexture;

	public PsUIScreenshot(UIComponent _parent, bool _touchable, string _tag, Vector2 _offset, PsGameLoop _minigameInfo, bool _loadImmediately = true, bool _createBorder = true, float _cornerSize = 0.045f, bool _useTutorialTexture = false)
		: base(_parent, _touchable, _tag, null, null, string.Empty)
	{
		m_cornerSize = _cornerSize;
		m_offset = _offset;
		m_useTutorialTexture = _useTutorialTexture;
		SetListenCameraEvents(true);
		m_loop = _minigameInfo;
		if (m_loop != null && !m_loop.m_loadingMetaData)
		{
			m_gameId = _minigameInfo.GetGameId();
		}
		m_fetchingScreenshot = false;
		if (!m_useTutorialTexture)
		{
			m_material = Object.Instantiate(ResourceManager.GetMaterial(RESOURCE.ScreenshotMat_Material));
			m_material.mainTexture = ResourceManager.GetTexture(RESOURCE.ScreenshotDefault_Texture2D);
		}
		else
		{
			m_material = Object.Instantiate(ResourceManager.GetMaterial(RESOURCE.ScreenshotTutorialMat_Material));
		}
		if (_loadImmediately)
		{
			LoadPicture();
		}
	}

	public void LoadPicture()
	{
		CacheItem<byte[]> item = PsCaches.m_screenshots.GetItem(m_gameId);
		if (item != null)
		{
			HandleScreenshot(item.GetContent());
			return;
		}
		byte[] content = PsCaches.m_screenshotsDiskCache.GetContent(m_gameId);
		if (content != null)
		{
			PsCaches.m_screenshots.AddItem(m_gameId, content);
			HandleScreenshot(content);
			return;
		}
		if (!m_fetchingScreenshot && m_loop.m_context != PsMinigameContext.Saved && m_loop.m_minigameMetaData.published && !string.IsNullOrEmpty(m_gameId))
		{
			HttpC c = Server.Screenshot.Get(m_gameId, ScreenshotRequestSUCCEED, ScreenshotRequestFAILED);
			EntityManager.AddComponentToEntity(m_TC.p_entity, c);
			m_fetchingScreenshot = true;
			return;
		}
		if (string.IsNullOrEmpty(m_gameId))
		{
			Debug.LogWarning("Empty m_gameId when fetching screenshot!");
		}
		m_material.mainTexture = ResourceManager.GetTexture(RESOURCE.ScreenshotDefault_Texture2D);
	}

	public void RemoveScreenshot()
	{
		if (m_texture != null)
		{
			m_material.mainTexture = ResourceManager.GetTexture(RESOURCE.ScreenshotDefault_Texture2D);
			Object.Destroy(m_texture);
			m_texture = null;
		}
	}

	private void ScreenshotRequestSUCCEED(byte[] _bytes)
	{
		PsCaches.m_screenshots.AddItem(m_gameId, _bytes);
		PsCaches.m_screenshotsDiskCache.AddItem(m_gameId, _bytes);
		if (!m_hidden)
		{
			HandleScreenshot(_bytes);
		}
		m_fetchingScreenshot = false;
	}

	private void ScreenshotRequestFAILED(HttpC _request)
	{
		m_fetchingScreenshot = false;
		Debug.Log("Screenshot FAILED");
	}

	private void HandleScreenshot(byte[] _bytes)
	{
		RemoveScreenshot();
		m_texture = new Texture2D(10, 10, TextureFormat.RGB24, false);
		m_texture.LoadImage(_bytes);
		if (m_texture.width > 255)
		{
			m_material.mainTexture = m_texture;
		}
		else
		{
			RemoveScreenshot();
		}
	}

	public override void Destroy()
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		if (m_material != null)
		{
			if (m_material.mainTexture != null && m_material.mainTexture.GetInstanceID() < 0)
			{
				Object.Destroy(m_material.mainTexture);
			}
			Object.Destroy(m_material);
		}
		if (m_texture != null)
		{
			Object.Destroy(m_texture);
		}
		m_texture = null;
		m_material = null;
		base.Destroy();
	}

	// The drawing library bakes the current scale of the component into the shapes it creates. The card is drawn again while its appear animation is
	// running (scale 0.4 - 0.6), which made it come out 1.6 - 2.2 times too big once the animation ended (the puzzle was the worst): it is always
	// drawn at its normal scale, the animation then scales the finished drawing.
	public override void DrawHandler(UIComponent _c)
	{
		Vector3 animatedScale = m_TC.transform.localScale;
		m_TC.transform.localScale = Vector3.one;
		try
		{
			DrawCard(_c);
		}
		finally
		{
			m_TC.transform.localScale = animatedScale;
		}
	}

	private void DrawCard(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity, false);
		if (m_texture != null)
		{
			m_material.mainTexture = m_texture;
		}
		else
		{
			m_material.mainTexture = ResourceManager.GetTexture(RESOURCE.ScreenshotDefault_Texture2D);
		}
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] bezierRect = DebugDraw.GetBezierRect(_c.m_actualWidth - m_cornerSize * 3f * (float)Screen.height, _c.m_actualHeight - m_cornerSize * 3f * (float)Screen.height, m_cornerSize * (float)Screen.height, 20, Vector2.zero);
		Vector2[] bezierRect2 = DebugDraw.GetBezierRect(_c.m_actualWidth - m_cornerSize * 2.5f * (float)Screen.height, _c.m_actualHeight - m_cornerSize * 2.5f * (float)Screen.height, m_cornerSize * (float)Screen.height, 20, Vector2.zero);
		UVRect normalizeRect = new UVRect(0f, 0.55f - _c.m_actualHeight / _c.m_actualWidth / 2f, 1f, _c.m_actualHeight / _c.m_actualWidth);
		uint num = DebugDraw.HexToUint("#0156b2");
		uint num2 = DebugDraw.HexToUint("#2e8599");
		uint num3 = DebugDraw.HexToUint("#1a64a8");
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 5f, bezierRect2, num, num2, ResourceManager.GetMaterial("Framework/SolidMat"), m_camera, string.Empty, normalizeRect);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 4f, bezierRect2, (float)Screen.height * 0.01f, DebugDraw.UIntToColor(num), DebugDraw.UIntToColor(num2), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 1f + Vector3.up * 2.5f, bezierRect, uint.MaxValue, uint.MaxValue, m_material, m_camera, string.Empty, normalizeRect);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -1f + Vector3.up * 2.5f, bezierRect, (float)Screen.height * 0.008f, DebugDraw.HexToColor("#41acee"), DebugDraw.HexToColor("#86d9f9"), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 0f + Vector3.up * 2.5f, bezierRect, (float)Screen.height * 0.03f, new Color(1f, 1f, 1f, 0.3f), ResourceManager.GetMaterial("Framework/Line8Gradient2Mat"), _c.m_camera, Position.Inside, true);
	}

	public static void AltDrawHandler(UIComponent _c)
	{
		PsUIScreenshot psUIScreenshot = _c as PsUIScreenshot;
		PrefabS.RemoveComponentsByEntity(psUIScreenshot.m_TC.p_entity, false);
		if (!psUIScreenshot.m_useTutorialTexture)
		{
			if (psUIScreenshot.m_texture != null)
			{
				psUIScreenshot.m_material.mainTexture = psUIScreenshot.m_texture;
			}
			else
			{
				psUIScreenshot.m_material.mainTexture = ResourceManager.GetTexture(RESOURCE.ScreenshotDefault_Texture2D);
			}
		}
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(psUIScreenshot.m_actualWidth, psUIScreenshot.m_actualHeight, psUIScreenshot.m_cornerSize * (float)Screen.height, 6, Vector2.zero);
		for (int i = 0; i < roundedRect.Length; i++)
		{
			roundedRect[i].x += roundedRect[i].y * 0.15f;
		}
		UVRect normalizeRect = new UVRect(0f, 0.55f - psUIScreenshot.m_actualHeight / psUIScreenshot.m_actualWidth / 2f, 1f, psUIScreenshot.m_actualHeight / psUIScreenshot.m_actualWidth);
		uint num = DebugDraw.HexToUint("#0156b2");
		uint num2 = DebugDraw.HexToUint("#2e8599");
		uint num3 = DebugDraw.HexToUint("#1a64a8");
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(psUIScreenshot.m_TC, Vector3.forward, roundedRect, uint.MaxValue, uint.MaxValue, psUIScreenshot.m_material, psUIScreenshot.m_camera, string.Empty, normalizeRect);
		PrefabS.CreatePathPrefabComponentFromVectorArray(psUIScreenshot.m_TC, Vector3.zero, roundedRect, (float)Screen.height * 0.01f, DebugDraw.HexToColor("#559bd8"), DebugDraw.HexToColor("#559bd8"), ResourceManager.GetMaterial("Framework/Line8Mat"), psUIScreenshot.m_camera, Position.Center, true);
	}
}
