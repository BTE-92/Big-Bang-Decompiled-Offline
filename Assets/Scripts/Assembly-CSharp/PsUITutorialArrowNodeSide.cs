using System;
using System.Collections.Generic;

public class PsUITutorialArrowNodeSide : PsUITutorialArrowNode
{
	public PsUITutorialArrowNodeSide()
		: base(0)
	{
	}

	public override void GetTarget()
	{
		m_targetLoop = PsPlanetManager.GetCurrentPlanet().m_mainPath.GetCurrentSidePathNodeInfo();
		if (m_targetLoop == null || m_targetLoop.m_node == null)
		{
			return;
		}
		m_newNode = null;
		List<PlanetRow> nodeRows = PsPlanetManager.GetCurrentPlanet().m_nodeRows;
		for (int i = 0; i < nodeRows.Count; i++)
		{
			for (int j = 0; j < nodeRows[i].m_nodes.Count; j++)
			{
				if (nodeRows[i].m_nodes[j].m_loop == m_targetLoop)
				{
					m_targetNode = nodeRows[i].m_nodes[j];
					Type type = m_targetLoop.m_node.GetType();
					m_newNode = Activator.CreateInstance(type, m_targetLoop, true) as PsPlanetNode;
					m_goTarget = m_newNode.m_prefab.p_gameObject.transform.parent.gameObject;
					m_touchArea = m_newNode.m_tac;
					hasTarget = true;
					m_transfound = true;
					break;
				}
			}
			if (hasTarget)
			{
				break;
			}
		}
		if (m_newNode != null)
		{
			GetTargetSprites(m_newNode.m_tc.p_entity);
		}
	}
}
