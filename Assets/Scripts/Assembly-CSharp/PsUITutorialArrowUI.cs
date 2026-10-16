using System;
using UnityEngine;

public class PsUITutorialArrowUI : ITutorial
{
	protected int layernumberUI;

	protected Camera m_tutorialCamera;

	protected Camera m_originalCamera;

	protected SpriteSheet m_fadeOutSpriteSheet;

	protected UICanvas m_canvas;

	protected UIFittedSprite m_finger;

	protected UIComponent offset;

	public UICanvas m_emptyCanvas;

	protected UIComponent m_uiTarget;

	protected TouchAreaC m_touchArea;

	protected bool m_destroy;

	private bool m_fadeIn;

	private Action m_callBack;

	public bool m_desroyed;

	public PsUITutorialArrowUI(UIComponent _target, bool _fadeIn = true, Action _callBack = null)
	{
		TouchAreaS.Enable();
		PsMetagameManager.m_tutorialArrow = this;
		m_uiTarget = _target;
		m_fadeIn = _fadeIn;
		m_callBack = _callBack;
		m_originalCamera = _target.m_camera;
		GetTouchArea();
		SetCamera();
		DrawCanvas();
	}

	public void GetTouchArea()
	{
		m_touchArea = m_uiTarget.m_TAC;
	}

