using System;
using UnityEngine;

public class EditorScene : IScene, IStatedObject
{
	private const int MAX_STORED_SCREENSHOTS = 3;

	public string _name;

	private StateMachine _stateMachine = new StateMachine();

	private bool _initComplete;

	private bool m_toTestState;

	public static GameObject m_screenshotHandler;

	public static Screenshot[] m_storedScreenshots;

	public static int m_nextScreenshotIndex;

	public static Camera m_screenshotCam;

	public static int m_cameraManShotDelay;

	public static int m_cameraManIdleTimer;

	private int mainTicker;

	public string m_name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public StateMachine m_stateMachine
	{
		get
		{
			return _stateMachine;
		}
		set
		{
			_stateMachine = value;
		}
	}

	public bool m_initComplete
	{
		get
		{
			return _initComplete;
		}
	}

	public EditorScene(string _sceneName, bool _toTestState = false)
	{
		PsState.m_UIComponentsHidden = false;
		m_name = _sceneName;
		PsState.m_drawLayer = 0;
		PsState.m_editorLastSelectedItemCategory = string.Empty;
		PsState.m_freeWizardLastSelectedItemCategory = string.Empty;
		m_toTestState = _toTestState;
		GameObject gameObject = new GameObject("ScreenshotCamera");
		m_screenshotCam = gameObject.AddComponent<Camera>();
		m_screenshotCam.cullingMask = CameraS.m_mainCameraCullingMask;
		m_screenshotCam.gameObject.layer = CameraS.m_mainCameraLayer;
		m_screenshotCam.fieldOfView = CameraS.m_mainCameraFov;
		m_screenshotCam.farClipPlane = CameraS.m_mainCameraFarClip;
		m_screenshotCam.nearClipPlane = CameraS.m_mainCameraNearClip;
		m_screenshotCam.fieldOfView = CameraS.m_mainCameraFov;
		m_screenshotCam.backgroundColor = Color.black;
		m_screenshotCam.gameObject.SetActive(false);
		m_screenshotCam.enabled = false;
		m_screenshotCam.clearFlags = CameraClearFlags.Color;
		m_screenshotHandler = new GameObject("ScreenshotHandler");
		m_storedScreenshots = new Screenshot[3];
		for (int i = 0; i < m_storedScreenshots.Length; i++)
		{
			m_storedScreenshots[i] = m_screenshotHandler.AddComponent(typeof(Screenshot)) as Screenshot;
			m_storedScreenshots[i].Initialize(512, 512);
		}
	}

	public void Load()
	{
		PsResources.LoadGameResources();
		Initialize();
	}

	public static void ClearScreenshots()
	{
		PsState.m_cameraManTakeShot = true;
		m_cameraManShotDelay = 0;
		m_nextScreenshotIndex = 0;
		for (int i = 0; i < m_storedScreenshots.Length; i++)
		{
			m_storedScreenshots[i].m_hasShot = false;
		}
	}

	public static void TakeScreenshot()
	{
		if (!PsState.m_activeMinigame.m_editing)
		{
			Debug.LogWarning("SCREENSHOT TAKEN");
			Vector3 position = PsState.m_activeMinigame.m_playerTC.transform.position;
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				position.z -= UnityEngine.Random.Range(500, 700);
			}
			else
			{
				position.z -= UnityEngine.Random.Range(1000, 1200);
			}
			float num = 0f - position.z;
			float num2 = 2f * num * Mathf.Tan(m_screenshotCam.fieldOfView * 0.5f * ((float)Math.PI / 180f));
			position.y = Mathf.Max(position.y, (float)(-LevelManager.m_currentLevel.m_currentLayer.m_layerHeight) * 0.5f + num2 * 0.2f);
			m_storedScreenshots[m_nextScreenshotIndex].TakeScreenshot(m_screenshotCam, position);
			m_nextScreenshotIndex = (m_nextScreenshotIndex + 1) % 3;
		}
	}

	public static Screenshot SwitchScreenshot(bool _next)
	{
		for (int i = 0; i < 3; i++)
		{
			int num = 0;
			num = ((!_next) ? ToolBox.getRolledValue(m_nextScreenshotIndex - i - 1, 0, 2) : ToolBox.getRolledValue(m_nextScreenshotIndex + i + 1, 0, 2));
			if (m_storedScreenshots[num].m_hasShot)
			{
				m_nextScreenshotIndex = num;
				return m_storedScreenshots[num];
			}
		}
		return null;
	}

	public static void SetSelectedScreenshot(int _index)
	{
		m_nextScreenshotIndex = _index;
	}

	public static Screenshot GetSelectedScreenshot()
	{
		if (m_storedScreenshots[m_nextScreenshotIndex].m_hasShot)
		{
			return m_storedScreenshots[m_nextScreenshotIndex];
		}
		return null;
	}

	public IState GetCurrentState()
	{
		return m_stateMachine.GetCurrentState();
	}

	public void Initialize()
	{
		if (PsState.m_activeGameLoop != null)
		{
			PsState.m_activeGameLoop.EnteredMinigameScene();
		}
		_initComplete = true;
	}

	public void Reset()
	{
		Destroy();
		Load();
	}

	public void Update()
	{
		mainTicker++;
		m_stateMachine.Update();
	}

	public void Destroy()
	{
		PsState.m_activeGameLoop.DestroyMinigame();
		if (m_screenshotHandler != null)
		{
			UnityEngine.Object.Destroy(m_screenshotCam.gameObject);
			UnityEngine.Object.Destroy(m_screenshotHandler);
			m_screenshotHandler = null;
			m_storedScreenshots = null;
			m_screenshotCam = null;
		}
		m_stateMachine.Destroy();
		PsResources.UnloadGameResources();
	}

	~EditorScene()
	{
	}
}
