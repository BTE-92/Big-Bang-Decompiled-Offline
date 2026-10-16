using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GeScene : IScene, IStatedObject
{
	public string _name;

	private StateMachine _stateMachine = new StateMachine();

	private bool _initComplete;

	public static bool m_typesLoaded = false;

	public static List<string> m_allGraphNodeSubclasses = new List<string>();

	public static List<string> m_allUnitSubclasses = new List<string>();

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

	public GeScene(string _sceneName)
	{
		m_name = _sceneName;
	}

	public IState GetCurrentState()
	{
		return m_stateMachine.GetCurrentState();
	}

	public void Load()
	{
		if (!m_typesLoaded)
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsClass)
				{
					if (types[i].IsSubclassOf(typeof(GraphNode)))
					{
						m_allGraphNodeSubclasses.Add(types[i].ToString());
					}
					else if (types[i].IsSubclassOf(typeof(Unit)))
					{
						m_allUnitSubclasses.Add(types[i].ToString());
					}
				}
			}
			m_typesLoaded = true;
		}
		Initialize();
	}

	public void Initialize()
	{
		CameraS.m_mainCamera.backgroundColor = DebugDraw.GetColor(47f, 107f, 165f);
		CameraS.SnapMainCameraPos(Vector2.zero);
		PsState.m_editorCameraZoom = 1500f;
		m_stateMachine = new StateMachine(this);
		m_stateMachine.ChangeState(new GeLoadState());
		_initComplete = true;
	}

	public void Reset()
	{
		Destroy();
		Load();
	}

	public void Update()
	{
		m_stateMachine.Update();
	}

	public void Destroy()
	{
		m_stateMachine.Destroy();
		LevelManager.DestroyCurrentLevel();
		EntityManager.RemoveAllEntities();
		CameraS.m_mainCamera.backgroundColor = Color.black;
	}

	~GeScene()
	{
	}
}
