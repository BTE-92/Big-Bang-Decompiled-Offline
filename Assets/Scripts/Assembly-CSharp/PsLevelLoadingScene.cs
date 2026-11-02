using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;

public class PsLevelLoadingScene : ILoadingScene, IStatedObject
{
	private StateMachine m_stateMachine;

	private IScene m_fromScene;

	private IScene m_toScene;

	private bool m_initComplete;

	private bool m_introComplete;

	private bool m_outroComplete;

	private bool m_introStarted;

	public bool m_outroStarted;

	public Camera m_camera;

	public Entity m_entity;

	public TransformC m_TC;

	private Texture2D m_texture;

	private Material m_material;

	private GameObject m_background;

	private SpriteSheet m_sheet;

	private SpriteSheet m_darkenSpriteSheet;

	private PsUIBaseState m_state;

	private bool m_fadeOut;

	private bool m_backgroundFinal;

	private bool m_backgroundIsFallback;

	private float m_blurWaitStart = -1f;

	private bool m_blurTimedOut;

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

	private void StartIntro()
	{
		m_introStarted = true;
		TouchAreaS.Enable();
	}

	public void StartOutro()
	{
		m_outroStarted = true;
		if (m_background == null)
		{
			CreateFallbackBackground();
		}
		CameraS.BringToFront(m_camera);
		m_state.BringToFront();
	}

	public void Load()
	{
		Initialize();
	}

