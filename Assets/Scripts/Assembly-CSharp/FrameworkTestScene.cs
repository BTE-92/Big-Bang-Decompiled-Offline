using System.Collections.Generic;
using UnityEngine;

public class FrameworkTestScene : IScene, IStatedObject
{
	private string _name;

	private StateMachine _stateMachine;

	private bool _initComplete;

	public static SpriteSheet m_spriteSheet;

	public static IState state_ServerTest = new ServerTestState();

	public static IState state_AutoGeometryTest = new AutoGeometryTestState();

	public static IState state_uiTest = new UITestState();

	public static IState state_touchAreaTest = new TouchAreaTestState();

	public static IState state_massCreationTest = new MassCreateTestState();

	public static IState state_spriteSortingTest = new SpriteSortingTestState();

	public static IState state_cameraTest = new CameraTestState();

	public static IState state_chipmunkTest = new ChipmunkTestState();

	public static List<IState> m_states;

	public static int m_currentStateId;

	// test to start with (0 touch areas, 1 UI, 2 physics, 3 mass creation, 4 sprite sorting); used once, set by the admin "Extras" page
	public static int m_startState;

	private static FrameworkTestScene s_current;

	private static GameObject s_overlay;

	public static int TestCount
	{
		get
		{
			return (m_states != null) ? m_states.Count : 0;
		}
	}

	// next test (the same as Space on a keyboard); used by the "Next test" button
	public static void NextTest()
	{
		if (s_current != null && m_states != null && m_states.Count > 0)
		{
			m_currentStateId = ToolBox.getRolledValue(m_currentStateId + 1, 0, m_states.Count - 1);
			s_current.m_stateMachine.ChangeState(m_states[m_currentStateId]);
		}
	}

	// back to the game (the same as Escape / Back): the game is restarted from its start-up scene, the way the admin "Reset Local Path" button
	// does it. Nothing is deleted (no account, no server data).
	public static void Exit()
	{
		// shared UI of the game that the tests may have left in a bad state
		try
		{
			PsMetagameManager.HideResources();
		}
		catch (System.Exception)
		{
			PsMetagameManager.m_menuResourceView = null;
		}
		try
		{
			PsMetagameManager.DestroySplashBG();
		}
		catch (System.Exception)
		{
		}
		PsState.m_UIComponentsHidden = false;
		if (TouchAreaS.m_disabled)
		{
			TouchAreaS.Enable();
		}
		// the UI camera of the game as CameraS.Initialize sets it up (the tests must not leave it resized)
		if (CameraS.m_uiCamera != null)
		{
			CameraS.m_uiCamera.rect = new Rect(0f, 0f, 1f, 1f);
			CameraS.m_uiCamera.orthographicSize = (float)Screen.height * 0.5f;
			CameraS.m_uiCamera.transform.position = new Vector3(0f, 0f, -500f);
		}
		s_rescue = true;
		// exactly what the admin "Reset Local Path" button does: the planet forgets its local nodes (the tests removed the entities they
		// pointed to), then the start-up scene runs again and the game loads everything again from the server. Nothing is deleted.
		try
		{
			PsPlanetManager.GetCurrentPlanet().ResetLocalProgression();
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("[FrameworkRescue] ResetLocalProgression failed: " + e);
		}
		Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"));
	}

	// set when the game comes back from the tests: the main menu then calls Rescue() when it is built and again a second later
	public static bool s_rescue;

	public static int s_rescueFrames;

	private static string RescueName(UIComponent _c)
	{
		return (_c == null) ? "null" : _c.GetType().Name;
	}

	private static bool Bad(Vector3 _v)
	{
		return float.IsNaN(_v.x) || float.IsNaN(_v.y) || float.IsNaN(_v.z) || float.IsInfinity(_v.x) || float.IsInfinity(_v.y) || float.IsInfinity(_v.z);
	}

