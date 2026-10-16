using System.Collections;
using System.Collections.Generic;

public class PsFloaterManager
{
	public PsPlanet m_planet;

	public PsPlanetPath m_floatingPath;

	public List<PsFloatingPlanetNode> m_floatingNodeList;

	public bool m_floatersAlive;

	public PsFloaterManager(PsPlanet _planet)
	{
		m_planet = _planet;
		m_floatingNodeList = new List<PsFloatingPlanetNode>();
		try
		{
			m_floatingPath = PsPlanetSerializer.LoadProgression(m_planet, "FloatingPath");
		}
		catch
		{
			m_floatingPath = null;
		}
	}

	public void Update()
	{
		for (int num = m_floatingNodeList.Count - 1; num >= 0; num--)
		{
			m_floatingNodeList[num].Update();
		}
	}

	public void ReplacePathWithServerPath(PsPlanetPath _floatingPathFromServer)
	{
		bool flag = true;
		if (PsState.m_activeGameLoop is PsTimedEventLoop)
		{
			flag = false;
		}
		if (m_floatingNodeList != null && m_floatingNodeList.Count > 0)
		{
			flag = false;
		}
		if (!flag)
		{
			return;
		}
		for (int i = 0; i < _floatingPathFromServer.m_nodeInfos.Count; i++)
		{
			if (_floatingPathFromServer.m_nodeInfos[i].m_levelNumber == -1)
			{
				_floatingPathFromServer.m_nodeInfos[i].m_levelNumber = _floatingPathFromServer.m_nodeInfos[i].m_nodeId;
			}
		}
		m_floatingPath = _floatingPathFromServer;
	}

	public void LoadFloatingPath()
	{
		if (m_floatingPath == null || m_floatingPath.m_nodeInfos.Count == 0)
		{
			Debug.LogWarning("Floatingpath was null");
			m_floatingPath = new PsPlanetPath("FloatingPath");
			m_floatingPath.m_overwrite = true;
			m_floatingPath.m_lane = -2;
		}
		else
		{
			Debug.LogWarning("Floating path found");
			m_floatingPath.m_lane = -2;
			m_floatingPath.m_overwrite = true;
			UpdateFloatingPath(m_floatingPath);
		}
		m_floatersAlive = true;
	}

	public void UpdateFloatingPath(PsPlanetPath _path)
	{
		bool flag = false;
		List<PsGameLoop> list = new List<PsGameLoop>();
		for (int i = 0; i < m_floatingPath.m_nodeInfos.Count; i++)
		{
			PsTimedEventLoop psTimedEventLoop = m_floatingPath.m_nodeInfos[i] as PsTimedEventLoop;
			if (psTimedEventLoop.m_eventOver)
			{
				if (psTimedEventLoop.m_domeDestroyed)
				{
					continue;
				}
				if (psTimedEventLoop.m_timeRunOut)
				{
					psTimedEventLoop.m_domeDestroyed = true;
				}
				else
				{
					CreateFloatingNodeAndDestroy(psTimedEventLoop);
					if (psTimedEventLoop.m_context != PsMinigameContext.Challenge)
					{
						list.Add(psTimedEventLoop);
					}
				}
				flag = true;
			}
			else
			{
				psTimedEventLoop.m_levelNumber = m_floatingNodeList.Count + 1;
				PsFloatingPlanetNode item = CreateExistingFloatingNode(psTimedEventLoop);
				m_floatingNodeList.Add(item);
			}
		}
		foreach (PsGameLoop item2 in list)
		{
			m_floatingPath.m_nodeInfos.Remove(item2);
		}
		if (flag)
		{
			Hashtable progressionJSON = ClientTools.GenerateProgressionFloatingPathJson(m_floatingPath);
			PsMetagameManager.SaveProgression(progressionJSON, false);
		}
	}

	public void CreateFloatingNodeAndDestroy(PsTimedEventLoop _loop)
	{
		_loop.m_domeDestroyed = true;
		_loop.m_levelNumber = m_floatingNodeList.Count + 1;
		PsFloatingPlanetNode psFloatingPlanetNode = CreateExistingFloatingNode(_loop);
		m_floatingNodeList.Add(psFloatingPlanetNode);
		psFloatingPlanetNode.TimedDestructionOfNode(1.3125f);
	}

