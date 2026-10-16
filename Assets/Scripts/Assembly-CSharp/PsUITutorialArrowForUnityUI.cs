using UnityEngine;
using UnityEngine.UI;

public class PsUITutorialArrowForUnityUI : ITutorial
{
	protected SpriteSheet m_fadeOutSpriteSheet;

	protected UICanvas m_canvas;

	protected UIFittedSprite m_finger;

	protected UIComponent offset;

	private Camera m_arrowCamera;

	private Vector2 m_offset;

	private Transform m_target;

	private Transform m_originalParent;

	private int m_originalSiblingIndex;

	private bool m_destroy;

	private bool hasTarget;

	private GameObject m_background;

	private Image m_backgroundImage;

	public bool m_destroyed;

	public PsUITutorialArrowForUnityUI(Transform _target, Vector2 _offset, bool _justDarken = false)
	{
		m_target = _target;
		m_offset = _offset;
		m_arrowCamera = CameraS.AddCamera("TutorialCamera", true);
		PsMetagameManager.m_tutorialArrow = this;
		m_background = new GameObject("Darken", typeof(CanvasRenderer));
		m_background.transform.SetParent(UnityUIManager.m_canvas.transform);
		m_background.transform.SetAsLastSibling();
		m_backgroundImage = m_background.AddComponent<Image>();
		m_backgroundImage.color = new Color(0f, 0f, 0f, 0.5f);
		RectTransform rectTransform = m_background.transform as RectTransform;
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.localScale = Vector3.one;
		if (!_justDarken)
		{
			CreateArrow();
		}
	}

	public void CreateArrow()
	{
		m_originalParent = m_target.parent;
		m_originalSiblingIndex = m_target.GetSiblingIndex();
		m_target.SetParent(UnityUIManager.m_canvas.transform, true);
		m_target.SetAsLastSibling();
		m_canvas = new UICanvas(null, false, "TutorialCanvas", null, string.Empty);
		m_canvas.m_camera = m_arrowCamera;
		m_canvas.m_TC.transform.gameObject.layer = 1 << m_arrowCamera.cullingMask;
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
		Vector3 vector = m_arrowCamera.WorldToViewportPoint(m_target.position);
		offset.SetAlign(vector.x, vector.y);
		uICanvas.SetAlign(0.5f + m_offset.x, 0.5f + m_offset.y);
		m_canvas.Update();
		m_finger.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find("WOE/Unlit/VertexColorUnlitDouble2");
		TweenC tweenC = TweenS.AddTransformTween(m_canvas.m_TC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f), 0.3f, 0f, true);
		TweenC c = TweenS.AddTransformTween(_startValue: new Vector3(-25f, 0f, 0f), _tc: m_finger.m_TC, _component: TweenedProperty.Position, _style: TweenStyle.QuadIn, _endValue: Vector3.zero, _duration: 0.4f, _delay: 0f, _removeComponentAtFinish: true);
		TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.QuadOut);
	}

	public virtual void Step()
	{
	}

	public virtual void Destroy()
	{
		if (PsMetagameManager.m_tutorialArrow == this)
		{
			PsMetagameManager.m_tutorialArrow = null;
		}
		m_target.SetParent(m_originalParent);
		m_target.SetSiblingIndex(m_originalSiblingIndex);
		if (m_canvas != null)
		{
			m_canvas.Destroy();
			m_canvas = null;
		}
		if (m_arrowCamera != null)
		{
			CameraS.RemoveCamera(m_arrowCamera);
		}
		if (m_background != null)
		{
			Object.Destroy(m_background);
		}
		m_destroyed = true;
	}
}
