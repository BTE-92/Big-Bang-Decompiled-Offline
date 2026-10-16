using System.Collections.Generic;
using UnityEngine;

public class MultiGizmo : BasicGizmo
{
	public MultiGizmo(List<GraphElement> _graphElements)
		: base(_graphElements)
	{
		float num = (float)Screen.height / 768f * 100f;
		float num2 = num * 2f;
		if (m_graphElements[0].m_isMoveable)
		{
			m_TAC = TouchAreaS.AddCircleArea(m_uiTC, "Move", num, CameraS.m_uiCamera);
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
			vector2.z = -10f;
			DragCameraAtBorders();
			Vector3 vector3 = vector2 - m_uiTC.transform.position;
			Vector3 step = vector3 * 0.382f;
			TransformS.GlobalMove(m_uiTC, step);
			Vector3 touchWorldPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, m_uiTC.transform.position + new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f), 0f - m_tacTCDepth);
			touchWorldPos = BasicGizmo.LimitInsideDome(touchWorldPos);
			TransformS.SetGlobalPosition(m_worldTC, touchWorldPos);
			for (int j = 0; j < m_graphElements.Count; j++)
			{
				GraphElement graphElement = m_graphElements[j];
				Vector3 position = touchWorldPos + m_selectionOffsets[j];
				TransformS.SetGlobalPosition(graphElement.m_TC, position);
				graphElement.m_position = position;
				for (int k = 0; k < graphElement.m_assembledClasses.Count; k++)
				{
					IAssembledClass assembledClass = graphElement.m_assembledClasses[k];
					assembledClass.SyncPositionToGraphElementPosition();
				}
			}
		}
		else if (_touchPhase != TouchAreaPhase.DragEnd && (_touchPhase == TouchAreaPhase.ReleaseIn || _touchPhase == TouchAreaPhase.ReleaseOut))
		{
			LevelManager.m_currentLevel.ItemChanged();
			new MoveUndoAction(m_graphElements, m_startPositions);
			m_readyToMove = false;
		}
	}
}
