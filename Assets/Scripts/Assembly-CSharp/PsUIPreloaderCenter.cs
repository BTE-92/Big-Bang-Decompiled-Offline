using System.Collections.Generic;
using UnityEngine;

public class PsUIPreloaderCenter : UICanvas
{
	private Entity m_loaderEntity;

	private UICanvas m_loadingBar;

	private List<PrefabC> m_prefabs = new List<PrefabC>();

	private int m_fadeOutTicks = 30;

	private bool m_fadeDone;

	private float m_fadeAlpha;

	private Material m_fadeMaterial;

	private UIVerticalList m_content;

	private HttpC m_download;

	private bool m_downloadFinished;

	private float m_progress;

	private UIText m_text;

	private float m_fakeProgress;

	public PsUIPreloaderCenter(UIComponent _parent)
		: base(_parent, false, "PreloaderCenter", null, string.Empty)
	{
		PsBackgroundDownloader.Initialize();
#if UNITY_WEBGL && !UNITY_EDITOR
		PsWebGLBankLoader.Begin();
#endif
		if (m_loaderEntity == null)
		{
			m_loaderEntity = EntityManager.AddEntity();
		}
		m_fadeDone = false;
		m_downloadFinished = false;
		m_fadeAlpha = 1f;
		m_progress = 0f;
		ServerManager.m_dontShowLoginPopup = true;
		m_fadeMaterial = new Material(Shader.Find("WOE/Unlit/ColorUnlitTransparent"));
		SetHeight(1f, RelativeTo.ScreenHeight);
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetDrawHandler(BackgroundDrawhandler);
		SetMargins(0.025f);
		m_loadingBar = new UICanvas(this, false, "loadingBar", null, string.Empty);
		m_loadingBar.SetWidth(0.5f, RelativeTo.ScreenWidth);
		m_loadingBar.SetHeight(0.06f, RelativeTo.ScreenHeight);
		m_loadingBar.SetVerticalAlign(0f);
		m_loadingBar.SetDrawHandler(LoadingBarDrawhandler);
		m_text = new UIText(m_loadingBar, false, string.Empty, "Checking for updates...", "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, "#ffffff");
		m_text.SetVerticalAlign(0.6f);
		PsMetrics.PreloadStart();
	}

	public void ChangeScene()
	{
		PsMetrics.PreloadFinished(m_downloadFinished);
		(Main.m_currentGame.m_currentScene as PreloadingScene).NextScene();
	}

	public override void Step()
	{
		if (!m_fadeDone)
		{
			m_fadeOutTicks--;
			if (m_fadeOutTicks <= 0)
			{
				m_fadeOutTicks = 0;
			}
			m_fadeAlpha = (float)m_fadeOutTicks / 30f;
			d_Draw(this);
			if (m_fadeOutTicks <= 0)
			{
				m_fadeDone = true;
			}
		}
		else
		{
			if (m_download != null && !m_downloadFinished)
			{
				if (m_download.www == null && m_downloadFinished)
				{
					m_progress = 1f;
				}
				else if (m_download.www != null)
				{
					m_progress = m_download.www.progress;
				}
				if (m_progress < 0.004f || m_progress < m_fakeProgress)
				{
					if (m_fakeProgress < 0.04f)
					{
						m_fakeProgress += 1E-05f;
					}
					m_progress = m_fakeProgress;
				}
				m_loadingBar.d_Draw(m_loadingBar);
			}
#if UNITY_WEBGL && !UNITY_EDITOR
			if (PsBackgroundDownloader.m_checksDone && PsWebGLBankLoader.IsReady)
#else
			if (PsBackgroundDownloader.m_checksDone)
#endif
			{
				ChangeScene();
			}
		}
		base.Step();
	}

	public void BackgroundDrawhandler(UIComponent _c)
	{
		for (int i = 0; i < m_prefabs.Count; i++)
		{
			PrefabS.RemoveComponent(m_prefabs[i]);
		}
		m_prefabs.Clear();
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth, _c.m_actualHeight, Vector2.zero);
		Color black = Color.black;
		black.a = m_fadeAlpha;
		uint num = DebugDraw.ColorToUInt(black);
		m_prefabs.Add(PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * -10f, rect, num, num, m_fadeMaterial, _c.m_camera, string.Empty));
		for (int j = 0; j < m_prefabs.Count; j++)
		{
			m_prefabs[j].p_gameObject.GetComponent<Renderer>().material.color = black;
		}
	}

	public void LoadingBarDrawhandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, _c.m_actualHeight / 3f, 8, Vector2.zero);
		Color color = DebugDraw.HexToColor("#06698f");
		uint num = DebugDraw.ColorToUInt(color);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * -1f, roundedRect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -0.5f, roundedRect, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		float num2 = _c.m_actualHeight / 2f;
		num = DebugDraw.HexToUint("#2fe217");
		Vector2[] rect = DebugDraw.GetRect((_c.m_actualWidth - num2) * m_progress, _c.m_actualHeight - num2 / 2f, -Vector2.right * (1f - m_progress) * 0.5f * (_c.m_actualWidth - num2));
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * -2f, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
	}

	public void GradientDrawhandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth, _c.m_actualHeight, Vector3.zero);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward, rect, DebugDraw.ColorToUInt(new Color(0f, 0f, 0f, 0.6f)), DebugDraw.ColorToUInt(new Color(0f, 0f, 0f, 0f)), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
	}

	public override void Destroy()
	{
		if (m_loaderEntity != null)
		{
			EntityManager.RemoveEntity(m_loaderEntity);
		}
		m_loaderEntity = null;
		Object.Destroy(m_fadeMaterial);
		m_fadeMaterial = null;
		base.Destroy();
	}
}
