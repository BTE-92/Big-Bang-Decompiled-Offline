using UnityEngine;

public class UIEditorPublishScreenshotSelect : UIVerticalList
{
	public class PsUIScreenshotButton : UICanvas
	{
		private Material m_material;

		public Screenshot m_screenshot;

		private bool m_highlighted;

		private PrefabC m_ps;

		private PrefabC m_p;

		public PsUIScreenshotButton(UIComponent _parent, Screenshot _screenshot)
			: base(_parent, true, string.Empty, null, string.Empty)
		{
			m_screenshot = _screenshot;
			SetDrawHandler(DrawHandler);
			m_material = Object.Instantiate(ResourceManager.GetMaterial(RESOURCE.ScreenshotMat_Material));
			if (m_screenshot != null)
			{
				m_material.mainTexture = m_screenshot.GetScreenshotTex2D();
			}
			SetHightlighted(false);
		}

		public override void DrawHandler(UIComponent _c)
		{
			float num = 0.007f;
			PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
			float num2 = (float)Screen.height * 0.005f;
			Vector2[] bezierRect = DebugDraw.GetBezierRect(_c.m_actualWidth - num * 3f * (float)Screen.height + num2, _c.m_actualHeight - num * 3f * (float)Screen.height + num2, num * (float)Screen.height + num2 / 2f, 10, Vector2.zero);
			m_ps = PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 3f, bezierRect, uint.MaxValue, uint.MaxValue, ResourceManager.GetMaterial("Framework/SolidMat"), m_camera, string.Empty);
			m_p = PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 2f, bezierRect, (float)Screen.height * 0.004f, Color.white, Color.white, ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
			PrefabS.SetVisibility(m_ps, m_highlighted);
			PrefabS.SetVisibility(m_p, m_highlighted);
			Vector2[] bezierRect2 = DebugDraw.GetBezierRect(_c.m_actualWidth - num * 3f * (float)Screen.height, _c.m_actualHeight - num * 3f * (float)Screen.height, num * (float)Screen.height, 10, Vector2.zero);
			UVRect normalizeRect = new UVRect(0f, 0.55f - _c.m_actualHeight / _c.m_actualWidth / 2f, 1f, _c.m_actualHeight / _c.m_actualWidth);
			PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 1f, bezierRect2, uint.MaxValue, uint.MaxValue, m_material, m_camera, string.Empty, normalizeRect);
			PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 0f, bezierRect2, (float)Screen.height * 0.004f, DebugDraw.HexToColor("#41acee"), DebugDraw.HexToColor("#86d9f9"), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		}

		public override void Destroy()
		{
			base.Destroy();
			Object.DestroyImmediate(m_material);
		}

		public void SetHightlighted(bool flag)
		{
			m_highlighted = flag;
			if (m_p != null)
			{
				PrefabS.SetVisibility(m_p, m_highlighted);
			}
			if (m_ps != null)
			{
				PrefabS.SetVisibility(m_ps, m_highlighted);
			}
			if (m_highlighted)
			{
				m_material.color = Color.white;
			}
			else
			{
				m_material.color = Color.gray;
			}
		}
	}

	private Material m_screenshotMaterial;

	private PsUIScreenshotButton m_selectedScreenshotButton;

	private PsUIScreenshotButton[] m_screenShotButtons;

	public UIEditorPublishScreenshotSelect(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		RemoveDrawHandler();
		m_screenshotMaterial = Object.Instantiate(ResourceManager.GetMaterial(RESOURCE.ScreenshotMat_Material));
		UIText uIText = new UIText(this, false, string.Empty, "Level Screenshot", "HurmeBold_Font", 0.03f, RelativeTo.ScreenHeight);
		uIText.SetColor("#a8e2ff");
		uIText.SetMargins(0.03f, 0f, 0f, 0f);
		uIText.SetHorizontalAlign(0f);
		UICanvas uICanvas = new UICanvas(this, false, "Screenshot", null, string.Empty);
		uICanvas.SetWidth(0.9f, RelativeTo.ParentWidth);
		uICanvas.SetHeight(0.65f, RelativeTo.OwnWidth);
		uICanvas.SetDrawHandler(ScreenshotDrawHandler);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, "swapButtons");
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		int num = 0;
		for (int i = 0; i < EditorScene.m_storedScreenshots.Length; i++)
		{
			if (EditorScene.m_storedScreenshots[i].m_hasShot)
			{
				num++;
			}
		}
		m_screenShotButtons = new PsUIScreenshotButton[Mathf.Max(1, num)];
		for (int j = 0; j < m_screenShotButtons.Length; j++)
		{
			m_screenShotButtons[j] = new PsUIScreenshotButton(uIHorizontalList, EditorScene.m_storedScreenshots[j]);
			m_screenShotButtons[j].SetWidth(0.12f, RelativeTo.ScreenShortest);
			m_screenShotButtons[j].SetHeight(0.65f, RelativeTo.OwnWidth);
		}
		if (m_screenShotButtons[0] != null)
		{
			SetScreenshot(0);
			m_selectedScreenshotButton = m_screenShotButtons[0];
			m_selectedScreenshotButton.SetHightlighted(true);
		}
	}

	public void SetScreenshot(Screenshot _screenshot)
	{
		if (_screenshot != null)
		{
			m_screenshotMaterial.mainTexture = _screenshot.GetScreenshotTex2D();
		}
		else
		{
			Debug.Log("TRYING TO SET NULL SCREENSHOT!");
		}
	}

	public void SetScreenshot(int _index)
	{
		if (m_screenShotButtons[_index].m_screenshot != null)
		{
			EditorScene.SetSelectedScreenshot(_index);
			m_screenshotMaterial.mainTexture = m_screenShotButtons[_index].m_screenshot.GetScreenshotTex2D();
		}
		else
		{
			Debug.Log("TRYING TO SET NULL SCREENSHOT!");
		}
	}

	public void ScreenshotDrawHandler(UIComponent _c)
	{
		float num = 0.02f;
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] bezierRect = DebugDraw.GetBezierRect(_c.m_actualWidth - num * 3f * (float)Screen.height, _c.m_actualHeight - num * 3f * (float)Screen.height, num * (float)Screen.height, 10, Vector2.zero);
		UVRect normalizeRect = new UVRect(0f, 0.55f - _c.m_actualHeight / _c.m_actualWidth / 2f, 1f, _c.m_actualHeight / _c.m_actualWidth);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 1f, bezierRect, uint.MaxValue, uint.MaxValue, m_screenshotMaterial, m_camera, string.Empty, normalizeRect);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 0f, bezierRect, (float)Screen.height * 0.006f, DebugDraw.HexToColor("#41acee"), DebugDraw.HexToColor("#86d9f9"), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -3f, bezierRect, (float)Screen.height * 0.025f, new Color(1f, 1f, 1f, 0.3f), ResourceManager.GetMaterial("Framework/Line8Gradient2Mat"), _c.m_camera, Position.Inside, true);
	}

	public override void Step()
	{
		for (int i = 0; i < m_screenShotButtons.Length; i++)
		{
			if (m_screenShotButtons[i] != m_selectedScreenshotButton && m_screenShotButtons[i].m_hit)
			{
				SetScreenshot(i);
				m_selectedScreenshotButton.SetHightlighted(false);
				m_selectedScreenshotButton = m_screenShotButtons[i];
				m_selectedScreenshotButton.SetHightlighted(true);
			}
		}
		base.Step();
	}

	public override void Destroy()
	{
		base.Destroy();
		Object.DestroyImmediate(m_screenshotMaterial);
	}
}
