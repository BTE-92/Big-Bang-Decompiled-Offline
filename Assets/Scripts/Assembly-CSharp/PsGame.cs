using System;
using UnityEngine;

public class PsGame : IGame
{
	private string _projectCode;

	private string _projectVersion;

	private IScene _currentScene;

	private SceneManager _sceneManager;

	private float m_delta;

	private float m_cumulatedFrameTime;

	private double m_lastOutOfFocusTime;

	private Shader m_editorReplaceShader;

	private static UICanvas m_debugInfoCanvas;

	private static UITextbox m_debugInfoTextbox;

	public string m_projectCode
	{
		get
		{
			return _projectCode;
		}
		set
		{
			_projectCode = value;
		}
	}

	public string m_projectVersion
	{
		get
		{
			return _projectVersion;
		}
		set
		{
			_projectVersion = value;
		}
	}

	public IScene m_currentScene
	{
		get
		{
			return _currentScene;
		}
		set
		{
			_currentScene = value;
		}
	}

	public SceneManager m_sceneManager
	{
		get
		{
			return _sceneManager;
		}
		set
		{
			_sceneManager = value;
		}
	}

	public bool PreloaderIsActive
	{
		get
		{
			return _currentScene is PreloadingScene;
		}
	}

	public PsGame(string _projectCode, string _projectVersion)
	{
        QualitySettings.vSyncCount = 0;
        m_lastOutOfFocusTime = 0.0;
		m_sceneManager = new SceneManager();
		Application.targetFrameRate = Main.m_targetFPS;
		m_projectCode = _projectCode;
		m_projectVersion = _projectVersion;
		#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
		Debug.Initialize(false, false, false, false);
#else
        Debug.Initialize(false, true, false, false);
#endif
		FacebookManager.Initialize();
		EveryplayManager.SetEnabled(PsState.m_everyplayEnabled);
		EveryplayManager.Initialize();
		PsAdMediation.Initialize();
		PsMetrics.Initialize();
		ResourceManager.m_preloadResourcesByDefault = false;
		ResourceManager.PreloadResourceGroupFromFolder("Framework");
		ResourcePool.Initialize("PlaySomething/ResourcePool");
		ResourcePool.LoadResourceGroup(RESOURCE_GROUP.SHARED);
		m_editorReplaceShader = Shader.Find("WOE/Editor/UnitMaskReplaceShader");
		TextAsset textAsset = ResourceManager.GetTextAsset(RESOURCE.RandomLevelNamesBase_TextAsset);
		PsResources.LevelNameGeneratorBaseStrings = textAsset.text.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.None);
		TextMeshS.Initialize();
		CacheManager.Initialize();
		PsCaches.Initialize();
		EntityManager.Initialize();
		TransformS.Initialize();
		PrefabS.Initialize();
		TouchAreaS.Initialize();
		TouchAreaS.Disable();
		TweenS.Initialize();
		TimerS.Initialize();
		SpriteS.Initialize();
		ProjectorS.Initialize();
		HttpS.Initialize();
		MonoBehaviourS.Initialize();
		CameraS.Initialize();
		CameraS.m_mainCameraRenderTexture = new RenderTexture(Screen.width, Screen.height, 0, ToolBox.GetSupportedRenderTextureFormat(RenderTextureFormat.RGB565));
		CameraS.m_mainCameraRenderTexture.useMipMap = false;
		CameraS.m_mainCameraRenderTexture.anisoLevel = 0;
		UnityUIManager.Initialize();
		SoundS.m_canPlaySounds = !PsState.m_muteSoundFX;
		NotificationManager.Initialize();
		InitPhysics();
		PsS.Initialize();
		PsMetagameManager.Initialize();
		PsAchievementManager.Initialize();
		AutoGeometryLayer.ENABLE_AUTODECOS = PsState.m_enableAutodecos;
		DebugDraw.m_lineWidth = 3f;
		DebugDraw.Initialize();
		PsState.m_uiSheet = SpriteS.AddSpriteSheet(CameraS.m_uiCamera, ResourceManager.GetMaterial(RESOURCE.UiAtlasMat_Material), ResourceManager.GetTextAsset(RESOURCE.UiAtlas_TextAsset), 1f);
	}

	public static void CreateSystemInfoBox()
	{
		m_debugInfoCanvas = new UICanvas(null, false, "SystemInfoCanvas", null, string.Empty);
		m_debugInfoCanvas.SetHeight(0.3f, RelativeTo.ScreenHeight);
		m_debugInfoCanvas.SetWidth(0.2f, RelativeTo.ScreenWidth);
		m_debugInfoCanvas.SetDrawHandler(UIDrawHandlers.EditorPopupContentArea);
		m_debugInfoCanvas.SetMargins(0.1f, 0.1f, 0.1f, 0.1f, RelativeTo.OwnHeight);
		m_debugInfoCanvas.SetVerticalAlign(0f);
		m_debugInfoCanvas.SetHorizontalAlign(1f);
		m_debugInfoCanvas.SetDepthOffset(-100f);
		m_debugInfoTextbox = new UITextbox(m_debugInfoCanvas, false, "SystemInfo", string.Empty, "HurmeSemiBold_Font", 0.012f, RelativeTo.ScreenShortest, true, Align.Center);
		m_debugInfoTextbox.SetVerticalAlign(1f);
		m_debugInfoCanvas.Update();
	}

	public static void UpdateSystemInfo()
	{
		if (Main.m_gameTicks % 60 == 0 && m_debugInfoTextbox != null)
		{
			m_debugInfoTextbox.SetText(Debug.GetSystemStats());
		}
	}

	public static void InitPhysics()
	{
		Array values = Enum.GetValues(typeof(PsCollisionType));
		ChipmunkProS.Initialize((int)values.GetValue(values.Length - 1));
		ChipmunkProWrapper.ucpSpaceSetCollisionSlop(2f);
		ChipmunkProWrapper.ucpSpaceSetSleepTimeThreshold(0.5f);
		ChipmunkProWrapper.ucpSpaceSetIdleSpeedThreshold(5f);
		ChipmunkProWrapper.ucpSpaceSetIterations(8);
		ChipmunkProWrapper.ucpSpaceSetGravity(new Vector2(0f, -550f));
		PsGlobalCollisionHandlers.Initialize();
	}

	public void OnApplicationPause(bool _pauseStatus)
	{
		Debug.Log("OnApplicationPause: " + _pauseStatus);
		if (m_currentScene == null || PreloaderIsActive || PsState.m_inStartupSequence)
		{
			return;
		}
		if (_pauseStatus)
		{
			m_lastOutOfFocusTime = Main.m_EPOCHSeconds;
			if (PsState.m_canAutoPause)
			{
				PsState.m_activeGameLoop.PauseMinigame();
				PsState.m_activeGameLoop.PauseElapsedTimer();
				if (m_currentScene.m_stateMachine != null)
				{
					m_currentScene.m_stateMachine.Update();
				}
				Debug.LogError("Game autopaused.");
			}
			return;
		}
		Teleport.m_forceRefresh = true;
		m_lastOutOfFocusTime = Main.m_EPOCHSeconds - m_lastOutOfFocusTime;
		Debug.LogWarning("Check for need to do a new login...");
		bool flag = false;
		if (!ServerManager.m_hasAlertViewOnScreen && !PsState.m_inLoginFlow)
		{
			if (!ServerManager.m_playerAuthenticated)
			{
				flag = true;
			}
			else if (!PsState.m_adWatched)
			{
				flag = true;
			}
		}
		if (flag)
		{
			Debug.Log("back to app: RELOGGING");
			ServerManager.Login();
		}
		else
		{
			Debug.Log("back to app: NO NEED TO RELOG");
			PsMetrics.ReturnedFromBackground();
		}
		PsState.m_adWatched = false;
	}

	public void OnApplicationFocus(bool _focusStatus)
	{
	}

	public void LateUpdate()
	{
		if (PsState.m_activeMinigame != null)
		{
			if (PsState.m_activeMinigame.m_editing)
			{
				CameraS.RenderMainCameraToTexture(m_editorReplaceShader);
			}
			else
			{
				CameraS.ClearMainCameraRenderTexture();
			}
		}
	}

	public void Initialize(IScene _scene)
	{
		m_sceneManager.ChangeScene(new PreloadingScene(_scene));
	}

	public void RemoveComponent(IComponent _c)
	{
		switch (_c.m_componentType)
		{
		case (ComponentType)32:
			PsS.RemoveGround(_c as GroundC);
			break;
		case (ComponentType)31:
			PsS.RemoveCustomObject(_c as CustomObjectC);
			break;
		case (ComponentType)30:
			PsS.RemoveUnit(_c as UnitC);
			break;
		}
	}

	public void Update()
	{
		float num = Main.m_dt * Main.m_timeScale;
		double num2 = Main.m_dtD * Main.m_timeScaleD;
		CameraS.ClearMainCameraRenderTexture();
		Main.m_gameDeltaTime = num;
		Main.m_gameTicks++;
		Main.m_gameTimeSinceAppStarted += num2;
		Main.m_resettingGameTime += num;
		TouchAreaS.Update();
		m_sceneManager.UpdateLogic();
		UIManager.Update();
		UnityUIManager.Update();
		PsMetagameManager.Update();
		PsBackgroundDownloader.Update();
		NotificationManager.Update();
		ChipmunkProS.Update((!PsState.m_physicsPaused) ? num : 0f);
		LevelManager.Update();
		PsS.Update();
		MonoBehaviourS.Update();
		EntityManager.UpdateLogic();
		AutoGeometryManager.Update();
		TransformS.Update();
		SpriteS.Update();
		PrefabS.Update();
		TextMeshS.Update();
		ProjectorS.Update();
		CameraS.Update();
		SoundS.Update();
		HttpS.Update();
		TimerS.Update();
		TweenS.Update();
		ServerManager.Update();
		CacheManager.Update();
		EntityManager.Update();
		GizmoManager.UpdatePosition();
		EveryplayManager.Update();
		UpdateSystemInfo();
		/*
		if (Input.GetKey(KeyCode.B))
		{
			OnApplicationPause(false);
		}
		*/
	}
}
