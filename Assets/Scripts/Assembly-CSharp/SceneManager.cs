using System;
using UnityEngine;

public class SceneManager
{
	public IScene m_currentScene;

	public IScene m_changeToScene;

	public ILoadingScene m_loadingScene;

	public bool m_sceneChanged;

	public bool m_currentSceneRemoved;

	public ILoadingScene m_oldLoadingScene;

	public UnloadUnusedAssets m_assetUnloader;

	public void ChangeScene(IScene _scene, ILoadingScene _loadingScene = null)
	{
		m_assetUnloader = null;
		m_changeToScene = _scene;
		if (_loadingScene != null)
		{
			if (m_loadingScene != null)
			{
				if (m_oldLoadingScene != null)
				{
					m_oldLoadingScene.Destroy();
					m_oldLoadingScene = null;
				}
				m_oldLoadingScene = m_loadingScene;
			}
			m_loadingScene = _loadingScene;
			if (!TouchAreaS.m_disabled)
			{
				TouchAreaS.Disable();
			}
			m_loadingScene.ToScene = m_changeToScene;
			m_loadingScene.FromScene = m_currentScene;
		}
		m_currentSceneRemoved = false;
		m_sceneChanged = false;
		Main.m_currentGame.m_currentScene = _scene;
	}

	public bool StopSceneChange()
	{
		if (m_loadingScene != null)
		{
			if (m_changeToScene != null)
			{
				m_changeToScene.Destroy();
				m_changeToScene = null;
			}
			Main.m_currentGame.m_currentScene = m_loadingScene.FromScene;
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(Main.m_currentGame.m_currentScene.m_stateMachine.GetCurrentState());
			m_loadingScene.Destroy();
			m_loadingScene = null;
			EntityManager.Update();
			if (TouchAreaS.m_disabled)
			{
				TouchAreaS.Enable();
			}
		}
		return false;
	}

	public IScene GetCurrentScene()
	{
		return m_currentScene;
	}

	private void DestroyCurrentScene()
	{
		if (m_currentScene != null)
		{
			m_currentScene.Destroy();
			m_currentScene = null;
			EntityManager.Update();
		}
		if (m_oldLoadingScene != null)
		{
			m_oldLoadingScene.Destroy();
			m_oldLoadingScene = null;
		}
	}

	private void StartAssetUnload()
	{
		if (m_assetUnloader == null)
		{
			Debug.Log("Unloading all assets and running GC.");
			GC.Collect();
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("UnusedResourceUnloader") as GameObject);
			gameObject.name = "UNUSED RESOURCE UNLOADER";
			m_assetUnloader = gameObject.GetComponent<UnloadUnusedAssets>();
		}
		else
		{
			Debug.LogError("FAIL");
		}
	}

	private bool isAssetUnloadComplete()
	{
		return m_assetUnloader.isDone;
	}

	private void RemoveAssetUnloader()
	{
		UnityEngine.Object.Destroy(m_assetUnloader.gameObject);
		m_assetUnloader = null;
	}

	public void UpdateLogic()
	{
		if (m_changeToScene != null)
		{
			if (m_loadingScene != null)
			{
				m_loadingScene.Load();
				m_changeToScene = null;
			}
			else if (m_assetUnloader == null)
			{
				DestroyCurrentScene();
				StartAssetUnload();
			}
			else if (isAssetUnloadComplete())
			{
				RemoveAssetUnloader();
				m_currentScene = m_changeToScene;
				m_currentScene.Load();
				GC.Collect();
				m_changeToScene = null;
			}
		}
		if (m_loadingScene != null && m_loadingScene.InitComplete)
		{
			if (!m_sceneChanged)
			{
				if (m_loadingScene.IntroComplete() && !m_currentSceneRemoved)
				{
					DestroyCurrentScene();
					StartAssetUnload();
					m_currentSceneRemoved = true;
				}
				if (m_currentSceneRemoved)
				{
					if (m_assetUnloader != null && isAssetUnloadComplete())
					{
						RemoveAssetUnloader();
						m_loadingScene.ToScene.Load();
						GC.Collect();
					}
					if (m_loadingScene.ToScene.m_initComplete)
					{
						m_currentScene = m_loadingScene.ToScene;
						m_sceneChanged = true;
						m_loadingScene.StartOutro();
					}
				}
			}
			m_loadingScene.Update();
			if (m_loadingScene.OutroComplete())
			{
				m_loadingScene.Destroy();
				m_loadingScene = null;
				if (TouchAreaS.m_disabled)
				{
					TouchAreaS.Enable();
				}
			}
		}
		if (m_currentScene != null && m_currentScene.m_initComplete)
		{
			m_currentScene.Update();
		}
	}
}