	public void Initialize()
	{
		m_camera = CameraS.AddCamera("Loading Screen Camera", true);
		m_entity = EntityManager.AddEntity("LoadingScreenEntity");
		m_entity.m_persistent = true;
		m_TC = TransformS.AddComponent(m_entity, "LoadingScreenTC", Vector3.forward * -240f);
		m_stateMachine = new StateMachine(this);
		if (PsState.m_activeGameLoop is PsGameLoopVersus || PsState.m_activeGameLoop is PsGameLoopEditorPuzzle || PsState.m_activeGameLoop is PsGameLoopEditorTemplate)
		{
			m_state = new PsUIBaseState(typeof(PsUICenterLevelLoading), typeof(PsUITopLevelLoadingVersus));
		}
		else
		{
			m_state = new PsUIBaseState(typeof(PsUICenterLevelLoading), typeof(PsUITopLevelLoading));
		}
		m_state.SetAction("StartGame", StartGame);
		m_state.SetAction("Exit", Exit);
		m_darkenSpriteSheet = SpriteS.AddSpriteSheet(m_camera, ResourceManager.GetMaterial(RESOURCE.LoadingScreenFadeMat_Material), 1f);
		SpriteC sprite = SpriteS.AddComponent(m_TC, new Frame(0f, 0f, Screen.width, Screen.height), m_darkenSpriteSheet);
		SpriteS.SetColor(sprite, new Color(0f, 0f, 0f, 0f));
		TweenS.AddTransformTween(m_TC, TweenedProperty.Alpha, TweenStyle.Linear, Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f), 0.2f, 0f, true);
		m_stateMachine.ChangeState(m_state);
		TryCreateBackground();
		m_initComplete = true;
		m_introComplete = PsState.m_activeGameLoop.m_minigameBytes != null;
		StartIntro();
	}

	public void LevelLoaded()
	{
		TryCreateBackground();
		m_introComplete = true;
	}

	private RenderTexture GetStoredBlurTexture()
	{
		PsPlanet loadingPlanet = PsPlanetManager.GetCurrentPlanet();
		if (loadingPlanet != null && loadingPlanet.m_planetCamera != null)
		{
			BlurOptimizedWOE blurComponent = loadingPlanet.m_planetCamera.gameObject.GetComponent<BlurOptimizedWOE>();
			if (blurComponent != null)
			{
				return blurComponent.storedTex;
			}
		}
		return null;
	}

	private void FillBackgroundTexture(RenderTexture _storedTex)
	{
		RenderTexture.active = _storedTex;
		m_texture.ReadPixels(new Rect(0f, 0f, _storedTex.width, _storedTex.height), 0, 0);
		m_texture.Apply();
		RenderTexture.active = null;
	}

	private void TryCreateBackground()
	{
		if (m_background != null)
		{
			return;
		}
		RenderTexture storedTex = GetStoredBlurTexture();
		if (storedTex != null)
		{
			m_texture = new Texture2D(storedTex.width, storedTex.height, TextureFormat.RGB24, false, true);
			m_texture.wrapMode = TextureWrapMode.Clamp;
			FillBackgroundTexture(storedTex);
			CreateBackgroundQuad();
			m_backgroundFinal = PsMenuScene.m_blur != null && PsMenuScene.m_blur.done;
		}
	}

	private void CreateBackgroundQuad()
	{
		m_material = Object.Instantiate(ResourceManager.GetMaterial(RESOURCE.LevelLoadingBackgroundMat_Material));
		m_material.mainTexture = m_texture;
		m_background = GameObject.CreatePrimitive(PrimitiveType.Quad);
		m_background.name = "BluredScreen";
		m_background.layer = m_camera.gameObject.layer;
		Object.Destroy(m_background.GetComponent<Collider>());
		m_background.transform.position = new Vector3(0f, 0f, 0f);
		m_background.transform.localScale = new Vector3(Screen.width, Screen.height, 1f);
		Renderer backgroundRenderer = m_background.GetComponent<Renderer>();
		backgroundRenderer.sharedMaterial = m_material;
		backgroundRenderer.receiveShadows = false;
		backgroundRenderer.shadowCastingMode = ShadowCastingMode.Off;
	}

	// No blurred picture of the menu yet (the blur has not been rendered once, as on the first level the first time the game runs): the menu is about
	// to be removed and the level to be built, which must not show before the player presses play. A plain dark cover is drawn in the meantime; it
	// is replaced by the blurred menu as soon as that exists.
	private void CreateFallbackBackground()
	{
		m_texture = new Texture2D(2, 2, TextureFormat.RGB24, false, true);
		m_texture.wrapMode = TextureWrapMode.Clamp;
		Color cover = new Color(0.07f, 0.12f, 0.2f);
		m_texture.SetPixels(new Color[4] { cover, cover, cover, cover });
		m_texture.Apply();
		CreateBackgroundQuad();
		m_backgroundIsFallback = true;
		m_backgroundFinal = false;
		Debug.LogError("[LoadingBG] no blurred menu yet: dark cover used");
	}

	private void StartGame()
	{
		TouchAreaS.Disable();
		if (!(PsState.m_activeGameLoop is PsGameLoopEditorPuzzle) && !(PsState.m_activeGameLoop is PsGameLoopEditorTemplate))
		{
			PsState.m_activeGameLoop.EnterMinigame();
		}
		else
		{
			PsState.m_activeGameLoop.StartEditorRecording();
		}
		m_fadeOut = true;
		TweenS.AddTransformTween(m_TC, TweenedProperty.Alpha, TweenStyle.Linear, new Vector3(0.5f, 0.5f, 0.5f), Vector3.zero, 0.2f, 0f, true);
		TimerS.AddComponent(EntityManager.AddEntity(), string.Empty, 0.25f, 0f, true, delegate
		{
			TouchAreaS.Enable();
			m_outroComplete = true;
		});
	}

	private void Exit()
	{
		if (m_outroStarted)
		{
			PsState.m_activeGameLoop.ExitMinigame();
			if (PsState.m_activeGameLoop is PsGameLoopEditorPuzzle)
			{
				(PsState.m_activeGameLoop as PsGameLoopEditorPuzzle).ExitEditor(false);
			}
			else if (PsState.m_activeGameLoop is PsGameLoopEditorTemplate)
			{
				(PsState.m_activeGameLoop as PsGameLoopEditorTemplate).ExitEditor(false);
			}
			else if (PsState.m_activeGameLoop is PsGameLoopFresh)
			{
				(PsState.m_activeGameLoop as PsGameLoopFresh).ExitRating();
			}
		}
		else
		{
			PsState.m_activeGameLoop.ExitLoadingScene();
		}
	}

	public void Update()
	{
		if (m_fadeOut && m_material != null)
		{
			Color color = m_material.color;
			color.a = Mathf.Lerp(m_material.color.a, 0f, 0.25f);
			m_material.color = color;
		}
		if (m_background == null)
		{
			TryCreateBackground();
		}
		if (m_background == null && m_blurTimedOut)
		{
			CreateFallbackBackground();
		}
		if (m_background != null && m_backgroundIsFallback && !m_outroStarted)
		{
			RenderTexture blurred = GetStoredBlurTexture();
			if (blurred != null && PsMenuScene.m_blur != null && PsMenuScene.m_blur.done)
			{
				m_texture.Resize(blurred.width, blurred.height, TextureFormat.RGB24, false);
				FillBackgroundTexture(blurred);
				m_backgroundIsFallback = false;
				m_backgroundFinal = true;
			}
		}
		if (m_background != null && !m_backgroundFinal && !m_backgroundIsFallback && PsMenuScene.m_blur != null && PsMenuScene.m_blur.done)
		{
			RenderTexture finalTex = GetStoredBlurTexture();
			if (finalTex != null && finalTex.width == m_texture.width && finalTex.height == m_texture.height)
			{
				FillBackgroundTexture(finalTex);
			}
			m_backgroundFinal = true;
		}
		m_stateMachine.Update();
	}

	public bool IntroComplete()
	{
		return m_introComplete && BackgroundReady();
	}

	// The menu is removed and the level built when the intro is complete. Before that the blurred picture of the menu has to be ready, whether it
	// is the first level the game opens or not: the first time the blur has not been rendered once yet, and the level showed through. The wait is
	// a few frames; it gives up after two seconds (the dark cover is used then).
	private bool BackgroundReady()
	{
		if (!(m_fromScene is PsMenuScene) || PsMenuScene.m_blur == null)
		{
			return true;
		}
		if (m_background != null && m_backgroundFinal && !m_backgroundIsFallback)
		{
			return true;
		}
		if (m_blurWaitStart < 0f)
		{
			m_blurWaitStart = Time.realtimeSinceStartup;
		}
		if (Time.realtimeSinceStartup - m_blurWaitStart > 2f)
		{
			m_blurTimedOut = true;
			return true;
		}
		return false;
	}

	public bool OutroComplete()
	{
		return m_outroComplete;
	}

	public void Destroy()
	{
		Debug.Log("LEVEL LOADING SCENE DESTROY");
		CameraS.RemoveCamera(m_camera);
		m_camera = null;
		if (m_darkenSpriteSheet != null)
		{
			SpriteS.RemoveSpriteSheet(m_darkenSpriteSheet);
			m_darkenSpriteSheet = null;
		}
		if (m_sheet != null)
		{
			SpriteS.RemoveSpriteSheet(m_sheet);
			m_sheet = null;
		}
		if (m_material != null)
		{
			Object.Destroy(m_material);
			m_material = null;
		}
		if (m_texture != null)
		{
			Object.Destroy(m_texture);
			m_texture = null;
		}
		if (m_background != null)
		{
			Object.Destroy(m_background);
			m_background = null;
		}
		EntityManager.RemoveEntity(m_entity);
		m_entity = null;
		TransformS.RemoveComponent(m_TC);
		m_TC = null;
		m_stateMachine.Destroy();
		m_stateMachine = null;
	}
}
