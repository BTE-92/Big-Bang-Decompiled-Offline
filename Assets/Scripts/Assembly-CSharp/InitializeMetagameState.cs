using System.Collections;
using System.Collections.Generic;
using Server;
using UnityEngine;

public class InitializeMetagameState : BasicState
{
	private StartupScene m_parent;

	private bool m_initialized;

	private Entity m_utilityEntity;

	public override void Enter(IStatedObject _parent)
	{
		m_parent = _parent as StartupScene;
		m_initialized = false;
		m_utilityEntity = EntityManager.AddEntity("UtilityEntity");
	}

	public override void Execute()
	{
		if (LoginFlow.m_userDataLoaded && !m_initialized)
		{
			Debug.Log("METAGAME DATA HANDLED");
			m_initialized = true;
			PsUnlock nextUnlock = PsMetagameData.GetNextUnlock(false);
			Debug.LogWarning("INITIALIZING METAGAME, NEXT UNLOCK IS: " + nextUnlock.m_name);
			if (nextUnlock.m_name == "unlock_start")
			{
				List<PsGameLoop> list = PsPlanetManager.GetCurrentPlanet().GeneratePersistentPathStrip();
				Hashtable progressionJson = ClientTools.GenerateProgressionPathJson(list, 1);
				PsMetagameManager.SetPlayerDataAndProgression(new Hashtable(), progressionJson, false);
				PsGameLoop psGameLoop = list[0];
				psGameLoop.StartLoop();
			}
			else
			{
				Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene", true), new FadeLoadingScene(Color.black));
			}
			ServerManager.m_dontShowLoginPopup = false;
		}
	}

	public override void Exit()
	{
		EntityManager.RemoveEntity(m_utilityEntity);
	}
}