	public void DrawCanvas()
	{
		bool flag = false;
		m_canvas = new UICanvas(null, false, "TutorialCanvas", null, string.Empty);
		m_canvas.SetMargins(0f, 0f, 0f, 0f);
		m_canvas.RemoveDrawHandler();
		m_canvas.SetDepthOffset(-230f);
		offset = new UIComponent(m_canvas, false, "Tutorial offset", null, null, string.Empty);
		offset.SetSize(0f, 0f, RelativeTo.ScreenWidth);
		offset.SetMargins(-1f, -1f, -1f, -1f);
		m_emptyCanvas = new UICanvas(offset, false, "Empty Tutorial Canvas", null, string.Empty);
		m_emptyCanvas.SetSize(0f, 0f, RelativeTo.ScreenWidth);
		m_emptyCanvas.SetDepthOffset(-10f);
		m_emptyCanvas.SetMargins(-1f, -1f, -1f, -1f);
		m_finger = new UIFittedSprite(m_emptyCanvas, false, "Tutorial Finger", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_pointer_finger"));
		m_finger.SetHeight(0.1f, RelativeTo.ScreenWidth);
		Vector3 vector = m_tutorialCamera.WorldToViewportPoint(m_uiTarget.m_TC.transform.position);
		float num = 0.022f;
		float num2 = 0.06f;
		offset.SetAlign(vector.x, vector.y);
		m_emptyCanvas.SetAlign(0.5f - num2, 0.5f - num);
		float num3 = (0f - m_uiTarget.m_actualWidth) / 2f;
		if ((double)m_tutorialCamera.WorldToViewportPoint(m_uiTarget.m_TC.transform.position).x < 0.5)
		{
			num3 *= -1f;
			flag = true;
		}
		Vector3 vector2 = m_tutorialCamera.WorldToViewportPoint(m_uiTarget.m_TC.transform.position + new Vector3(num3, 0f, 0f));
		offset.SetAlign(vector2.x, vector2.y);
		m_canvas.Update();
		m_finger.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find("WOE/Unlit/VertexColorUnlitDouble2");
		PrefabS.SetCamera(m_canvas.m_TC.transform.gameObject, m_tutorialCamera);
		if (flag)
		{
			offset.m_TC.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		Camera camera = null;
		GameObject gameObject = GameObject.Find("OverlayCamera");
		if (gameObject != null && gameObject.GetComponent<Camera>() != null)
		{
			camera = gameObject.GetComponent<Camera>();
		}
		if (camera == null)
		{
			camera = m_originalCamera;
		}
		m_fadeOutSpriteSheet = SpriteS.AddSpriteSheet(camera, ResourceManager.GetMaterial(RESOURCE.LoadingScreenFadeMat_Material), 1f);
		SpriteC sprite = SpriteS.AddComponent(m_canvas.m_TC, new Frame(0f, 0f, Screen.width, Screen.height), m_fadeOutSpriteSheet);
		Color black = Color.black;
		if (m_fadeIn)
		{
			SpriteS.SetColor(sprite, new Color(black.r, black.g, black.b, 0f));
			TweenC tweenC = TweenS.AddTransformTween(m_canvas.m_TC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f), 0.3f, 0f, true);
			tweenC.useUnscaledDeltaTime = true;
		}
		else
		{
			SpriteS.SetColor(sprite, new Color(black.r, black.g, black.b, 0.5f));
		}
		TweenC tweenC2 = TweenS.AddTransformTween(_startValue: new Vector3(-25f, 0f, 0f), _tc: m_finger.m_TC, _component: TweenedProperty.Position, _style: TweenStyle.QuadIn, _endValue: Vector3.zero, _duration: 0.4f, _delay: 0f, _removeComponentAtFinish: true);
		tweenC2.useUnscaledDeltaTime = true;
		TweenS.SetAdditionalTweenProperties(tweenC2, -1, true, TweenStyle.QuadOut);
	}

	public void SetCamera()
	{
		layernumberUI = 11;
		GameObject gameObject = new GameObject("Tutorial Camera");
		m_tutorialCamera = gameObject.AddComponent<Camera>();
		CameraS.m_cameras.Add(m_tutorialCamera);
		m_tutorialCamera.CopyFrom(CameraS.m_uiCamera);
		gameObject.transform.position += new Vector3(0f, 0f, -200f);
		gameObject.layer = layernumberUI;
		m_tutorialCamera.cullingMask = 0;
		m_tutorialCamera.cullingMask |= 1 << layernumberUI;
		m_tutorialCamera.depth = 15f;
		SetTouchAreas(m_tutorialCamera);
		PrefabS.SetCamera(m_uiTarget.m_TC.transform.gameObject, m_tutorialCamera);
		m_uiTarget.m_camera = m_tutorialCamera;
	}

	public void SetTouchAreas(Camera _camera)
	{
		TouchEventDelegate a = TouchHandler;
		a = (TouchEventDelegate)Delegate.Combine(a, m_touchArea.d_TouchEventDelegate);
		m_touchArea.d_TouchEventDelegate = a;
		TouchAreaS.SetCamera(m_touchArea, _camera);
		TouchAreaS.SetTouchCameraFilter(_camera);
	}

	public virtual void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchCount == 1 && (_touchPhase == TouchAreaPhase.ReleaseIn || (!_touchArea.m_allowPrimary && _touchPhase == TouchAreaPhase.RollIn)) && !_touches[0].m_dragged)
		{
			if (m_callBack != null)
			{
				m_callBack();
			}
			TouchAreaS.RemoveTouchCameraFilter();
			Debug.LogWarning("touchi: " + _touchArea.m_name);
			Destroy();
		}
	}

	public virtual void Step()
	{
		if (m_destroy)
		{
		}
		float num = (0f - m_uiTarget.m_actualWidth) / 2f;
		if ((double)m_tutorialCamera.WorldToViewportPoint(m_uiTarget.m_TC.transform.position).x < 0.5)
		{
			num *= -1f;
		}
		Vector3 vector = m_tutorialCamera.WorldToViewportPoint(m_uiTarget.m_TC.transform.position + new Vector3(num, 0f, 0f));
		offset.SetAlign(vector.x, vector.y);
		offset.UpdateAlign();
		if (m_uiTarget.m_TC.transform.lossyScale == Vector3.zero)
		{
			if (m_callBack != null)
			{
				m_callBack();
			}
			TouchAreaS.RemoveTouchCameraFilter();
			Destroy();
		}
	}

	public virtual void Destroy()
	{
		if (PsMetagameManager.m_tutorialArrow == this)
		{
			PsMetagameManager.m_tutorialArrow = null;
		}
		if (m_fadeOutSpriteSheet != null)
		{
			SpriteS.RemoveSpriteSheet(m_fadeOutSpriteSheet);
			m_fadeOutSpriteSheet = null;
		}
		m_canvas.Destroy();
		TouchAreaS.RemoveTouchCameraFilter();
		TouchAreaS.RemoveTouchEventListener(m_touchArea, TouchHandler);
		TouchAreaS.SetCamera(m_touchArea, m_originalCamera);
		PrefabS.SetCamera(m_uiTarget.m_TC.transform.gameObject, m_originalCamera);
		m_uiTarget.SetCamera(m_originalCamera, true, false);
		CameraS.m_cameras.Remove(m_tutorialCamera);
		UnityEngine.Object.Destroy(m_tutorialCamera.gameObject);
		m_desroyed = true;
	}

	public static void DrawHandler(UIComponent _c)
	{
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth, _c.m_actualHeight, Vector2.zero);
		Color color = new Color(0f, 0f, 0f, 0.3f);
		uint num = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, rect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
	}
}
