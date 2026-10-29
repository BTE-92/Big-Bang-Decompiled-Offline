using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class PsMenuScene : IScene, IStatedObject
{
	private string _name;

	private StateMachine _stateMachine;

	private bool _initComplete;

	private Entity m_utilityEntity;

	private TransformC m_utilityTC;

	private SoundC m_music;

	private static int m_menuMusicState;

	public static string m_lastState;

	private static bool m_updateBlur;

	public static BlurOptimizedWOE m_blur;

	private static bool m_blurFadeIn;

	private static bool m_blurFadeOut;

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

	public PsMenuScene(string _sceneName, bool _restart = false)
	{
		PsState.m_UIComponentsHidden = false;
		m_name = _sceneName;
		if (_restart)
		{
			m_lastState = null;
		}
		if (m_lastState == null)
		{
			m_menuMusicState = 0;
		}
		else
		{
			m_menuMusicState = UnityEngine.Random.Range(1, 3);
		}
		m_utilityEntity = EntityManager.AddEntity("MenuSceneUtilityEntity");
		m_utilityEntity.m_persistent = true;
		m_utilityTC = TransformS.AddComponent(m_utilityEntity, "MenuSceneUtilityTC");
	}

	public void Load()
	{
		NotificationManager.Resume();
		PsResources.LoadMenuResources();
		Initialize();
	}

	public IState GetCurrentState()
	{
		return m_stateMachine.GetCurrentState();
	}

	public void Reset()
	{
		Destroy();
		Load();
	}

	public void Update()
	{
		m_stateMachine.Update();
		UpdateBlur();
	}

	public void Destroy()
	{
		EntityManager.RemoveEntity(m_utilityEntity);
		m_utilityEntity = null;
		m_utilityTC = null;
		m_stateMachine.Destroy();
		if (m_blur != null)
		{
			UnityEngine.Object.Destroy(m_blur);
			m_blur = null;
		}
		PsPlanetManager.GetCurrentPlanet().DestroySpaceScene();
		CameraS.m_mainCamera.enabled = true;
		PsResources.UnloadMenuResources();
	}

	public void Initialize()
	{
		SoundS.RemoveAllSounds();
		RenderSettings.ambientLight = DebugDraw.GetColor(255f, 255f, 255f);
		PsPlanetManager.GetCurrentPlanet().CreateSpaceScene();
		InitBlur();
		UpdateMusic();
		m_stateMachine = new StateMachine(this);
		IState newState;
		if (m_lastState != null)
		{
			object[] args = new object[0];
			newState = Activator.CreateInstance(Type.GetType(m_lastState), args) as IState;
		}
		else
		{
			newState = new PsMainMenuState();
		}
		_initComplete = true;
		m_stateMachine.ChangeState(newState);
	}

	public void UpdateMusic()
	{
		if (!PsState.m_muteMusic)
		{
			if (m_music == null)
			{
				m_music = SoundS.AddComponent(m_utilityTC, "/Music/PlanetMusic", 1f, true);
				SoundS.PlaySound(m_music, true);
				SoundS.SetSoundParameter(m_music, "MusicState", m_menuMusicState);
			}
			else
			{
				SoundS.ResumeSound(m_music);
			}
		}
		else if (m_music != null)
		{
			SoundS.PauseSound(m_music);
		}
	}

	public static void InitBlur()
	{
		if (m_blur == null && PsPlanetManager.GetCurrentPlanet().m_planetCamera != null && PsPlanetManager.GetCurrentPlanet().m_planetCamera.gameObject != null)
		{
			m_blur = PsPlanetManager.GetCurrentPlanet().m_planetCamera.gameObject.AddComponent<BlurOptimizedWOE>();
			m_blur.blurShader = Shader.Find("Hidden/FastBlur");
			m_blur.done = false;
			m_blur.update = false;
			m_blur.blurSize = 0f;
		}
		m_blur.enabled = false;
		m_updateBlur = false;
	}

	public static void CreateBlur()
	{
		if (m_blur == null)
		{
			return;
		}
		if (!m_blur.enabled)
		{
			PsPlanet blurPlanet = PsPlanetManager.GetCurrentPlanet();
			if (blurPlanet != null && blurPlanet.m_blurCamera != null)
			{
				blurPlanet.m_blurCamera.enabled = true;
			}
			m_blur.blurSize = 0f;
			m_blur.downsample = 1;
			m_blur.enabled = true;
			m_blurFadeIn = true;
			m_blurFadeOut = false;
			m_updateBlur = true;
			m_blur.done = false;
			m_blur.update = true;
		}
	}

	private static void UpdateBlur()
	{
		if (!m_updateBlur)
		{
			return;
		}
		if (m_blurFadeIn && !m_blurFadeOut)
		{
			m_blur.blurSize += 0.5f;
			if (m_blur.blurSize >= 2f)
			{
				m_blur.blurSize = 2f;
				m_blurFadeIn = false;
				m_updateBlur = false;
				m_blur.update = false;
				PsPlanetManager.GetCurrentPlanet().HideSpaceScene();
				m_blur.done = true;
			}
		}
		if (m_blurFadeOut)
		{
			m_blur.blurSize -= 0.5f;
			if (m_blur.blurSize <= 0f)
			{
				m_blur.blurSize = 0f;
				m_blurFadeOut = false;
				m_updateBlur = false;
				DestroyBlur();
			}
		}
	}

	public static void RemoveBlur()
	{
		if (m_blur != null && m_blur.enabled)
		{
			Debug.Log("REMOVING BLUR");
			m_blurFadeOut = true;
			m_updateBlur = true;
			PsPlanetManager.GetCurrentPlanet().ShowSpaceScene();
			m_blur.update = true;
			m_blur.done = false;
			m_blur.hideSpaceScene = false;
		}
	}

	private static void DestroyBlur()
	{
		m_blur.enabled = false;
	}
}
