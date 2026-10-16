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
		m_currentStateId = 0;
		m_states = new List<IState>();
		m_states.Add(state_touchAreaTest);
		m_states.Add(state_uiTest);
		m_states.Add(state_ServerTest);
		m_states.Add(state_chipmunkTest);
		m_states.Add(state_massCreationTest);
		m_states.Add(state_spriteSortingTest);
		m_spriteSheet = SpriteS.AddSpriteSheet(Camera.main, ResourceManager.GetMaterial("Framework/SolidMat"), 1f);
		m_stateMachine = new StateMachine();
		m_stateMachine.ChangeState(m_states[m_currentStateId]);
		Debug.LogInfo("Framework testscene loaded");
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
		if (Input.GetKeyDown(KeyCode.Space))
		{
			m_currentStateId = ToolBox.getRolledValue(m_currentStateId + 1, 0, m_states.Count - 1);
			m_stateMachine.ChangeState(m_states[m_currentStateId]);
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
		m_stateMachine.Destroy();
		EntityManager.RemoveAllEntities();
	}

	~FrameworkTestScene()
	{
		Debug.Log(string.Concat(this, ": Memory Freed"));
	}
}
