using System.Collections.Generic;
using UnityEngine;

public abstract class PsUITutorialArrowBase : ITutorial
{
	protected Camera m_3Dcamera;

	protected Camera m_originalCamera;

	protected SpriteSheet m_fadeOutSpriteSheet;

	protected UICanvas m_canvas;

	protected UIFittedSprite m_finger;

	protected UIComponent offset;

	protected List<SpriteC> m_spriteList;

	protected bool m_spriteSheetSetToCamera;

	protected GameObject m_goTarget;

	protected TouchAreaC m_touchArea;

	protected bool m_destroy;

	public virtual void Step()
	{
	}

	public virtual void Destroy()
	{
	}

	protected virtual void GetTarget()
	{
	}

	protected void DrawCanvas()
	{
		m_canvas = new UICanvas(null, false, "TutorialCanvas", null, string.Empty);
		m_canvas.SetMargins(0f, 0f, 0f, 0f);
		m_canvas.RemoveDrawHandler();
		m_canvas.SetDepthOffset(-230f);
		offset = new UIComponent(m_canvas, false, "Tutorial offset", null, null, string.Empty);
		offset.SetSize(0f, 0f, RelativeTo.ScreenWidth);
		offset.SetMargins(-1f, -1f, -1f, -1f);
		UICanvas uICanvas = new UICanvas(offset, false, "Empty Tutorial Canvas", null, string.Empty);
		uICanvas.SetSize(0f, 0f, RelativeTo.ScreenWidth);
		uICanvas.SetMargins(-1f, -1f, -1f, -1f);
		m_finger = new UIFittedSprite(uICanvas, false, "Tutorial Finger", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_pointer_finger"));
		m_finger.SetHeight(0.1f, RelativeTo.ScreenWidth);
		Vector3 vector = m_3Dcamera.WorldToViewportPoint(m_goTarget.transform.position);
		float num = 0.022f;
		float num2 = 0.1f;
		offset.SetAlign(vector.x, vector.y);
		uICanvas.SetAlign(0.5f - num2, 0.5f - num);
		m_canvas.Update();
		m_finger.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find("WOE/Unlit/VertexColorUnlitDouble2");
		m_fadeOutSpriteSheet = SpriteS.AddSpriteSheet(m_canvas.m_camera, ResourceManager.GetMaterial(RESOURCE.LoadingScreenFadeMat_Material), 1f);
		SpriteC sprite = SpriteS.AddComponent(m_canvas.m_TC, new Frame(0f, 0f, Screen.width, Screen.height), m_fadeOutSpriteSheet);
		Color black = Color.black;
		SpriteS.SetColor(sprite, new Color(black.r, black.g, black.b, 0f));
		TweenC tweenC = TweenS.AddTransformTween(m_canvas.m_TC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f), 0.3f, 0f, true);
		TweenC c = TweenS.AddTransformTween(_startValue: new Vector3(-25f, 0f, 0f), _tc: m_finger.m_TC, _component: TweenedProperty.Position, _style: TweenStyle.QuadIn, _endValue: Vector3.zero, _duration: 0.4f, _delay: 0f, _removeComponentAtFinish: true);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.QuadOut);
	}

	protected virtual void SetCamera()
	{
		SetSpriteSheetToCamera();
		PrefabS.SetCamera(m_goTarget, m_3Dcamera);
		TouchAreaS.AddTouchEventListener(m_touchArea, TouchHandler);
		TouchAreaS.SetCamera(m_touchArea, m_3Dcamera);
		TouchAreaS.SetTouchCameraFilter(m_3Dcamera);
	}

	public void GetTargetSprites(Entity e)
	{
		m_spriteList = EntityManager.GetComponentsByEntity(ComponentType.Sprite, e).ConvertAll((IComponent x) => x as SpriteC);
	}

	protected virtual void SetSpriteSheetToCamera()
	{
		if (m_spriteList.Count > 0)
		{
			m_spriteList[0].p_spriteSheet.SetCamera(m_3Dcamera);
			m_spriteSheetSetToCamera = true;
		}
	}

	public virtual void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchCount == 1 && _touchPhase == TouchAreaPhase.ReleaseIn && !_touches[0].m_dragged)
		{
			TouchAreaS.RemoveTouchCameraFilter();
			Debug.LogWarning("touchi: " + _touchArea.m_name);
			Destroy();
		}
	}
}
