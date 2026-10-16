using UnityEngine;

public class PsPlanetVersusNode : PsPlanetLevelNode
{
	private bool m_moveToFloatingPath;

	public PsPlanetVersusNode(PsGameLoop _loop, bool _tutorial = false)
		: base(_loop, _tutorial)
	{
	}

	public override string GetTransformName()
	{
		return "PlanetVersusNode";
	}

	public override void CreatePrefab()
	{
		if (m_prefab != null)
		{
			PrefabS.RemoveComponent(m_prefab);
			m_prefab = null;
		}
		m_moveToFloatingPath = false;
		GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.PathButtonTournamentPrefab_GameObject);
		m_prefab = PrefabS.AddComponent(m_tc, Vector3.zero, gameObject, "VersusNode", true, false);
		PrefabS.SetCamera(m_prefab.p_gameObject, m_loop.m_planet.m_planetCamera);
		m_checkpointEffect = m_prefab.p_gameObject.GetComponent<EffectCheckpoint>();
		if ((m_loop as PsGameLoopVersus).m_createFlyingNode)
		{
			m_moveToFloatingPath = true;
		}
	}

	public override void Update()
	{
		base.Update();
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.L))
		{
			new PsPlanetFlyingNode(m_loop, m_loop.m_planet.m_floaterManager.m_floatingNodeList[0].m_loop);
		}
		if ((!m_moveToFloatingPath || !m_loop.m_planet.m_floaterManager.m_floatersAlive) && !Input.GetKeyDown(KeyCode.P))
		{
			return;
		}
		m_moveToFloatingPath = false;
		VersusMetaData versusData = m_loop.m_versusData;
		if (versusData != null && !string.IsNullOrEmpty(versusData.winner))
		{
			return;
		}
		PsTimedEventLoop psTimedEventLoop = null;
		for (int i = 0; i < m_loop.m_planet.m_floaterManager.m_floatingNodeList.Count; i++)
		{
			PsTimedEventLoop psTimedEventLoop2 = m_loop.m_planet.m_floaterManager.m_floatingNodeList[i].m_loop as PsTimedEventLoop;
			if (psTimedEventLoop2.m_timedEventId == (m_loop as PsGameLoopVersus).m_timedEventId)
			{
				psTimedEventLoop = psTimedEventLoop2;
				break;
			}
		}
		if (psTimedEventLoop == null && versusData != null)
		{
			psTimedEventLoop = new PsGameLoopVersus(PsMinigameContext.Versus, versusData, versusData.timedEventId, versusData.gameId, PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingPath, PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingNodeList.Count + 1, PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingNodeList.Count + 1, 0, versusData.timeLeft, true);
			PsFloatingVersusNode item = new PsFloatingVersusNode(psTimedEventLoop);
			PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingPath.m_nodeInfos.Add(psTimedEventLoop);
			PsPlanetManager.GetCurrentPlanet().m_floaterManager.m_floatingNodeList.Add(item);
		}
		new PsPlanetFlyingNode(m_loop, psTimedEventLoop);
		(m_loop as PsGameLoopVersus).m_createFlyingNode = false;
	}

	public override void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (!(m_loop as PsGameLoopVersus).m_removedFromPath)
		{
			base.TouchHandler(_touchArea, _touchPhase, _touchIsSecondary, _touchCount, _touches);
		}
	}

	public override void CreateUI()
	{
		CreateSidePathUI();
		if (m_updateUI)
		{
			Update();
		}
	}

	public override void SetState()
	{
		bool keepNodeHidden = m_loop.m_keepNodeHidden;
		bool flag = m_loop.m_nodeId > m_loop.m_path.m_currentNodeId;
		bool flag2 = m_loop.m_nodeId < m_loop.m_path.m_currentNodeId;
		bool flag3 = m_loop.m_nodeId == m_loop.m_path.m_currentNodeId;
		bool flag4 = m_loop == PsState.m_activeGameLoop;
		bool flag5 = PsState.m_activeGameLoop != null && (PsState.m_activeGameLoop.m_nodeId + 1 == m_loop.m_nodeId || (PsState.m_activeGameLoop.m_sidePath == m_loop.m_path && m_loop.m_nodeId == 1));
		if ((m_loop as PsGameLoopVersus).m_removedFromPath)
		{
			SetRemoved();
		}
		else if (keepNodeHidden)
		{
			SetHidden();
		}
		else if (flag || (flag3 && flag5))
		{
			SetInactive();
		}
		else if (flag4 || flag3)
		{
			SetActive();
		}
		else if (flag2)
		{
			SetClaimed();
		}
	}

	public override void SetHidden()
	{
		if ((m_loop as PsGameLoopVersus).m_removedFromPath)
		{
			SetRemoved();
		}
		else
		{
			base.SetHidden();
		}
	}

	public override void SetInactive()
	{
		if ((m_loop as PsGameLoopVersus).m_removedFromPath)
		{
			SetRemoved();
		}
		else
		{
			base.SetInactive();
		}
	}

	public override void SetActive()
	{
		if ((m_loop as PsGameLoopVersus).m_removedFromPath)
		{
			SetRemoved();
		}
		else
		{
			base.SetActive();
		}
	}

	public override void SetClaimed()
	{
		if ((m_loop as PsGameLoopVersus).m_removedFromPath)
		{
			SetRemoved();
		}
		else
		{
			base.SetClaimed();
		}
	}

	public void SetRemoved()
	{
		m_checkpointEffect.Hide();
		m_checkpointEffect.SetBorderState(true);
		m_roadWidth = 1f;
		m_roadPhase = 1f;
		m_unlocked = true;
	}
}
