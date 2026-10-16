using UnityEngine;

public class FadeLoadingScene : ILoadingScene
{
	private StateMachine m_stateMachine;

	private IScene m_fromScene;

	private IScene m_toScene;

	private bool m_initComplete;

	private bool m_introComplete;

	private bool m_outroComplete;

	private bool m_introStarted;

	private bool m_outroStarted;

	public Camera m_camera;

	public SpriteSheet m_loadingScreenSpriteSheet;

	public Entity m_loadingScreenEntity;

	public TransformC m_loadingScreenTC;

	public Color m_fadeColor;

	public bool m_fadeIn;

	public float m_fadeDuration;

	public UICanvas m_bg;

	public StateMachine StateMachine
	{
		get
		{
			return m_stateMachine;
		}
		set
		{
			m_stateMachine = value;
		}
	}

	public IScene FromScene
	{
		get
		{
			return m_fromScene;
		}
		set
		{
			m_fromScene = value;
		}
	}

	public IScene ToScene
	{
		get
		{
			return m_toScene;
		}
		set
		{
			m_toScene = value;
		}
	}

	public bool InitComplete
	{
		get
		{
			return m_initComplete;
		}
	}

	public FadeLoadingScene(Color _fadeColor, bool _fadeIn = true, float _fadeDuration = 0.25f)
	{
		m_fadeColor = _fadeColor;
		m_fadeIn = _fadeIn;
		m_fadeDuration = _fadeDuration;
	}

	private void StartIntro()
	{
		if (m_fadeIn)
		{
			TweenC tweenC = TweenS.AddTransformTween(m_bg.m_TC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.zero, Vector3.one, m_fadeDuration, 0f, false);
			TweenS.SetTweenAlphaProperties(tweenC, false, true, false);
			TweenS.AddTweenEndEventListener(tweenC, IntroTweenEventHandler);
		}
		else
		{
			new PsServerQueueFlow(delegate
			{
				m_introComplete = true;
			}, null, null);
			new PsUILoadingText(m_bg);
			m_bg.Update();
		}
		m_introStarted = true;
	}

	public void StartOutro()
	{
		if (m_bg != null)
		{
			m_bg.DestroyChildren();
		}
		TweenC tweenC = TweenS.AddTransformTween(m_bg.m_TC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.one, Vector3.zero, m_fadeDuration, 0f, false);
		TweenS.SetTweenAlphaProperties(tweenC, false, true, false);
		TweenS.AddTweenEndEventListener(tweenC, OutroTweenEventHandler);
		m_outroStarted = true;
	}

	public void Load()
	{
		m_camera = CameraS.AddCamera("Loading Screen Camera", true);
		m_loadingScreenSpriteSheet = SpriteS.AddSpriteSheet(m_camera, ResourceManager.GetMaterial(RESOURCE.LoadingScreenFadeMat_Material), 1f);
		Initialize();
	}

	public void Initialize()
	{
		m_bg = new UICanvas(null, false, string.Empty, null, string.Empty);
		m_bg.SetCamera(m_camera, true, false);
		m_bg.SetWidth(1f, RelativeTo.ScreenWidth);
		m_bg.SetHeight(1f, RelativeTo.ScreenHeight);
		m_bg.SetDrawHandler(BackgroundDrawhandler);
		m_bg.Update();
		m_initComplete = true;
		StartIntro();
	}

	private void BackgroundDrawhandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(_c.m_actualWidth, _c.m_actualHeight, Vector2.zero);
		PrefabC prefabC = PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, rect, DebugDraw.HexToUint("#000000"), DebugDraw.HexToUint("#000000"), ResourceManager.GetMaterial(RESOURCE.MenuPopupBackgroundMat_Material), _c.m_camera, string.Empty);
		prefabC.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find("WOE/Unlit/ColorUnlitTransparentBg");
		Color color = new Color(0f, 0f, 0f, 1f);
		prefabC.p_gameObject.GetComponent<Renderer>().material.color = color;
	}

	private void IntroTweenEventHandler(TweenC _c)
	{
		new PsServerQueueFlow(delegate
		{
			m_introComplete = true;
		}, null, null);
		TweenS.RemoveTweenEndEventListener(_c, IntroTweenEventHandler);
		TweenS.RemoveComponent(_c);
		new PsUILoadingText(m_bg);
		m_bg.Update();
	}

	private void OutroTweenEventHandler(TweenC _c)
	{
		m_outroComplete = true;
		TweenS.RemoveTweenEndEventListener(_c, OutroTweenEventHandler);
		TweenS.RemoveComponent(_c);
	}

	public void Update()
	{
	}

	public void Destroy()
	{
		Debug.Log("FADE LOADING SCENE DESTROY");
		CameraS.RemoveCamera(m_camera);
		if (m_bg != null)
		{
			m_bg.Destroy();
			m_bg = null;
		}
	}

	public bool IntroComplete()
	{
		return m_introComplete;
	}

	public bool OutroComplete()
	{
		return m_outroComplete;
	}

	~FadeLoadingScene()
	{
	}
}
