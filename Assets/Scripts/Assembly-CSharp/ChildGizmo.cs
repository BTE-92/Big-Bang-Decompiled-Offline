using System.Collections.Generic;
using UnityEngine;

public class ChildGizmo : BasicGizmo
{
	private float m_minDistanceFromParent;

	private float m_maxDistanceFromParent;

	public ChildGizmo(List<GraphElement> _graphElements)
		: base(_graphElements)
	{
		m_minDistanceFromParent = (m_graphElements[0] as GraphNode).m_minDistanceFromParent;
		m_maxDistanceFromParent = (m_graphElements[0] as GraphNode).m_maxDistanceFromParent;
		float num = (float)Screen.height / 768f * 50f;
		float num2 = num * 2f;
		if (m_graphElements[0].m_isMoveable)
		{
			TransformC transformC = TransformS.AddComponent(m_uiTC.p_entity, m_uiTC.transform.position + Vector3.forward * -10f);
			TransformS.ParentComponent(transformC, m_uiTC);
			m_TAC = TouchAreaS.AddCircleArea(transformC, "Move", num, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(m_TAC, TouchHandler);
			SpriteC spriteC = SpriteS.AddComponent(m_uiTC, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_selection_circle"), PsState.m_uiSheet);
			SpriteS.SetDimensions(spriteC, num2, num2);
			SpriteS.SetOffset(spriteC, new Vector3(0f, 0f, 50f), 0f);
			SpriteS.ConvertSpritesToPrefabComponent(m_uiTC, true);
		}
	}

	public override void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchIsSecondary || _touchCount != 1 || !(_touchArea.m_name == "Move"))
		{
			return;
		}
		if (_touchPhase == TouchAreaPhase.Began || _touchPhase == TouchAreaPhase.DragStart)
		{
			if (!m_readyToMove)
			{
				Vector3 vector = _touches[0].m_currentPosition;
				vector.x -= (float)Screen.width * 0.5f;
				vector.y -= (float)Screen.height * 0.5f;
				vector.z = -10f;
				m_touchOffset = m_uiTC.transform.position - vector;
				m_readyToMove = true;
				m_startPositions = new List<Vector3>();
				for (int i = 0; i < m_graphElements.Count; i++)
				{
					m_startPositions.Add(m_graphElements[i].m_position);
				}
			}
		}
		else if (m_readyToMove && (_touchPhase == TouchAreaPhase.MoveIn || _touchPhase == TouchAreaPhase.MoveOut || _touchPhase == TouchAreaPhase.StationaryIn || _touchPhase == TouchAreaPhase.StationaryOut))
		{
			Vector3 vector2 = (Vector3)_touches[0].m_currentPosition + m_touchOffset;
			vector2.x -= (float)Screen.width * 0.5f;
			vector2.y -= (float)Screen.height * 0.5f;
			vector2.z = 0f;
			DragCameraAtBorders();
			Vector3 vector3 = vector2 - m_uiTC.transform.position;
			Vector3 step = vector3 * 0.382f;
			TransformS.GlobalMove(m_uiTC, step);
			Vector3 vector4 = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, m_uiTC.transform.position + new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f), 0f - m_tacTCDepth);
			if (m_minDistanceFromParent != 0f || m_maxDistanceFromParent != 0f)
			{
				Vector2 vector5 = (Vector2)vector4 - (Vector2)m_graphElements[0].m_parentElement.m_TC.transform.position;
				if (vector5.sqrMagnitude < m_minDistanceFromParent * m_minDistanceFromParent)
				{
					vector5 = vector5.normalized * m_minDistanceFromParent;
					vector4 = m_graphElements[0].m_parentElement.m_TC.transform.position + (Vector3)vector5;
				}
				else if (vector5.sqrMagnitude > m_maxDistanceFromParent * m_maxDistanceFromParent)
				{
					vector5 = vector5.normalized * m_maxDistanceFromParent;
					vector4 = m_graphElements[0].m_parentElement.m_TC.transform.position + (Vector3)vector5;
				}
			}
			vector4 = BasicGizmo.LimitInsideDome(vector4);
			vector4.z = m_worldTC.transform.position.z;
			TransformS.SetGlobalPosition(m_worldTC, vector4);
			for (int j = 0; j < m_graphElements.Count; j++)
			{
				GraphElement graphElement = m_graphElements[j];
				Vector3 position = vector4 + m_selectionOffsets[j];
				TransformS.SetGlobalPosition(graphElement.m_TC, position);
				graphElement.m_position = position;
				for (int k = 0; k < graphElement.m_assembledClasses.Count; k++)
				{
					graphElement.m_assembledClasses[k].SyncPositionToGraphElementPosition();
				}
			}
		}
		else if (_touchPhase != TouchAreaPhase.DragEnd && (_touchPhase == TouchAreaPhase.ReleaseIn || _touchPhase == TouchAreaPhase.ReleaseOut))
		{
			m_readyToMove = false;
			LevelManager.m_currentLevel.ItemChanged();
			new MoveUndoAction(m_graphElements, m_startPositions);
		}
	}
}
