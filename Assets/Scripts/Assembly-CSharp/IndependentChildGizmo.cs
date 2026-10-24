using System;
using System.Collections.Generic;
using UnityEngine;

public class IndependentChildGizmo : BasicGizmo
{
	private float m_minDistanceFromParent;

	private float m_maxDistanceFromParent;

	public IndependentChildGizmo(List<GraphElement> _graphElements)
		: base(_graphElements)
	{
		float num = (float)Screen.height / 768f * 100f;
		float num2 = num * 2f;
		float num3 = 0.6f * num2;
		float num4 = 1f * num2;
		float num5 = 0.05f * num2;
		float num6 = 1f * num;
		float num7 = 0.35f * num2;
		m_minDistanceFromParent = (m_graphElements[0] as GraphNode).m_minDistanceFromParent;
		m_maxDistanceFromParent = (m_graphElements[0] as GraphNode).m_maxDistanceFromParent;
		if (m_graphElements[0].m_isMoveable)
		{
			m_TAC = TouchAreaS.AddCircleArea(m_uiTC, "Move", num, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(m_TAC, TouchHandler);
			SpriteC spriteC = SpriteS.AddComponent(m_uiTC, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_selection_circle"), PsState.m_uiSheet);
			SpriteS.SetDimensions(spriteC, num2, num2);
			SpriteS.SetOffset(spriteC, new Vector3(0f, 0f, 50f), 0f);
			SpriteS.ConvertSpritesToPrefabComponent(m_uiTC, true);
		}
		if (m_graphElements[0].m_isRotateable)
		{
			float num8 = 135f;
			TransformC transformC = TransformS.AddComponent(m_uiTC.p_entity, "GizmoRotateTC");
			TransformS.ParentComponent(transformC, m_uiTC, new Vector3(num6 * Mathf.Sin(num8 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num8 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c = TouchAreaS.AddCircleArea(transformC, "RotateZ", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c, TouchHandler);
			SpriteC spriteC2 = SpriteS.AddComponent(transformC, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_rotate"), PsState.m_uiSheet);
			SpriteS.SetDimensions(spriteC2, num7, num7);
			SpriteS.SetOffset(spriteC2, new Vector3(0f, 0f, 50f), 0f);
			SpriteS.ConvertSpritesToPrefabComponent(transformC, true);
		}
		if (m_graphElements[0].m_isCopyable)
		{
			float num9 = -135f;
			TransformC transformC2 = TransformS.AddComponent(m_uiTC.p_entity, "GizmoCopyTC");
			transformC2.forceRotation = true;
			TransformS.ParentComponent(transformC2, m_uiTC, new Vector3(num6 * Mathf.Sin(num9 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num9 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c2 = TouchAreaS.AddCircleArea(transformC2, "Copy", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c2, TouchHandler);
			SpriteC spriteC3 = SpriteS.AddComponent(transformC2, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_copy"), PsState.m_uiSheet);
			SpriteS.SetDimensions(spriteC3, num7, num7);
			SpriteS.SetOffset(spriteC3, new Vector3(0f, 0f, 50f), 0f);
			SpriteS.ConvertSpritesToPrefabComponent(transformC2, true);
		}
		if (ParentGizmo.RemovableOrExtra(m_graphElements[0]))
		{
			float num10 = -45f;
			TransformC transformC3 = TransformS.AddComponent(m_uiTC.p_entity, "GizmoRemoveTC");
			transformC3.forceRotation = true;
			TransformS.ParentComponent(transformC3, m_uiTC, new Vector3(num6 * Mathf.Sin(num10 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num10 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c3 = TouchAreaS.AddCircleArea(transformC3, "Remove", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c3, TouchHandler);
			SpriteC spriteC4 = SpriteS.AddComponent(transformC3, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_delete"), PsState.m_uiSheet);
			SpriteS.SetDimensions(spriteC4, num7, num7);
			SpriteS.SetOffset(spriteC4, new Vector3(0f, 0f, 50f), 0f);
			SpriteS.ConvertSpritesToPrefabComponent(transformC3, true);
		}
		if (m_graphElements[0].m_isFlippable)
		{
			float num11 = 70f;
			TransformC transformC4 = TransformS.AddComponent(m_uiTC.p_entity, "GizmoFlipTC");
			transformC4.forceRotation = false;
			TransformS.ParentComponent(transformC4, m_uiTC, new Vector3(num6 * Mathf.Sin(num11 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num11 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c4 = TouchAreaS.AddCircleArea(transformC4, "Flip", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c4, TouchHandler);
			SpriteC spriteC5 = SpriteS.AddComponent(transformC4, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_flip"), PsState.m_uiSheet);
			SpriteS.SetDimensions(spriteC5, num7, num7);
			SpriteS.SetOffset(spriteC5, new Vector3(0f, 0f, 50f), 0f);
			SpriteS.ConvertSpritesToPrefabComponent(transformC4, true);
		}
		if (m_graphElements[0].m_isForegroundable)
		{
			float num12 = 15f;
			TransformC transformC5 = TransformS.AddComponent(m_uiTC.p_entity, "GizmoFgTC");
			transformC5.forceRotation = true;
			TransformS.ParentComponent(transformC5, m_uiTC, new Vector3(num6 * Mathf.Sin(num12 * ((float)Math.PI / 180f)), num6 * Mathf.Cos(num12 * ((float)Math.PI / 180f)), -10f));
			TouchAreaC c5 = TouchAreaS.AddCircleArea(transformC5, "Foreground", num7 * 0.5f, CameraS.m_uiCamera);
			TouchAreaS.AddTouchEventListener(c5, TouchHandler);
			SpriteC spriteC6 = SpriteS.AddComponent(transformC5, PsState.m_uiSheet.m_atlas.GetFrame("hud_gizmo_depth"), PsState.m_uiSheet);
			SpriteS.SetDimensions(spriteC6, num7, num7);
			SpriteS.SetOffset(spriteC6, new Vector3(0f, 0f, 50f), 0f);
			SpriteS.ConvertSpritesToPrefabComponent(transformC5, true);
		}
		if (m_graphElements[0].m_isModifiable)
		{
		}
		if (!m_graphElements[0].m_isReplaceable)
		{
		}
	}

	public override void TouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchIsSecondary || _touchCount != 1)
		{
			return;
		}
		if (_touchArea.m_name == "Copy")
		{
			if (_touchPhase != TouchAreaPhase.ReleaseIn)
			{
				return;
			}
			List<GraphElement> list = new List<GraphElement>();
			List<GraphElement> list2 = new List<GraphElement>();
			List<GraphElement> parents = GizmoManager.GetParents(m_graphElements);
			if ((parents[0] as GraphNode).m_assembleClassType != null && !PsMetagameData.CanPlaceUnit((parents[0] as GraphNode).m_assembleClassType.ToString()))
			{
				return;
			}
			for (int i = 0; i < parents.Count; i++)
			{
				list.Add(parents[i]);
				GraphElement graphElement = parents[i].DeepCopy();
				graphElement.m_id = LevelManager.GetUniqueId();
				Vector3 vector = Vector3.right + Vector3.down;
				Vector3 vector2 = vector * UnityEngine.Random.Range(0f, 0.2f);
				graphElement.m_position += vector * 15f + vector2;
				GraphNode graphNode = graphElement as GraphNode;
				if (graphNode.m_childElements.Count != 0)
				{
					for (int j = 0; j < graphNode.m_childElements.Count; j++)
					{
						graphNode.m_childElements[j].m_position += vector * 15f + vector2;
					}
				}
				parents[i].m_parentElement.AddElement(graphElement);
				graphElement.Assemble();
				graphElement.Copied();
				list2.Add(graphElement);
			}
			LevelManager.m_currentLevel.ItemChanged();
			new CopyUndoAction(list.ToArray(), list2.ToArray());
			GizmoManager.ClearSelection();
			GizmoManager.AddToSelection(list2);
			GizmoManager.Update();
			SoundS.PlaySingleShot("/UI/ItemCreate", Vector3.zero);
		}
		else if (_touchArea.m_name == "Remove")
		{
			if (_touchPhase == TouchAreaPhase.ReleaseIn)
			{
				List<GraphElement> list3 = new List<GraphElement>();
				List<GraphElement> selection = GizmoManager.GetSelection();
				while (selection.Count > 0)
				{
					int index = selection.Count - 1;
					selection[index].Removed();
					list3.Add(selection[index].DeepCopy());
					selection[index].Dispose();
					selection.RemoveAt(index);
				}
				GizmoManager.ClearSelection();
				GizmoManager.Update();
				LevelManager.m_currentLevel.ItemChanged();
				new RemoveUndoAction(list3.ToArray());
				SoundS.PlaySingleShot("/UI/ItemDelete", Vector3.zero);
			}
		}
		else if (_touchArea.m_name == "Flip")
		{
			if (_touchPhase == TouchAreaPhase.ReleaseIn)
			{
				List<GraphElement> list4 = new List<GraphElement>();
				List<GraphElement> parents2 = GizmoManager.GetParents(m_graphElements);
				for (int k = 0; k < parents2.Count; k++)
				{
					parents2[k].Flipped();
					parents2[k].m_flipped = !parents2[k].m_flipped;
					parents2[k].Reset();
					list4.Add(parents2[k]);
				}
				GizmoManager.Update();
				LevelManager.m_currentLevel.ItemChanged();
				new FlipUndoAction(list4.ToArray());
				SoundS.PlaySingleShot("/UI/ItemFlip", Vector3.zero);
			}
		}
		else if (_touchArea.m_name == "Foreground")
		{
			if (_touchPhase == TouchAreaPhase.ReleaseIn)
			{
				List<GraphElement> list5 = new List<GraphElement>();
				List<GraphElement> parents3 = GizmoManager.GetParents(m_graphElements);
				for (int l = 0; l < parents3.Count; l++)
				{
					parents3[l].Foregrounded();
					parents3[l].m_inFront = !parents3[l].m_inFront;
					parents3[l].Reset();
					list5.Add(parents3[l]);
				}
				GizmoManager.Update();
				LevelManager.m_currentLevel.ItemChanged();
				new FlipUndoAction(list5.ToArray());
				SoundS.PlaySingleShot("/UI/ItemFlip", Vector3.zero);
			}
		}
		else if (_touchArea.m_name == "Move")
		{
			if (_touchPhase == TouchAreaPhase.Began || _touchPhase == TouchAreaPhase.DragStart)
			{
				if (!m_readyToMove)
				{
					Vector3 vector3 = _touches[0].m_currentPosition;
					vector3.x -= (float)Screen.width * 0.5f;
					vector3.y -= (float)Screen.height * 0.5f;
					vector3.z = -10f;
					m_touchOffset = m_uiTC.transform.position - vector3;
					m_readyToMove = true;
					m_startPositions = new List<Vector3>();
					for (int m = 0; m < m_graphElements.Count; m++)
					{
						m_startPositions.Add(m_graphElements[m].m_position);
					}
				}
			}
			else if (m_readyToMove && (_touchPhase == TouchAreaPhase.MoveIn || _touchPhase == TouchAreaPhase.MoveOut || _touchPhase == TouchAreaPhase.StationaryIn || _touchPhase == TouchAreaPhase.StationaryOut))
			{
				Vector3 vector4 = (Vector3)_touches[0].m_currentPosition + m_touchOffset;
				vector4.x -= (float)Screen.width * 0.5f;
				vector4.y -= (float)Screen.height * 0.5f;
				vector4.z = 0f;
				DragCameraAtBorders();
				Vector3 vector5 = vector4 - m_uiTC.transform.position;
				Vector3 step = vector5 * 0.382f;
				TransformS.GlobalMove(m_uiTC, step);
				Vector3 vector6 = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, m_uiTC.transform.position + new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f), 0f - m_tacTCDepth);
				if (m_minDistanceFromParent != 0f || m_maxDistanceFromParent != 0f)
				{
					Vector2 vector7 = (Vector2)vector6 - (Vector2)m_graphElements[0].m_parentElement.m_TC.transform.position;
					if (vector7.sqrMagnitude < m_minDistanceFromParent * m_minDistanceFromParent)
					{
						vector7 = vector7.normalized * m_minDistanceFromParent;
						vector6 = m_graphElements[0].m_parentElement.m_TC.transform.position + (Vector3)vector7;
					}
					else if (vector7.sqrMagnitude > m_maxDistanceFromParent * m_maxDistanceFromParent)
					{
						vector7 = vector7.normalized * m_maxDistanceFromParent;
						vector6 = m_graphElements[0].m_parentElement.m_TC.transform.position + (Vector3)vector7;
					}
				}
				vector6 = BasicGizmo.LimitInsideDome(vector6);
				vector6.z = m_worldTC.transform.position.z;
				TransformS.SetGlobalPosition(m_worldTC, vector6);
				for (int n = 0; n < m_graphElements.Count; n++)
				{
					GraphElement graphElement2 = m_graphElements[n];
					Vector3 position = vector6 + m_selectionOffsets[n];
					TransformS.SetGlobalPosition(graphElement2.m_TC, position);
					graphElement2.m_position = position;
					for (int num = 0; num < graphElement2.m_assembledClasses.Count; num++)
					{
						graphElement2.m_assembledClasses[num].SyncPositionToGraphElementPosition();
					}
				}
			}
			else
			{
				switch (_touchPhase)
				{
				case TouchAreaPhase.DragEnd:
					new MoveUndoAction(m_graphElements, m_startPositions);
					break;
				case TouchAreaPhase.ReleaseIn:
				case TouchAreaPhase.ReleaseOut:
					LevelManager.m_currentLevel.ItemChanged();
					m_readyToMove = false;
					break;
				}
			}
		}
		else
		{
			if (!(_touchArea.m_name == "RotateZ"))
			{
				return;
			}
			Vector3 touchWorldPos = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, m_uiTC.transform.position + new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
			Vector3 vector8 = _touches[0].m_currentPosition;
			Vector3 touchWorldPos2 = TouchAreaS.GetTouchWorldPos(CameraS.m_mainCamera, vector8);
			switch (_touchPhase)
			{
			case TouchAreaPhase.Began:
			{
				m_startRotations = new List<Vector3>();
				for (int num8 = 0; num8 < m_graphElements.Count; num8++)
				{
					m_startRotations.Add(m_graphElements[num8].m_rotation);
				}
				Vector2 vector10 = touchWorldPos2 - touchWorldPos;
				float startAngle = Mathf.Atan2(vector10.y, vector10.x) * 57.29578f;
				m_startAngle = startAngle;
				break;
			}
			case TouchAreaPhase.MoveIn:
			case TouchAreaPhase.MoveOut:
			case TouchAreaPhase.StationaryIn:
			case TouchAreaPhase.StationaryOut:
			{
				Vector2 vector9 = touchWorldPos2 - touchWorldPos;
				float num2 = Mathf.Atan2(vector9.y, vector9.x) * 57.29578f;
				for (int num3 = 0; num3 < m_graphElements.Count; num3++)
				{
					GraphElement graphElement3 = m_graphElements[num3];
					graphElement3.m_rotation = Vector3.forward * (num2 - m_startAngle) + m_startRotations[num3];
					TransformS.SetGlobalRotation(graphElement3.m_TC, graphElement3.m_rotation);
					for (int num4 = 0; num4 < graphElement3.m_assembledClasses.Count; num4++)
					{
						IAssembledClass assembledClass = graphElement3.m_assembledClasses[num4];
						for (int num5 = 0; num5 < assembledClass.m_assembledEntities.Count; num5++)
						{
							List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, assembledClass.m_assembledEntities[num5]);
							for (int num6 = 0; num6 < componentsByEntity.Count; num6++)
							{
								ChipmunkBodyC chipmunkBodyC = componentsByEntity[num6] as ChipmunkBodyC;
								ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC.body, ToolBox.getCappedAngle(graphElement3.m_rotation.z) * ((float)Math.PI / 180f));
							}
							List<IComponent> componentsByEntity2 = EntityManager.GetComponentsByEntity(ComponentType.Transform, assembledClass.m_assembledEntities[num5]);
							for (int num7 = 0; num7 < componentsByEntity2.Count; num7++)
							{
								TransformC c = componentsByEntity2[num7] as TransformC;
								TransformS.SetGlobalRotation(c, graphElement3.m_rotation);
							}
						}
					}
				}
				break;
			}
			case TouchAreaPhase.ReleaseIn:
			case TouchAreaPhase.ReleaseOut:
				LevelManager.m_currentLevel.ItemChanged();
				new RotateUndoAction(m_graphElements, m_startRotations);
				break;
			}
		}
	}
}
