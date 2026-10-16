using System.Collections.Generic;
using UnityEngine;

public class PsUITutorialArrowNode : PsUITutorialArrow
{
	protected int m_nodeId;

	protected PsGameLoop m_targetLoop;

	protected PsPlanetNode m_targetNode;

	protected PsPlanetNode m_newNode;

	public PsUITutorialArrowNode(int _nodeId)
		: base(string.Empty, string.Empty, _nodeId)
	{
	}

	public override void GetTarget()
	{
		m_nodeId = m_id;
		List<PsGameLoop> row = PsPlanetManager.GetCurrentPlanet().GetRow(m_nodeId);
		foreach (PsGameLoop item in row)
		{
			if (item.m_nodeId == m_nodeId)
			{
				m_targetLoop = item;
				break;
			}
		}
		if (m_targetLoop == null)
		{
			return;
		}
		m_newNode = new PsPlanetLevelNode(m_targetLoop, true);
		List<PlanetRow> nodeRows = PsPlanetManager.GetCurrentPlanet().m_nodeRows;
		for (int i = 0; i < nodeRows.Count; i++)
		{
			for (int j = 0; j < nodeRows[i].m_nodes.Count; j++)
			{
				if (nodeRows[i].m_nodes[j].m_loop.m_nodeId == m_nodeId)
				{
					m_targetNode = nodeRows[i].m_nodes[j];
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
		GetTargetSprites(m_newNode.m_tc.p_entity);
	}

	public override void Step()
	{
		if (m_destroy)
		{
			Destroy();
		}
		else
		{
			if (!hasTarget)
			{
				return;
			}
			if (m_targetLoop == null)
			{
				hasTarget = false;
				TouchAreaS.RemoveTouchCameraFilter();
				m_canvas.Destroy();
			}
			else if (m_3Dcamera != null)
			{
				Vector3 vector = m_3Dcamera.WorldToViewportPoint(m_goTarget.transform.position);
				offset.SetAlign(vector.x, vector.y);
				offset.UpdateAlign();
				if (!(vector.x >= 0f) || !(vector.x <= 1f) || !(vector.y >= 0f) || !(vector.y <= 1f))
				{
					m_destroy = true;
				}
			}
		}
	}

	public override void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchCount == 1 && _touchPhase == TouchAreaPhase.ReleaseIn && !_touches[0].m_dragged)
		{
			TouchAreaS.RemoveTouchCameraFilter();
			Debug.LogWarning("touchi: " + _touchArea.m_name);
			Destroy();
		}
	}

	public override void Destroy()
	{
		if (m_newNode != null)
		{
			m_newNode.Destroy();
			m_newNode = null;
		}
		m_targetLoop.m_node = m_targetNode;
		m_targetLoop = null;
		m_targetNode = null;
		base.Destroy();
	}
}