	// Writes in the console what state the UI is in after the tests ("[FrameworkRescue] ...") and repairs what is usually behind a UI
	// that is not drawn or not clickable: UI camera off, touch areas off, UI entities invisible, UI positions that are not numbers.
	public static void Rescue(string _when)
	{
		try
		{
			Camera cam = CameraS.m_uiCamera;
			if (cam == null)
			{
				Debug.LogWarning("[FrameworkRescue " + _when + "] UI camera is null");
			}
			else
			{
				Debug.LogWarning("[FrameworkRescue " + _when + "] UI camera: enabled=" + cam.enabled + " active=" + cam.gameObject.activeInHierarchy + " cullingMask=" + cam.cullingMask + " position=" + cam.transform.position + " orthographicSize=" + cam.orthographicSize);
				cam.gameObject.SetActive(true);
				cam.enabled = true;
			}
			Debug.LogWarning("[FrameworkRescue " + _when + "] touch areas disabled: " + TouchAreaS.m_disabled);
			TouchAreaS.Enable();
			List<Entity> invisible = EntityManager.SetVisibilityOfEntitiesWithTag("UIComponent", true);
			Debug.LogWarning("[FrameworkRescue " + _when + "] UI entities that were invisible (now visible again): " + invisible.Count);
			int hidden = 0;
			int badPositions = 0;
			for (int i = 0; i < UIManager.m_uiComponents.Count; i++)
			{
				UIComponent c = UIManager.m_uiComponents[i];
				if (c == null || c.m_TC == null || c.m_TC.transform == null)
				{
					Debug.LogWarning("[FrameworkRescue " + _when + "] UI root " + i + " is dead (" + RescueName(c) + ")");
					continue;
				}
				if (c.m_hidden)
				{
					hidden++;
				}
				if (Bad(c.m_TC.transform.position))
				{
					badPositions++;
					Debug.LogWarning("[FrameworkRescue " + _when + "] UI root " + i + " (" + RescueName(c) + ") has position " + c.m_TC.transform.position);
				}
			}
			Debug.LogWarning("[FrameworkRescue " + _when + "] UI roots: " + UIManager.m_uiComponents.Count + ", hidden: " + hidden + ", with a position that is not a number: " + badPositions);
			if (PsMainMenuState.m_createButton != null && PsMainMenuState.m_createButton.m_TC != null)
			{
				Debug.LogWarning("[FrameworkRescue " + _when + "] create button: position=" + PsMainMenuState.m_createButton.m_TC.transform.position + " hidden=" + PsMainMenuState.m_createButton.m_hidden);
			}
			else
			{
				Debug.LogWarning("[FrameworkRescue " + _when + "] the main menu has no create button");
			}
			UIManager.ScreenSizeChanged();
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("[FrameworkRescue " + _when + "] failed: " + e);
		}
	}

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

	public FrameworkTestScene(string _sceneName)
	{
		m_name = _sceneName;
	}

	public IState GetCurrentState()
	{
		return m_stateMachine.GetCurrentState();
	}

	public void Load()
	{
		Initialize();
	}

	public void Initialize()
	{
		m_currentStateId = Mathf.Clamp(m_startState, 0, 4);
		m_startState = 0;
		m_states = new List<IState>();
		m_states.Add(state_touchAreaTest);
		m_states.Add(state_uiTest);
		m_states.Add(state_chipmunkTest);
		m_states.Add(state_massCreationTest);
		m_states.Add(state_spriteSortingTest);
		m_spriteSheet = SpriteS.AddSpriteSheet(Camera.main, ResourceManager.GetMaterial("Framework/SolidMat"), 1f);
		m_stateMachine = new StateMachine();
		m_stateMachine.ChangeState(m_states[m_currentStateId]);
		Debug.LogInfo("Framework testscene loaded");
		s_current = this;
		if (s_overlay == null)
		{
			s_overlay = new GameObject("Framework test overlay");
			s_overlay.AddComponent<FrameworkTestOverlay>();
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
		// Escape (the Back button on Android) leaves the test scene and restarts the game
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Exit();
			return;
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			NextTest();
		}
		m_stateMachine.Update();
		if (mainTicker % 10 == 0)
		{
			string empty = string.Empty;
			string text = empty;
			empty = text + "Entities: " + EntityManager.m_entities.m_aliveCount + " (" + EntityManager.m_entities.m_currentLength + ")\n";
			text = empty;
			empty = text + "Transforms: " + TransformS.m_components.m_aliveCount + " (" + TransformS.m_components.m_currentLength + ")\n";
			text = empty;
			empty = text + "Chipmunk: " + ChipmunkProS.m_bodies.m_aliveCount + " (" + ChipmunkProS.m_bodies.m_currentLength + ")\n";
			text = empty;
			empty = text + "Prefabs: " + PrefabS.m_components.m_aliveCount + " (" + PrefabS.m_components.m_currentLength + ")\n";
			text = empty;
			empty = text + "Tweens: " + TweenS.m_components.m_aliveCount + " (" + TweenS.m_components.m_currentLength + ")\n";
			text = empty;
			empty = text + "CameraTargets: " + CameraS.m_cameraTargetComponents.m_aliveCount + " (" + CameraS.m_cameraTargetComponents.m_currentLength + ")\n";
			text = empty;
			empty = text + "UI Components: " + UIComponent.m_instanceCount + "\n";
			text = empty;
			empty = text + "Resources: " + ResourceManager.GetGroupCount() + "/" + ResourceManager.GetResourceCount() + "\n";
			text = empty;
			empty = text + "Sprite instances (" + SpriteC.m_componentCount + ")\n";
			for (int i = 0; i < SpriteS.m_sheets.m_aliveCount; i++)
			{
				SpriteSheet spriteSheet = SpriteS.m_sheets.m_array[SpriteS.m_sheets.m_aliveIndices[i]];
				text = empty;
				empty = text + "   " + spriteSheet.m_index + ": " + spriteSheet.m_components.m_aliveCount + "\n";
			}
		}
	}

	public void Destroy()
	{
		if (s_overlay != null)
		{
			Object.Destroy(s_overlay);
			s_overlay = null;
		}
		if (s_current == this)
		{
			s_current = null;
		}
		m_stateMachine.Destroy();
		EntityManager.RemoveAllEntities();
	}

	~FrameworkTestScene()
	{
		Debug.Log(string.Concat(this, ": Memory Freed"));
	}
}