	public PsFloatingPlanetNode CreateExistingFloatingNode(PsTimedEventLoop _loop)
	{
		if (_loop.m_context == PsMinigameContext.Challenge)
		{
			return new PsFloatingChallengeNode(_loop as PsGameLoopChallenge);
		}
		if (_loop.m_context == PsMinigameContext.Fresh)
		{
			return new PsFloatingFreshAndFreeNode(_loop as PsGameLoopFresh);
		}
		if (_loop.m_context == PsMinigameContext.Versus)
		{
			return new PsFloatingVersusNode(_loop as PsGameLoopVersus);
		}
		if (_loop.m_context == PsMinigameContext.Friendly)
		{
			return new PsFloatingVersusFriendlyNode(_loop as PsGameLoopFriendly);
		}
		Debug.LogError("Not suitable floating node context: " + _loop.m_context);
		return null;
	}

	public void LoadTimedEvents()
	{
		if (m_planet.m_spaceEntity != null && PsPlanetManager.m_timedEvents != null)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			List<VersusMetaData> list = new List<VersusMetaData>(PsPlanetManager.m_timedEvents.versusChallenges);
			List<VersusMetaData> list2 = new List<VersusMetaData>(PsPlanetManager.m_timedEvents.friendlyChallenges);
			for (int num = m_floatingPath.m_nodeInfos.Count - 1; num >= 0; num--)
			{
				PsTimedEventLoop psTimedEventLoop = m_floatingPath.m_nodeInfos[num] as PsTimedEventLoop;
				if (psTimedEventLoop.m_context == PsMinigameContext.Fresh)
				{
					if (psTimedEventLoop.m_eventOver && psTimedEventLoop.m_domeDestroyed)
					{
						m_floatingPath.m_nodeInfos.Remove(psTimedEventLoop);
						flag3 = true;
					}
					else
					{
						flag = true;
					}
				}
				else if (psTimedEventLoop.m_context == PsMinigameContext.Challenge)
				{
					flag2 = true;
					if (psTimedEventLoop.m_eventOver && psTimedEventLoop.m_domeDestroyed)
					{
						if (ReplaceTimedEventNode(psTimedEventLoop))
						{
							flag3 = true;
						}
					}
					else if (PsPlanetManager.m_timedEvents.HasDiamondChallenge())
					{
						for (int i = 0; i < PsPlanetManager.m_timedEvents.diamondChallenges.Count; i++)
						{
							if (psTimedEventLoop.m_timedEventId == PsPlanetManager.m_timedEvents.diamondChallenges[i].timedEventId)
							{
								psTimedEventLoop.UpdateInfo(PsPlanetManager.m_timedEvents.diamondChallenges[i].timeLeft);
								break;
							}
						}
					}
				}
				else if (psTimedEventLoop.m_context == PsMinigameContext.Versus)
				{
					if (psTimedEventLoop.m_eventOver && psTimedEventLoop.m_domeDestroyed)
					{
						m_floatingPath.m_nodeInfos.Remove(psTimedEventLoop);
						flag3 = true;
					}
					else
					{
						bool flag4 = false;
						for (int j = 0; j < list.Count; j++)
						{
							if (psTimedEventLoop.m_timedEventId == list[j].timedEventId)
							{
								if ((psTimedEventLoop as PsGameLoopVersus).UpdateInfo(list[j]))
								{
									flag3 = true;
								}
								list.Remove(list[j]);
								flag4 = true;
								break;
							}
						}
						if (!flag4)
						{
							m_floatingPath.m_nodeInfos.Remove(psTimedEventLoop);
							psTimedEventLoop.m_domeDestroyed = (psTimedEventLoop.m_eventOver = true);
							flag3 = true;
							if (psTimedEventLoop.m_node != null)
							{
								(psTimedEventLoop.m_node as PsFloatingPlanetNode).DestroyNode();
							}
						}
					}
				}
				else if (psTimedEventLoop.m_context == PsMinigameContext.Friendly)
				{
					if (psTimedEventLoop.m_eventOver && psTimedEventLoop.m_domeDestroyed)
					{
						m_floatingPath.m_nodeInfos.Remove(psTimedEventLoop);
						flag3 = true;
					}
					else
					{
						bool flag5 = false;
						for (int k = 0; k < list2.Count; k++)
						{
							if (psTimedEventLoop.m_timedEventId == list2[k].timedEventId)
							{
								if ((psTimedEventLoop as PsGameLoopVersus).UpdateInfo(list2[k]))
								{
									flag3 = true;
								}
								list2.Remove(list2[k]);
								flag5 = true;
								break;
							}
						}
						if (!flag5)
						{
							m_floatingPath.m_nodeInfos.Remove(psTimedEventLoop);
							psTimedEventLoop.m_domeDestroyed = (psTimedEventLoop.m_eventOver = true);
							flag3 = true;
							if (psTimedEventLoop.m_node != null)
							{
								(psTimedEventLoop.m_node as PsFloatingPlanetNode).DestroyNode();
							}
						}
					}
				}
			}
			if (CreateFriendRaceNodes(list2))
			{
				flag3 = true;
			}
			if (CreateVersusChallengeNodes(list))
			{
				flag3 = true;
			}
			if (!flag && CreateFreshAndFreePlanet())
			{
				flag3 = true;
			}
			m_floatingPath.m_overwrite = true;
			if (flag3)
			{
				Hashtable progressionJSON = ClientTools.GenerateProgressionFloatingPathJson(m_floatingPath);
				PsMetagameManager.SaveProgression(progressionJSON, false);
			}
		}
		else
		{
			if (m_planet.m_spaceEntity == null)
			{
				Debug.LogWarning("space entity was null");
			}
			if (PsPlanetManager.m_timedEvents == null)
			{
				Debug.LogWarning("Timed events were null");
			}
		}
	}

	public void UpdateVersus(PsGameLoopVersus _versusLoop, VersusMetaData _versusData)
	{
	}

	public bool ReplaceTimedEventNode(PsTimedEventLoop _loop)
	{
		if (PsPlanetManager.m_timedEvents != null && _loop.m_context == PsMinigameContext.Challenge && PsPlanetManager.m_timedEvents.HasDiamondChallenge())
		{
			if (PsPlanetManager.m_timedEvents.diamondChallenges[0].timedEventId != _loop.m_timedEventId)
			{
				m_floatingPath.m_nodeInfos.Remove(_loop);
				for (int i = 0; i < m_floatingPath.m_nodeInfos.Count; i++)
				{
					if (m_floatingPath.m_nodeInfos[i].m_context == PsMinigameContext.Challenge)
					{
						return false;
					}
				}
				PsChallengeMetaData metadata = PsPlanetManager.m_timedEvents.diamondChallenges[0];
				PsGameLoopChallenge psGameLoopChallenge = CreateDiamondChallengePlanetFromMetadata(metadata);
				m_floatingPath.m_nodeInfos.Add(psGameLoopChallenge);
				PsFloatingPlanetNode psFloatingPlanetNode = new PsFloatingChallengeNode(psGameLoopChallenge);
				m_floatingNodeList.Add(psFloatingPlanetNode);
				psFloatingPlanetNode.SetNewNode();
				return true;
			}
			Debug.Log("No new diamond challenge");
		}
		return false;
	}

	public void RearrangeFloaters(PsFloatingPlanetNode _deleteNode)
	{
		Debug.LogWarning("Arranging floatingpath");
		int num = m_floatingNodeList.IndexOf(_deleteNode) + 1;
		for (int i = num; i < m_floatingNodeList.Count; i++)
		{
			m_floatingNodeList[i].m_loop.m_levelNumber--;
			m_floatingNodeList[i].RearrangeNodePosition();
		}
	}

	public bool CreateFriendRaceNodes(List<VersusMetaData> _friendRaces)
	{
		for (int i = 0; i < _friendRaces.Count; i++)
		{
			VersusMetaData versusMetaData = _friendRaces[i];
			if (!(versusMetaData.status == "waiting") || !(versusMetaData.challengedPlayer == PlayerPrefsX.GetUserId()) || string.IsNullOrEmpty(versusMetaData.currentPlayer) || !(versusMetaData.currentPlayer == versusMetaData.GetOpponentInfo().score.playerId))
			{
				PsGameLoopFriendly psGameLoopFriendly = new PsGameLoopFriendly(PsMinigameContext.Friendly, versusMetaData, versusMetaData.timedEventId, versusMetaData.gameId, m_floatingPath, m_floatingNodeList.Count + 1, m_floatingNodeList.Count + 1, 0, versusMetaData.timeLeft, true);
				m_floatingPath.m_nodeInfos.Add(psGameLoopFriendly);
				PsFloatingVersusFriendlyNode psFloatingVersusFriendlyNode = new PsFloatingVersusFriendlyNode(psGameLoopFriendly);
				m_floatingNodeList.Add(psFloatingVersusFriendlyNode);
				psFloatingVersusFriendlyNode.SetNewNode();
			}
		}
		return _friendRaces.Count > 0;
	}

	public bool CreateVersusChallengeNodes(List<VersusMetaData> _versusList)
	{
		for (int i = 0; i < _versusList.Count; i++)
		{
			VersusMetaData versusMetaData = _versusList[i];
			PsGameLoopVersus psGameLoopVersus = new PsGameLoopVersus(PsMinigameContext.Versus, versusMetaData, versusMetaData.timedEventId, versusMetaData.gameId, m_floatingPath, m_floatingNodeList.Count + 1, m_floatingNodeList.Count + 1, 0, versusMetaData.timeLeft, true);
			m_floatingPath.m_nodeInfos.Add(psGameLoopVersus);
			PsFloatingVersusNode psFloatingVersusNode = new PsFloatingVersusNode(psGameLoopVersus);
			m_floatingNodeList.Add(psFloatingVersusNode);
			psFloatingVersusNode.SetNewNode();
		}
		return _versusList.Count > 0;
	}

	public bool CreateFreshAndFreePlanet()
	{
		if (PsMetagameData.GetUnlockableByIdentifier("Rating").m_unlocked && PsMetagameManager.IsFreshLevelAvailable() && PsPlanetManager.m_timedEvents != null && PsPlanetManager.m_timedEvents.HasFreshAndFree())
		{
			PsMetagameManager.FreshLevelIsCreated();
			PsGameLoopFresh loop = CreateFreshAndFreePlanetFromMetadata(PsPlanetManager.m_timedEvents.freshAndFree[0]);
			m_floatingPath.m_nodeInfos.Add(loop);
			TimerS.AddComponent(m_planet.m_spaceEntity, "Floater spawn timer", 0f, 0.2f, false, delegate(TimerC _c)
			{
				TimerS.RemoveComponent(_c);
				if (m_planet.m_spaceEntity != null)
				{
					loop.m_levelNumber = m_floatingNodeList.Count + 1;
					PsFloatingPlanetNode psFloatingPlanetNode = new PsFloatingFreshAndFreeNode(loop);
					m_floatingNodeList.Add(psFloatingPlanetNode);
					psFloatingPlanetNode.SetNewNode();
				}
			});
			return true;
		}
		return false;
	}

	public PsGameLoopFresh CreateFreshAndFreePlanetFromMetadata(PsMinigameMetaData _metadata)
	{
		int num = PlayerPrefsX.GetClientConfig().freshFreeInterval * 60;
		return new PsGameLoopFresh(PsMinigameContext.Fresh, _metadata.id, m_floatingPath, m_floatingNodeList.Count + 1, m_floatingNodeList.Count + 1, 0, num, num);
	}

	public void CreateFreshAndFreePlanetForced()
	{
		for (int i = 0; i < m_floatingPath.m_nodeInfos.Count; i++)
		{
			if (m_floatingPath.m_nodeInfos[i].m_context == PsMinigameContext.Fresh)
			{
				return;
			}
		}
		string fixedFreshMinigameId = PsState.m_fixedFreshMinigameId;
		int num = PlayerPrefsX.GetClientConfig().freshFreeInterval * 60;
		PsGameLoopFresh psGameLoopFresh = new PsGameLoopFresh(PsMinigameContext.Fresh, fixedFreshMinigameId, m_floatingPath, m_floatingNodeList.Count + 1, m_floatingNodeList.Count + 1, 0, num, num);
		m_floatingPath.m_nodeInfos.Add(psGameLoopFresh);
		PsFloatingPlanetNode psFloatingPlanetNode = new PsFloatingFreshAndFreeNode(psGameLoopFresh, true);
		m_floatingNodeList.Add(psFloatingPlanetNode);
		psFloatingPlanetNode.SetNewNode();
	}

	public bool CreateDiamondChallengePlanet()
	{
		if (PsMetagameData.GetUnlockableByIdentifier("DailyDiamond").m_unlocked && PsPlanetManager.m_timedEvents != null && PsPlanetManager.m_timedEvents.HasDiamondChallenge())
		{
			PsGameLoopChallenge loop = CreateDiamondChallengePlanetFromMetadata(PsPlanetManager.m_timedEvents.diamondChallenges[0]);
			m_floatingPath.m_nodeInfos.Add(loop);
			TimerS.AddComponent(m_planet.m_spaceEntity, "Floater spawn timer", 0f, 0.2f, false, delegate(TimerC _c)
			{
				TimerS.RemoveComponent(_c);
				if (m_planet.m_spaceEntity != null)
				{
					loop.m_levelNumber = m_floatingNodeList.Count + 1;
					PsFloatingPlanetNode psFloatingPlanetNode = new PsFloatingChallengeNode(loop);
					m_floatingNodeList.Add(psFloatingPlanetNode);
					psFloatingPlanetNode.SetNewNode();
				}
			});
			return true;
		}
		return false;
	}

	public PsGameLoopChallenge CreateDiamondChallengePlanetFromMetadata(PsChallengeMetaData _metadata)
	{
		return new PsGameLoopChallenge(PsMinigameContext.Challenge, _metadata.timedEventId, _metadata.minigameMetaData.id, m_floatingPath, m_floatingNodeList.Count + 1, m_floatingNodeList.Count + 1, 0, _metadata.timeLeft, _metadata.duration);
	}

	public void CreateDiamondChallengePlanetForced()
	{
		for (int i = 0; i < m_floatingPath.m_nodeInfos.Count; i++)
		{
			if (m_floatingPath.m_nodeInfos[i].m_context == PsMinigameContext.Challenge)
			{
				return;
			}
		}
		if (PsPlanetManager.m_timedEvents != null && PsPlanetManager.m_timedEvents.HasDiamondChallenge())
		{
			PsGameLoopChallenge psGameLoopChallenge = CreateDiamondChallengePlanetFromMetadata(PsPlanetManager.m_timedEvents.diamondChallenges[0]);
			m_floatingPath.m_nodeInfos.Add(psGameLoopChallenge);
			psGameLoopChallenge.m_levelNumber = m_floatingNodeList.Count + 1;
			PsFloatingPlanetNode psFloatingPlanetNode = new PsFloatingChallengeNode(psGameLoopChallenge, true);
			m_floatingNodeList.Add(psFloatingPlanetNode);
			psFloatingPlanetNode.SetNewNode();
		}
		else
		{
			Debug.Log("Refreshing timed events for dailydiamond");
			PsPlanetManager.m_timedEvents = null;
			m_planet.GetTimedEventsFromServer();
		}
	}

	public void AddVersus()
	{
	}

	public void AddFreshAndFree()
	{
	}

	public void AddDiamondChallenge()
	{
	}

	public void Destroy()
	{
		ClearFloatingNodes();
	}

	public void ClearFloatingNodes()
	{
		if (m_floatingNodeList != null)
		{
			for (int num = m_floatingNodeList.Count - 1; num >= 0; num--)
			{
				m_floatingNodeList[num].Destroy();
			}
			m_floatingNodeList.Clear();
		}
		m_floatersAlive = false;
	}
}
