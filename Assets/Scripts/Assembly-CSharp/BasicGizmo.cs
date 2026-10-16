using System.Collections.Generic;
using UnityEngine;

public class BasicGizmo
{
	public List<GraphElement> m_graphElements;

	public Vector3[] m_selectionOffsets;

	public TransformC m_uiTC;

	public TransformC m_worldTC;

	public TouchAreaC m_TAC;

	public bool m_readyToMove;

	public bool m_readyToLink;

	public List<PrefabC> m_linkPrefabC;

	public GraphElement m_linkTarget;

	public Vector3 m_touchOffset;

	public List<Vector3> m_startPositions;

	public List<Vector3> m_startRotations;

	public List<Vector3> m_startScales;

	public float m_startAngle;

	public float m_tacTCDepth;

	public BasicGizmo(List<GraphElement> _graphElements)
	{
		m_graphElements = _graphElements;
		m_linkPrefabC = new List<PrefabC>();
		m_readyToMove = false;
		m_startAngle = 0f;
		m_startPositions = new List<Vector3>();
		m_startRotations = new List<Vector3>();
		m_startScales = new List<Vector3>();
		string[] tags = new string[1] { "TransformGizmo" };
		m_uiTC = EntityManager.AddEntityWithTC(tags);
		m_uiTC.p_entity.m_persistent = true;
		m_uiTC.transform.gameObject.name = "GizmoTC";
		m_worldTC = TransformS.AddComponent(m_uiTC.p_entity, "GizmoWorldTC");
		int count = m_graphElements.Count;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < count; i++)
		{
			zero += m_graphElements[i].m_TC.transform.position;
		}
		zero /= (float)count;
		m_selectionOffsets = new Vector3[count];
		for (int j = 0; j < count; j++)
		{
			m_selectionOffsets[j] = zero - m_graphElements[j].m_TC.transform.position;
		}
		Vector3 rotation = Vector3.zero;
		if (GizmoManager.GetSelection().Count == 1)
		{
			rotation = m_graphElements[0].m_rotation;
		}
		Vector3 position = CameraS.m_mainCamera.WorldToScreenPoint(zero);
		position.x -= (float)Screen.width * 0.5f;
		position.y -= (float)Screen.height * 0.5f;
		position.z = 0f;
		TransformS.SetGlobalPosition(m_worldTC, zero);
		TransformS.SetGlobalRotation(m_worldTC, rotation);
		TransformS.SetGlobalPosition(m_uiTC, position);
		m_tacTCDepth = 0f;
		int num = 0;
		for (int k = 0; k < m_graphElements.Count; k++)
		{
			ParentToGizmo(m_graphElements[k]);
			if (m_graphElements[k].m_TAC != null)
			{
				m_tacTCDepth += m_graphElements[k].m_TAC.m_TC.transform.position.z;
				num++;
			}
		}
		if (num > 0)
		{
			m_tacTCDepth /= num;
		}
	}

	public static Vector3 LimitInsideDome(Vector3 _coord)
	{
		if (Main.m_currentGame.m_currentScene.m_name != "EditorScene")
		{
			return _coord;
		}
		Vector3 vector = _coord;
		Vector2 p = ((Vector2)vector - AutoGeometryManager.m_tileCacheOffset) / 16f;
		float num = ToolBox.PointInsideEllipse(p, AutoGeometryManager.m_domeCenter, AutoGeometryManager.m_domeDiameter * 0.5f, 0f);
		if (num >= 1f)
		{
			Vector2 vector2 = AutoGeometryManager.m_domeCenter * 16f + AutoGeometryManager.m_tileCacheOffset;
			float num2 = AutoGeometryManager.m_domeDiameter.x / AutoGeometryManager.m_domeDiameter.y;
			Vector2 vector3 = ((Vector2)vector - vector2) * 0.5f;
			vector -= (Vector3)(vector3 * (num - 1f));
		}
		vector.y = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * 0.5f - 35f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f + 35f, vector.y));
		return vector;
	}

	public void DragCameraAtBorders()
	{
		float num = (float)Screen.height * 0.2f;
		Vector3 position = PsState.m_editorCamTarget.targetTC.transform.position;
		bool flag = false;
		Vector2 vector = m_uiTC.transform.position;
		if (vector.x < (float)Screen.width * -0.5f + num || vector.x > (float)Screen.width * 0.5f - num)
		{
			if (vector.x < 0f)
			{
				position.x -= (num - (vector.x + (float)Screen.width * 0.5f)) * 0.25f;
			}
			else
			{
				position.x += (num - ((float)Screen.width * 0.5f - vector.x)) * 0.25f;
			}
			position.x = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * 0.5f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerWidth * -0.5f, position.x));
			flag = true;
		}
		if (vector.y < (float)Screen.height * -0.5f + num || vector.y > (float)Screen.height * 0.5f - num)
		{
			if (vector.y < 0f)
			{
				position.y -= (num - (vector.y + (float)Screen.height * 0.5f)) * 0.25f;
			}
			else
			{
				position.y += (num - ((float)Screen.height * 0.5f - vector.y)) * 0.25f;
			}
			position.y = Mathf.Min((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * 0.5f, Mathf.Max((float)LevelManager.m_currentLevel.m_currentLayer.m_layerHeight * -0.5f, position.y));
			flag = true;
		}
		if (flag)
		{
			PsState.m_editorCamTarget.targetTC.transform.position = position;
		}
		PsState.m_editorCameraPos = position;
	}

	public virtual void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
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
			Vector3 touchWorldPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, m_uiTC.transform.position + new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f), 0f - m_tacTCDepth);
			touchWorldPos = LimitInsideDome(touchWorldPos);
			TransformS.SetGlobalPosition(m_worldTC, touchWorldPos);
			for (int j = 0; j < m_graphElements.Count; j++)
			{
				GraphElement graphElement = m_graphElements[j];
				Vector3 position = touchWorldPos + m_selectionOffsets[j];
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
			LevelManager.m_currentLevel.ItemChanged();
			new MoveUndoAction(m_graphElements, m_startPositions);
			m_readyToMove = false;
		}
	}

	public void Destroy()
	{
		UnparentFromGizmo();
		if (m_uiTC != null)
		{
			EntityManager.RemoveEntity(m_uiTC.p_entity, true, true);
		}
		m_uiTC = null;
	}

	public void MoveGizmo(Vector3 _worldPos)
	{
		TransformS.SetGlobalPosition(m_worldTC, _worldPos);
		for (int i = 0; i < m_graphElements.Count; i++)
		{
			GraphElement graphElement = m_graphElements[i];
			Vector3 position = _worldPos + m_selectionOffsets[i];
			TransformS.SetGlobalPosition(graphElement.m_TC, position);
			graphElement.m_position = position;
			for (int j = 0; j < graphElement.m_assembledClasses.Count; j++)
			{
				IAssembledClass assembledClass = graphElement.m_assembledClasses[j];
				assembledClass.SyncPositionToGraphElementPosition();
			}
		}
	}

	public void UpdatePosition()
	{
		if (m_graphElements.Count <= 0)
		{
			return;
		}
		int count = m_graphElements.Count;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			zero += m_graphElements[i].m_TC.transform.position;
			if (m_graphElements[i].m_TAC != null)
			{
				zero2 += m_graphElements[i].m_TAC.m_TC.transform.position;
				num++;
			}
		}
		zero /= (float)count;
		if (num > 0)
		{
			zero2 /= (float)num;
		}
		m_selectionOffsets = new Vector3[count];
		for (int j = 0; j < count; j++)
		{
			m_selectionOffsets[j] = m_graphElements[j].m_TC.transform.position - zero;
		}
		Vector3 rotation = Vector3.zero;
		if (m_graphElements.Count == 1)
		{
			rotation = m_graphElements[0].m_rotation;
		}
		Vector3 position = CameraS.m_mainCamera.WorldToScreenPoint(zero2);
		position.x -= (float)Screen.width * 0.5f;
		position.y -= (float)Screen.height * 0.5f;
		position.z = 50f;
		TransformS.SetGlobalPosition(m_uiTC, position);
		TransformS.SetGlobalRotation(m_uiTC, rotation);
		TransformS.SetGlobalPosition(m_worldTC, zero);
		for (int k = 0; k < m_graphElements.Count; k++)
		{
			GraphElement graphElement = m_graphElements[k];
			Vector3 position2 = zero + m_selectionOffsets[k];
			TransformS.SetGlobalPosition(graphElement.m_TC, position2);
			graphElement.m_position = position2;
			for (int l = 0; l < graphElement.m_assembledClasses.Count; l++)
			{
				IAssembledClass assembledClass = graphElement.m_assembledClasses[l];
				assembledClass.SyncPositionToGraphElementPosition();
			}
		}
	}

	public void ParentToGizmo(GraphElement _element)
	{
		for (int i = 0; i < _element.m_assembledClasses.Count; i++)
		{
			IAssembledClass assembledClass = _element.m_assembledClasses[i];
			for (int j = 0; j < assembledClass.m_assembledEntities.Count; j++)
			{
				List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, assembledClass.m_assembledEntities[j]);
				for (int k = 0; k < componentsByEntity.Count; k++)
				{
					ChipmunkBodyC chipmunkBodyC = componentsByEntity[k] as ChipmunkBodyC;
					chipmunkBodyC.m_active = false;
				}
				List<IComponent> componentsByEntity2 = EntityManager.GetComponentsByEntity(ComponentType.Transform, assembledClass.m_assembledEntities[j]);
				for (int l = 0; l < componentsByEntity2.Count; l++)
				{
					TransformC transformC = componentsByEntity2[l] as TransformC;
					if (transformC.parent == null)
					{
						TransformS.ParentComponent(transformC, m_worldTC);
					}
				}
			}
		}
	}

	public void UnparentFromGizmo()
	{
		for (int i = 0; i < m_graphElements.Count; i++)
		{
			GraphElement graphElement = m_graphElements[i];
			for (int j = 0; j < graphElement.m_assembledClasses.Count; j++)
			{
				IAssembledClass assembledClass = graphElement.m_assembledClasses[j];
				for (int k = 0; k < assembledClass.m_assembledEntities.Count; k++)
				{
					List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, assembledClass.m_assembledEntities[k]);
					for (int l = 0; l < componentsByEntity.Count; l++)
					{
						ChipmunkBodyC chipmunkBodyC = componentsByEntity[l] as ChipmunkBodyC;
						chipmunkBodyC.m_active = chipmunkBodyC.m_wasActive;
					}
					List<IComponent> componentsByEntity2 = EntityManager.GetComponentsByEntity(ComponentType.Transform, assembledClass.m_assembledEntities[k]);
					for (int m = 0; m < componentsByEntity2.Count; m++)
					{
						TransformC transformC = componentsByEntity2[m] as TransformC;
						if (transformC.parent == m_worldTC)
						{
							TransformS.UnparentComponent(transformC);
						}
					}
				}
			}
		}
	}
}
